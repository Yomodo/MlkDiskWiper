using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using MlkDiskWiper.PlatformInvoke;
using MlkDiskWiper.PlatformInvoke.SetupApi;
using MlkDiskWiper.PlatformInvoke.winnt;
using WinApiTypes;

namespace MlkDiskWiper.Hardware
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    struct SetupApiDeviceEnumerator
    {
        public int i;
        public bool wasError;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 2048)]
        public string current;

        public IntPtr handle;
    };

    public class DeviceInfo
    {
        public string FriendlyName { get; set; }
        public string Path { get; set; }
    }

    public class SetupApiQueryer : IDriveEnumerator
    {
        public IEnumerable<DeviceInfo> GetDrives()
        {
            return GetDevices(DevInterfaceIds.GUID_DEVINTERFACE_DISK, DiGetClassFlags.DIGCF_PRESENT);
        }

        public static IEnumerable<DeviceInfo> GetDevices(Guid deviceClass, DiGetClassFlags flags = DiGetClassFlags.DIGCF_DEFAULT)
        {
            var enumerator = default(SetupApiDeviceEnumerator);
            if (!SetupApi_Enumerate(deviceClass, flags, ref enumerator))
                throw new Win32Exception();

            try
            {
                while (SetupApi_Next(ref enumerator))
                {
                    yield return new DeviceInfo
                    {
                        FriendlyName = ReadDeviceProperty(ref enumerator, DeviceRegistryCode.SPDRP_FRIENDLYNAME) as string,
                        Path = enumerator.current,
                    };
                }

                if (enumerator.wasError)
                    throw new Win32Exception();
            }
            finally
            {
                SetupApi_Done(ref enumerator);
            }
        }

        static object ReadDeviceProperty(ref SetupApiDeviceEnumerator enumerator, DeviceRegistryCode property)
        {
            uint requiredSize;
            RegistryDataType dataType;

            SetupApi_ReadProperty(ref enumerator, property, out dataType, null, 0, out requiredSize);

            var propertyBuffer = new byte[requiredSize];
            if (!SetupApi_ReadProperty(ref enumerator, property, out dataType, propertyBuffer, propertyBuffer.Length, out requiredSize))
            {
                var error = (WinError)Marshal.GetLastWin32Error();
                if (error == WinError.ERROR_INVALID_DATA)
                    return null;

                throw new Win32Exception(error);
            }

            return ParseRegistryValue(propertyBuffer, dataType);
        }

        [DllImport("MlkDiskWiperWinApiAdapter.dll", SetLastError = true)]
        static extern bool SetupApi_Enumerate(Guid deviceClass, DiGetClassFlags flags, ref SetupApiDeviceEnumerator enumerator);

        [DllImport("MlkDiskWiperWinApiAdapter.dll", SetLastError = true)]
        static extern bool SetupApi_Next(ref SetupApiDeviceEnumerator enumerator);

        [DllImport("MlkDiskWiperWinApiAdapter.dll", SetLastError = true)]
        static extern bool SetupApi_ReadProperty(
            ref SetupApiDeviceEnumerator enumerator,
            DeviceRegistryCode property,
            out RegistryDataType propertyRegDataType,
            byte[] propertyBuffer,
            int propertyBufferSize,
            out uint requiredSize);

        [DllImport("MlkDiskWiperWinApiAdapter.dll")]
        static extern void SetupApi_Done(ref SetupApiDeviceEnumerator enumerator);

        static object ParseRegistryValue(byte[] buffer, RegistryDataType type)
        {
            switch (type)
            {
                case RegistryDataType.REG_BINARY:
                    return buffer;

                case RegistryDataType.REG_DWORD:
                    return BitConverter.ToUInt32(buffer, 0);

                case RegistryDataType.REG_MULTI_SZ:
                    var str = Encoding.UTF8.GetString(buffer);
                    return str.Split(new[] { '\0' }, StringSplitOptions.RemoveEmptyEntries);

                case RegistryDataType.REG_NONE:
                case RegistryDataType.REG_SZ:
                default:
                    return Encoding.Unicode.GetString(buffer).TrimEnd('\0');
            }
        }
    }
}
