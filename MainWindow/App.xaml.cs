using System;
using System.Windows;

namespace MainWindow
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            MusicPlayer.Instance.Play();
        }
    }
}
