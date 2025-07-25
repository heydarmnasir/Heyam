using BE;
using BLL;
using PL;
using System;
using System.Data.SqlClient;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;

namespace Heyam
{    
    public partial class Settings : Window
    {
        private bool isDarkMode = false;
        private bool isMenuOpen = false;
    
        public Settings()
        {
            InitializeComponent();
            Refresh();
            var user = AppSession.CurrentUser;
            if (user.Role == BE.User.UserRole.Secretary)
            {
                UserManagementItem.IsEnabled = false;
                BackupRestoreItem.IsEnabled = false;
            }
            else if (user.Role == BE.User.UserRole.Lawyer)
            {
                UserManagementItem.IsEnabled = true;
                BackupRestoreItem.IsEnabled = true;
            }           
            // بررسی تم ذخیره‌شده و تنظیم مقدار چک‌باکس
            if (Heyam.Properties.Settings.Default.IsDarkMode)
            {
                DarkModeCheckBox.IsChecked = true;
                ApplyDarkMode();
            }
            else
            {
                DarkModeCheckBox.IsChecked = false;
                ApplyLightMode();
            }
            SettingsMenu.SelectedIndex = 0; // انتخاب پیش‌فرض
            AlertsCheckBox.IsChecked = Properties.Settings.Default.EnableAlerts;    
        }
        MainWindow mainwindow = new MainWindow();
        User_BLL ubll = new User_BLL();

        #region Methods
        public void OpenBlurWindow(Window window)
        {
            BlurEffect blureffect = new BlurEffect();
            this.Effect = blureffect;
            blureffect.Radius = 10;
            window.ShowDialog();
            this.Effect = null;
        }
        private void ClearControls(DependencyObject parent)
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);

                if (child is System.Windows.Controls.TextBox textBox)
                    textBox.Clear();
                else if (child is System.Windows.Controls.ComboBox comboBox)
                    comboBox.SelectedIndex = 0;
                else if (child is System.Windows.Controls.DatePicker datePicker)
                    datePicker.SelectedDate = null;
                else if (child is System.Windows.Controls.CheckBox checkbox)
                    checkbox.IsChecked = false;

                ClearControls(child); // فراخوانی بازگشتی
            }
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
        public void Refresh()
        {
            UsersDGV.ItemsSource = null;
            UsersDGV.ItemsSource = ubll.ReadAllForListView();
        }
        #endregion

        #region General
        private void SettingsMenu_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (SettingsMenu.SelectedItem is ListBoxItem selectedItem)
            {
                string selectedTag = selectedItem.Tag.ToString();
                // مخفی‌سازی تمام پنل‌ها
                GeneralSettings.Visibility = Visibility.Collapsed;
                UsersSettings.Visibility = Visibility.Collapsed;
                BackupSettings.Visibility = Visibility.Collapsed;
                AboutUsSettings.Visibility = Visibility.Collapsed;
                // نمایش محتوای مناسب
                switch (selectedTag)
                {
                    case "General":
                        GeneralSettings.Visibility = Visibility.Visible;
                        break;
                    case "UsersManagement":
                        UsersSettings.Visibility = Visibility.Visible;
                        break;
                    case "Backup":
                        BackupSettings.Visibility = Visibility.Visible;
                        break;
                    case "AboutUs":
                        AboutUsSettings.Visibility = Visibility.Visible;
                        break;
                }
            }
        }
        private void ApplyDarkMode()
        {
            ApplyTheme("Resources/Themes/DarkTheme.xaml");
        }
        private void ApplyLightMode()
        {
            ApplyTheme("Resources/Themes/LightTheme.xaml");
        }
        private void ChangeTheme_Click(object sender, RoutedEventArgs e)
        {
            isDarkMode = !isDarkMode;
            ApplyTheme(isDarkMode ? "Resources/Themes/LightTheme.xaml" : "Resources/Themes/DarkTheme.xaml");
        }
        private void ApplyTheme(string themePath)
        {
            ResourceDictionary newTheme = new ResourceDictionary { Source = new Uri(themePath, UriKind.Relative) };
            // حذف تم قبلی و افزودن تم جدید          
            System.Windows.Application.Current.Resources.MergedDictionaries.Add(newTheme);
            // ذخیره وضعیت در تنظیمات برنامه (Registry یا فایل تنظیمات)
            Heyam.Properties.Settings.Default.IsDarkMode = isDarkMode;
            Heyam.Properties.Settings.Default.Save();
        }
        private void LoadTheme()
        {
            isDarkMode = Heyam.Properties.Settings.Default.IsDarkMode;
            ApplyTheme(isDarkMode ? "Resources/Themes/LightTheme.xaml" : "Resources/Themes/DarkTheme.xaml");
        }
        private void AlertsCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.EnableAlerts = true;
            Properties.Settings.Default.Save();
        }
        private void AlertsCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.EnableAlerts = false;
            Properties.Settings.Default.Save();
        }      
        private void LanguageSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (LanguageSelector.SelectedItem is ComboBoxItem selectedItem)
            {
                string selectedLanguage = selectedItem.Tag.ToString();
                (System.Windows.Application.Current as App)?.ChangeLanguage(selectedLanguage);
            }
        }
        private void DarkModeCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            ApplyDarkMode();
            Heyam.Properties.Settings.Default.IsDarkMode = true;
            Heyam.Properties.Settings.Default.Save();
            mainwindow.Calender.Background = new SolidColorBrush(Colors.Black);
            mainwindow.Calender.Foreground = new SolidColorBrush(Colors.White);
        }
        private void DarkModeCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            ApplyLightMode();
            Heyam.Properties.Settings.Default.IsDarkMode = false;
            Heyam.Properties.Settings.Default.Save();
            mainwindow.Calender.Background = new SolidColorBrush(Colors.White);
            mainwindow.Calender.Foreground = new SolidColorBrush(Colors.Black);
        }
        #endregion

        #region Backup Database
        private string backupPath = @"C:\Backup"; // مسیر پیش‌فرض        
        private void SelectPathDatabaseBTN_Click(object sender, RoutedEventArgs e)
        {
            using (var folderDialog = new FolderBrowserDialog())
            {
                if (folderDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    backupPath = folderDialog.SelectedPath;
                    BackupPathText.Text = $"مسیر ذخیره‌سازی: {backupPath}";
                    BackupDatabaseBTN.IsEnabled = true;
                }
            }
        }
        private string connectionString = @"Data Source=.\SQLEXPRESS;Database=Heyam_DB;Trusted_Connection=True;"; // رشته اتصال پایگاه داده
        public void BackupDatabase(string backupPath)
        {            
            try
            {
                string fileName = $"Heyam_DB_Backup_{DateTime.Now:yyyyMMddHHmmss}.bak";
                string fullPath = System.IO.Path.Combine(backupPath, fileName);

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    string query = $"BACKUP DATABASE Heyam_DB TO DISK = '{fullPath}'";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        conn.Open();
                        cmd.ExecuteNonQuery();
                    }
                    ShowNotification("پشتیبان‌گیری با موفقیت انجام شد", "success");
                }
            }
            catch (Exception ex)
            {
                CustomMessageBox msgbox = new CustomMessageBox("نرم افزار هـــــیام", $"خطا در پشتیبان‌گیری: {ex.Message}", false, CustomMessageBox.MessageType.Error);
                mainwindow.OpenWindowMethod(msgbox);
            }
        }        
        private void BackupDatabaseBTN_Click(object sender, RoutedEventArgs e)
        {
            BackupDatabase(backupPath); // مسیر ذخیره‌سازی نسخه پشتیبان
        }      
        #endregion          

        #region UserManagement
        private void AddUserBTN_Click(object sender, RoutedEventArgs e)
        {
            AddUserPopup.Visibility = Visibility.Visible;
            SettingsGrid.Effect = new BlurEffect() { Radius = 5 };
        }     
        private void UserNameTB_TextChanged(object sender, TextChangedEventArgs e)
        {
            var textBox = sender as System.Windows.Controls.TextBox;

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
        private void RoleCB_SelectionChanged(object sender, SelectionChangedEventArgs e)
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
                double targetHeight = isMenuOpen ? 0 : 75; // ارتفاع لیست (مثلاً 100 پیکسل)
                DoubleAnimation animation = new DoubleAnimation
                {
                    To = targetHeight,
                    Duration = TimeSpan.FromMilliseconds(300),
                    EasingFunction = new QuadraticEase() // انیمیشن نرم‌تر               
                };
                DropdownMenu.BeginAnimation(FrameworkElement.HeightProperty, animation);
            }
        }     
        private void SubmitUser_Click(object sender, RoutedEventArgs e)
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
                        ShowNotification("نام و نام خانوادگی وکیل را وارد کنید!","error");
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
        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            AddUserPopup.Visibility = Visibility.Collapsed;
            SettingsGrid.Effect = null;
        }
        #endregion
        private void CloseBTN_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }       
    }
}