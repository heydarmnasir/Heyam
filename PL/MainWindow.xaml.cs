using BE;
using BLL;
using Heyam;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Media;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Threading;
using static Heyam.NotificationWindow;

namespace PL
{   
    public partial class MainWindow : Window
    {          
        private bool isDarkMode = false;

        private NotifyIcon notifyIcon;
        private DispatcherTimer _timer;
        private List<string> _messages; // لیستی از متون
        private int _currentIndex;

        public User LoginInUser = new User();
        Interaction_BLL interaction_bll = new Interaction_BLL();
        Correspondence_BLL correspondence_bll = new Correspondence_BLL();
        LegalBrief_BLL legalBrief_bll = new LegalBrief_BLL();   
        Payment_BLL pbll = new Payment_BLL();
        Meeting_BLL mbll = new Meeting_BLL();
        PersonalNote_BLL pnbll = new PersonalNote_BLL();
        Reminder_BLL rbll = new Reminder_BLL();
        Dashboard_BLL Dashboard_BLL = new Dashboard_BLL();

        public MainWindow()
        {
            InitializeComponent();
            ShowReminders();
            RefreshPage();
                   
            // بررسی تم ذخیره‌شده و تنظیم مقدار چک‌باکس
            if (Heyam.Properties.Settings.Default.IsDarkMode)
            {
                ThemeToggle.IsChecked = true;
                ApplyDarkMode();
            }
            else
            {
                ThemeToggle.IsChecked = false;
                ApplyLightMode();
            }

            // تعریف لیست پیام‌ها
            _messages = new List<string>
            {
                "هر پرونده ای که یک وکیل برمیدارد\n!گامی است به سوی عادلانه تر کردن جهان",
                "برای هر شخصی عدالت\r\n!و برای عدالت نیز وکیل لازم است",
                "وکیل همان کسی است که در میان هزارتوی قانون\r\nراه عدالت را به روشنی می‌ یابد و به سوی آن هدایت می‌ کند",
                "وکیل مانند پلی است که بین قانون و عدالت کشیده می‌ شود\r\nتا حقوق انسان‌ ها را به هم پیوند دهد",
                "وکالت یعنی تبدیل قوانین سخت و خشک\r\nبه ابزارهایی برای دفاع از حقوق و حقیقت",
                "در زندگی یک وکیل هر روز فرصتی است برای تغییر سرنوشت افراد\r\nاو با دانش و شهامت خود، عدالت را به دادگاه‌ ها می‌ آورد",
                "وکیل همچون نویسنده‌ ای است که با قلم قانون سرنوشت پرونده‌ ها را می‌ نویسد\r\nاو با استدلال‌ هایش، قاضی را به سوی تصمیم عادلانه هدایت می‌ کند",
                "در وکالت هر پرونده داستانی است که وکیل باید با دقت و هوشمندی آن را روایت کند\r\nاو صدای کسانی است که نیاز به دفاع دارند",
                "وکالت سفر به دنیای پیچیده قوانین است جایی که وکیل با هر قدم به حقیقت نزدیک‌ تر می‌ شود\r\nاین شغل تعهدی است به دفاع از عدالت و حمایت از حقوق انسان‌ ها",
                "وکیل مانند فرمانده‌ ای است که با دانش و توانایی خود\r\nدر میدان نبرد قانونی برای عدالت می‌ جنگد",
                "وکیل کسی است که با تکیه بر قوانین\r\nاز حقوق کسانی که تحت ظلم قرار گرفته‌ اند دفاع می‌ کند",
                "وکالت یعنی قدرت استفاده از کلمات برای دفاع از عدالت\r\nو حقوق کسانی که به آن نیاز دارند",
                "وکیل صدای کسانی است که در پیچیدگی‌ های قانون گم شده‌ اند\r\nو نیاز به راهنمایی دارند",
                "یک وکیل نه تنها مدافع قانون\r\nبلکه نگهبان عدالت در دنیایی است که اغلب توسط پیچیدگی‌ ها کور می‌ شود",
                "وکالت شغلی است که در آن وکیل با صداقت و شجاعت خود سعی می کند\r\nحقیقت را از میان پیچ و خم‌ های قانون بیرون بکشد",
                "در دستان یک وکیل ماهر کلمات به ابزارهای قدرتمندی تبدیل می‌ شوند\n که می‌ توانند مسیر عدالت را شکل دهند"
            };
            _currentIndex = 0;

            // تنظیم تایمر
            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromSeconds(6); // تغییر متن هر 6 ثانیه
            _timer.Tick += Timer_Tick;
            _timer.Start();
          
            // تعریف منوی Context
            ContextMenuStrip contextMenu = new ContextMenuStrip();
            contextMenu.Items.Add("خروج از نرم افزار", null, Exit_Click);
            if (notifyIcon != null)
            {
                notifyIcon.ContextMenuStrip = contextMenu;
            }
        }

        #region Methods
        public void RefreshPage()
        {
            UserNameText.Text = LoginInUser.Username;
            NameText.Text = LoginInUser.FullName;
            ClientAmountText.Text = Dashboard_BLL.ClientsCount();
            ActiveCasesText.Text = Dashboard_BLL.ActiveCasesCount();
            ClosedCasesText.Text = Dashboard_BLL.ClosedCasesCount();
            TodayRemindsText.Text = Dashboard_BLL.UserReminderCount(LoginInUser).Count().ToString();
            if (LoginInUser.Role == User.UserRole.Lawyer)
            {
                Lawyer_Name.Text = LoginInUser.FullName;
            }
            else if (LoginInUser.Role == User.UserRole.Secretary)
            {
                Lawyer_Name.Text = LoginInUser.FullNameLawyer;
            }    

            int a = 0;
            foreach (var item in Dashboard_BLL.UserReminderCount(LoginInUser))
            {
                if (a < 7)
                {
                    // ساخت کنترل
                    UC_Reminder uC_Reminder = new UC_Reminder();
                    uC_Reminder.ReminderData = item; // ست کردن داده‌ی کامل یادآور

                    // مقداردهی به متن‌ها
                    uC_Reminder.ReminderUserTypeText.Text = $"کاربر: {item.UserTypeText}";
                    uC_Reminder.ReminderTitleText.Text = $"عنوان: {item.Title}";

                    // تنظیم محل نمایش در Grid
                    Grid.SetRow(uC_Reminder, 5 + a); // ردیف از 5 تا 11
                    Grid.SetColumnSpan(uC_Reminder, 7);

                    // اضافه کردن به گرید اصلی
                    maingrid.Children.Add(uC_Reminder);
                    a++;
                }
            }           
        }
        public void OpenWindowMethod(Window form)
        {
            BlurEffect blureffect = new BlurEffect();
            this.Effect = blureffect;
            blureffect.Radius = 10;
            form.ShowDialog();
            Effect = null;
        }
        private void ShowReminders()
        {
            var Interactionsreminders = interaction_bll.GetTodayReminders();
            var Correspondencereminders = correspondence_bll.GetTodayReminders();
            var legalbriefreminders = legalBrief_bll.GetTodayReminders();
            var paymentreminders = pbll.GetTodayReminders();
            var meetingreminders = mbll.GetTodayAndPreReminderMeetings();
            var personalnotereminders = pnbll.GetTodayReminders();

            foreach (var reminder in Interactionsreminders)
            {
                var notif = new NotificationWindow($"یادآوری تعامل با موکل",
                    $"موکل: {reminder.Client.FullName}\nتاریخ آخرین تعامل: {reminder.InteractionDate}\nنوع تعامل: {reminder.Type}",
                    description: reminder.Note,
                    NotificationType.Info
                    );
                notif.Show();
                PlayNotificationSound();
            }
            foreach (var reminder in Correspondencereminders)
            {
                var notif = new NotificationWindow($"یادآوری مکاتبه: {reminder.Title}",
                    $"پرونده: {reminder.Case.CaseNumber}\nموکل: {reminder.Case.Client.FullName}\nتاریخ: {reminder.ReminderDate?.ToString("yyyy/MM/dd")}",
                    description: reminder.CorrespondenceDescription,
                    NotificationType.Info
                    );
                notif.Show();
                PlayNotificationSound();
            }

            Dictionary<int, string> titleMappings = new Dictionary<int, string>
            {
                { 0, "درخواست مطالعه پرونده" },
                { 1, "استرداد اعتراض به نظریه کارشناسی" },
                { 2, "استرداد دادخواست" },
                { 3, "اسقاط حق تجدیدنظرخواهی یا فرجام‌خواهی" },
                { 4, "اعتراض به بهای خواسته" },
                { 5, "اظهار به تقدیم دادخواست جلب ثالث" },
                { 6, "لایحه دفاعیه (در دعاوی حقوقی)" },
                { 7, "اعتراض به رأی بدوی" },
                { 8, "دفاعیه در دعاوی کیفری" },
                { 9, "درخواست استمهال (تمدید مهلت)" },
                { 10, "درخواست اجرای حکم" },
            };

            foreach (var reminder in legalbriefreminders)
            {
                string titleText = titleMappings.ContainsKey(reminder.Title)
                ? titleMappings[reminder.Title]
                : "عنوان نامشخص";

                var notif = new NotificationWindow($"یادآوری ارجاع لایحه",
                $"پرونده: {reminder.Case?.CaseNumber ?? "نامشخص"}\n موکل: {reminder.Client?.FullName ?? "نامشخص"}\nتاریخ: {reminder.DeliveryDate?.ToString("yyyy/MM/dd") ?? "بدون تاریخ"}",
                description: $"عنوان لایحه: {titleText}",
                NotificationType.Info
                );
                notif.Show();
                PlayNotificationSound();
            }

            foreach (var reminder in paymentreminders)
            {
                var notif = new NotificationWindow($"یادآوری پرداخت موکل",
                $"پرونده: {reminder.Case?.CaseNumber ?? "نامشخص"}\nموضوع پرونده: {reminder.Case?.CaseSubject ?? "نامشخص"}\nموکل: {reminder.Client?.FullName ?? "نامشخص"}\nتاریخ آخرین پرداخت: {reminder.PaymentDate.ToString("yyyy/MM/dd") ?? "بدون تاریخ"}",
                description: $"آخرین مبلغ پرداختی: {string.Format("{0:N0} ریال", reminder.Amount)}\nتوضیحات: {reminder.Description}",
                NotificationType.Info
                );
                notif.Show();
                PlayNotificationSound();
            }

            foreach (var reminder in meetingreminders)
            {
                var notif = new NotificationWindow($"یادآوری جلسه: {reminder.MeetingSubject}",
                    $"پرونده: {reminder.Case.CaseNumber}\nموکل: {reminder.Case.Client.FullName}\nتاریخ: {reminder.MeetingDateTime.ToString()}\nمکان: {reminder.MeetingPlace}",
                    description: reminder.Description,
                    NotificationType.Info
                    );
                notif.Show();
                PlayNotificationSound();
            }
            Dictionary<int, string> UserTypeMappings = new Dictionary<int, string>
            {
                { 0, "وکیل" },
                { 1, "منشی" },
            };
            foreach (var reminder in personalnotereminders)
            {
                string userTypeText = UserTypeMappings.ContainsKey(reminder.UserRole)
                ? UserTypeMappings[reminder.UserRole]
                : "عنوان نامشخص";

                var notif = new NotificationWindow($"یادآوری یادداشت: {userTypeText}",
                    $"عنوان یادداشت: {reminder.Title}\nدسته بندی: {reminder.Category}\nتاریخ یادآوری: {reminder.ReminderDate?.ToString("yyyy/MM/dd") ?? "بدون تاریخ"}",
                    description: reminder.Description,
                    NotificationType.Info
                    );
                notif.Show();
                PlayNotificationSound();
            }
        }
        private void PlayNotificationSound()
        {
            try
            {
                var soundPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", "Sounds", "NotificationSound.wav");
                if (System.IO.File.Exists(soundPath))
                {
                    SoundPlayer player = new SoundPlayer(soundPath);
                    player.Play();
                }
            }
            catch (Exception)
            {
                // مدیریت خطا
                ShowNotification("خطا در پخش صدا", "error");
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
        private void Timer_Tick(object sender, EventArgs e)
        {
            // تغییر متن لیبل
            ChangingLabel.Text = _messages[_currentIndex];

            // رفتن به پیام بعدی
            _currentIndex++;
            if (_currentIndex >= _messages.Count)
            {
                _currentIndex = 0; // بازگشت به اولین متن
            }
        }
        #endregion

        private void ThemeToggle_Checked(object sender, RoutedEventArgs e)
        {
            ApplyDarkMode();
            Heyam.Properties.Settings.Default.IsDarkMode = true;
            Heyam.Properties.Settings.Default.Save();
            ScreenModeText.Text = "🌙";
            Calender.Background = new SolidColorBrush(Colors.Black);
            Calender.Foreground = new SolidColorBrush(Colors.White);           
        }
        private void ThemeToggle_Unchecked(object sender, RoutedEventArgs e)
        {
            ApplyLightMode();
            Heyam.Properties.Settings.Default.IsDarkMode = false;
            Heyam.Properties.Settings.Default.Save();
            ScreenModeText.Text = "🌞";
            Calender.Background = new SolidColorBrush(Colors.White);
            Calender.Foreground = new SolidColorBrush(Colors.Black);
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
        private void Client_TextBlock_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            ClientManagement window = new ClientManagement(this);
            OpenWindowMethod(window);
        }
        private void CaseManagement_TextBlock_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {           
            CaseManagement window = new CaseManagement(this);
            OpenWindowMethod(window);
        }
        private void Automation_TextBlock_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            LegalAutomation window = new LegalAutomation(this);
            OpenWindowMethod(window);
        }    
        private void FinancialManagementForm_TextBlock_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            FinancialManagementForm window = new FinancialManagementForm(this);
            OpenWindowMethod(window);           
        }
        private void LegalInfoBank_TextBlock_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            LegalInfoBank window = new LegalInfoBank();
            OpenWindowMethod(window);
        }
        private void Activities_TextBlock_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            ActivityReminderForm window = new ActivityReminderForm(this);
            OpenWindowMethod(window);
        }
        private void SMSPanel_TextBlock_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            SMSPanel window = new SMSPanel();
            OpenWindowMethod(window);
        }
        private void Reports_TextBlock_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Reports window = new Reports();
            OpenWindowMethod(window);
        }
        private void Settings_TextBlock_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Settings window = new Settings();          
            OpenWindowMethod(window);           
        }    
        private void Image_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            CustomMessageBox window = new CustomMessageBox("نرم افزار هـــــیام", "آیا قصد خروج از نرم افزار را دارید؟", true, CustomMessageBox.MessageType.Warning);
            OpenWindowMethod(window);
            if (window.Result)
            {
                System.Windows.Application.Current.Shutdown();
            }                                   
        }
        private void Exit_Click(object sender, EventArgs e)
        {
            System.Windows.Application.Current.Shutdown();
        }
    }
}