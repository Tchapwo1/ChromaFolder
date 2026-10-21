using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace FolderColorizer.Core
{
    public class FolderAssignment
    {
        public string FolderPath { get; set; }
        public string Color { get; set; }
        public string IconPath { get; set; }
        public bool HadOriginalDesktopIni { get; set; }
        public string OriginalDesktopIniContent { get; set; }
        public string CreatedAt { get; set; }
        public string Status { get; set; } // active, reset
    }

    public static class LedgerManager
    {
        private static string GetLedgerPath()
        {
            string appData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            string dir = Path.Combine(appData, @"ChromaFolder\data");
            string legacyDir = Path.Combine(appData, @"FolderColorizer\data");
            string ledgerPath = Path.Combine(dir, "assignments.json");
            string legacyLedger = Path.Combine(legacyDir, "assignments.json");

            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }

            // Automatically migrate existing ledger if new one doesn't exist yet
            if (!File.Exists(ledgerPath) && File.Exists(legacyLedger))
            {
                try { File.Copy(legacyLedger, ledgerPath, true); } catch { }
            }

            return ledgerPath;
        }

        public static List<FolderAssignment> LoadAssignments()
        {
            List<FolderAssignment> list = new List<FolderAssignment>();
            string path = GetLedgerPath();
            if (!File.Exists(path))
            {
                return list;
            }

            try
            {
                string text = File.ReadAllText(path, Encoding.UTF8);
                int assnIdx = text.IndexOf("\"assignments\"", StringComparison.OrdinalIgnoreCase);
                if (assnIdx == -1) return list;

                int cur = assnIdx;
                while (true)
                {
                    int openBrace = text.IndexOf('{', cur);
                    if (openBrace == -1) break;

                    int closeBrace = text.IndexOf('}', openBrace);
                    if (closeBrace == -1) break;

                    string block = text.Substring(openBrace, closeBrace - openBrace + 1);
                    if (block.Contains("\"folderPath\""))
                    {
                        FolderAssignment item = new FolderAssignment();
                        item.FolderPath = ExtractJsonValue(block, "folderPath");
                        item.Color = ExtractJsonValue(block, "color");
                        item.IconPath = ExtractJsonValue(block, "iconPath");
                        item.HadOriginalDesktopIni = block.Contains("\"hadOriginalDesktopIni\": true");
                        item.OriginalDesktopIniContent = ExtractJsonValue(block, "originalDesktopIniContent");
                        item.CreatedAt = ExtractJsonValue(block, "createdAt");
                        item.Status = ExtractJsonValue(block, "status");
                        if (!string.IsNullOrEmpty(item.FolderPath))
                        {
                            list.Add(item);
                        }
                    }
                    cur = closeBrace + 1;
                }
            }
            catch
            {
                // Fallback to empty list if unreadable
            }
            return list;
        }

        public static void RecordAssignment(string folderPath, string color, string iconPath, bool hadOriginal, string originalContent)
        {
            List<FolderAssignment> list = LoadAssignments();
            string normTarget = folderPath.TrimEnd('\\').ToLowerInvariant();

            FolderAssignment existing = null;
            foreach (FolderAssignment item in list)
            {
                if (item.FolderPath.TrimEnd('\\').ToLowerInvariant() == normTarget)
                {
                    existing = item;
                    break;
                }
            }

            if (existing != null)
            {
                existing.Color = color;
                existing.IconPath = iconPath;
                existing.Status = "active";
            }
            else
            {
                FolderAssignment newAssn = new FolderAssignment();
                newAssn.FolderPath = folderPath;
                newAssn.Color = color;
                newAssn.IconPath = iconPath;
                newAssn.HadOriginalDesktopIni = hadOriginal;
                newAssn.OriginalDesktopIniContent = originalContent;
                newAssn.CreatedAt = DateTime.UtcNow.ToString("o");
                newAssn.Status = "active";
                list.Add(newAssn);
            }

            SaveAssignments(list);
        }

        public static FolderAssignment MarkReset(string folderPath)
        {
            List<FolderAssignment> list = LoadAssignments();
            string normTarget = folderPath.TrimEnd('\\').ToLowerInvariant();
            FolderAssignment found = null;

            foreach (FolderAssignment item in list)
            {
                if (item.FolderPath.TrimEnd('\\').ToLowerInvariant() == normTarget)
                {
                    item.Status = "reset";
                    found = item;
                    break;
                }
            }

            SaveAssignments(list);
            return found;
        }

        private static void SaveAssignments(List<FolderAssignment> list)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("{");
            sb.AppendLine("  \"version\": \"1.0.0\",");
            sb.AppendLine("  \"lastUpdated\": \"" + EscapeJson(DateTime.UtcNow.ToString("o")) + "\",");
            sb.AppendLine("  \"assignments\": [");

            for (int i = 0; i < list.Count; i++)
            {
                FolderAssignment a = list[i];
                sb.AppendLine("    {");
                sb.AppendLine("      \"folderPath\": \"" + EscapeJson(a.FolderPath) + "\",");
                sb.AppendLine("      \"color\": \"" + EscapeJson(a.Color) + "\",");
                sb.AppendLine("      \"iconPath\": \"" + EscapeJson(a.IconPath) + "\",");
                sb.AppendLine("      \"hadOriginalDesktopIni\": " + (a.HadOriginalDesktopIni ? "true" : "false") + ",");
                sb.AppendLine("      \"originalDesktopIniContent\": " + (a.OriginalDesktopIniContent == null ? "null" : "\"" + EscapeJson(a.OriginalDesktopIniContent) + "\"") + ",");
                sb.AppendLine("      \"createdAt\": \"" + EscapeJson(a.CreatedAt ?? "") + "\",");
                sb.AppendLine("      \"status\": \"" + EscapeJson(a.Status ?? "active") + "\"");
                sb.Append("    }");
                if (i < list.Count - 1) sb.AppendLine(",");
                else sb.AppendLine();
            }

            sb.AppendLine("  ]");
            sb.AppendLine("}");

            File.WriteAllText(GetLedgerPath(), sb.ToString(), Encoding.UTF8);
        }

        private static string EscapeJson(string s)
        {
            if (string.IsNullOrEmpty(s)) return "";
            return s.Replace("\\", "\\\\").Replace("\"", "\\\"").Replace("\r", "\\r").Replace("\n", "\\n");
        }

        private static string ExtractJsonValue(string block, string key)
        {
            string pattern = "\"" + key + "\": \"";
            int idx = block.IndexOf(pattern, StringComparison.OrdinalIgnoreCase);
            if (idx == -1) return null;
            int start = idx + pattern.Length;
            int end = block.IndexOf("\"", start);
            if (end == -1) return null;
            string val = block.Substring(start, end - start);
            return val.Replace("\\\"", "\"").Replace("\\\\", "\\").Replace("\\r", "\r").Replace("\\n", "\n");
        }
    }
}
