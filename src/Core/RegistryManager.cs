using System;
using Microsoft.Win32;
using System.IO;

namespace FolderColorizer.Core
{
    public static class RegistryManager
    {
        private const string RootShellKey = @"Software\Classes\Directory\shell\ChromaFolder";
        private const string BackgroundShellKey = @"Software\Classes\Directory\Background\shell\ChromaFolder";
        private const string LegacyRootShellKey = @"Software\Classes\Directory\shell\FolderColorizer";
        private const string LegacyBackgroundShellKey = @"Software\Classes\Directory\Background\shell\FolderColorizer";

        public static bool RegisterContextMenu(string exePath, string assetsDir, out string errorMessage)
        {
            errorMessage = null;
            try
            {
                // Clean up any legacy registry keys first
                try
                {
                    Registry.CurrentUser.DeleteSubKeyTree(LegacyRootShellKey, false);
                    Registry.CurrentUser.DeleteSubKeyTree(LegacyBackgroundShellKey, false);
                }
                catch { }

                RegisterForTarget(RootShellKey, exePath, assetsDir, "%1");
                RegisterForTarget(BackgroundShellKey, exePath, assetsDir, "%V");
                ShellHelper.NotifyShellAssociationsChanged();
                return true;
            }
            catch (Exception ex)
            {
                errorMessage = "Failed to register context menu: " + ex.Message;
                return false;
            }
        }

        private static void RegisterForTarget(string rootPath, string exePath, string assetsDir, string paramPlaceholder)
        {
            using (RegistryKey key = Registry.CurrentUser.CreateSubKey(rootPath))
            {
                if (key == null) throw new InvalidOperationException("Could not create registry key: " + rootPath);

                key.SetValue("", "ChromaFolder");
                key.SetValue("MUIVerb", "ChromaFolder");
                key.SetValue("SubCommands", "");

                string appIcon = Path.Combine(assetsDir, "app_icon.ico");
                if (File.Exists(appIcon))
                {
                    key.SetValue("Icon", appIcon);
                }

                // Create subkeys for each color
                string[] colors = new string[] { "red", "blue", "green", "yellow", "orange", "purple" };
                string[] displayNames = new string[] { "Red", "Blue", "Green", "Yellow", "Orange", "Purple" };

                using (RegistryKey shellKey = key.CreateSubKey("shell"))
                {
                    for (int i = 0; i < colors.Length; i++)
                    {
                        string c = colors[i];
                        string name = displayNames[i];
                        string subkeyName = string.Format("{0:D2}_{1}", i + 1, name);

                        using (RegistryKey itemKey = shellKey.CreateSubKey(subkeyName))
                        {
                            itemKey.SetValue("", name);
                            itemKey.SetValue("MUIVerb", name);

                            string iconPath = Path.Combine(assetsDir, "folder_" + c + ".ico");
                            if (File.Exists(iconPath))
                            {
                                itemKey.SetValue("Icon", iconPath);
                            }

                            using (RegistryKey cmdKey = itemKey.CreateSubKey("command"))
                            {
                                cmdKey.SetValue("", string.Format("\"{0}\" apply \"{1}\" {2}", exePath, paramPlaceholder, c));
                            }
                        }
                    }

                    // Reset option
                    using (RegistryKey resetKey = shellKey.CreateSubKey("07_Reset"))
                    {
                        resetKey.SetValue("", "Reset to Default");
                        resetKey.SetValue("MUIVerb", "Reset to Default");
                        resetKey.SetValue("Icon", "shell32.dll,238");
                        resetKey.SetValue("CommandFlags", 0x20, RegistryValueKind.DWord); // Separator before item

                        using (RegistryKey cmdKey = resetKey.CreateSubKey("command"))
                        {
                            cmdKey.SetValue("", string.Format("\"{0}\" reset \"{1}\"", exePath, paramPlaceholder));
                        }
                    }
                }
            }
        }

        public static bool UnregisterContextMenu(out string errorMessage)
        {
            errorMessage = null;
            try
            {
                Registry.CurrentUser.DeleteSubKeyTree(RootShellKey, false);
                Registry.CurrentUser.DeleteSubKeyTree(BackgroundShellKey, false);

                try
                {
                    Registry.CurrentUser.DeleteSubKeyTree(LegacyRootShellKey, false);
                    Registry.CurrentUser.DeleteSubKeyTree(LegacyBackgroundShellKey, false);
                }
                catch { }

                ShellHelper.NotifyShellAssociationsChanged();
                return true;
            }
            catch (Exception ex)
            {
                errorMessage = "Failed to unregister context menu: " + ex.Message;
                return false;
            }
        }

        public static bool IsContextMenuRegistered()
        {
            using (RegistryKey key = Registry.CurrentUser.OpenSubKey(RootShellKey))
            {
                return key != null;
            }
        }
    }
}
