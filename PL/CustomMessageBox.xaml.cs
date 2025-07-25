using System;
using System.Windows;
using System.Windows.Media.Animation;

namespace Heyam
{    
    public partial class CustomMessageBox : Window
    {
        public enum MessageType
        {
            Info,
            Warning,
            Success,
            Error
        }

        public bool Result { get; private set; } = false;
        public CustomMessageBox(string title, string message,bool Button, MessageType type)
        {
            InitializeComponent();
            MessageTitle.Text = title;
            MessageText.Text = message;

            // تنظیم ظاهر بر اساس نوع پیام
            switch (type)
            {
                case MessageType.Info:
                    MessageIcon.Text = "ℹ";
                    MessageIcon.Foreground = System.Windows.Media.Brushes.Blue;
                    break;
                case MessageType.Warning:
                    MessageIcon.Text = "⚠";
                    MessageIcon.Foreground = System.Windows.Media.Brushes.Orange;
                    break;
                case MessageType.Success:
                    MessageIcon.Text = "✅";
                    MessageIcon.Foreground = System.Windows.Media.Brushes.Green;
                    break;
                case MessageType.Error:
                    MessageIcon.Text = "⛔";
                    MessageIcon.Foreground = System.Windows.Media.Brushes.Red;
                    break;
            }
            if (Button)
            {
                BtnOk.Content = "خیر";
                BtnYes.Visibility = Visibility.Visible;
            }
            else
            {
                BtnYes.Visibility= Visibility.Collapsed;
                BtnOk.Content = "خروج";
            } 
        }
       
        private void BtnYes_Click(object sender, RoutedEventArgs e)
        {
            Result = true; // تأیید کاربر
            CloseWithAnimation();
            //this.Close();
        }   
        private void BtnOk_Click(object sender, RoutedEventArgs e)
        {
            if (BtnOk.Content.ToString() == "خروج")
            {
                Result = true; // لغو کاربر
                CloseWithAnimation();
                //this.Close();
            }
            else
            {
                Result = false; // لغو کاربر
                CloseWithAnimation();
                this.Close();
            }        
        }
        private void CloseWithAnimation()
        {
            // ایجاد انیمیشن برای شفافیت
            var animation = new DoubleAnimation(1, 0, new Duration(TimeSpan.FromSeconds(0.2)));
            animation.Completed += (s, e) => this.Close(); // بستن پنجره بعد از پایان انیمیشن
            this.BeginAnimation(OpacityProperty, animation);
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // انیمیشن ورود به پنجره
            this.BeginAnimation(OpacityProperty, new DoubleAnimation(0, 1, new Duration(TimeSpan.FromSeconds(0.1))));
        }
    }
}