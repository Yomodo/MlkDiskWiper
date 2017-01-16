using System.Collections.Generic;
using System.Management;

namespace MlkDiskWiper.Hardware
{
    public class WmiWin32DiskDrive : IDriveEnumerator
    {
        public IEnumerable<DeviceInfo> GetDrives()
        {
            using (var diskDriveClass = new ManagementClass("Win32_DiskDrive"))
            using (var disks = diskDriveClass.GetInstances())
            {
                foreach (ManagementObject disk in disks)
                {
                    yield return new DeviceInfo
                    {
                        FriendlyName = (string)disk.Properties["Caption"].Value,
                        Path = (string)disk.Properties["DeviceID"].Value,
                    };
                }
            }
        }
    }
}
