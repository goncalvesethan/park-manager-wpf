using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ParkManagerWPF.Assets
{
    public class WindowsNotification
    {
        public static NotifyIcon make(int time, string message, ToolTipIcon icon)
        {
            NotifyIcon notify = new NotifyIcon();
            notify.Icon = System.Drawing.SystemIcons.Information;
            notify.Visible = true;
            notify.ShowBalloonTip(time, "Gestion du parc", message, icon);
            notify.Dispose();

            return notify;
        }
    }
}
