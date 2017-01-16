using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;
using MlkDiskWiper.PlatformInvoke.devioctl;
using MlkDiskWiper.PlatformInvoke.ntddstor;

namespace MlkDiskWiper.Hardware
{
    [StructLayout(LayoutKind.Sequential)]
    public struct DiskGeometry
    {
        public long DiskSize;
        public long Cylinders;
        public uint TracksPerCylinder;
        public uint SectorsPerTrack;
        public uint BytesPerSector;
        public uint DiskSignature;
        public Guid DiskId;
    }

    public static class IoctlQueryer
    {
        public static DiskGeometry GetDiskGeometryForDevice(string devicePath)
        {
            using (var fileHandle = OpenDevice(devicePath))
            {
                return GetDiskGeometry(fileHandle);
            }
        }

        public static DiskGeometry GetDiskGeometry(SafeFileHandle deviceHandle)
        {
            var result = default(DiskGeometry);

            if (!IoctlDiskGetDriveGeometryEx(deviceHandle, ref result))
                throw new Win32Exception();

            return result;
        }

        public static uint GetDiskDeviceNumberForDevice(string devicePath)
        {
            using (var fileHandle = OpenDevice(devicePath))
            {
                return GetDiskDeviceNumber(fileHandle);
            }
        }

        public static uint GetDiskDeviceNumber(SafeFileHandle deviceHandle)
        {
            var storageDeviceNumber = new STORAGE_DEVICE_NUMBER();
            if (!IoctlStorageGetDeviceNumber(deviceHandle, storageDeviceNumber))
                throw new Win32Exception();

            if (storageDeviceNumber.deviceType != DeviceType.FILE_DEVICE_DISK)
                throw new Exception($"Expected FILE_DEVICE_DISK. Got '{storageDeviceNumber.deviceType}'.");

            return storageDeviceNumber.deviceNumber;
        }

        public static long GetDiskLength(SafeFileHandle deviceHandle)
        {
            long length;

            if (!IoctlDiskGetLengthInfo(deviceHandle, out length))
                throw new Win32Exception();

            return length;
        }

        public static IReadOnlyCollection<uint> GetDiskDeviceNumbersForVolume(string devicePath)
        {
            using (var fileHandle = OpenDevice(devicePath))
            {
                return GetDiskDeviceNumbersForVolume(fileHandle);
            }
        }

        public static IReadOnlyCollection<uint> GetDiskDeviceNumbersForVolume(SafeFileHandle deviceHandle)
        {
            var result = new List<uint>();

            var enumerator = default(IoctlVolumeGetVolumeDiskExtentsEnumerator);
            if (!IoctlVolumeGetVolumeDiskExtents_Enumerate(deviceHandle, ref enumerator))
                throw new Win32Exception();

            try
            {
                while (IoctlVolumeGetVolumeDiskExtents_Next(ref enumerator))
                    result.Add(enumerator.current.diskNumber);

                return result;
            }
            finally
            {
                IoctlVolumeGetVolumeDiskExtents_Done(ref enumerator);
            }
        }

        static SafeFileHandle OpenDevice(string path)
        {
            return FileOpener.OpenPath(path.TrimEnd('\\'), FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);
        }

        [DllImport("MlkDiskWiperWinApiAdapter.dll", SetLastError = true)]
        static extern bool IoctlDiskGetDriveGeometryEx(SafeFileHandle device, ref DiskGeometry geometry);

        [DllImport("MlkDiskWiperWinApiAdapter.dll", SetLastError = true)]
        static extern bool IoctlStorageGetDeviceNumber(SafeFileHandle handle, STORAGE_DEVICE_NUMBER storageDeviceNumber);

        [DllImport("MlkDiskWiperWinApiAdapter.dll", SetLastError = true)]
        static extern bool IoctlDiskGetLengthInfo(SafeFileHandle handle, out long length);

        [DllImport("MlkDiskWiperWinApiAdapter.dll", SetLastError = true)]
        static extern bool IoctlVolumeGetVolumeDiskExtents_Enumerate(SafeFileHandle device, ref IoctlVolumeGetVolumeDiskExtentsEnumerator enumerator);

        [DllImport("MlkDiskWiperWinApiAdapter.dll")]
        static extern bool IoctlVolumeGetVolumeDiskExtents_Next(ref IoctlVolumeGetVolumeDiskExtentsEnumerator enumerator);

        [DllImport("MlkDiskWiperWinApiAdapter.dll")]
        static extern void IoctlVolumeGetVolumeDiskExtents_Done(ref IoctlVolumeGetVolumeDiskExtentsEnumerator enumerator);
    }
}
