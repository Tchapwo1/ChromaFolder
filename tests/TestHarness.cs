using System;
using System.IO;
using FolderColorizer.Core;

namespace FolderColorizer.Tests
{
    public class TestHarness
    {
        public static int Main(string[] args)
        {
            Console.WriteLine("=================================================");
            Console.WriteLine("  FOLDER COLORIZER AUTOMATED VERIFICATION SUITE   ");
            Console.WriteLine("=================================================");

            string baseDir = @"D:\Downloads\FolderColorizer";
            string assetsDir = Path.Combine(baseDir, "Assets");
            string testRoot = Path.Combine(baseDir, @"tests\temp_sandbox");

            if (Directory.Exists(testRoot))
            {
                try { Directory.Delete(testRoot, true); } catch { }
            }
            Directory.CreateDirectory(testRoot);

            int passed = 0;
            int failed = 0;

            // TEST 1: Basic Color Application (Blue)
            Console.Write("[TEST 1] Apply blue to standard directory... ");
            string dir1 = Path.Combine(testRoot, "StandardDir");
            Directory.CreateDirectory(dir1);
            string err;
            if (FolderManager.ApplyColor(dir1, "blue", assetsDir, out err))
            {
                string ini = Path.Combine(dir1, "desktop.ini");
                uint fAttr = ShellHelper.GetFileAttributes(dir1);
                uint iAttr = ShellHelper.GetFileAttributes(ini);
                bool hasReadOnly = (fAttr & ShellHelper.FILE_ATTRIBUTE_READONLY) != 0;
                bool hasHiddenSystem = (iAttr & (ShellHelper.FILE_ATTRIBUTE_HIDDEN | ShellHelper.FILE_ATTRIBUTE_SYSTEM)) != 0;

                if (File.Exists(ini) && hasReadOnly && hasHiddenSystem)
                {
                    Console.WriteLine("PASSED");
                    passed++;
                }
                else
                {
                    Console.WriteLine("FAILED (Attributes mismatch: folderAttr={0}, iniAttr={1})", fAttr, iAttr);
                    failed++;
                }
            }
            else
            {
                Console.WriteLine("FAILED: " + err);
                failed++;
            }

            // TEST 2: All 6 predefined colors
            Console.Write("[TEST 2] Verify all 6 predefined colors... ");
            string[] colors = new string[] { "red", "blue", "green", "yellow", "orange", "purple" };
            bool allColorsOk = true;
            foreach (string c in colors)
            {
                string colorDir = Path.Combine(testRoot, "Dir_" + c);
                Directory.CreateDirectory(colorDir);
                if (!FolderManager.ApplyColor(colorDir, c, assetsDir, out err))
                {
                    allColorsOk = false;
                    Console.WriteLine("Failed for color: " + c + " - " + err);
                    break;
                }
            }
            if (allColorsOk)
            {
                Console.WriteLine("PASSED (All 6 colors verified)");
                passed++;
            }
            else
            {
                failed++;
            }

            // TEST 3: Unicode & Spaces in Folder Path
            Console.Write("[TEST 3] Unicode characters & spaces in path... ");
            string unicodeDir = Path.Combine(testRoot, "Folder with spaces & 漢字 日本語 🚀");
            Directory.CreateDirectory(unicodeDir);
            if (FolderManager.ApplyColor(unicodeDir, "red", assetsDir, out err))
            {
                string ini = Path.Combine(unicodeDir, "desktop.ini");
                if (File.Exists(ini))
                {
                    Console.WriteLine("PASSED");
                    passed++;
                }
                else
                {
                    Console.WriteLine("FAILED: desktop.ini missing");
                    failed++;
                }
            }
            else
            {
                Console.WriteLine("FAILED: " + err);
                failed++;
            }

            // TEST 4: Reset functionality (AC-04)
            Console.Write("[TEST 4] Reset folder to default appearance... ");
            string resetDir = Path.Combine(testRoot, "DirToReset");
            Directory.CreateDirectory(resetDir);
            FolderManager.ApplyColor(resetDir, "green", assetsDir, out err);
            // Put a dummy user file inside to prove non-destructive nature
            string dummyFile = Path.Combine(resetDir, "important_user_doc.txt");
            File.WriteAllText(dummyFile, "Important content that must never be deleted.");

            if (FolderManager.ResetColor(resetDir, out err))
            {
                string ini = Path.Combine(resetDir, "desktop.ini");
                uint fAttr = ShellHelper.GetFileAttributes(resetDir);
                bool readOnlyCleared = (fAttr & ShellHelper.FILE_ATTRIBUTE_READONLY) == 0;
                bool fileSurvives = File.Exists(dummyFile);

                if (!File.Exists(ini) && readOnlyCleared && fileSurvives)
                {
                    Console.WriteLine("PASSED (desktop.ini removed, +r cleared, user files untouched)");
                    passed++;
                }
                else
                {
                    Console.WriteLine("FAILED (iniExists={0}, readOnlyCleared={1}, userFileSurvives={2})", File.Exists(ini), readOnlyCleared, fileSurvives);
                    failed++;
                }
            }
            else
            {
                Console.WriteLine("FAILED: " + err);
                failed++;
            }

            // TEST 5: Re-color existing folder (Change color)
            Console.Write("[TEST 5] Change color on already colored folder... ");
            string changeDir = Path.Combine(testRoot, "ChangeColorDir");
            Directory.CreateDirectory(changeDir);
            FolderManager.ApplyColor(changeDir, "yellow", assetsDir, out err);
            if (FolderManager.ApplyColor(changeDir, "purple", assetsDir, out err))
            {
                string iniContent = File.ReadAllText(Path.Combine(changeDir, "desktop.ini"));
                if (iniContent.Contains("folder_purple.ico"))
                {
                    Console.WriteLine("PASSED");
                    passed++;
                }
                else
                {
                    Console.WriteLine("FAILED: desktop.ini did not update to purple");
                    failed++;
                }
            }
            else
            {
                Console.WriteLine("FAILED: " + err);
                failed++;
            }

            // TEST 6: Invalid color validation
            Console.Write("[TEST 6] Reject invalid color name... ");
            string invDir = Path.Combine(testRoot, "InvColorDir");
            Directory.CreateDirectory(invDir);
            if (!FolderManager.ApplyColor(invDir, "neon_pink", assetsDir, out err))
            {
                Console.WriteLine("PASSED (Correctly rejected: " + err + ")");
                passed++;
            }
            else
            {
                Console.WriteLine("FAILED (Allowed invalid color)");
                failed++;
            }

            // TEST 7: Missing folder error handling
            Console.Write("[TEST 7] Handle non-existent target path... ");
            string nonExistent = Path.Combine(testRoot, "ThisFolderDoesNotExist_12345");
            if (!FolderManager.ApplyColor(nonExistent, "blue", assetsDir, out err))
            {
                Console.WriteLine("PASSED (Correctly reported: " + err + ")");
                passed++;
            }
            else
            {
                Console.WriteLine("FAILED (Did not report missing folder)");
                failed++;
            }

            // TEST 8: Ledger recording and persistence
            Console.Write("[TEST 8] Ledger recording and verification... ");
            var ledger = LedgerManager.LoadAssignments();
            bool ledgerFound = false;
            foreach (var item in ledger)
            {
                if (item.FolderPath.Equals(changeDir, StringComparison.OrdinalIgnoreCase) && item.Color == "purple")
                {
                    ledgerFound = true;
                    break;
                }
            }
            if (ledgerFound)
            {
                Console.WriteLine("PASSED (Ledger entry recorded & verified)");
                passed++;
            }
            else
            {
                Console.WriteLine("FAILED (Item not found in ledger)");
                failed++;
            }

            // TEST 9: Registry context menu registration & removal
            Console.Write("[TEST 9] HKCU Context Menu registration & cleanup... ");
            string exePath = Path.Combine(baseDir, "FolderColorizer.exe");
            if (RegistryManager.RegisterContextMenu(exePath, assetsDir, out err))
            {
                bool isReg = RegistryManager.IsContextMenuRegistered();
                RegistryManager.UnregisterContextMenu(out err);
                bool isUnreg = !RegistryManager.IsContextMenuRegistered();

                if (isReg && isUnreg)
                {
                    Console.WriteLine("PASSED (Registered and cleaned up cleanly)");
                    passed++;
                }
                else
                {
                    Console.WriteLine("FAILED: RegState={0}, UnregState={1}", isReg, isUnreg);
                    failed++;
                }
            }
            else
            {
                Console.WriteLine("FAILED: " + err);
                failed++;
            }

            // Cleanup sandbox
            try { Directory.Delete(testRoot, true); } catch { }

            Console.WriteLine("=================================================");
            Console.WriteLine("SUMMARY: {0} PASSED, {1} FAILED", passed, failed);
            Console.WriteLine("=================================================");

            return failed == 0 ? 0 : 1;
        }
    }
}
