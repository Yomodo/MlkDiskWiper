using System.Runtime.InteropServices;
using MlkDiskWiper.PlatformInvoke.devioctl;

namespace MlkDiskWiper.PlatformInvoke.ntddstor
{
    // Source: ntddstor.h
    [StructLayout(LayoutKind.Sequential)]
    public class STORAGE_DEVICE_NUMBER
    {
        public DeviceType deviceType;
        public uint deviceNumber;
        public uint partitionNumber;
    }
}
