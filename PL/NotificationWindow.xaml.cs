using System;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace Heyam
{
    public partial class NotificationWindow : Window
    {
        public enum NotificationType
        {
            Info,
            Success,
            Warning,
            Error
        }
        public static void RearrangeNotifications()
        {
            var notifications = Application.Current.Windows
                .OfType<NotificationWindow>()
                .OrderBy(w => w.Top) // مهم: از بالا به پایین مرتب
                .ToList();

            var desktopWorkingArea = SystemParameters.WorkArea;
            for (int i = 0; i < notifications.Count; i++)
            {
                double targetTop = desktopWorkingArea.Bottom - notifications[i].Height - 10 - i * (notifications[i].Height + 10);

                // انیمیشن جابه‌جایی نوتیف به موقعیت جدید
                var anim = new DoubleAnimation
                {
                    To = targetTop,
                    Duration = TimeSpan.FromMilliseconds(200)
                };
                notifications[i].BeginAnimation(Window.TopProperty, anim);
            }
        }
        private DispatcherTimer _timer;
        private string _description;
        public NotificationWindow(string title, string message, string description, NotificationType type, int durationSeconds = 3)
        {
            InitializeComponent();
            txtTitle.Text = title;
            txtMessage.Text = message;
            _description = description;
            SetStyle(type);

            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromSeconds(durationSeconds);
            _timer.Tick += (s, e) =>
            {
                //_timer.Stop();
                CloseWithAnimation();
            };
        }
        private void SetStyle(NotificationType type)
        {
            switch (type)
            {
                case NotificationType.Info:
                    BorderRoot.Background = new SolidColorBrush(Color.FromRgb(41, 128, 185)); // آبی
                    IconImage.Source = new BitmapImage(new Uri("pack://application:,,,/Resources/Images/info-blue.png"));
                    break;
                case NotificationType.Success:
                    BorderRoot.Background = new SolidColorBrush(Color.FromRgb(39, 174, 96)); // سبز
                    IconImage.Source = new BitmapImage(new Uri("pack://application:,,,/Resources/Images/success.png"));
                    break;
                case NotificationType.Warning:
                    BorderRoot.Background = new SolidColorBrush(Color.FromRgb(243, 156, 18)); // نارنجی
                    IconImage.Source = new BitmapImage(new Uri("pack://application:,,,/Resources/Images/warning1.png"));
                    break;
                case NotificationType.Error:
                    BorderRoot.Background = new SolidColorBrush(Color.FromRgb(192, 57, 43)); // قرمز
                    IconImage.Source = new BitmapImage(new Uri("pack://application:,,,/Resources/Images/error1.png"));
                    break;
            }
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // نمایش پایین سمت راست با offset بر اساس تعداد پنجره باز
            var openNotifications = Application.Current.Windows.OfType<NotificationWindow>().Count();
            var desktopWorkingArea = SystemParameters.WorkArea;
            Left = desktopWorkingArea.Right - Width - 10;
            Top = desktopWorkingArea.Bottom - Height - 10 - (openNotifications - 1) * (Height + 10);

            // انیمیشن ظاهر شدن
            var sb = new Storyboard();
            var fadeIn = new DoubleAnimation(0, 1, TimeSpan.FromSeconds(0.3));
            Storyboard.SetTargetProperty(fadeIn, new PropertyPath("Opacity"));
            sb.Children.Add(fadeIn);
            sb.Begin(this);

            //_timer.Start();
        }
        private void CloseWithAnimation()
        {
            var sb = new Storyboard();
            var fadeOut = new DoubleAnimation(1, 0, TimeSpan.FromSeconds(0.3));
            Storyboard.SetTargetProperty(fadeOut, new PropertyPath("Opacity"));
            fadeOut.Completed += (s, e) => this.Close();
            sb.Children.Add(fadeOut);
            sb.Begin(this);
            RearrangeNotifications(); // جابه‌جایی نوتیف‌های باقی‌مانده

        }
        private void ShowDescriptionBTN_Click(object sender, RoutedEventArgs e)
        {
            CustomMessageBox msgbox = new CustomMessageBox("توضیحات یادآور", _description ,false,CustomMessageBox.MessageType.Info);
            msgbox.ShowDialog();    
        }
        private void Close_Click(object sender, RoutedEventArgs e)
        {
            //_timer.Stop();
            CloseWithAnimation();
        }
    }
}