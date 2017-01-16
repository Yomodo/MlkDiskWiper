using System.Collections.Generic;

namespace MlkDiskWiper.Hardware
{
    public class DiskQueryer
    {
        private readonly IDriveEnumerator _driveEnumerator;

        public DiskQueryer(IDriveEnumerator driveEnumerator)
        {
            _driveEnumerator = driveEnumerator;
        }

        public IEnumerable<DiskDrive> GetDiskDrives()
        {
            foreach (var device in _driveEnumerator.GetDrives())
            {
                var devNumber = IoctlQueryer.GetDiskDeviceNumberForDevice(device.Path);

                yield return new DiskDrive
                {
                    DeviceNumber = (int)devNumber,
                    Name = device.FriendlyName,
                };
            }
        }
    }
}
