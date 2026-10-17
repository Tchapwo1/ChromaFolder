using System;
using System.Runtime.InteropServices;

namespace FolderColorizer.Core
{
    public static class ShellHelper
    {
        public const uint SHCNE_UPDATEITEM = 0x00002000;
        public const uint SHCNE_ASSOCCHANGED = 0x08000000;
        public const uint SHCNF_PATHW = 0x0005;
        public const uint SHCNF_FLUSH = 0x1000;
        public const uint SHCNF_IDLIST = 0x0000;

        public const uint FILE_ATTRIBUTE_READONLY = 0x00000001;
        public const uint FILE_ATTRIBUTE_HIDDEN = 0x00000002;
        public const uint FILE_ATTRIBUTE_SYSTEM = 0x00000004;
        public const uint FILE_ATTRIBUTE_DIRECTORY = 0x00000010;
        public const uint FILE_ATTRIBUTE_NORMAL = 0x00000080;
        public const uint INVALID_FILE_ATTRIBUTES = 0xFFFFFFFF;

        [DllImport("shell32.dll", CharSet = CharSet.Unicode)]
        public static extern void SHChangeNotify(
            uint wEventId,
            uint uFlags,
            IntPtr dwItem1,
            IntPtr dwItem2
        );

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern bool SetFileAttributes(string lpFileName, uint dwFileAttributes);

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern uint GetFileAttributes(string lpFileName);

        public static void NotifyFolderChanged(string folderPath)
        {
            if (string.IsNullOrEmpty(folderPath)) return;

            IntPtr pPath = Marshal.StringToHGlobalUni(folderPath);
            try
            {
                // Notify Explorer to update this specific folder item
                SHChangeNotify(SHCNE_UPDATEITEM | SHCNF_FLUSH, SHCNF_PATHW, pPath, IntPtr.Zero);
            }
            finally
            {
                Marshal.FreeHGlobal(pPath);
            }

            // Also signal association / icon refresh
            SHChangeNotify(SHCNE_ASSOCCHANGED | SHCNF_FLUSH, SHCNF_IDLIST, IntPtr.Zero, IntPtr.Zero);
        }

        public static bool AddAttribute(string path, uint attribute)
        {
            uint current = GetFileAttributes(path);
            if (current == INVALID_FILE_ATTRIBUTES) return false;
            return SetFileAttributes(path, current | attribute);
        }

        public static bool RemoveAttribute(string path, uint attribute)
        {
            uint current = GetFileAttributes(path);
            if (current == INVALID_FILE_ATTRIBUTES) return false;
            return SetFileAttributes(path, current & ~attribute);
        }
    }
}
