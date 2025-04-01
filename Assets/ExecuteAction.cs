using System.Diagnostics;
using System.Runtime.InteropServices;

namespace ParkManagerWPF.Assets
{
    public static class ExecuteAction
    {
        [DllImport("user32.dll")]
        private static extern void LockWorkStation();

        public static void Lock()
        {
            LockWorkStation();
        }

        public static void Shutdown()
        {
            Process.Start(new ProcessStartInfo("shutdown", "/s /t 0")
            {
                CreateNoWindow = true,
                UseShellExecute = false
            });
        }

        public static void Reboot()
        {
            Process.Start(new ProcessStartInfo("shutdown", "/r /t 0")
            {
                CreateNoWindow = true,
                UseShellExecute = false
            });
        }
    }
}
