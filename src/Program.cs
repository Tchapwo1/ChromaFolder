using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using FolderColorizer.Core;

namespace FolderColorizer
{
    public class Program
    {
        [DllImport("kernel32.dll")]
        private static extern bool AttachConsole(int dwProcessId);
        private const int ATTACH_PARENT_PROCESS = -1;

        [STAThread]
        public static int Main(string[] args)
        {
            // Attach to caller's console if executed from terminal
            if (AttachConsole(ATTACH_PARENT_PROCESS))
            {
                var stdOut = new StreamWriter(Console.OpenStandardOutput()) { AutoFlush = true };
                var stdErr = new StreamWriter(Console.OpenStandardError()) { AutoFlush = true };
                Console.SetOut(stdOut);
                Console.SetError(stdErr);
            }

            string exeDir = AppDomain.CurrentDomain.BaseDirectory.TrimEnd('\\');
            string assetsDir = Path.Combine(exeDir, "Assets");
            if (!Directory.Exists(assetsDir))
            {
                // Fallback to LocalAppData if running from installation
                string appData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                string candidate = Path.Combine(appData, @"FolderColorizer\icons");
                if (Directory.Exists(candidate))
                {
                    assetsDir = candidate;
                }
            }

            if (args.Length == 0)
            {
                // If double clicked or launched with no arguments, open the GUI!
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                using (Form gui = CreateGuiWindow(assetsDir))
                {
                    Application.Run(gui);
                }
                return 0;
            }

            string verb = args[0].ToLowerInvariant();

            if (verb == "apply")
            {
                if (args.Length < 3)
                {
                    Console.Error.WriteLine("Error: Missing arguments. Usage: FolderColorizer.exe apply <folderPath> <color>");
                    return 1;
                }

                // If folder path had spaces and wasn't quoted, combine middle args
                string color = args[args.Length - 1];
                string targetFolder = string.Join(" ", args, 1, args.Length - 2).Trim('\"');

                string error;
                bool ok = FolderManager.ApplyColor(targetFolder, color, assetsDir, out error);
                if (!ok)
                {
                    Console.Error.WriteLine("Error: " + error);
                    MessageBox.Show(error, "Folder Colorizer", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return 1;
                }
                Console.WriteLine("Successfully colored folder '" + targetFolder + "' to " + color + ".");
                return 0;
            }

            if (verb == "reset")
            {
                if (args.Length < 2)
                {
                    Console.Error.WriteLine("Error: Missing argument. Usage: FolderColorizer.exe reset <folderPath>");
                    return 1;
                }

                string targetFolder = string.Join(" ", args, 1, args.Length - 1).Trim('\"');
                string error;
                bool ok = FolderManager.ResetColor(targetFolder, out error);
                if (!ok)
                {
                    Console.Error.WriteLine("Error: " + error);
                    MessageBox.Show(error, "Folder Colorizer", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return 1;
                }
                Console.WriteLine("Successfully reset folder '" + targetFolder + "' to default.");
                return 0;
            }

            if (verb == "register")
            {
                string currentExe = Application.ExecutablePath;
                string error;
                bool ok = RegistryManager.RegisterContextMenu(currentExe, assetsDir, out error);
                if (!ok)
                {
                    Console.Error.WriteLine("Registration failed: " + error);
                    return 1;
                }
                Console.WriteLine("Successfully registered Folder Colorizer context menu.");
                return 0;
            }

            if (verb == "unregister")
            {
                string error;
                bool ok = RegistryManager.UnregisterContextMenu(out error);
                if (!ok)
                {
                    Console.Error.WriteLine("Unregistration failed: " + error);
                    return 1;
                }
                Console.WriteLine("Successfully unregistered Folder Colorizer context menu.");
                return 0;
            }

            if (verb == "status")
            {
                var assignments = LedgerManager.LoadAssignments();
                Console.WriteLine("=== Folder Colorizer Assignments Ledger ===");
                Console.WriteLine(string.Format("Total tracked: {0}", assignments.Count));
                foreach (var a in assignments)
                {
                    Console.WriteLine(string.Format(" - [{0}] {1} -> {2} ({3})", a.Status.ToUpper(), a.FolderPath, a.Color, a.CreatedAt));
                }
                return 0;
            }

            if (verb == "gui")
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                using (Form gui = CreateGuiWindow(assetsDir))
                {
                    Application.Run(gui);
                }
                return 0;
            }

            ShowUsage();
            return 0;
        }

        private static void ShowUsage()
        {
            Console.WriteLine("Folder Colorizer for Windows 11");
            Console.WriteLine("Usage:");
            Console.WriteLine("  FolderColorizer.exe apply <folderPath> <color>");
            Console.WriteLine("  FolderColorizer.exe reset <folderPath>");
            Console.WriteLine("  FolderColorizer.exe register");
            Console.WriteLine("  FolderColorizer.exe unregister");
            Console.WriteLine("  FolderColorizer.exe status");
            Console.WriteLine("  FolderColorizer.exe gui");
            Console.WriteLine();
            Console.WriteLine("Colors: red, blue, green, yellow, orange, purple");
        }

        private static Form CreateGuiWindow(string assetsDir)
        {
            Form form = new Form();
            form.Text = "Folder Colorizer — Palette Preview & Manager";
            form.Width = 520;
            form.Height = 380;
            form.FormBorderStyle = FormBorderStyle.FixedDialog;
            form.MaximizeBox = false;
            form.StartPosition = FormStartPosition.CenterScreen;

            string appIconPath = Path.Combine(assetsDir, "app_icon.ico");
            if (File.Exists(appIconPath))
            {
                form.Icon = new System.Drawing.Icon(appIconPath);
            }

            Label lblDrop = new Label();
            lblDrop.Text = "Drop any folder here or browse to apply a color:";
            lblDrop.Top = 15;
            lblDrop.Left = 20;
            lblDrop.Width = 460;
            form.Controls.Add(lblDrop);

            TextBox txtFolder = new TextBox();
            txtFolder.Top = 40;
            txtFolder.Left = 20;
            txtFolder.Width = 360;
            form.Controls.Add(txtFolder);

            Button btnBrowse = new Button();
            btnBrowse.Text = "Browse...";
            btnBrowse.Top = 38;
            btnBrowse.Left = 390;
            btnBrowse.Width = 90;
            btnBrowse.Click += delegate
            {
                using (FolderBrowserDialog fbd = new FolderBrowserDialog())
                {
                    fbd.Description = "Select a folder to colorize";
                    if (fbd.ShowDialog() == DialogResult.OK)
                    {
                        txtFolder.Text = fbd.SelectedPath;
                    }
                }
            };
            form.Controls.Add(btnBrowse);

            form.AllowDrop = true;
            form.DragEnter += delegate(object s, DragEventArgs e)
            {
                if (e.Data.GetDataPresent(DataFormats.FileDrop))
                {
                    e.Effect = DragDropEffects.Copy;
                }
            };
            form.DragDrop += delegate(object s, DragEventArgs e)
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                if (files != null && files.Length > 0 && Directory.Exists(files[0]))
                {
                    txtFolder.Text = files[0];
                }
            };

            Label lblColors = new Label();
            lblColors.Text = "Choose a palette color:";
            lblColors.Top = 80;
            lblColors.Left = 20;
            lblColors.Width = 460;
            form.Controls.Add(lblColors);

            string[] colors = new string[] { "red", "blue", "green", "yellow", "orange", "purple" };
            System.Drawing.Color[] rgbColors = new System.Drawing.Color[]
            {
                System.Drawing.Color.FromArgb(220, 38, 38),
                System.Drawing.Color.FromArgb(37, 99, 235),
                System.Drawing.Color.FromArgb(22, 163, 74),
                System.Drawing.Color.FromArgb(234, 179, 8),
                System.Drawing.Color.FromArgb(234, 88, 12),
                System.Drawing.Color.FromArgb(147, 51, 234)
            };

            for (int i = 0; i < colors.Length; i++)
            {
                string c = colors[i];
                Button btnColor = new Button();
                btnColor.Text = char.ToUpper(c[0]) + c.Substring(1);
                btnColor.Top = 110;
                btnColor.Left = 20 + (i * 78);
                btnColor.Width = 72;
                btnColor.Height = 45;
                btnColor.BackColor = rgbColors[i];
                btnColor.ForeColor = (c == "yellow") ? System.Drawing.Color.Black : System.Drawing.Color.White;
                btnColor.FlatStyle = FlatStyle.Flat;
                btnColor.Click += delegate
                {
                    if (string.IsNullOrEmpty(txtFolder.Text))
                    {
                        MessageBox.Show("Please select or drop a folder first.", "Folder Colorizer", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }
                    string err;
                    if (FolderManager.ApplyColor(txtFolder.Text, c, assetsDir, out err))
                    {
                        MessageBox.Show("Folder successfully colorized to " + c + "!", "Folder Colorizer", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        MessageBox.Show("Error: " + err, "Folder Colorizer", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                };
                form.Controls.Add(btnColor);
            }

            Button btnReset = new Button();
            btnReset.Text = "Reset Folder to Default Icon";
            btnReset.Top = 175;
            btnReset.Left = 20;
            btnReset.Width = 460;
            btnReset.Height = 35;
            btnReset.Click += delegate
            {
                if (string.IsNullOrEmpty(txtFolder.Text))
                {
                    MessageBox.Show("Please select or drop a folder first.", "Folder Colorizer", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                string err;
                if (FolderManager.ResetColor(txtFolder.Text, out err))
                {
                    MessageBox.Show("Folder restored to default icon.", "Folder Colorizer", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("Error: " + err, "Folder Colorizer", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            };
            form.Controls.Add(btnReset);

            Label lblStatus = new Label();
            lblStatus.Text = "Context menu: " + (RegistryManager.IsContextMenuRegistered() ? "Registered (Active)" : "Not Registered");
            lblStatus.Top = 230;
            lblStatus.Left = 20;
            lblStatus.Width = 300;
            form.Controls.Add(lblStatus);

            Button btnToggleMenu = new Button();
            btnToggleMenu.Text = RegistryManager.IsContextMenuRegistered() ? "Unregister Context Menu" : "Register Context Menu";
            btnToggleMenu.Top = 225;
            btnToggleMenu.Left = 320;
            btnToggleMenu.Width = 160;
            btnToggleMenu.Click += delegate
            {
                string err;
                if (RegistryManager.IsContextMenuRegistered())
                {
                    RegistryManager.UnregisterContextMenu(out err);
                }
                else
                {
                    RegistryManager.RegisterContextMenu(Application.ExecutablePath, assetsDir, out err);
                }
                lblStatus.Text = "Context menu: " + (RegistryManager.IsContextMenuRegistered() ? "Registered (Active)" : "Not Registered");
                btnToggleMenu.Text = RegistryManager.IsContextMenuRegistered() ? "Unregister Context Menu" : "Register Context Menu";
            };
            form.Controls.Add(btnToggleMenu);

            return form;
        }
    }
}
