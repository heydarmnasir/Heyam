using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Collections.ObjectModel;
using BE;
using BLL;
using BE.ViewModel;
using System.Text.Json;
using System.IO;
using PL;
using IPE.SmsIrClient;

namespace Heyam
{  
    public partial class ActivityReminderForm : Window
    {
        private MainWindow _mainWindow;
        private bool isFormOpen = false; // وضعیت باز بودن یا بسته بودن فرم

        Client_BLL cbll = new Client_BLL();
        Interaction_BLL ibll = new Interaction_BLL();
        Correspondence_BLL correspondencebll = new Correspondence_BLL();
        LegalBrief_BLL lbll = new LegalBrief_BLL();
        Meeting_BLL mbll = new Meeting_BLL();
        Payment_BLL pbll = new Payment_BLL();
        PersonalNote_BLL pnbll = new PersonalNote_BLL();
        Reminder_BLL rbll = new Reminder_BLL();

        private ObservableCollection<string> _categories;
        private readonly List<string> _defaultCategories = new List<string>
        {
            "شخصی", "پرونده‌ها", "جلسات", "ایده‌ها", "سایر"
        };
        private readonly string _categoryFilePath =
        System.IO.Path.Combine("D:\\Privet\\Projects\\Heyam\\PL\\Resources\\NoteCategory", "categories.json");

        public ActivityReminderForm(MainWindow mainWindow)
        {
            InitializeComponent();
            LoadCategories();
            RefreshMeeting();
            RefreshPersonalNotes();
            LoadInteractions();
            var reminders = rbll.GetAllReminders();
            RemindersDGV.ItemsSource = reminders;

            _mainWindow = mainWindow;

            #region Fill ClientName & CaseNumber Field ComboBox
            var clients = cbll.GetAllClients(); // از BLL           
            SelectClientCB.ItemsSource = clients;
            SelectClientCB.DisplayMemberPath = "FullName"; // فرض بر اینکه پراپرتی FullName داری
            SelectClientCB.SelectedValuePath = "Id";
            #endregion  
        }
        SmsIr smsIr = new SmsIr("XwrI6iBRvxaWJ4AVcqaQddyRF5Ux3iHesb7XuQGbcSVag4Ut");

        #region Methods
        public void OpenBlurWindow(Window window)
        {           
            BlurEffect blurEffect = new BlurEffect();
            this.Effect = blurEffect;
            blurEffect.Radius = 10;
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
        private void SaveChanges()
        {
            _mainWindow.RefreshPage(); // صدا زدن متد از MainWindow
        }
        public void RefreshMeeting()
        {
            MeetingsDGV.ItemsSource = null;
            MeetingsDGV.ItemsSource = mbll.GetAllMeetingsForListView();
        }
        public void RefreshPersonalNotes()
        {
            PersonalNotesDGV.ItemsSource = null;
            PersonalNotesDGV.ItemsSource = pnbll.GetPersonalNotesForListView();
        }
        private void LoadCategories()
        {
            if (File.Exists(_categoryFilePath))
            {
                string json = File.ReadAllText(_categoryFilePath);
                var list = JsonSerializer.Deserialize<List<string>>(json) ?? new List<string>();
                _categories = new ObservableCollection<string>(list);
            }
            else
            {
                // اگر فایل نبود، از دسته‌بندی‌های پیش‌فرض استفاده کن
                _categories = new ObservableCollection<string>(_defaultCategories);
                SaveCategoriesToFile(); // ذخیره اولیه در فایل
                //_categories = new ObservableCollection<string>();
            }

            // قبل از تنظیم ItemsSource، اطمینان حاصل کن که Items خالیه
            CategoryListBox.ItemsSource = null;
            CategoryListBox.Items.Clear();
            CategoryListBox.ItemsSource = _categories;

            // تنظیم مجدد ComboBox
            NoteCategoryCB.Items.Clear();
            foreach (var category in _categories)
            {
                NoteCategoryCB.Items.Add(new ComboBoxItem { Content = category });
            }
        }        
        private void SaveCategoriesToFile()
        {
            try
            {
                Directory.CreateDirectory(System.IO.Path.GetDirectoryName(_categoryFilePath)); // ← این خط مهمه
                string json = JsonSerializer.Serialize(_categories.ToList());
                File.WriteAllText(_categoryFilePath, json);
            }
            catch (Exception ex)
            {
                MessageBox.Show("خطا در ذخیره دسته‌بندی‌ها: " + ex.Message);
            }
        }
        #endregion

        #region Menu
        private void AddMeetingBTN_Click(object sender, RoutedEventArgs e)
        {
            if (isFormOpen)
            {
                // انیمیشن بستن
                AnimateSlide(350, 0);
                // رنگ جدید با استفاده از BrushConverter
                var brushConverter = new BrushConverter();
                MeetingBox.Background = (Brush)brushConverter.ConvertFromString("#2B2B3A");
            }
            else
            {
                // انیمیشن باز کردن
                AnimateSlide(0, 452);
                MeetingBox.Background = new SolidColorBrush(Colors.DarkGreen);
            }
            isFormOpen = !isFormOpen;
        }
        private void MeetingListBTN_Click(object sender, RoutedEventArgs e)
        {
            if (isFormOpen)
            {
                // انیمیشن بستن
                AnimateSlide1(350, 0);
                // رنگ جدید با استفاده از BrushConverter
                var brushConverter = new BrushConverter();
                MeetingBox.Background = (Brush)brushConverter.ConvertFromString("#2B2B3A");
            }
            else
            {
                // انیمیشن باز کردن
                AnimateSlide1(0, 452);
                MeetingBox.Background = new SolidColorBrush(Colors.DarkGreen);
            }
            isFormOpen = !isFormOpen;
        }
        private void RemindersListBTN_Click(object sender, RoutedEventArgs e)
        {
            if (isFormOpen)
            {
                // انیمیشن بستن
                AnimateSlide2(350, 0);
                // رنگ جدید با استفاده از BrushConverter
                var brushConverter = new BrushConverter();
                RemindersBox.Background = (Brush)brushConverter.ConvertFromString("#2B2B3A");
            }
            else
            {
                // انیمیشن باز کردن
                AnimateSlide2(0, 452);
                RemindersBox.Background = new SolidColorBrush(Colors.DarkGreen);
            }
            isFormOpen = !isFormOpen;
        }
        private void NotesManagementBTN_Click(object sender, RoutedEventArgs e)
        {
            if (isFormOpen)
            {
                // انیمیشن بستن
                AnimateSlide3(350, 0);
                // رنگ جدید با استفاده از BrushConverter
                var brushConverter = new BrushConverter();
                NotesBox.Background = (Brush)brushConverter.ConvertFromString("#2B2B3A");
            }
            else
            {
                // انیمیشن باز کردن
                AnimateSlide3(0, 515);
                NotesBox.Background = new SolidColorBrush(Colors.DarkGreen);                
            }
            isFormOpen = !isFormOpen;
        }
        private void AnimateSlide(double fromHeight, double toHeight)
        {
            // ایجاد انیمیشن
            var animation = new DoubleAnimation
            {
                From = fromHeight,
                To = toHeight,
                Duration = TimeSpan.FromSeconds(0.5), // مدت زمان انیمیشن
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseInOut } // حالت انیمیشن
            };
            AddMeetingSlideDownForm.BeginAnimation(HeightProperty, animation);            
        }
        private void AnimateSlide1(double fromHeight, double toHeight)
        {
            // ایجاد انیمیشن
            var animation = new DoubleAnimation
            {
                From = fromHeight,
                To = toHeight,
                Duration = TimeSpan.FromSeconds(0.5), // مدت زمان انیمیشن
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseInOut } // حالت انیمیشن
            };
            SlideDownFormMeetingList.BeginAnimation(HeightProperty, animation);
        }
        private void AnimateSlide2(double fromHeight, double toHeight)
        {
            // ایجاد انیمیشن
            var animation = new DoubleAnimation
            {
                From = fromHeight,
                To = toHeight,
                Duration = TimeSpan.FromSeconds(0.5), // مدت زمان انیمیشن
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseInOut } // حالت انیمیشن
            };
            SlideDownFormRemindersList.BeginAnimation(HeightProperty, animation);
        }
        private void AnimateSlide3(double fromHeight, double toHeight)
        {
            // ایجاد انیمیشن
            var animation = new DoubleAnimation
            {
                From = fromHeight,
                To = toHeight,
                Duration = TimeSpan.FromSeconds(0.5), // مدت زمان انیمیشن
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseInOut } // حالت انیمیشن
            };
            SlideDownFormNotes.BeginAnimation(HeightProperty, animation);
        }
        #endregion

        #region Meetings
        private void SelectClientCB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (SelectClientCB.SelectedItem is Client selectedClient)
            {
                // به شرط اینکه Case شامل Client باشد
                SelectCaseCB.ItemsSource = selectedClient.Cases;
                SelectCaseCB.DisplayMemberPath = "CaseNumber";
                SelectCaseCB.SelectedValuePath = "Id";
                CaseSubjectLabel.Text = "";
            }
        }
        private void SelectCaseCB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (SelectCaseCB.SelectedItem is Case selectedCase)
            {
                // به شرط اینکه Case شامل Client باشد
                CaseSubjectLabel.Text = $"موضوع پرونده: {selectedCase.CaseSubject ?? "ناشناخته"}";
            }
        }   
        private void SearchMeetings_TextChanged(object sender, TextChangedEventArgs e)
        {
            string keyword = SearchMeetings.Text.Trim();
            List<MeetingDto> filtered;

            if (string.IsNullOrWhiteSpace(keyword))
            {
                MeetingsPlaceholderText.Visibility = Visibility.Visible;
                filtered = mbll.GetAllMeetingsForListView();
                MeetingsDGV.ItemsSource = filtered;
            }
            else
            {
                filtered = mbll.SearchMeetingForDGV(keyword);
                MeetingsDGV.ItemsSource = filtered;
                MeetingsPlaceholderText.Visibility = Visibility.Hidden;
            }
        }
        private void MeetingDateTime_SelectedDateTimeChanged(object sender, HandyControl.Data.FunctionEventArgs<DateTime?> e)
        {
            if (!string.IsNullOrWhiteSpace(MeetingDateTime.Text))
            {
                ReminderMeetingCheckBox.IsEnabled = true;
            }
            else
            {
                ReminderMeetingCheckBox.IsEnabled = false;
            }
        }
        private void ReminderMeetingCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            MeetingReminderText.Visibility = Visibility.Visible;
        }
        private void ReminderMeetingCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            MeetingReminderText.Visibility = Visibility.Hidden;
        }
        private void MeetingCalendar_SelectedDatesChanged(object sender, SelectionChangedEventArgs e)
        {
            if (MeetingCalendar.SelectedDate.HasValue)
            {
                var selectedDate = MeetingCalendar.SelectedDate.Value.Date;
                var filteredMeetings = mbll.SearchMeetingForDGV(null, selectedDate); // فقط تاریخ، بدون نام موکل
                MeetingsDGV.ItemsSource = filteredMeetings;
            }
        }
        private void RefreshMeetingDataGridBTN_Click(object sender, RoutedEventArgs e)
        {
            RefreshMeeting();
        }
        private void MeetingDetailsPopupCloseBTN_Click(object sender, RoutedEventArgs e)
        {
            MeetingDetailsPopup.Visibility = Visibility.Collapsed;
            MainGrid.Effect = null;
        }
        private void SubmitMeetingBTN_Click(object sender, RoutedEventArgs e)
        {
            if ((string.IsNullOrWhiteSpace(SelectClientCB.Text) || string.IsNullOrWhiteSpace(SelectCaseCB.Text) || string.IsNullOrWhiteSpace(MeetingSubject.Text) || string.IsNullOrWhiteSpace(MeetingPlace.Text) || string.IsNullOrWhiteSpace(MeetingDateTime.Text)))
            {
                ShowNotification("اطلاعات ضروری را وارد کنید!", "error");
            }
            else
            {
                try
                {
                    if (SelectClientCB.SelectedItem == null)
                    {
                        ShowNotification("موکل در سیستم موجود نمیباشد!", "error");
                        return;
                    }
                    // بررسی وجود پرونده در ComboBox
                    if (SelectCaseCB.SelectedItem == null)
                    {
                        ShowNotification("پرونده در سیستم موجود نمیباشد!", "error");
                        return;
                    }
                    var user = AppSession.CurrentUser;
                    var newMeeting = new Meeting
                    {
                        UserId = user.Id,
                        UserRole = (int)user.Role,
                        ClientId = (int)SelectClientCB.SelectedValue,
                        CaseId = (int)SelectCaseCB.SelectedValue,
                        MeetingSubject = MeetingSubject.Text,
                        MeetingPlace = MeetingPlace.Text,
                        MeetingDateTime = MeetingDateTime.SelectedDateTime.Value,
                        IsReminderMeetingSet = ReminderMeetingCheckBox.IsChecked == true,
                        IsReminderMeetingDone = false,
                        Description = MeetingDescriptionTB.Text,
                        MeetingStartDate = MeetingStartDateDP.SelectedDate,
                        MeetingEndDate = MeetingEndDateDP.SelectedDate,
                        CreatedAt = DateTime.Now
                    };
                    int meetingnId = mbll.Create(newMeeting);
                    if (meetingnId > 0)
                    {
                        ShowNotification("جلسه با موفقیت ثبت شد", "success");

                        if (ReminderMeetingCheckBox.IsChecked == true && MeetingDateTime.SelectedDateTime.Value != null)
                        {
                            var reminder = new Reminder
                            {
                                Title = $"یادآور جلسه با {SelectClientCB.Text}",
                                Description = MeetingDescriptionTB.Text,
                                ReminderDate = MeetingDateTime.SelectedDateTime.Value,
                                UserId = user.Id,
                                MeetingId = meetingnId
                            };
                            rbll.Create(reminder);
                            var bulkSendResult = smsIr.BulkSendAsync(30007732011420, $"شما یک جلسه برای تاریخ: {MeetingDateTime.SelectedDateTime.Value.ToString("yyyy/MM/dd ساعت: HH:mm")} ثبت کردید\nموکل: {SelectClientCB.Text}\nموضوع جلسه: {MeetingSubject.Text}\nمکان جلسه: {MeetingPlace.Text}\n\n نرم افزار حقوقی هیام", new string[] { user.PhoneNumber });
                        }
                        RefreshMeeting();
                        SaveChanges();
                        ClearControls(AddMeetingSlideDownForm);
                        CaseSubjectLabel.Text = "";
                    }
                    else
                    {
                        ShowNotification("خطا در ثبت جلسه!", "error");
                    }                    
                }
                catch (Exception)
                {
                    ShowNotification("خطا در ثبت جلسه!", "error");
                }
            }
        }
        private void ShowMeetingDetailsBTN_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            if (btn?.Tag is int meetingId)
            {
                MeetingDetailsPopup.Tag = meetingId;
                var _meeting = mbll.GetMeetingById(meetingId);
                // پر کردن جزئیات در پاپ‌آپ               
                CaseNumberPopupTB.Text = _meeting.Case.CaseNumber;
                MeetingPlacePopupTB.Text = _meeting.MeetingPlace;
                MeetingStartDatePopupTB.Text = _meeting.MeetingStartDate?.ToShortDateString() ?? "";
                MeetingEndDatePopupTB.Text = _meeting.MeetingEndDate?.ToShortDateString() ?? "";
                MeetingDescriptionPopupTB.Text = _meeting.Description;
                MeetingDetailsPopup.Visibility = Visibility.Visible;
                MainGrid.Effect = new BlurEffect() { Radius = 5 };
            }
        }
        #endregion

        #region Reminders
        public void LoadAllReminders()
        {
            if (RemindersDGV != null)
            {
                RemindersDGV.Columns.Clear();
                RemindersDGV.Columns.Add(new DataGridTextColumn { Header = "کاربر", Width = 55, Binding = new Binding("UserTypeText") });
                RemindersDGV.Columns.Add(new DataGridTextColumn { Header = "عنوان", Width = 250, Binding = new Binding("Title") });
                RemindersDGV.Columns.Add(new DataGridTextColumn { Header = "توضیحات", Width = 550, Binding = new Binding("Description") });
                RemindersDGV.Columns.Add(new DataGridTextColumn { Header = "تاریخ", Width = 100, Binding = new Binding("SetDateShamsi") });
                var reminders = rbll.GetAllReminders();
                RemindersDGV.ItemsSource = reminders;
            }
        }
        public void LoadInteractions()
        {
            RemindersDGV.Columns.Clear();
            RemindersDGV.Columns.Add(new DataGridTextColumn { Header = "کاربر", Width = 55, Binding = new Binding("UserTypeText") });
            RemindersDGV.Columns.Add(new DataGridTextColumn { Header = "نام موکل", Width = 175, Binding = new Binding("ClientName") });
            RemindersDGV.Columns.Add(new DataGridTextColumn { Header = "نوع تعامل", Width = 175, Binding = new Binding("Type") });
            RemindersDGV.Columns.Add(new DataGridTextColumn { Header = "تاریخ تعامل", Width = 175, Binding = new Binding("InteractionDate") });
            RemindersDGV.Columns.Add(new DataGridTextColumn { Header = "یادآوری دارد؟", Width = 175, Binding = new Binding("IsReminderSet") });
            var interactions = ibll.GetReminders();
            RemindersDGV.ItemsSource = interactions;
        }
        public void LoadCorrespondences()
        {
            RemindersDGV.Columns.Clear();
            RemindersDGV.Columns.Add(new DataGridTextColumn { Header = "کاربر", Width = 55, Binding = new Binding("UserTypeText") });
            RemindersDGV.Columns.Add(new DataGridTextColumn { Header = "شماره پرونده", Width = 150, Binding = new Binding("CaseNumber") });
            RemindersDGV.Columns.Add(new DataGridTextColumn { Header = "نام موکل", Width = 150, Binding = new Binding("ClientName") });
            RemindersDGV.Columns.Add(new DataGridTextColumn { Header = "نوع مکاتبه", Width = 150, Binding = new Binding("Type") });
            RemindersDGV.Columns.Add(new DataGridTextColumn { Header = "عنوان", Width = 150, Binding = new Binding("Title") });
            RemindersDGV.Columns.Add(new DataGridTextColumn { Header = "وضعیت", Width = 150, Binding = new Binding("StatusText") });
            RemindersDGV.Columns.Add(new DataGridTextColumn { Header = "یادآوری دارد؟", Width = 150, Binding = new Binding("IsReminderSet") });
            var correspondences = correspondencebll.GetReminders();
            RemindersDGV.ItemsSource = correspondences;
        }
        public void LoadLegalBriefs()
        {
            RemindersDGV.Columns.Clear();
            RemindersDGV.Columns.Add(new DataGridTextColumn { Header = "کاربر", Width = 55, Binding = new Binding("UserTypeText") });
            RemindersDGV.Columns.Add(new DataGridTextColumn { Header = "نام موکل", Width = 150, Binding = new Binding("ClientName") });
            RemindersDGV.Columns.Add(new DataGridTextColumn { Header = "شماره پرونده", Width = 150, Binding = new Binding("CaseNumber") });
            RemindersDGV.Columns.Add(new DataGridTextColumn { Header = "عنوان لایحه", Width = 250, Binding = new Binding("LegalBriefTitleText") });
            RemindersDGV.Columns.Add(new DataGridTextColumn { Header = "تاریخ تنظیم", Width = 130, Binding = new Binding("SetDate") });
            RemindersDGV.Columns.Add(new DataGridTextColumn { Header = "یادآوری دارد؟", Width = 130, Binding = new Binding("IsDeliverySet") });
            RemindersDGV.Columns.Add(new DataGridTextColumn { Header = "تاریخ ارجاع", Width = 130, Binding = new Binding("DeliveryDate") });          
            var legalBriefs = lbll.GetReminders();
            RemindersDGV.ItemsSource = legalBriefs;
        }
        public void LoadMeetings()
        {
            RemindersDGV.Columns.Clear();
            RemindersDGV.Columns.Add(new DataGridTextColumn { Header = "کاربر", Width = 55, Binding = new Binding("UserTypeText") });
            RemindersDGV.Columns.Add(new DataGridTextColumn { Header = "نام موکل", Width = 175, Binding = new Binding("ClientName") });
            RemindersDGV.Columns.Add(new DataGridTextColumn { Header = "موضوع جلسه", Width = 250, Binding = new Binding("MeetingSubject") });
            RemindersDGV.Columns.Add(new DataGridTextColumn { Header = "زمان", Width = 160, Binding = new Binding("SetDateShamsi") });
            RemindersDGV.Columns.Add(new DataGridTextColumn { Header = "یادآوری دارد؟", Width = 160, Binding = new Binding("IsReminderNextMeetingSet") });
            var meetings = mbll.GetReminders();
            RemindersDGV.ItemsSource = meetings;
        }
        public void LoadPayments()
        {
            RemindersDGV.Columns.Clear();
            RemindersDGV.Columns.Add(new DataGridTextColumn { Header = "کاربر", Width = 55, Binding = new Binding("UserTypeText") });
            RemindersDGV.Columns.Add(new DataGridTextColumn { Header = "نام موکل", Width = 175, Binding = new Binding("ClientName") });
            RemindersDGV.Columns.Add(new DataGridTextColumn { Header = "تلفن همراه", Width = 150, Binding = new Binding("ClientPhoneNumber") });
            RemindersDGV.Columns.Add(new DataGridTextColumn { Header = "شماره پرونده", Width = 150, Visibility = Visibility.Collapsed, Binding = new Binding("ClientCaseNumber") });
            RemindersDGV.Columns.Add(new DataGridTextColumn { Header = "هزینه پرداختی", Width = 150, Visibility = Visibility.Collapsed, Binding = new Binding("Cost") });
            RemindersDGV.Columns.Add(new DataGridTextColumn { Header = "موضوع پرونده", Width = 150, Binding = new Binding("ClientCaseSubject") });
            RemindersDGV.Columns.Add(new DataGridTextColumn { Header = "نوع خدمت", Width = 150, Binding = new Binding("ServiceTypeText") });
            RemindersDGV.Columns.Add(new DataGridTextColumn { Header = "وضعیت", Width = 150, Binding = new Binding("PaymentStatusTypeText") });
            RemindersDGV.Columns.Add(new DataGridTextColumn { Header = "یادآوری؟", Width = 150, Binding = new Binding("IsReminderSet") });
            var payments = pbll.GetReminders();
            RemindersDGV.ItemsSource = payments;
        }
        public void LoadPersonalNotes()
        {
            RemindersDGV.Columns.Clear();
            RemindersDGV.Columns.Add(new DataGridTextColumn { Header = "کاربر", Width = 150, Binding = new Binding("UserTypeText") });
            RemindersDGV.Columns.Add(new DataGridTextColumn { Header = "عنوان", Width = 175, Binding = new Binding("Title") });          
            RemindersDGV.Columns.Add(new DataGridTextColumn { Header = "دسته بندی", Width = 150, Binding = new Binding("Category") });
            RemindersDGV.Columns.Add(new DataGridTextColumn { Header = "یادآوری دارد؟", Width = 150, Binding = new Binding("IsReminderSet") });          
            var personalNotes = pnbll.GetReminders();
            RemindersDGV.ItemsSource = personalNotes;
        }
        private void RemindersRB_Checked(object sender, RoutedEventArgs e)
        {
            LoadAllReminders();
        }
        private void InteractionRB_Checked(object sender, RoutedEventArgs e)
        {
            LoadInteractions();
        }
        private void CorrespondenceRB_Checked(object sender, RoutedEventArgs e)
        {
            LoadCorrespondences();
        }
        private void LegalBriefRB_Checked(object sender, RoutedEventArgs e)
        {
            LoadLegalBriefs();
        }
        private void MeetingsRB_Checked(object sender, RoutedEventArgs e)
        {
            LoadMeetings();
        }
        private void PaymentRB_Checked(object sender, RoutedEventArgs e)
        {
            LoadPayments();
        }
        private void PersonalNoteRB_Checked(object sender, RoutedEventArgs e)
        {
            LoadPersonalNotes();
        }
        #endregion

        #region PersonalNotes             
        private void AddNoteBTN_Click(object sender, RoutedEventArgs e)
        {
            AddNotePopup.Visibility = Visibility.Visible;
            MainGrid.Effect = new BlurEffect() { Radius = 5};
        }
        private void DeleteNoteBTN_Click(object sender, RoutedEventArgs e)
        {
            var selectedItem = PersonalNotesDGV.SelectedItem as PersonalNotesDto;
            if (selectedItem != null)
            {
               ShowNotification(pnbll.Delete(selectedItem.Id),"success");                    
            }
        }
        private void PersonalNotesDGV_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (PersonalNotesDGV.SelectedItem is PersonalNotesDto selectedPersonalNotes)
            {
                DeleteNoteBTN.IsEnabled = true;
            }
            else
            {
                DeleteNoteBTN.IsEnabled = false;
            }
        }
        private void SearchNote_TextChanged(object sender, TextChangedEventArgs e)
        {
            string keyword = SearchNote.Text.Trim();
            List<PersonalNotesDto> filtered;

            if (string.IsNullOrWhiteSpace(keyword))
            {
                NotesPlaceholderText.Visibility = Visibility.Visible;
                filtered = pnbll.GetPersonalNotesForListView();
                PersonalNotesDGV.ItemsSource = filtered;
            }
            else
            {
                filtered = pnbll.SearchPersonalNotesForDGV(keyword);
                PersonalNotesDGV.ItemsSource = filtered;
                NotesPlaceholderText.Visibility = Visibility.Hidden;
            }
        }
        private void NoteReminderCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            ReminderDateDP.IsEnabled = true;
        }
        private void NoteReminderCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            ReminderDateDP.IsEnabled = false;
        }
        private void CancelNoteBTN_Click(object sender, RoutedEventArgs e)
        {
            AddNotePopup.Visibility = Visibility.Collapsed;
            MainGrid.Effect = null;
            ClearControls(AddNotePopup);
        }
        private void NoteDetailsPopupCloseBTN_Click(object sender, RoutedEventArgs e)
        {
            NoteDetailsPopup.Visibility = Visibility.Collapsed;
            MainGrid.Effect = null;
        }
        private void CategoryTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            CategoryPlaceholderText.Visibility = string.IsNullOrWhiteSpace(CategoryTextBox.Text) ?
                Visibility.Visible : Visibility.Hidden;
        }
        private void AddCategoryBTN_Click(object sender, RoutedEventArgs e)
        {
            string newCategory = CategoryTextBox.Text.Trim();

            if (!string.IsNullOrWhiteSpace(newCategory))
            {
                if (!_categories.Contains(newCategory))
                {
                    _categories.Add(newCategory);
                    SaveCategoriesToFile();
                    LoadCategories();
                    CategoryTextBox.Clear();
                }
                else
                {
                    ShowNotification("این دسته‌بندی قبلاً اضافه شده است.", "info");
                }
            }
            else
            {
                ShowNotification("لطفاً یک متن وارد کنید.", "error");
            }
        }
        private void RemoveCategoryBTN_Click(object sender, RoutedEventArgs e)
        {
            if (CategoryListBox.SelectedItem != null)
            {
                string selectedCategory = CategoryListBox.SelectedItem.ToString();
                _categories.Remove(selectedCategory); // حذف از حافظه
                SaveCategoriesToFile();               // ذخیره در فایل

                // همچنین حذف از کمبوباکس
                foreach (var item in NoteCategoryCB.Items.OfType<ComboBoxItem>().ToList())
                {
                    if (item.Content.ToString() == selectedCategory)
                    {
                        NoteCategoryCB.Items.Remove(item);
                        break;
                    }
                }
            }
            else
            {
                ShowNotification("لطفاً یک دسته‌بندی برای حذف انتخاب کنید.", "warning");
            }
        }
        private void CategoryListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // بررسی اینکه آیا رکوردی انتخاب شده است
            if (CategoryListBox.SelectedItem != null)
            {
                RemoveCategoryBTN.IsEnabled = true; // دکمه فعال شود
            }
            else
            {
                RemoveCategoryBTN.IsEnabled = false; // دکمه غیرفعال شود
            }
        }
        private void SubmitNoteBTN_Click(object sender, RoutedEventArgs e)
        {
            if ((string.IsNullOrWhiteSpace(NoteTitleTB.Text) || string.IsNullOrWhiteSpace(NoteCategoryCB.Text) || string.IsNullOrWhiteSpace(NoteDescriptionTB.Text)))
            {
                ShowNotification("اطلاعات ضروری را وارد کنید!", "error");
            }
            else
            {
                try
                {
                    var user = AppSession.CurrentUser;
                    var newPersonalNote = new PersonalNote
                    {
                        UserId = user.Id,
                        UserRole = (int)user.Role,
                        Title = NoteTitleTB.Text,
                        Category = NoteCategoryCB.Text,
                        Description = NoteDescriptionTB.Text,
                        IsReminderSet = NoteReminderCheckBox.IsChecked == true,
                        ReminderDate = NoteReminderCheckBox.IsChecked == true ? ReminderDateDP.SelectedDate : null,
                        IsReminderDone = false,
                        CreatedAt = DateTime.Now
                    };
                    int personalnoteId = pnbll.Create(newPersonalNote);
                    if (personalnoteId > 0)
                    {
                        ShowNotification("یادداشت با موفقیت ثبت شد", "success");

                        if (NoteReminderCheckBox.IsChecked == true && ReminderDateDP.SelectedDate.Value != null)
                        {
                            var reminder = new Reminder
                            {
                                Title = $"یادآور یادداشت {NoteTitleTB.Text}",
                                Description = NoteDescriptionTB.Text,
                                ReminderDate = ReminderDateDP.SelectedDate.Value,
                                UserId = user.Id,
                                PersonalNotesId = personalnoteId
                            };
                            rbll.Create(reminder);
                            var bulkSendResult = smsIr.BulkSendAsync(30007732011420, $"یادآوری یادداشت: {NoteTitleTB.Text} \nدسته بندی: {NoteCategoryCB.Text} \nکاربر: {user.UserTypeRole}\nتاریخ یادآوری: {ReminderDateDP.SelectedDate.Value.ToString("yyyy/MM/dd")}\n\n نرم افزار حقوقی هیام", new string[] { user.PhoneNumber });
                        }                        
                        RefreshPersonalNotes();
                        SaveChanges();
                        ClearControls(AddNotePopup);
                        AddNotePopup.Visibility = Visibility.Collapsed;
                        MainGrid.Effect = null;
                    }
                    else
                    {
                        ShowNotification("خطا در ثبت یادداشت!", "error");
                    }                  
                }
                catch (Exception)
                {
                    ShowNotification("خطا در ثبت یادداشت!", "error");
                }
            }
        }
        private void ShowNoteDetailsBTN_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            if (btn?.Tag is int noteId)
            {
                NoteDetailsPopup.Tag = noteId;
                var _note = pnbll.GetPersonalNoteId(noteId);
                // پر کردن جزئیات در پاپ‌آپ               
                NoteReminderDatePopupTB.Text = _note.ReminderDate?.ToShortDateString() ?? "";
                NoteDescriptionPopupTB.Text = _note.Description;
                NoteDetailsPopup.Visibility = Visibility.Visible;
                MainGrid.Effect = new BlurEffect() { Radius = 5 };
            }
        }       
        #endregion
        private void CloseBTN_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }      
    }
}