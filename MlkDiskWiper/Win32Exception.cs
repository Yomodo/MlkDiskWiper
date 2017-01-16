using System;
using System.Runtime.InteropServices;
using WinApiTypes;

namespace MlkDiskWiper
{
    public class Win32Exception : Exception
    {
        public WinError WindowsError { get; private set; }

        public Win32Exception()
            : this((WinError)Marshal.GetLastWin32Error())
        {
        }

        public Win32Exception(WinError error)
            : base(GetMessage(error))
        {
            WindowsError = error;
        }

        static string GetMessage(WinError error)
        {
            var builtinException = new System.ComponentModel.Win32Exception((int)error);
            return $"{error}: {builtinException.Message}";
        }
    }
}
