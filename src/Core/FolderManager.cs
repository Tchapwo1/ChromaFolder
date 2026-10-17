using System;
using System.IO;
using System.Text;

namespace FolderColorizer.Core
{
    public static class FolderManager
    {
        public static readonly string[] ValidColors = new string[]
        {
            "red", "blue", "green", "yellow", "orange", "purple"
        };

        public static bool ApplyColor(string folderPath, string colorName, string assetsDir, out string errorMessage)
        {
            errorMessage = null;

            if (string.IsNullOrEmpty(folderPath))
            {
                errorMessage = "Folder path cannot be empty.";
                return false;
            }

            folderPath = Path.GetFullPath(folderPath).TrimEnd('\\');

            if (!Directory.Exists(folderPath))
            {
                errorMessage = "The specified folder does not exist: " + folderPath;
                return false;
            }

            // Check for root directory restrictions
            DirectoryInfo dInfo = new DirectoryInfo(folderPath);
            if (dInfo.Parent == null)
            {
                errorMessage = "Cannot customize the root of a drive (" + folderPath + "). Please select an individual folder.";
                return false;
            }

            colorName = colorName.Trim().ToLowerInvariant();
            bool validColor = false;
            foreach (string c in ValidColors)
            {
                if (c == colorName)
                {
                    validColor = true;
                    break;
                }
            }

            if (!validColor)
            {
                errorMessage = "Invalid color '" + colorName + "'. Valid options: " + string.Join(", ", ValidColors);
                return false;
            }

            string iconFile = "folder_" + colorName + ".ico";
            string iconPath = Path.Combine(assetsDir, iconFile);

            if (!File.Exists(iconPath))
            {
                errorMessage = "Icon asset not found: " + iconPath;
                return false;
            }

            string iniPath = Path.Combine(folderPath, "desktop.ini");
            bool hadOriginal = false;
            string originalContent = null;

            try
            {
                if (File.Exists(iniPath))
                {
                    // Read original content before modifying
                    try
                    {
                        originalContent = File.ReadAllText(iniPath);
                        hadOriginal = true;
                    }
                    catch { }

                    // Strip Hidden/System/ReadOnly to allow overwriting
                    ShellHelper.SetFileAttributes(iniPath, ShellHelper.FILE_ATTRIBUTE_NORMAL);
                }

                // Construct desktop.ini content
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("[.ShellClassInfo]");
                sb.AppendLine("IconResource=" + iconPath + ",0");
                sb.AppendLine("[ViewState]");
                sb.AppendLine("Mode=");
                sb.AppendLine("Vid=");
                sb.AppendLine("FolderType=Generic");

                File.WriteAllText(iniPath, sb.ToString(), Encoding.Unicode);

                // Set desktop.ini to Hidden + System
                ShellHelper.SetFileAttributes(iniPath, ShellHelper.FILE_ATTRIBUTE_HIDDEN | ShellHelper.FILE_ATTRIBUTE_SYSTEM);

                // Set folder attribute to ReadOnly (mandatory for Explorer to recognize desktop.ini)
                ShellHelper.AddAttribute(folderPath, ShellHelper.FILE_ATTRIBUTE_READONLY);

                // Record in ledger
                LedgerManager.RecordAssignment(folderPath, colorName, iconPath, hadOriginal, originalContent);

                // Invalidate Explorer cache & refresh
                ShellHelper.NotifyFolderChanged(folderPath);

                return true;
            }
            catch (UnauthorizedAccessException)
            {
                errorMessage = "Access denied: Windows restricted modification of this folder. Ensure you have write permissions.";
                return false;
            }
            catch (Exception ex)
            {
                errorMessage = "Failed to colorize folder: " + ex.Message;
                return false;
            }
        }

        public static bool ResetColor(string folderPath, out string errorMessage)
        {
            errorMessage = null;

            if (string.IsNullOrEmpty(folderPath))
            {
                errorMessage = "Folder path cannot be empty.";
                return false;
            }

            folderPath = Path.GetFullPath(folderPath).TrimEnd('\\');

            if (!Directory.Exists(folderPath))
            {
                errorMessage = "The specified folder does not exist: " + folderPath;
                return false;
            }

            string iniPath = Path.Combine(folderPath, "desktop.ini");

            try
            {
                FolderAssignment prior = LedgerManager.MarkReset(folderPath);

                if (File.Exists(iniPath))
                {
                    // Clear attributes to allow manipulation
                    ShellHelper.SetFileAttributes(iniPath, ShellHelper.FILE_ATTRIBUTE_NORMAL);

                    if (prior != null && prior.HadOriginalDesktopIni && !string.IsNullOrEmpty(prior.OriginalDesktopIniContent))
                    {
                        // Restore previous non-FolderColorizer desktop.ini
                        File.WriteAllText(iniPath, prior.OriginalDesktopIniContent, Encoding.Unicode);
                        ShellHelper.SetFileAttributes(iniPath, ShellHelper.FILE_ATTRIBUTE_HIDDEN | ShellHelper.FILE_ATTRIBUTE_SYSTEM);
                    }
                    else
                    {
                        // Delete desktop.ini cleanly
                        File.Delete(iniPath);
                        // Remove ReadOnly from folder
                        ShellHelper.RemoveAttribute(folderPath, ShellHelper.FILE_ATTRIBUTE_READONLY);
                    }
                }
                else
                {
                    // Just ensure ReadOnly is cleared
                    ShellHelper.RemoveAttribute(folderPath, ShellHelper.FILE_ATTRIBUTE_READONLY);
                }

                // Invalidate Explorer cache & refresh
                ShellHelper.NotifyFolderChanged(folderPath);

                return true;
            }
            catch (UnauthorizedAccessException)
            {
                errorMessage = "Access denied: Windows restricted modification of this folder.";
                return false;
            }
            catch (Exception ex)
            {
                errorMessage = "Failed to reset folder: " + ex.Message;
                return false;
            }
        }
    }
}
