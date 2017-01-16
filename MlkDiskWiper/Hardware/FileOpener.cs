using System;
using System.IO;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;

namespace MlkDiskWiper.Hardware
{
    public static class FileOpener
    {
        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        static extern SafeFileHandle CreateFile(
	        string fileName,
	        [MarshalAs(UnmanagedType.U4)] FileAccess fileAccess,
	        [MarshalAs(UnmanagedType.U4)] FileShare fileShare,
	        IntPtr securityAttributes,
	        [MarshalAs(UnmanagedType.U4)] FileMode creationDisposition,
	        [MarshalAs(UnmanagedType.U4)] FileAttributes flags,
	        IntPtr template);

        public static SafeFileHandle OpenPath(
            string path,
            FileMode mode = FileMode.Open,
            FileAccess access = FileAccess.Read,
            FileShare fileShare = FileShare.ReadWrite,
            FileAttributes attributes = FileAttributes.Normal)
        {
            var fileHandle = CreateFile(
		        fileName:				path,
		        fileAccess: 			access,
		        fileShare: 				fileShare,
		        securityAttributes: 	IntPtr.Zero,
		        creationDisposition: 	mode,
		        flags: 					FileAttributes.Normal,
		        template: 				IntPtr.Zero);

            if (fileHandle.IsInvalid)
                throw new Win32Exception();

            return fileHandle;
        }
    }
}
