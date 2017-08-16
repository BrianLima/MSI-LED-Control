using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace MSI_LED_Custom.Lib
{
    static class _DeviceManagerDLL
    {
        [DllImport("Lib\\DeviceManagerDLL.dll", CallingConvention = CallingConvention.StdCall)]
        public static extern void GetDeviceInfo(Guid Class_GUID, [MarshalAs(UnmanagedType.BStr)] string DisplayName, [MarshalAs(UnmanagedType.BStr)] string DEVPKEY, [MarshalAs(UnmanagedType.BStr)] out string str, bool bAudio = false, bool bLED = false, [MarshalAs(UnmanagedType.BStr)] string CheckID = "");

    }
}
