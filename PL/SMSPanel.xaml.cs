using BE;
using BE.ViewModel;
using BLL;
using IPE.SmsIrClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Effects;

namespace Heyam
{  
    public partial class SMSPanel : Window
    {
        public SMSPanel()
        {
            InitializeComponent();
            SingleSMSRefresh();
            LoadGroupClients();
            GroupSMSRefresh();

            #region Clients FullNames In Interaction ComboBox
            var clients = cbll.GetAllClients(); // از BLL           
            SingleSendSelectClientCB.ItemsSource = clients;
            SingleSendSelectClientCB.DisplayMemberPath = "FullName"; // فرض بر اینکه پراپرتی FullName داری
            SingleSendSelectClientCB.SelectedValuePath = "Id";
            #endregion
        }
        Client_BLL cbll = new Client_BLL();
        SmsPanel_BLL smsbll = new SmsPanel_BLL();
        private Guid _lastGroupId;
        SmsIr smsIr = new SmsIr("XwrI6iBRvxaWJ4AVcqaQddyRF5Ux3iHesb7XuQGbcSVag4Ut");

        #region Methods      
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
        public void SingleSMSRefresh()
        {         
            SingleSMSDGV.ItemsSource = null;
            SingleSMSDGV.ItemsSource = smsbll.GetAllForSingleSMS();
        }
        private void LoadGroupClients()
        {
            var clientList = smsbll.GetAllForGroupClientCheckList();
            GroupClientCheckList.ItemsSource = clientList;
        }
        public void GroupSMSRefresh()
        {        
            GroupSMSDGV.ItemsSource = null;
            GroupSMSDGV.ItemsSource = smsbll.GetAllForGroupSMS();
        }
        #endregion

        #region SingleSMS
        string clientPhonenumber = "";
        private void SingleSendSelectClientCB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (SingleSendSelectClientCB.SelectedItem is Client selectedclient)
            {
                SingleSendShowClientNameText.Text = $"تلفن همراه: {selectedclient.PhoneNumber ?? "ناشناخته"}";
                clientPhonenumber = selectedclient.PhoneNumber;
            }
            else
            {
                SingleSendShowClientNameText.Text = "";
            }
        }
        private void SingleMessageTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(SingleMessageTB.Text))
            {
                SingleSendMessagePlaceholderText.Visibility = Visibility.Visible;
            }
            else
            {
                SingleSendMessagePlaceholderText.Visibility = Visibility.Collapsed;
            }
        }
        private void ShowHide_SingleSMSListBTN_Click(object sender, RoutedEventArgs e)
        {
            if (SingleSMSDGV.Visibility == Visibility.Collapsed && SingleSearchBox.Visibility == Visibility.Collapsed && SingleSendSearchPlaceholderText.Visibility == Visibility.Collapsed)
            {
                // نمایش جدول و پنهان‌سازی فرم
                SingleSMSDGV.Visibility = Visibility.Visible;
                SingleSearchBox.Visibility = Visibility.Visible;
                SingleSendSearchPlaceholderText.Visibility = Visibility.Visible;
                SinglePanelGB.Visibility = Visibility.Collapsed;
            }
            else
            {
                // پنهان‌سازی جدول و نمایش فرم
                SingleSMSDGV.Visibility = Visibility.Collapsed;
                SingleSearchBox.Visibility = Visibility.Collapsed;
                SingleSendSearchPlaceholderText.Visibility = Visibility.Collapsed;
                SinglePanelGB.Visibility = Visibility.Visible;
            }
        }
        private void SingleSearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string searchName = SingleSearchBox.Text.Trim();
            List<SMSPanelDto> filtered;
            List<SMSPanelDto> filtered1;          
            if (string.IsNullOrWhiteSpace(searchName))
            {
                SingleSendSearchPlaceholderText.Visibility = Visibility.Visible;
                filtered = smsbll.GetAllForSingleSMS(); 
                filtered1 = smsbll.GetAllForGroupSMS(); 
                SingleSMSDGV.ItemsSource = filtered;
                GroupSMSDGV.ItemsSource = filtered1;
            }
            else
            {
                filtered = smsbll.SearchForSingleSMS(searchName);
                filtered1 = smsbll.SearchForGroupSMS(searchName); 
                SingleSendSearchPlaceholderText.Visibility = Visibility.Hidden;
                SingleSMSDGV.ItemsSource = filtered;
                GroupSMSDGV.ItemsSource = filtered1;
            }
        }
        private async void SingleSendSmsBTN_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(SingleSendSelectClientCB.Text) || string.IsNullOrWhiteSpace(SingleMessageTB.Text))
            {
                ShowNotification("اطلاعات ضروری را وارد کنید!", "error");
            }
            else
            {
                try
                {
                    // بررسی وجود موکل در ComboBox
                    if (SingleSendSelectClientCB.SelectedItem == null)
                    {
                        ShowNotification("موکل در سیستم ثبت نشده است!", "error");
                        return;
                    }
                    var user = AppSession.CurrentUser;

                    var smsText = $"موکل گرامی {SingleSendSelectClientCB.Text}\n{SingleMessageTB.Text}";
                    bool isSuccess = true;
                    try
                    {
                        await smsIr.BulkSendAsync(30007732011420, smsText, new string[] { clientPhonenumber });
                    }
                    catch (Exception)
                    {
                        isSuccess = false;
                    }
                    var newSmspanel = new SmsPanel
                    {
                        UserId = user.Id,
                        UserRole = (int)user.Role,
                        ClientId = (int)SingleSendSelectClientCB.SelectedValue,
                        Message = SingleMessageTB.Text,
                        SentAt = DateTime.Now,
                        SenderLineNumber = "30007732011420",
                        IsSuccess = isSuccess
                    };
                    ShowNotification(smsbll.Add(newSmspanel), "success");
                    SingleSMSRefresh();
                    ClearControls(MainGrid);
                }
                catch (Exception)
                {
                    ShowNotification("موکل در سیستم موجود نمیباشد!", "error");
                }
            }
        }
        private void ShowSingleSMSDetailsBTN_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            if (btn?.Tag is int SingleSMSId)
            {
                SingleSMSDetailsPopup.Tag = SingleSMSId;
                var _SingleSMSId = smsbll.GetSMSById(SingleSMSId);
                SingleMessageTBPopup.Text = _SingleSMSId.Message;
                SingleSMSDetailsPopup.Visibility = Visibility.Visible;
                MainGrid.Effect = new BlurEffect() { Radius = 5 };
            }
        }
        private void SingleMessageDetailsPopupCloseBTN_Click(object sender, RoutedEventArgs e)
        {
            SingleSMSDetailsPopup.Visibility = Visibility.Collapsed;
            MainGrid.Effect = null;
        }
        #endregion

        #region GroupSMS
        private void GroupMessageTB_TextChanged(object sender, TextChangedEventArgs e)
        {
            GroupSendMessagePlaceholderText.Visibility =  string.IsNullOrWhiteSpace(GroupMessageTB.Text) ? Visibility.Visible : Visibility.Hidden;
        }
        private void GroupSearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string searchName = GroupSearchBox.Text.Trim();
            List<SMSPanelDto> filtered;
            if (string.IsNullOrWhiteSpace(searchName))
            {
                GroupSendSearchPlaceholderText.Visibility = Visibility.Visible;
                filtered = smsbll.GetAllForGroupClientCheckList(); // تمام تعامل‌ها
                GroupClientCheckList.ItemsSource = filtered;
            }
            else
            {
                filtered = smsbll.SearchClientsForGroupSms(searchName); // جستجو
                GroupSendSearchPlaceholderText.Visibility = Visibility.Hidden;
                GroupClientCheckList.ItemsSource = filtered;
            }
        }
        private void ShowHide_GroupSMSListBTN_Click(object sender, RoutedEventArgs e)
        {
            if (GroupSMSDGV.Visibility == Visibility.Collapsed && SingleSearchBox.Visibility == Visibility.Collapsed && SingleSendSearchPlaceholderText.Visibility == Visibility.Collapsed)
            {
                // نمایش جدول و پنهان‌سازی فرم
                GroupSMSDGV.Visibility = Visibility.Visible;
                SingleSearchBox.Visibility = Visibility.Visible;
                SingleSendSearchPlaceholderText.Visibility = Visibility.Visible;
                GroupPanelGP.Visibility = Visibility.Collapsed;
            }
            else
            {
                // پنهان‌سازی جدول و نمایش فرم
                GroupSMSDGV.Visibility = Visibility.Collapsed;
                SingleSearchBox.Visibility = Visibility.Collapsed;
                SingleSendSearchPlaceholderText.Visibility = Visibility.Collapsed;
                GroupPanelGP.Visibility = Visibility.Visible;
            }
        }
        private void SelectAllCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            var clients = GroupClientCheckList.ItemsSource as List<SMSPanelDto>;
            if (clients != null)
            {
                foreach (var client in clients)
                {
                    client.IsSelected = true;
                }

                // برای بروزرسانی UI
                GroupClientCheckList.Items.Refresh();
            }
        }
        private void SelectAllCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            var clients = GroupClientCheckList.ItemsSource as List<SMSPanelDto>;
            if (clients != null)
            {
                foreach (var client in clients)
                {
                    client.IsSelected = false;
                }

                GroupClientCheckList.Items.Refresh();
            }
        }
        private async void GroupSendSmsBTN_Click(object sender, RoutedEventArgs e)
        {
            var groupId = Guid.NewGuid();
            _lastGroupId = groupId; // 👈 این خط را اضافه کن

            var selectedClients = GroupClientCheckList.ItemsSource
            .OfType<SMSPanelDto>()
            .Where(c => c.IsSelected)
            .ToList();

            string messageText = GroupMessageTB.Text.Trim();

            if (selectedClients.Count == 0 || string.IsNullOrWhiteSpace(messageText))
            {
                ShowNotification("موکل یا متن پیامک انتخاب نشده است!", "error");
                return;
            }

            var user = AppSession.CurrentUser;
            foreach (var client in selectedClients)
            {
                var message = $"موکل گرامی {client.ClientName}\n{messageText}";

                bool isSuccess = true;
                try
                {
                    await smsIr.BulkSendAsync(30007732011420, message, new string[] { client.PhoneNumber });
                }
                catch (Exception)
                {
                    isSuccess = false;
                }
                var newSmsGrouppanel = new SmsPanel
                {
                    UserId = user.Id,
                    UserRole = (int)user.Role,
                    ClientId = client.Id,
                    Message = message,
                    SentAt = DateTime.Now,
                    SenderLineNumber = "30007732011420",
                    BulkGroupId = groupId,
                    IsSuccess = isSuccess
                };
                smsbll.Add(newSmsGrouppanel);
            }
            ShowNotification("پیامک‌ها با موفقیت ارسال و ثبت شدند", "success");
            GroupSMSRefresh();
            ClearControls(MainGrid);
        }
        private void ShowGroupSMSDetailsBTN_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            if (btn?.Tag is int SingleSMSId)
            {
                GroupSMSDetailsPopup.Tag = SingleSMSId;
                var _SingleSMSId = smsbll.GetSMSById(SingleSMSId);
                GroupMessageTBPopup.Text = _SingleSMSId.Message;
                GroupSMSDetailsPopup.Visibility = Visibility.Visible;
                MainGrid.Effect = new BlurEffect() { Radius = 5 };
            }
        }
        private void GroupMessageDetailsPopupCloseBTN_Click(object sender, RoutedEventArgs e)
        {
            GroupSMSDetailsPopup.Visibility = Visibility.Collapsed;
            MainGrid.Effect = null;
        }
        #endregion

        private void CloseBTN_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }       
    }
}