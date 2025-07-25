using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Text.RegularExpressions;
using BE;
using BLL;
using System.Collections.Generic;
using BE.ViewModel;
using PL;
using IPE.SmsIrClient;

namespace Heyam
{
    public partial class ClientManagement : System.Windows.Window
    {
        private MainWindow _mainWindow;
        public ClientManagement(MainWindow mainWindow)
        {
            InitializeComponent();         
            ClientRefresh();
            InteractionRefresh();
            _mainWindow = mainWindow;

            #region Clients FullNames In Interaction ComboBox
            var clients = cbll.GetAllClients(); // از BLL           
            SelectClientInteractionCB.ItemsSource = clients;
            SelectClientInteractionCB.DisplayMemberPath = "FullName"; // فرض بر اینکه پراپرتی FullName داری
            SelectClientInteractionCB.SelectedValuePath = "Id";
            #endregion
        }       
        Client_BLL cbll = new Client_BLL();
        Interaction interaction = new Interaction();
        Interaction_BLL ibll = new Interaction_BLL();
        Reminder_BLL rbll = new Reminder_BLL();
        Dashboard_BLL dbll = new Dashboard_BLL();

        SmsIr smsIr = new SmsIr("XwrI6iBRvxaWJ4AVcqaQddyRF5Ux3iHesb7XuQGbcSVag4Ut");

        #region Methods
        public void ClientRefresh()
        {
            ClientsDGV.ItemsSource = null;
            ClientsDGV.ItemsSource = cbll.GetClientsForListView();          
        }
        private void SaveChanges()
        {
            _mainWindow.RefreshPage(); // صدا زدن متد از MainWindow
        }
        public void InteractionRefresh()
        {
            InteractionsListView.ItemsSource = null;
            InteractionsListView.ItemsSource = ibll.GetInteractionsForListView();
        }
        public void OpenBlurWindow(Window window)
        {
            BlurEffect blureffect = new BlurEffect();
            this.Effect = blureffect;
            blureffect.Radius = 10;
            window.ShowDialog();
            Effect = null;
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
        public void Search_Filter()
        {
            string keyword = SearchClient.Text.Trim();
            string selectedGender = GenderFilterComboBox.SelectedValue?.ToString();
            var result = cbll.SearchForListView(keyword, selectedGender);
            ClientsDGV.ItemsSource = null;
            ClientsDGV.ItemsSource = result;
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

        #region Clients
        private void SearchClient_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(SearchClient.Text))
            {
                ClientRefresh();
                SearchPlaceholderText.Visibility = Visibility.Visible;
            }
            else
            {
                Search_Filter();
                SearchPlaceholderText.Visibility = Visibility.Hidden;
            }
        }
        private void ClientDetailsPopup_Opened(object sender, EventArgs e)
        {
            MainGrid.Effect = new BlurEffect() { Radius = 5 };
        }
        private void ClientDetailsPopup_Closed(object sender, EventArgs e)
        {
            MainGrid.Effect = null;
        }      
        private void ShowHide_ClientListBTN_Click(object sender, RoutedEventArgs e)
        {
            if (ClientsDGV.Visibility == Visibility.Collapsed && SearchClient.Visibility == Visibility.Collapsed && SearchPlaceholderText.Visibility == Visibility.Collapsed)
            {
                // نمایش جدول و پنهان‌سازی فرم
                ClientsDGV.Visibility = Visibility.Visible;
                SearchClient.Visibility = Visibility.Visible;
                SearchPlaceholderText.Visibility = Visibility.Visible;
                GenderFilterPanel.Visibility = Visibility.Visible;
                RegisterPanel.Visibility = Visibility.Collapsed;
            }
            else
            {
                // پنهان‌سازی جدول و نمایش فرم
                ClientsDGV.Visibility = Visibility.Collapsed;
                SearchClient.Visibility = Visibility.Collapsed;
                SearchPlaceholderText.Visibility = Visibility.Collapsed;
                GenderFilterPanel.Visibility = Visibility.Collapsed;
                RegisterPanel.Visibility = Visibility.Visible;
            }
        }
        private void EmailTB_TextChanged(object sender, TextChangedEventArgs e)
        {
            var textBox = sender as TextBox;

            // بررسی اینکه فقط حروف انگلیسی باشند
            if (!Regex.IsMatch(textBox.Text, @"[a-zA-Z0-9@._\-]"))
            {
                //textBox.Tag = "Invalid"; // فعال کردن استایل خط قرمز
                EmailErrorTextBlock.Visibility = Visibility.Visible;
            }
            else
            {
                //textBox.Tag = null; // استایل خط قرمز حذف
                EmailErrorTextBlock.Visibility = Visibility.Collapsed;
            }
            if (string.IsNullOrWhiteSpace(EmailTB.Text))
            {
                EmailErrorTextBlock.Visibility = Visibility.Collapsed;
            }
        }
        private void ClientDetailsPopupCloseBTN_Click(object sender, RoutedEventArgs e)
        {
            ClientDetailsPopup.IsOpen = false;
        }
        private void ShowClientDetailsBTN_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            if (btn?.Tag is int clientId)
            {
                ClientDetailsPopup.Tag = clientId;
                var selectedClient = cbll.GetClientById(clientId);
                // پر کردن جزئیات در پاپ‌آپ
                BirthDateText.Text = selectedClient.BirthDate.ToString("yyyy/MM/dd");
                PhoneNumberText.Text = selectedClient.PhoneNumber;
                EmailText.Text = selectedClient.Email;
                AddressText.Text = selectedClient.Address;
                NationalCodeHiddenText.Text = selectedClient.NationalCode; // فیلد پنهان برای نگهداری کد ملی
                // نمایش پاپ‌آپ
                ClientDetailsPopup.IsOpen = true;
            }           
        }
        private void GenderFilterComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Search_Filter();
        }      
        private void EmailText_TextChanged(object sender, TextChangedEventArgs e)
        {
            var textBox = sender as TextBox;

            // بررسی اینکه فقط حروف انگلیسی باشند
            if (!Regex.IsMatch(textBox.Text, @"[a-zA-Z0-9@._\-]"))
            {
                EmailErrorTextPopup.Visibility = Visibility.Visible;
            }
            else
            {
                EmailErrorTextPopup.Visibility = Visibility.Collapsed;
            }
            if (string.IsNullOrWhiteSpace(EmailText.Text))
            {
                EmailErrorTextPopup.Visibility = Visibility.Collapsed;
            }
        }   
        private void SubmitClientBTN_Click(object sender, RoutedEventArgs e)
        {
            var user = AppSession.CurrentUser;
            if (string.IsNullOrWhiteSpace(FullNameTB.Text) || string.IsNullOrEmpty(FatherNameTB.Text) || string.IsNullOrEmpty(NationalCodeTB.Text) || string.IsNullOrEmpty(BirthDateDP.Text) || string.IsNullOrEmpty(PhoneNumberTB.Text) || string.IsNullOrEmpty(AddressTB.Text))
            {
                ShowNotification("همه اطلاعات ضروری را وارد کنید!", "error");
            }
            else if (NationalCodeTB.Text.Trim().Length != 10 || PhoneNumberTB.Text.Trim().Length != 11)
            {
                ShowNotification("کد ملی یا تلفن همراه نامعتبر است!", "error");
            }
            else
            {
                string fullName = FullNameTB.Text;
                string fathername = FatherNameTB.Text;
                string nationalcode = NationalCodeTB.Text;
                DateTime birthdate = BirthDateDP.SelectedDate.Value;
                string phonenumber = PhoneNumberTB.Text;
                string email = EmailTB.Text;
                string address = AddressTB.Text;
                string gender = GenderCB.Text;

                // ثبت موکل جدید
                Client newClient = new Client
                {
                    UserId = user.Id,
                    UserRole = (int)user.Role,
                    FullName = fullName,
                    FatherName = fathername,
                    NationalCode = nationalcode,
                    BirthDate = birthdate,
                    PhoneNumber = phonenumber,
                    Email = email,
                    Address = address,
                    Gender = gender,
                    CreatedAt = DateTime.Now,
                };

                if (cbll.IsExist(newClient) == false)
                {
                    ShowNotification(cbll.Create(newClient),"success");
                    ClientRefresh();
                    SaveChanges();
                    ClearControls(MainGrid);
                    var clients = cbll.GetAllClients(); // از BLL        
                    SelectClientInteractionCB.ItemsSource = clients;
                }
                else
                {
                    ShowNotification("موکل در سیستم موجود میباشد!", "error");
                }
            }
        }
        private void ClientDetailsPopupUpdateBTN_Click(object sender, RoutedEventArgs e)
        {
            string nationalCode = NationalCodeHiddenText.Text.Trim(); // از فیلد پنهان
            string phonenumber = PhoneNumberText.Text.Trim();
            string email = EmailText.Text.Trim();
            string address = AddressText.Text.Trim();
            
            if (PhoneNumberText.Text.Trim().Length == 11)
            {
                bool success = cbll.UpdateClientContactInfo(nationalCode, phonenumber, email, address);
                if (success)
                {                   
                    ShowNotification("ویرایش اطلاعات با موفقیت انجام شد", "success");
                    ClientRefresh(); // لیست رو رفرش کن
                    SaveChanges();
                }
                else
                {
                    ShowNotification("خطا در ویرایش اطلاعات!", "error");
                }
            }
            else
            {
                ShowNotification("تلفن همراه نامعتبر است!", "error");
            }
        }
        #endregion

        #region Interactions
        private void SearchInteractions_TextChanged(object sender, TextChangedEventArgs e)
        {
            string searchName = SearchInteractions.Text.Trim();
            List<InteractionDto> filtered;
            if (string.IsNullOrWhiteSpace(searchName))
            {           
                InteractionsPlaceholderText.Visibility = Visibility.Visible;
                filtered = ibll.GetInteractionsForListView(); // تمام تعامل‌ها
                InteractionsListView.ItemsSource = filtered;
            }
            else
            {
                filtered = ibll.SearchInteractionForDGV(searchName); // جستجو
                InteractionsPlaceholderText.Visibility = Visibility.Hidden;
                InteractionsListView.ItemsSource = filtered;
            }           
        }
        private void chkReminder_Checked(object sender, RoutedEventArgs e)
        {
            ReminderDateTimeDTP.IsEnabled = true;
        }
        private void chkReminder_Unchecked(object sender, RoutedEventArgs e)
        {
            ReminderDateTimeDTP.IsEnabled = false;
            ReminderDateTimeDTP.SelectedDateTime = null;
        }
        private void InteractionsDetailsPopup_Opened(object sender, EventArgs e)
        {
            MainGrid.Effect = new BlurEffect() { Radius = 5 };
        }
        private void InteractionsDetailsPopup_Closed(object sender, EventArgs e)
        {
            MainGrid.Effect = null;
        }      
        private void ShowInteractionDetails_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            if (btn?.Tag is int interactionId)
            {
                InteractionsDetailsPopup.Tag = interactionId;

                var interaction = ibll.GetInteractionById(interactionId);

                InteractionType.Text = interaction.Type;
                InteractionNote.Text = interaction.Note;
                ReminderCheckBoxPopup.IsChecked = interaction.IsReminderSet;
                ReminderDatePopup.SelectedDate = interaction.ReminderDate;

                InteractionsDetailsPopup.IsOpen = true;
            }            
        }      
        private void InteractionClosePopupBTN_Click(object sender, RoutedEventArgs e)
        {
            InteractionsDetailsPopup.IsOpen = false;
        }
        private void ReminderCheckBoxPopup_Checked(object sender, RoutedEventArgs e)
        {
            ReminderDatePopup.IsEnabled = true;
        }
        private void ReminderCheckBoxPopup_Unchecked(object sender, RoutedEventArgs e)
        {
            ReminderDatePopup.IsEnabled = false;
            ReminderDatePopup.SelectedDate = null;
        }
        private void AddInteractionsBTN_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(SelectClientInteractionCB.Text) || string.IsNullOrWhiteSpace(InteractionTypeCB.Text) || string.IsNullOrWhiteSpace(InteractionDateDTP.Text))
            {
                ShowNotification("اطلاعات ضروری را وارد کنید!", "error");
            }
            else
            {
                try
                {
                    // بررسی وجود موکل در ComboBox
                    if (SelectClientInteractionCB.SelectedItem == null)
                    {
                        ShowNotification("موکل در سیستم ثبت نشده است!", "error");
                        return;
                    }
                    var user = AppSession.CurrentUser;
                    var newInteraction = new Interaction
                    {
                        UserId = user.Id,
                        UserRole = (int)user.Role,
                        ClientId = (int)SelectClientInteractionCB.SelectedValue,
                        Type = InteractionTypeCB.Text,
                        InteractionDate = InteractionDateDTP.Text,
                        Note = NotesTB.Text,
                        CreatedAt = DateTime.Now,
                        IsReminderSet = ReminderCheckBox.IsChecked == true,
                        ReminderDate = ReminderCheckBox.IsChecked == true ? ReminderDateTimeDTP.SelectedDateTime : null,
                        IsReminderDone = false
                    };
                    int interactionId = ibll.Create(newInteraction);

                    if (interactionId > 0)
                    {
                        ShowNotification("تعامل با موفقیت ثبت شد", "success");

                        if (ReminderCheckBox.IsChecked == true && ReminderDateTimeDTP.SelectedDateTime != null)
                        {
                            var reminder = new Reminder
                            {
                                Title = $"یادآور تعامل با {SelectClientInteractionCB.Text}",
                                Description = NotesTB.Text,
                                ReminderDate = ReminderDateTimeDTP.SelectedDateTime.Value,
                                UserId = user.Id,
                                InteractionId = interactionId
                            };
                            rbll.Create(reminder);
                            var bulkSendResult = smsIr.BulkSendAsync(30007732011420, $"یادآوری تعامل با موکل: {SelectClientInteractionCB.Text} \nنوع تعامل: {InteractionTypeCB.Text} \nتاریخ تعامل بعدی: {ReminderDateTimeDTP.SelectedDateTime.Value.ToString("yyyy/MM/dd ساعت: HH:mm")}\n\n نرم افزار حقوقی هیام", new string[] { user.PhoneNumber });
                        }
                        InteractionRefresh();
                        SaveChanges();
                        ClearControls(MainGrid);
                    }
                    else
                    {
                        ShowNotification("خطا در ثبت تعامل!", "error");
                    }                  
                }
                catch (Exception)
                {
                    ShowNotification("موکل در سیستم موجود نمیباشد!", "error");
                }
            }
        }
        private void InteractionSaveChangesBTN_Click(object sender, RoutedEventArgs e)
        {
            if (!(InteractionsDetailsPopup.Tag is int interactionId))
            {
                ShowNotification("آیدی تعامل مشخص نیست!", "error");
                return;
            }

            var interaction = ibll.GetInteractionById(interactionId);
            if (interaction == null)
            {
                ShowNotification("تعامل موردنظر یافت نشد!", "error");
                return;
            }

            interaction.Note = InteractionNote.Text;
            interaction.IsReminderSet = ReminderCheckBoxPopup.IsChecked == true;
            interaction.ReminderDate = ReminderCheckBoxPopup.IsChecked == true ? ReminderDatePopup.SelectedDate : null;
            interaction.IsReminderDone = false;
            var result = ibll.UpdateInteraction(interaction);
            ShowNotification(result, "success");

            InteractionRefresh(); // بازخوانی لیست
            SaveChanges();
            InteractionsDetailsPopup.IsOpen = false;
        }
        #endregion
        private void CloseBTN_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }    
}