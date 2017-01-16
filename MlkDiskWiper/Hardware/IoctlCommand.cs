using System;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;
using MlkDiskWiper.PlatformInvoke.ntddisk;

namespace MlkDiskWiper.Hardware
{
    public class MbrPartitionInfo
    {
        public long StartingOffset { get; set; }
        public long PartitionLength { get; set; }
        public MbrPartitionType PartitionType { get; set; }
        public bool Bootable { get; set; }
        public uint HiddenSectorSize { get; set; }
    }

    public static class IoctlCommand
    {
        public static void CreateSinglePartitionGpt(SafeFileHandle device, MbrPartitionInfo partitionInfo)
        {
            var diskId = Guid.NewGuid();

            if (!IoctlCreateGptDisk(device, diskId, maxPartitionCount: 0))
                throw new Win32Exception();

            if (!IoctlDiskUpdateProperties(device))
                throw new Win32Exception();
        }

        public static void CreateSinglePartitionMbr(SafeFileHandle device, MbrPartitionInfo partitionInfo)
        {
            var uniqueSignature = (uint)new Random().Next();

            if (!IoctlCreateMbrDisk(device, uniqueSignature))
                throw new Win32Exception();

            if (!IoctlDiskUpdateProperties(device))
                throw new Win32Exception();

            if (!IoctlSetMbrDriveLayout(
                handle:             device,
                uniqueSignature:    uniqueSignature,
                startingOffset:     partitionInfo.StartingOffset,
                partitionLength:    partitionInfo.PartitionLength,
                partitionType:      (byte)partitionInfo.PartitionType,
                bootable:           partitionInfo.Bootable,
                hiddenSectors:      partitionInfo.HiddenSectorSize))
            {
                throw new Win32Exception();
            }

            if (!IoctlDiskUpdateProperties(device))
                throw new Win32Exception();
        }

        [DllImport("MlkDiskWiperWinApiAdapter.dll", SetLastError = true)]
        static extern bool IoctlCreateMbrDisk(
            SafeFileHandle handle,
            uint uniqueSignature);

        [DllImport("MlkDiskWiperWinApiAdapter.dll", SetLastError = true)]
        static extern bool IoctlCreateGptDisk(
            SafeFileHandle handle,
            Guid diskId,
            uint maxPartitionCount);

        [DllImport("MlkDiskWiperWinApiAdapter.dll", SetLastError = true)]
        static extern bool IoctlSetMbrDriveLayout(
            SafeFileHandle handle,
            uint uniqueSignature,
            long startingOffset,
            long partitionLength,
            byte partitionType,
            bool bootable,
            uint hiddenSectors);

        [DllImport("MlkDiskWiperWinApiAdapter.dll", SetLastError = true)]
        static extern bool IoctlDiskUpdateProperties(SafeFileHandle handle);
    }
}
