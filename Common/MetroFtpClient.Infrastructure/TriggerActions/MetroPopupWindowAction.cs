using MahApps.Metro.Controls;
using Prism.Interactivity;
using System.Windows;

namespace MetroFtpClient.Infrastructure.TriggerActions
{
    public class MetroPopupWindowAction : PopupWindowAction
    {
        protected override Window CreateWindow()
        {
            return new MetroWindow();
        }
    }
}
