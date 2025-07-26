using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using FoxLearn.License;
using BE;
using BLL;
using System.Text.RegularExpressions;
using IPE.SmsIrClient;
using System.IO;
using System.Linq;

namespace Heyam
{    
    public partial class RegisterWindow : Window
    {
        private bool isMenuOpen = false;
        User_BLL ubll = new User_BLL();
        public RegisterWindow()
        {
            InitializeComponent();

            SystemCodeTB.Text = ComputerInfo.GetComputerId();
            LicenseKeyTB.Focus();
        }
        
        #region Methods
        public void OpenBlurWindow(Window window)
        {
            BlurEffect blureffect = new BlurEffect();
            this.Effect = blureffect;
            blureffect.Radius = 10;
            window.ShowDialog();
            Effect = null;
        }
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
       
        private void CheckLicenseBTN_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(LicenseKeyTB.Text))
            {
                CustomMessageBox window = new CustomMessageBox("نرم افزار هـــــیام", "!فیلد لایسنس خالی است", false, CustomMessageBox.MessageType.Error);
                OpenBlurWindow(window);
                return;
            }

            string productKey = LicenseKeyTB.Text;
            KeyManager km = new KeyManager(SystemCodeTB.Text);

            // ساخت مسیر امن برای ذخیره فایل
            var appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            var appFolder = Path.Combine(appDataPath, "Heyam");
            Directory.CreateDirectory(appFolder);

            var usedKeysFile = Path.Combine(appFolder, "UsedLicenseKeys.txt");

            try
            {
                // بررسی تکراری بودن لایسنس
                if (File.Exists(usedKeysFile))
                {
                    var usedKeys = File.ReadAllLines(usedKeysFile);
                    if (usedKeys.Contains(productKey))
                    {
                        CustomMessageBox window = new CustomMessageBox("نرم افزار هـــــیام", "این کلید قبلاً استفاده شده و دیگر معتبر نیست!", false, CustomMessageBox.MessageType.Error);
                        OpenBlurWindow(window);
                        return;
                    }
                }

                // بررسی صحت لایسنس
                if (km.ValidKey(ref productKey))
                {
                    KeyValuesClass kv = new KeyValuesClass();
                    if (km.DisassembleKey(productKey, ref kv))
                    {
                        LicenseInfo lic = new LicenseInfo
                        {
                            ProductKey = productKey,
                            FullName = "Personal accounting"
                        };

                        if (kv.Type == LicenseType.TRIAL)
                        {
                            lic.Day = kv.Expiration.Day;
                            lic.Month = kv.Expiration.Month;
                            lic.Year = kv.Expiration.Year;
                        }

                        // ذخیره فایل لایسنس در مسیر امن
                        var licensePath = Path.Combine(appFolder, "Key.lic");
                        km.SaveSuretyFile(licensePath, lic);

                        // ذخیره کلید در لیست استفاده‌شده‌ها
                        File.AppendAllText(usedKeysFile, productKey + Environment.NewLine);

                        // نمایش موفقیت
                        CustomMessageBox window = new CustomMessageBox("نرم افزار هـــــیام", "تبریک! نرم افزار با موفقیت فعال شد", false, CustomMessageBox.MessageType.Success);
                        OpenBlurWindow(window);
                        ActivatedProgramLabel.Visibility = Visibility.Visible;
                        RegisterPanel.IsEnabled = true;
                        LicensePanel.IsEnabled = false;
                        FullNameTB.Focus();
                    }
                }
                else
                {
                    CustomMessageBox window = new CustomMessageBox("نرم افزار هـــــیام", "!لایسنس وارد شده صحیح نمی باشد", false, CustomMessageBox.MessageType.Error);
                    OpenBlurWindow(window);
                    LicenseKeyTB.Focus();
                }
            }
            catch (Exception ex)
            {
                // جلوگیری از کرش و نمایش خطا
                CustomMessageBox window = new CustomMessageBox("خطای غیرمنتظره", "خطایی رخ داد: " + ex.Message, false, CustomMessageBox.MessageType.Error);
                OpenBlurWindow(window);
            }
        }
        private void RegisterBTN_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(FullNameTB.Text) || string.IsNullOrEmpty(UserNameTB.Text) || string.IsNullOrEmpty(PasswordPB.Password) || string.IsNullOrEmpty(RepeatPasswordPB.Password) || string.IsNullOrEmpty(PhoneNumberTB.Text) || string.IsNullOrEmpty(RoleCB.Text))
            {
                CustomMessageBox window = new CustomMessageBox("نرم افزار هـــــیام", "!همه اطلاعات را وارد کنید", false, CustomMessageBox.MessageType.Error);
                OpenBlurWindow(window);
            }
            else if (PasswordPB.Password.Trim().Length < 4)
            {
                CustomMessageBox window = new CustomMessageBox("نرم افزار هـــــیام", "!کلمه عبور حداقل باید 4 رقم باشد", false, CustomMessageBox.MessageType.Error);
                OpenBlurWindow(window);
            }
            else
            {               
                if (PasswordPB.Password == RepeatPasswordPB.Password)
                {                   
                    string fullName = FullNameTB.Text;
                    string username = UserNameTB.Text;
                    string password = PasswordPB.Password;
                    string phonenumber = PhoneNumberTB.Text;
                    string fullnamelawyerTB = FullNameLawyerTB.Text;                    
                    // گرفتن نقش انتخاب شده
                    ComboBoxItem selectedItem = (ComboBoxItem)RoleCB.SelectedItem;
                    string roleTag = selectedItem.Tag.ToString();
                    User.UserRole selectedRole = (User.UserRole)Enum.Parse(typeof(User.UserRole), roleTag);
                    if (RoleCB.SelectedIndex == 1 && FullNameLawyerTB.Text.Trim().Length == 0)
                    {
                        ShowNotification("نام و نام خانوادگی وکیل را وارد کنید!", "error");
                        return;
                    }
                    // ساخت کاربر جدید
                    User newUser = new User
                    {
                        FullName = fullName,
                        Username = username,
                        Password = password,
                        PhoneNumber = phonenumber,
                        Role = selectedRole,
                        UserTypeRole = RoleCB.SelectedIndex,
                        FullNameLawyer = fullnamelawyerTB,
                        RegDate = DateTime.Now
                    };                 
                    if (!ubll.Exist(newUser))
                    {
                        //user.Picture = SavePicture();                 
                        CustomMessageBox window = new CustomMessageBox("نرم افزار هـــــیام", ubll.Create(newUser), false, CustomMessageBox.MessageType.Success);
                        OpenBlurWindow(window);
                        // فرض: تعیین کاربر جاری
                        //AppSession.CurrentUser = newUser;
                        this.Close();

                        SmsIr smsIr = new SmsIr("XwrI6iBRvxaWJ4AVcqaQddyRF5Ux3iHesb7XuQGbcSVag4Ut");
                        var bulkSendResult = smsIr.BulkSendAsync(30007732011420, $"آقای/خانم {fullName}\nثبت نام شما در نرم افزار حقوقی هیام را تبریک عرض میکنم\nجهت راهنمایی و یا ارسال درخواست، به شماره زیر:\n+989023349043 پیغام بدهید \n سربلند و پیروز باشید", new string[] { phonenumber });
                        //var verificationSendResult = smsIr.VerifySendAsync(phonenumber, 100000, new VerifySendParameter[] { new VerifySendParameter("Code", "12345") });
                    }
                    else
                    {
                        CustomMessageBox window = new CustomMessageBox("نرم افزار هـــــیام", "نام کاربری در سیستم موجود میباشد!", false, CustomMessageBox.MessageType.Error);
                        OpenBlurWindow(window);
                    }    
                }
                else
                {
                    CustomMessageBox window = new CustomMessageBox("نرم افزار هـــــیام", "!کلمه عبور و تکرار آن با یکدیگر همخوانی ندارند", false, CustomMessageBox.MessageType.Error);
                    OpenBlurWindow(window);
                }
            }
        }        
        private void CBRole_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (RoleCB.SelectedIndex == 0)
            {
                FullNameLawyertxtblock.Visibility = Visibility.Hidden;
                FullNameLawyerTB.Visibility = Visibility.Hidden;
                double targetHeight = isMenuOpen ? 0 : 0; // ارتفاع لیست (مثلاً 100 پیکسل)
                DoubleAnimation animation = new DoubleAnimation
                {
                    To = targetHeight,
                    Duration = TimeSpan.FromMilliseconds(300),
                    EasingFunction = new QuadraticEase() // انیمیشن نرم‌تر               
                };
                DropdownMenu.BeginAnimation(FrameworkElement.HeightProperty, animation);              
            }
            else if (RoleCB.SelectedIndex == 1)
            {
                FullNameLawyertxtblock.Visibility = Visibility.Visible;
                FullNameLawyerTB.Visibility = Visibility.Visible;
                double targetHeight = isMenuOpen ? 0 : 60; // ارتفاع لیست (مثلاً 100 پیکسل)
                DoubleAnimation animation = new DoubleAnimation
                {
                    To = targetHeight,
                    Duration = TimeSpan.FromMilliseconds(300),
                    EasingFunction = new QuadraticEase() // انیمیشن نرم‌تر               
                };
                DropdownMenu.BeginAnimation(FrameworkElement.HeightProperty, animation);             
            }
        }               
        private void UserNameTB_TextChanged(object sender, TextChangedEventArgs e)
        {
            var textBox = sender as TextBox;

            // بررسی اینکه فقط حروف انگلیسی باشند
            if (!Regex.IsMatch(textBox.Text, @"^[a-zA-Z]*$"))
            {
                textBox.Tag = "Invalid"; // فعال کردن استایل خط قرمز
                UsernameErrorTextBlock.Visibility = Visibility.Visible;
            }
            else
            {
                textBox.Tag = null; // استایل خط قرمز حذف
                UsernameErrorTextBlock.Visibility = Visibility.Collapsed;
            }
        }
        private void PasswordPB_PasswordChanged(object sender, RoutedEventArgs e)
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
        private void RepeatPasswordPB_PasswordChanged(object sender, RoutedEventArgs e)
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
        private void BackToLoginBTN_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}