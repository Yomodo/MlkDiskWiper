using System.Collections.Generic;

namespace MlkDiskWiper.Hardware
{
    public interface IDriveEnumerator
    {
        IEnumerable<DeviceInfo> GetDrives();
    }
}
