using System.Windows;

namespace MetroFtpClient.Behaviors
{
    public class SplashScreenBehavior
    {
        #region Dependency Properties

        public static readonly DependencyProperty EnabledProperty = DependencyProperty.RegisterAttached(
          "Enabled", typeof(bool), typeof(SplashScreenBehavior), new PropertyMetadata(OnEnabledChanged));

        public static bool GetEnabled(DependencyObject obj)
        {
            return (bool)obj.GetValue(EnabledProperty);
        }

        public static void SetEnabled(DependencyObject obj, bool value)
        {
            obj.SetValue(EnabledProperty, value);
        }

        #endregion Dependency Properties

        #region Event Handlers

        private static void OnEnabledChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            var splash = obj as Window;
            if (splash != null && args.NewValue is bool && (bool)args.NewValue)
            {
                splash.Closed += (s, e) =>
                {
                    splash.DataContext = null;
                    splash.Dispatcher.InvokeShutdown();
                };
                splash.MouseDoubleClick += (s, e) => splash.Close();
                splash.MouseLeftButtonDown += (s, e) => splash.DragMove();
            }
        }

        #endregion Event Handlers
    }
}