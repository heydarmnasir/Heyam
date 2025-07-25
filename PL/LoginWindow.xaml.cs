using BE;
using BLL;
using PL;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Threading;

namespace Heyam
{   
    public partial class LoginWindow : Window
    {                
        public LoginWindow()
        {
            InitializeComponent();         
        }
        DispatcherTimer fadeOutTimer = new DispatcherTimer();
        DispatcherTimer fadeInTimer = new DispatcherTimer();
        MainWindow w;
        public User user = new User();
        User_BLL ubll = new User_BLL();
        private int? _foundUserId = null;

        #region Methods
        public void ShowNotification(string message, string type)
        {
            switch (type.ToLower())
            {
                case "info":
                    HandyControl.Controls.Growl.Info(new HandyControl.Data.GrowlInfo
                    {
                        Message = message,
                        ShowPersianDateTime = true,
                        WaitTime = 8, // مدت زمان نمایش (ثانیه)
                        StaysOpen = false,
                    });
                    break;

                case "success":
                    HandyControl.Controls.Growl.Success(new HandyControl.Data.GrowlInfo
                    {
                        Message = message,
                        ShowPersianDateTime = true,
                        WaitTime = 8,
                        StaysOpen = false,
                    });
                    break;

                case "warning":
                    HandyControl.Controls.Growl.Warning(new HandyControl.Data.GrowlInfo
                    {
                        Message = message,
                        ShowPersianDateTime = true,
                        WaitTime = 8,
                        StaysOpen = false,
                    });
                    break;

                case "error":
                    HandyControl.Controls.Growl.Error(new HandyControl.Data.GrowlInfo
                    {
                        Message = message,
                        ShowPersianDateTime = true,
                        WaitTime = 8,
                        StaysOpen = false,
                    });
                    break;

                default:
                    HandyControl.Controls.Growl.Info(new HandyControl.Data.GrowlInfo
                    {
                        Message = "نوع پیام مشخص نیست!",
                        ShowPersianDateTime = true,
                        WaitTime = 8,
                        StaysOpen = false,
                    });
                    break;
            }
        }
        #endregion

        private void RegisterText_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            RegisterWindow window = new RegisterWindow();
            window.ShowDialog();
        }
        private void RegisterText_MouseEnter(object sender, MouseEventArgs e)
        {
            RegisterText.Foreground = new SolidColorBrush(Colors.GreenYellow);
        }
        private void RegisterText_MouseLeave(object sender, MouseEventArgs e)
        {
            RegisterText.Foreground = new SolidColorBrush(Colors.Yellow);
        }
        private void ForgotPasswordText_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            ForgotPasswordPopup.IsOpen = true;
        }
        private void ForgotPasswordText_MouseEnter(object sender, MouseEventArgs e)
        {
            ForgotPasswordText.Foreground = new SolidColorBrush(Colors.Crimson);
        }
        private void ForgotPasswordText_MouseLeave(object sender, MouseEventArgs e)
        {
            ForgotPasswordText.Foreground = new SolidColorBrush(Colors.Yellow);
        }           
        private void CancelBTN_Click(object sender, RoutedEventArgs e)
        {
            ForgotPasswordPopup.IsOpen = false;         
            Usernametxt.Clear();
            ForgotPasswordPB.Clear();
            ForgotRepeatPasswordPB.Clear();
            Usernametxt.IsEnabled = true;
            CheckUsernameBTN.IsEnabled = true;
            ErrorInvalidUsernameText.Text = "نام کاربری موجود نمیباشد!";
            ErrorInvalidUsernameText.Foreground = new SolidColorBrush(Colors.Crimson);
            ForgotPasswordPB.IsEnabled = false;
            ForgotRepeatPasswordPB.IsEnabled = false;
            ErrorInvalidUsernameText.Visibility = Visibility.Collapsed;
            SubmitInfoBTN.IsEnabled = false;
        }             
        private void ForgotPasswordPB_PasswordChanged(object sender, RoutedEventArgs e)
        {
            var password = sender as PasswordBox;

            // بررسی اینکه فقط حروف انگلیسی باشند
            if (!Regex.IsMatch(password.Password, @"^[a-zA-Z0-Z9]*$"))
            {
                password.Tag = "Invalid"; // فعال کردن استایل خط قرمز
                PasswordErrorTextBlock.Visibility = Visibility.Visible;
            }
            else
            {
                password.Tag = null; // استایل خط قرمز حذف
                PasswordErrorTextBlock.Visibility = Visibility.Collapsed;
            }            
        }
        private void ForgotRepeatPasswordPB_PasswordChanged(object sender, RoutedEventArgs e)
        {
            var password = sender as PasswordBox;

            // بررسی اینکه فقط حروف انگلیسی باشند
            if (!Regex.IsMatch(password.Password, @"^[a-zA-Z0-Z9]*$"))
            {
                password.Tag = "Invalid"; // فعال کردن استایل خط قرمز
                RepeatPasswordErrorTextBlock.Visibility = Visibility.Visible;
            }
            else
            {
                password.Tag = null; // استایل خط قرمز حذف
                RepeatPasswordErrorTextBlock.Visibility = Visibility.Collapsed;
            }
        }
        private async void LoginBTN_Click(object sender, RoutedEventArgs e)
        {
            user = ubll.Login(UsernameTB.Text, PasswordPB.Password);
            if (user != null)
            {
                AppSession.CurrentUser = user;

                LoadingWindow loading = new LoadingWindow();
                loading.Show();
                MainGrid.Effect = new BlurEffect() { Radius = 5 };
                await Task.Delay(300); // اجازه بده پنجره لودینگ نمایش پیدا کنه

                await Task.Run(() =>
                {
                    // عملیات سنگین مثل بررسی لاگین
                    System.Threading.Thread.Sleep(2000); // فقط شبیه‌سازیه
                });

                loading.Close();
                MainGrid.Effect = null;
                w = new MainWindow();
                w.LoginInUser = user;
                w.RefreshPage();
                w.Show();
                this.Close();
            }
            else
            {
                ShowNotification("نام کاربری یا رمز عبور اشتباه است!", "error");
                ErrorText.Visibility = Visibility.Visible;
            }
        }
        private void CheckUsernameBTN_Click(object sender, RoutedEventArgs e)
        {
            string username = Usernametxt.Text.Trim();
            _foundUserId = ubll.CheckUserExists(username);

            if (_foundUserId != null)
            {
                Usernametxt.IsEnabled = false;
                CheckUsernameBTN.IsEnabled = false;
                ErrorInvalidUsernameText.Text = "نام کاربری یافته شد. لطفا رمز جدید را وارد کنید";
                ErrorInvalidUsernameText.Foreground = new SolidColorBrush(Colors.ForestGreen);
                ForgotPasswordPB.IsEnabled = true;
                ForgotRepeatPasswordPB.IsEnabled = true;
                ErrorInvalidUsernameText.Visibility = Visibility.Visible;
                SubmitInfoBTN.IsEnabled = true;
            }
            else
            {
                ErrorInvalidUsernameText.Visibility = Visibility.Visible;
            }
        }
        private void SubmitInfoBTN_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(ForgotPasswordPB.Password) || string.IsNullOrEmpty(ForgotRepeatPasswordPB.Password))
            {
                CustomMessageBox window = new CustomMessageBox("نرم افزار هـــــیام", "!همه اطلاعات را وارد کنید", false, CustomMessageBox.MessageType.Error);
                window.Owner = this; // تنظیم مالکیت پنجره مسیج باکس
                window.Topmost = true; // همیشه بالاتر از سایر پنجره‌ها
                window.ShowDialog();
            }
            else if (ForgotPasswordPB.Password.Trim().Length < 4)
            {
                CustomMessageBox window = new CustomMessageBox("نرم افزار هـــــیام", "!کلمه عبور حداقل باید 4 رقم باشد", false, CustomMessageBox.MessageType.Error);
                window.Owner = this; // تنظیم مالکیت پنجره مسیج باکس
                window.Topmost = true; // همیشه بالاتر از سایر پنجره‌ها
                window.ShowDialog();
            }
            else
            {
                if (ForgotPasswordPB.Password == ForgotRepeatPasswordPB.Password)
                {
                    string newPassword = ForgotPasswordPB.Password.Trim();
                    bool result = ubll.ChangeUserPassword(_foundUserId.Value, newPassword);

                    if (result)
                    {
                        CustomMessageBox window = new CustomMessageBox("نرم افزار هـــیام", "رمز عبور با موفقیت تغییر کرد", false, CustomMessageBox.MessageType.Success);
                        window.Owner = this; // تنظیم مالکیت پنجره مسیج باکس
                        window.Topmost = true; // همیشه بالاتر از سایر پنجره‌ها
                        window.ShowDialog();
                        Usernametxt.Text = "";
                        Usernametxt.IsEnabled = true;
                        CheckUsernameBTN.IsEnabled = true;
                        ErrorInvalidUsernameText.Visibility = Visibility.Collapsed;
                        ForgotPasswordPB.Password = "";
                        ForgotPasswordPB.IsEnabled = false;
                        ForgotRepeatPasswordPB.Password = "";
                        ForgotRepeatPasswordPB.IsEnabled = false;
                        SubmitInfoBTN.IsEnabled = false;
                    }
                    else
                    {
                        CustomMessageBox window = new CustomMessageBox("نرم افزار هـــیام", "خطا در تغییر رمز عبور!", false, CustomMessageBox.MessageType.Error);
                        window.Owner = this; // تنظیم مالکیت پنجره مسیج باکس
                        window.Topmost = true; // همیشه بالاتر از سایر پنجره‌ها
                        window.ShowDialog();
                    }
                }
                else
                {
                    CustomMessageBox window = new CustomMessageBox("نرم افزار هـــــیام", "!کلمه عبور و تکرار آن با یکدیگر همخوانی ندارند", false, CustomMessageBox.MessageType.Error);
                    window.Owner = this; // تنظیم مالکیت پنجره مسیج باکس
                    window.Topmost = true; // همیشه بالاتر از سایر پنجره‌ها
                    window.ShowDialog();
                }
            }
        }
    }
}