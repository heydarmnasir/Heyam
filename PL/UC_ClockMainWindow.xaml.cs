using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace PL
{  
    public partial class UC_ClockMainWindow : UserControl
    {
        public UC_ClockMainWindow()
        {
            InitializeComponent();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            ClockText.Text = DateTime.Now.ToString("HH:mm:ss");
        }
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(500);
            timer.Tick += new EventHandler(Timer_Tick);
            timer.Start();
        }
    }
}