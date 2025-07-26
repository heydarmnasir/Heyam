using System;
using System.Windows;
using System.Windows.Threading;

namespace Heyam
{   
    public partial class SplashScreen : Window
    {
        public SplashScreen()
        {
            InitializeComponent();
        }

        DispatcherTimer timer = new DispatcherTimer();
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            timer.Interval = TimeSpan.FromMilliseconds(4000);
            timer.Tick += new EventHandler(Timer_Tick);
            timer.Start();
        }
        private void Timer_Tick(object sender, EventArgs e)
        {
            timer.Stop();
            LoginWindow loginwindow = new LoginWindow();
            loginwindow.Show();
            this.Close();
        }
    }
}