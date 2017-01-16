using System.Runtime.InteropServices;

namespace MlkDiskWiper.PlatformInvoke.WinIoCtl
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct DISK_EXTENT
    {
        public uint diskNumber;
        public long startingOffset;
        public long extentLength;
    }
}
