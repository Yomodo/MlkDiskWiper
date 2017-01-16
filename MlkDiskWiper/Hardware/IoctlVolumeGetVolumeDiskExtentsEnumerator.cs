using System;
using System.Runtime.InteropServices;
using MlkDiskWiper.PlatformInvoke.WinIoCtl;

namespace MlkDiskWiper.Hardware
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct IoctlVolumeGetVolumeDiskExtentsEnumerator
    {
        public short i;
        public DISK_EXTENT current;
        public IntPtr handle;
    }
}
