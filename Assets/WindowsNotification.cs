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
        public static NotifyIcon make(int time, string title, string message)
        {
            NotifyIcon notify = new NotifyIcon();
            notify.Icon = System.Drawing.SystemIcons.Information;
            notify.Visible = true;
            notify.ShowBalloonTip(2000, "Gestion du parc", "Les informations ont bien été remontées", ToolTipIcon.Info);
            notify.Dispose();

            return notify;
        }
    }
}
