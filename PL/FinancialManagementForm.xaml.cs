using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Effects;
using LiveCharts.Wpf;
using LiveCharts;
using System.Windows.Media.Imaging;
using System.Linq;
using BLL;
using BE.ViewModel;
using BE;
using System.Globalization;
using HandyControl.Tools.Extension;
using PdfSharp.Pdf;
using PdfSharp.Drawing;
using PL;
using IPE.SmsIrClient;

namespace Heyam
{
    public partial class FinancialManagementForm : Window
    {
        private MainWindow _mainWindow;

        Payment_BLL pbll = new Payment_BLL();
        Client_BLL cbll = new Client_BLL();
        Contract_BLL contract_bll = new Contract_BLL();
        LawyerExpense_BLL lebll = new LawyerExpense_BLL();
        Reminder_BLL rbll = new Reminder_BLL();

        #region PaymentAttachmentList
        private List<PaymentAttachment> attachments = new List<PaymentAttachment>();
        private List<PaymentAttachment> deletedAttachments = new List<PaymentAttachment>();
        private List<PaymentAttachment> popupattachments = new List<PaymentAttachment>();       
        #endregion

        public FinancialManagementForm(MainWindow mainWindow)
        {
            InitializeComponent();
            RefreshPayment();        
            RefreshExpense();
            _mainWindow = mainWindow;

            RefreshContractsReport();
            RefreshPaymentsReport();
            RefreshExpenseReport();
            PopulateYears();
            
            #region Fill ClientName & CaseNumber Field ComboBox
            var clients = cbll.GetAllClients(); // از BLL           
            SelectClientNameCB.ItemsSource = clients;           
            SelectClientNameCB.DisplayMemberPath = "FullName"; // فرض بر اینکه پراپرتی FullName داری
            SelectClientNameCB.SelectedValuePath = "Id";           
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

                // پاک کردن تمام آیتم‌های لیست باکس پیوست‌ها
                attachments.Clear();              

                if (child is System.Windows.Controls.TextBox textBox)
                    textBox.Clear();
                else if (child is System.Windows.Controls.ComboBox comboBox)
                    comboBox.Text = "";
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
        public void RefreshPayment()
        {
            PaymentDGV.ItemsSource = null;
            PaymentDGV.ItemsSource = pbll.GetPaymentsForListView();
            PaymentTotalRecordsText.Text = pbll.GetCount().ToString();
            PaymentTotalPaidText.Text = string.Format("{0:N0} ريال",pbll.GetTotalAmount());
        }
        private void RefreshFilesList()
        {
            FilesList.ItemsSource = null;
            FilesList.ItemsSource = attachments;
        }
        private void RefreshPopupFilesList()
        {
            PopupFilesList.ItemsSource = null;
            PopupFilesList.ItemsSource = popupattachments;
        }
        private Payment _payment;
        private void LoadPaymentForEdit(Payment paymentToEdit)
        {
            _payment = paymentToEdit;
            popupattachments = paymentToEdit.PaymentAttachments?.ToList() ?? new List<PaymentAttachment>();
            RefreshPopupFilesList();
        }
        public void RefreshExpense()
        {
            ExpensesDGV.ItemsSource = null;
            ExpensesDGV.ItemsSource = lebll.GetAllExpenseForListView();
            ExpenseTotalCountText.Text = lebll.GetCount().ToString();
            ExpenseTotalAmountText.Text = string.Format("{0:N0} ريال", lebll.GetTotalAmount());          
        }     
        public void RefreshContractsReport()
        {       
            ReportContractsDGV.ItemsSource = null;
            ReportContractsDGV.ItemsSource = contract_bll.GetContractsForListView();
            TotalRecordsText.Text = contract_bll.GetCount().ToString();
            TotalPaidText.Text = string.Format("{0:N0} ريال", contract_bll.GetTotalAmount());
        }
        public void RefreshPaymentsReport()
        {
            PaymentReportDGV.ItemsSource = null;
            PaymentReportDGV.ItemsSource = pbll.GetPaymentsForListView();
            PaymentReportTotalRecordsText.Text = pbll.GetCount().ToString();
            PaymentReportTotalPaidText.Text = string.Format("{0:N0} ريال", pbll.GetTotalAmount());
        }
        public void RefreshExpenseReport()
        {
            ReportExpensesDGV.ItemsSource = null;
            ReportExpensesDGV.ItemsSource = lebll.GetAllExpenseForListView();
            ReportExpenseTotalCountText.Text = lebll.GetCount().ToString();
            ReportExpenseTotalAmountText.Text = string.Format("{0:N0} ريال", lebll.GetTotalAmount());
        }        
        private void DrawChart(List<ChartDataPoint> data)
        {            
            ReportChart.Series = new SeriesCollection
            {
                new ColumnSeries
                {
                    Title = "درآمد",
                    Values = new ChartValues<double>(data.Select(d => (double)d.Income))
                },
                new ColumnSeries
                {
                    Title = "هزینه",
                    Values = new ChartValues<double>(data.Select(d => (double)d.Cost))
                },
                new LineSeries
                {                   
                    PointForeground = new SolidColorBrush(Colors.ForestGreen),
                    Title = "سود",
                    Values = new ChartValues<double>(data.Select(d => (double)d.Profit))
                }
            };

            ReportChart.AxisX.Clear();
            ReportChart.AxisX.Add(new Axis
            {
                Labels = data.Select(d => d.Label).ToArray(),
                Title = "بازه",
                
            });

            ReportChart.AxisY.Clear();
            ReportChart.AxisY.Add(new Axis
            {
                Title = "مبلغ (ریال)",
                LabelFormatter = value =>
                {
                    var absValue = Math.Abs(value);
                    var formatted = absValue.ToString("N0"); // عدد بدون علامت منفی، دو رقم اعشار
                    return value < 0 ? "-" + formatted + " ریال" : formatted + " ریال";
                },                     
            });
        }
        private void PopulateYears()
        {
            int currentYear = new PersianCalendar().GetYear(DateTime.Now);

            for (int year = currentYear - 14; year <= currentYear + 96; year++)
            {
                YearFromCB.Items.Add(year);
                YearToCB.Items.Add(year);
            }

            YearFromCB.SelectedItem = currentYear - 3;
            YearToCB.SelectedItem = currentYear;
        }
        #endregion

        #region Payments
        private void CloseBTN_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        private void ShowFile_Click(object sender, RoutedEventArgs e)
        {
            if (FilesList.SelectedItem is BE.PaymentAttachment selectedAttachment)
            {
                try
                {
                    System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                    {
                        FileName = selectedAttachment.FilePath,
                        UseShellExecute = true
                    });
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"خطا در باز کردن فایل: {ex.Message}");
                }
            }
            else
            {
                MessageBox.Show("لطفاً یک فایل را انتخاب کنید.");
            }
        }
        private void DropBoxDocuments_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                DropBoxDocuments.Background = new SolidColorBrush(Color.FromRgb(220, 240, 255)); // تغییر رنگ پس‌زمینه هنگام ورود فایل
                e.Effects = DragDropEffects.Copy;
            }
            else
            {
                e.Effects = DragDropEffects.None;
            }
        }
        private void DropBoxDocuments_DragLeave(object sender, DragEventArgs e)
        {
            DropBoxDocuments.Background = new SolidColorBrush(Color.FromRgb(230, 244, 255)); // بازگرداندن رنگ اولیه
        }
        private void DropBoxDocuments_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

                foreach (var file in files)
                {
                    // بررسی تکراری نبودن فایل بر اساس مسیر فایل اصلی
                    if (!attachments.Any(a => a.FilePath == file))
                    {
                        attachments.Add(new PaymentAttachment
                        {
                            FileName = Path.GetFileName(file),
                            FilePath = file // مسیر اصلی فعلاً
                        });
                    }
                }
                RefreshFilesList();
            }
            ShowDocBTN.IsEnabled = FilesList.SelectedItem != null;          
        }
        private void FilesList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ShowDocBTN.IsEnabled = FilesList.SelectedItem != null; // دکمه فقط وقتی فعال شود که فایلی انتخاب شده باشد
        }
        private void AddDocBTN_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new Microsoft.Win32.OpenFileDialog
            {
                Multiselect = true,
                Filter = "All Files|*.*"
            };
            if (openFileDialog.ShowDialog() == true)
            {
                foreach (var path in openFileDialog.FileNames)
                {
                    string fileName = Path.GetFileName(path);

                    if (!attachments.Any(a => a.FileName == fileName && a.FilePath == path)) // FilePath فعلاً مسیر موقت
                    {
                        attachments.Add(new PaymentAttachment
                        {
                            FileName = fileName,
                            FilePath = path, // ⛔ فعلاً فایل رو جابجا نکن، فقط آدرس موقتی نگه دار
                            UploadedAt = DateTime.Now
                        });
                    }
                }
                RefreshFilesList();
            }           
        }
        private void DeleteFile_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (button?.Tag is BE.PaymentAttachment attachmentToDelete)
            {
                try
                {
                    // حذف از لیست
                    attachments.Remove(attachmentToDelete);
                    // رفرش مجدد لیست
                    FilesList.ItemsSource = null;
                    FilesList.ItemsSource = attachments;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"خطا در حذف فایل: {ex.Message}", "خطا", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string keyword = SearchBox.Text.Trim();
            int? statusFilter = null;
            List<PaymentDto> filtered;

            if (string.IsNullOrWhiteSpace(keyword))
            {
                SearchPlaceholderText.Visibility = Visibility.Visible;
                filtered = pbll.GetPaymentsForListView(); // دریافت همه رکوردها بدون فیلتر
            }
            else
            {
                filtered = pbll.SearchPaymentsForDGV(keyword, statusFilter); // جستجو و فیلتر همزمان                
                SearchPlaceholderText.Visibility = Visibility.Hidden;
            }
            PaymentDGV.ItemsSource = filtered;
        }
        private void ReminderFilterCB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string searchTerm = SearchBox?.Text?.Trim() ?? string.Empty;
            int? statusFilter = null;
            if (int.TryParse(ReminderFilterCB.SelectedValue?.ToString(), out int parsedStatus))
            {
                statusFilter = parsedStatus;
            }
            var result = pbll.SearchPaymentsForDGV(searchTerm, statusFilter);
            PaymentDGV.ItemsSource = null;
            PaymentDGV.ItemsSource = result;
        }
        private void SelectClientNameCB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (SelectClientNameCB.SelectedItem is Client selectedClient)
            {
                // به شرط اینکه Case شامل Client باشد
                SelectCaseNumberCB.ItemsSource = selectedClient.Cases;
                SelectCaseNumberCB.DisplayMemberPath = "CaseNumber";
                SelectCaseNumberCB.SelectedValuePath = "Id";
            }
        }
        private void AmountTB_TextChanged(object sender, TextChangedEventArgs e)
        {
            // اگر متن خالی است یا متن همان "ریال" است، ادامه نده
            if (string.IsNullOrWhiteSpace(AmountTB.Text) || AmountTB.Text == "ریال")
                return;

            // مکان‌نما را ذخیره می‌کنیم تا بتوانیم بعد از فرمت‌دهی آن را تنظیم کنیم
            int selectionStart = AmountTB.SelectionStart;

            // حذف کاراکترهای غیر عددی (مثل کاما و ریال)
            string rawText = AmountTB.Text.Replace(",", "").Replace(" ریال", "");

            // چک کردن برای اطمینان از اینکه متن فقط شامل اعداد است
            if (decimal.TryParse(rawText, out decimal amount))
            {
                // فرمت دادن به عدد به صورت سه‌رقمی و اضافه کردن "ریال" در انتها
                AmountTB.Text = string.Format("{0:N0} ", amount);

                // بازگرداندن مکان‌نما به موقعیت قبلی
                AmountTB.SelectionStart = AmountTB.Text.Length - 1; // از انتهای متن 5 کاراکتر برای " ریال" کم می‌کنیم
            }
        }
        private void PopupFilesList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            PopupShowDocBTN.IsEnabled = PopupFilesList.SelectedItem != null; // دکمه فقط وقتی فعال شود که فایلی انتخاب شده باشد
        }
        private void PopupShowDocBTN_Click(object sender, RoutedEventArgs e)
        {
            if (PopupFilesList.SelectedItem is BE.PaymentAttachment selectedAttachment)
            {
                try
                {
                    System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                    {
                        FileName = selectedAttachment.FilePath,
                        UseShellExecute = true
                    });
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"خطا در باز کردن فایل: {ex.Message}");
                }
            }
            else
            {
                MessageBox.Show("لطفاً یک فایل را انتخاب کنید.");
            }
        }
        private void PaymentStatusCB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (PaymentStatusCB.SelectedIndex == 0)
            {
                ReminderDebtCheckBox.IsEnabled = true;
            }
            else
            {
                ReminderDebtCheckBox.IsChecked = false;
                ReminderDebtCheckBox.IsEnabled = false;
                ReminderDP.SelectedDate = null;
            }
        }
        private void PopupPaymentStatusCheckBox_Checked(object sender, RoutedEventArgs e)
        {          
            PopupAmountTB.IsEnabled = true;
            PopupReminderSetCheckBox.IsEnabled = false;
            PopupReminderSetDP.IsEnabled = false;
            PopupReminderSetDP.SelectedDate = null;
        }
        private void PopupPaymentStatusCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            PopupAmountTB.IsEnabled = false;
            PopupReminderSetCheckBox.IsEnabled = true;
            PopupAmountTB.Text = "";
        }
        private void PopupReminderSetCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            PopupPaymentStatusCheckBox.IsEnabled = false;
            PopupReminderSetDP.IsEnabled = true;
            PopupAmountTB.Text = "";
        }
        private void PopupReminderSetCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            PopupPaymentStatusCheckBox.IsEnabled = true;
            PopupAmountTB.IsEnabled = false;
            PopupReminderSetDP.IsEnabled = false;
            PopupReminderSetDP.SelectedDate = null;
        }
        private void PopupAmountTB_TextChanged(object sender, TextChangedEventArgs e)
        {
            // اگر متن خالی است یا متن همان "ریال" است، ادامه نده
            if (string.IsNullOrWhiteSpace(PopupAmountTB.Text) || PopupAmountTB.Text == "ریال")
                return;

            // مکان‌نما را ذخیره می‌کنیم تا بتوانیم بعد از فرمت‌دهی آن را تنظیم کنیم
            int selectionStart = PopupAmountTB.SelectionStart;

            // حذف کاراکترهای غیر عددی (مثل کاما و ریال)
            string rawText = PopupAmountTB.Text.Replace(",", "").Replace(" ریال", "");

            // چک کردن برای اطمینان از اینکه متن فقط شامل اعداد است
            if (decimal.TryParse(rawText, out decimal amount))
            {
                // فرمت دادن به عدد به صورت سه‌رقمی و اضافه کردن "ریال" در انتها
                PopupAmountTB.Text = string.Format("{0:N0} ", amount);

                // بازگرداندن مکان‌نما به موقعیت قبلی
                PopupAmountTB.SelectionStart = PopupAmountTB.Text.Length - 1; // از انتهای متن 5 کاراکتر برای " ریال" کم می‌کنیم
            }
        }
        private void CancelPaymentBTN_Click(object sender, RoutedEventArgs e)
        {
            AddPaymentPopup.Visibility = Visibility.Collapsed;
            MainGrid.Effect = null;
            ClearControls(RegPaymentPanel);
            ShowDocBTN.IsEnabled = false;
        }
        private void AddPaymentBTN_Click(object sender, RoutedEventArgs e)
        {
            // نمایش پاپ‌آپ
            AddPaymentPopup.Visibility = Visibility.Visible;
            MainGrid.Effect = new BlurEffect { Radius = 5 };
        }
        private void PaymentDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // بررسی اینکه آیا رکوردی انتخاب شده است
            if (PaymentDGV.SelectedItem != null)
            {
                //popupPaymentDetails.IsOpen = true;
                PrintInvoiceBTN.IsEnabled = true; // دکمه فعال شود
            }
            else
            {
                PrintInvoiceBTN.IsEnabled = false; // دکمه غیرفعال شود
            }
        }
        private void ReminderDebtCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            ReminderDP.IsEnabled = true;
        }
        private void ReminderDebtCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            ReminderDP.IsEnabled = false;
            ReminderDP.SelectedDate = null;
        }
        private void PaymentDetailsPopupCloseBTN_Click(object sender, RoutedEventArgs e)
        {
            PaymentDetailsPopup.Visibility = Visibility.Collapsed;
            MainGrid.Effect = null;
        }       
        private void SubmitPaymentBTN_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(SelectClientNameCB.Text) || string.IsNullOrWhiteSpace(ServiceCB.Text) || string.IsNullOrWhiteSpace(AmountTB.Text) || string.IsNullOrWhiteSpace(PaymentDP.Text) || string.IsNullOrWhiteSpace(PaymentTypeCB.Text))
            {
                ShowNotification("لطفاً فیلدهای ضروری را پر کنید", "warning");
                return;
            }
            if (SelectClientNameCB.SelectedItem == null)
            {
                ShowNotification("موکل در سیستم موجود نمیباشد!", "error");
                return;
            }
            foreach (var attachment in attachments)
            {
                string clientName = SelectClientNameCB.Text.Trim(); // یا هر فیلدی که نمایش نام موکل را دارد
                string paymentStatus = PaymentStatusCB.Text.Trim();

                string documentsPath = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                    "Heyam", "PaymentAttachments", clientName, paymentStatus);

                if (!Directory.Exists(documentsPath))
                    Directory.CreateDirectory(documentsPath);

                string targetPath = Path.Combine(documentsPath, attachment.FileName);

                // فقط اگر فایل هنوز کپی نشده
                if (!File.Exists(targetPath))
                {
                    File.Copy(attachment.FilePath, targetPath, true);
                }

                attachment.FilePath = targetPath; // آدرس جدید فایل رو ست کن که ذخیره بشه
            }
            var user = AppSession.CurrentUser;
            var newPayment = new Payment
            {
                UserId = user.Id,
                UserRole = (int)user.Role,
                ClientId = (int)SelectClientNameCB.SelectedValue,
                CaseId = SelectCaseNumberCB.SelectedValue as int?,
                Service = ServiceCB.SelectedIndex,
                PaymentStatus = PaymentStatusCB.SelectedIndex,
                Amount = Convert.ToDecimal(AmountTB.Text),
                PaymentDate = PaymentDP.SelectedDate.Value,
                PaymentType = PaymentTypeCB.Text,
                Description = DescriptionTB.Text,
                PaymentAttachments = attachments.ToList(),
                IsReminderSet = ReminderDebtCheckBox.IsChecked == true,
                DueDate = ReminderDebtCheckBox.IsChecked == true ? ReminderDP.SelectedDate : null,
                IsReminderDone = false
            };
            if (ReminderDebtCheckBox.IsChecked == true && ReminderDP.Text == "")
            {
                ShowNotification("تاریخ را وارد کنید!", "error");
            }
            else
            {
                int paymentId = pbll.Create(newPayment);
                if (paymentId > 0)
                {
                    ShowNotification("پرداختی با موفقیت ثبت شد", "success");

                    if (ReminderDebtCheckBox.IsChecked == true && ReminderDP.SelectedDate.Value != null)
                    {
                        var reminder = new Reminder
                        {
                            Title = $"یادآور پرداختی برای {SelectClientNameCB.Text}",
                            Description = DescriptionTB.Text,
                            ReminderDate = ReminderDP.SelectedDate.Value,
                            UserId = user.Id,
                            PaymentId = paymentId
                        };
                        rbll.Create(reminder);
                        var bulkSendResult = smsIr.BulkSendAsync(30007732011420, $"یادآوری پرداختی برای موکل: {SelectClientNameCB.Text}\n خدمات: {ServiceCB.Text}\n وضعیت پرداخت: {PaymentStatusCB.Text} \nمبلغ پرداختی: {AmountTB.Text}\n روش پرداختی: {PaymentTypeCB.Text}\n تاریخ پرداخت: {PaymentDP.SelectedDate.Value.ToString("yyyy/MM/dd")}\n\n نرم افزار حقوقی هیام", new string[] { user.PhoneNumber });
                    }
                    RefreshPayment();
                    RefreshPaymentsReport();
                    SaveChanges();
                    ClearControls(RegPaymentPanel);
                    AddPaymentPopup.Visibility = Visibility.Collapsed;
                    MainGrid.Effect = null;
                    ReminderDebtCheckBox.IsChecked = false;
                    ReminderDP.SelectedDate = null;
                }
                else
                {
                    ShowNotification("خطا در ثبت پرداختی!", "error");
                }           
            }
        }
        private void ShowPaymentsDetailsBTN_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            if (btn?.Tag is int paymentId)
            {
                PaymentDetailsPopup.Tag = paymentId;
                var _payment = pbll.GetPaymentId(paymentId);
                ClientCaseNumberText.Text = _payment.Case.CaseNumber;
                AmountText.Text = string.Format("{0:N0} ریال", _payment.Amount);
                PaymentDateText.Text = _payment.PaymentDate.ToString("yyyy/MM/dd");
                PaymentTypeText.Text = _payment.PaymentType;
                DescriptionText.Text = _payment.Description;
                PopupReminderSetDP.SelectedDate = _payment.DueDate;
                LoadPaymentForEdit(_payment);

                if (_payment.PaymentStatus == 1)
                {
                    PopupPaymentStatusCheckBox.IsChecked = true;
                    PopupPaymentStatusCheckBox.IsEnabled = false;
                    PopupReminderSetCheckBox.IsEnabled = false;
                    if (PopupPaymentStatusCheckBox.IsChecked == true)
                    {
                        PopupAmountTB.IsEnabled = false;
                    }
                }
                else
                {
                    PopupPaymentStatusCheckBox.IsChecked = false;
                    PopupPaymentStatusCheckBox.IsEnabled = true;
                    PopupReminderSetCheckBox.IsEnabled = true;
                }
                PaymentDetailsPopup.Visibility = Visibility.Visible;
                MainGrid.Effect = new BlurEffect { Radius = 5 };
            }
        }
        private void PaymentDetailsPopupUpdateBTN_Click(object sender, RoutedEventArgs e)
        {
            if (!(PaymentDetailsPopup.Tag is int paymentId))
            {
                ShowNotification("آیدی پرداختی مشخص نیست!", "error");
                return;
            }
            var _payment = pbll.GetPaymentId(paymentId);
            if (_payment == null)
            {
                ShowNotification("پرداختی موردنظر یافت نشد!", "error");
                return;
            }
            else
            {
                if (PopupPaymentStatusCheckBox.IsChecked == true)
                {
                    _payment.PaymentStatus = 1;
                    _payment.IsAmountPaid = true;
                    if (!string.IsNullOrWhiteSpace(PopupAmountTB.Text))
                    {
                        _payment.PaidAmount = Convert.ToDecimal(PopupAmountTB.Text);
                    }
                    else
                    {
                        ShowNotification("مبلغ پرداخت شده را وارد کنید!", "error");
                        return;
                    }
                }
                else if (PopupReminderSetCheckBox.IsChecked == true)
                {
                    _payment.IsReminderSet = true;
                    _payment.IsReminderDone = false;
                    if (!string.IsNullOrWhiteSpace(PopupReminderSetDP.SelectedDate.ToString()))
                    {
                        _payment.DueDate = PopupReminderSetDP.SelectedDate;
                    }
                    else
                    {
                        ShowNotification("تاریخ یادآوری را وارد کنید!", "error");
                        return;
                    }
                }
                bool result = pbll.UpdatePayments(_payment);
                ShowNotification($"ویرایش اطلاعات با موفقیت انجام شد", "success");
                RefreshPayment();
                SaveChanges();
                PaymentDetailsPopup.Visibility = Visibility.Collapsed;
                MainGrid.Effect = null;
            }
        }

        //صورتحساب
        private void PaymentDGV_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (PaymentDGV.SelectedItem is PaymentDto selectedPayment)
            {
                PrintInvoiceBTN.IsEnabled = true;
            }
            else
            {
                PrintInvoiceBTN.IsEnabled = false;
            }
        }
        private void PrintInvoiceBTN_Click(object sender, RoutedEventArgs e)
        {
            var selectedItem = PaymentDGV.SelectedItem as PaymentDto;
            if (selectedItem != null)
            {
                // گرفتن اطلاعات کامل Payment شامل Case و Client
                var fullPayment = pbll.GetPaymentId(selectedItem.Id); // فرض بر این است که PaymentDTO دارای Id است          
                // ساختن فاکتور با اطلاعات کامل
                GenerateInvoiceTemplate(fullPayment);

                // عملیات چاپ
                PrintDialog printDialog = new PrintDialog();
                if (printDialog.ShowDialog() == true)
                {
                    IDocumentPaginatorSource document = InvoiceGenerate.Document;
                    printDialog.PrintDocument(document.DocumentPaginator, "صورتحساب");
                }
            }
        }
        private void GenerateInvoiceTemplate(Payment fullPayment)
        {
            FlowDocument doc = new FlowDocument
            {
                PagePadding = new Thickness(50),
                ColumnWidth = 700,
                PageWidth = 850, // عرض صفحه کوچکتر
                PageHeight = 600, // ارتفاع صفحه کوچکتر
                FontFamily = new FontFamily("IRANSansWeb(FaNum)"),
                FontSize = 14,
            };
            var brushConverter = new System.Windows.Media.BrushConverter();
            // محتوای اصلی صورتحساب در یک StackPanel قرار می‌گیرد
            Section mainContent = new Section();
            Paragraph title = new Paragraph(new Run("صورتحساب خدمات حقوقی"))
            {
                FontSize = 20,
                FontWeight = FontWeights.Bold,
                TextAlignment = TextAlignment.Center,
                Foreground = (Brush)brushConverter.ConvertFromString("#1259D5"),
                Margin = new Thickness(0, 0, 0, 20)
            };
            mainContent.Blocks.Add(title);

            // Client Information Section
            Paragraph clientInfo = new Paragraph
            {
                TextAlignment = TextAlignment.Left,
                Inlines =
        {
            new Run("\t\t"+"نام موکل: ") { FontWeight = FontWeights.Bold, Foreground = (Brush)brushConverter.ConvertFromString("#1259D5")},
            new Run(fullPayment.Client.FullName + "\t\t\t\t"),
            new Run("تلفن همراه: ") { FontWeight = FontWeights.Bold, Foreground = (Brush)brushConverter.ConvertFromString("#1259D5")},
            new Run(fullPayment.Client.PhoneNumber + "\n"),

            new Run("\t\t"+"شماره پرونده: ") { FontWeight = FontWeights.Bold, Foreground = (Brush)brushConverter.ConvertFromString("#1259D5")},
            new Run(fullPayment.Case.CaseNumber + "\t\t"),
            new Run("تاریخ صدور صورتحساب: ") { FontWeight = FontWeights.Bold, Foreground = (Brush)brushConverter.ConvertFromString("#1259D5") },
            new Run($"{DateTime.Now.ToShortDateString()}\n"),
            new Run("\t\t"+"موضوع پرونده: ") { FontWeight = FontWeights.Bold, Foreground = (Brush)brushConverter.ConvertFromString("#1259D5") },
            new Run(fullPayment.Case.CaseSubject)
        }
            };
            mainContent.Blocks.Add(clientInfo);

            // جدول خدمات
            Table servicesTable = new Table
            {
                TextAlignment = TextAlignment.Center,
                CellSpacing = 3,
                Margin = new Thickness(100, 10, 100, 10),
                BorderBrush = Brushes.Black,
                BorderThickness = new Thickness(1)
            };

            servicesTable.Columns.Add(new TableColumn { Width = new GridLength(130) });
            servicesTable.Columns.Add(new TableColumn { Width = new GridLength(100) });
            servicesTable.Columns.Add(new TableColumn { Width = new GridLength(130) });
            servicesTable.Columns.Add(new TableColumn { Width = new GridLength(100) });

            // Header Row
            TableRowGroup headerGroup = new TableRowGroup();
            TableRow headerRow = new TableRow();
            headerRow.Cells.Add(new TableCell(new Paragraph(new Run("شرح خدمات"))) { FontWeight = FontWeights.Bold, Foreground = (Brush)brushConverter.ConvertFromString("#1259D5") });
            headerRow.Cells.Add(new TableCell(new Paragraph(new Run("تاریخ"))) { FontWeight = FontWeights.Bold, Foreground = (Brush)brushConverter.ConvertFromString("#1259D5") });
            headerRow.Cells.Add(new TableCell(new Paragraph(new Run("هزینه (ریال)"))) { FontWeight = FontWeights.Bold, Foreground = (Brush)brushConverter.ConvertFromString("#1259D5") });
            headerRow.Cells.Add(new TableCell(new Paragraph(new Run("روش پرداخت"))) { FontWeight = FontWeights.Bold, Foreground = (Brush)brushConverter.ConvertFromString("#1259D5") });
            headerGroup.Rows.Add(headerRow);
            servicesTable.RowGroups.Add(headerGroup);

            Dictionary<int, string> titleMappings = new Dictionary<int, string>
            {
                { 0, "مشاوره" },
                { 1, "حق الوکاله" },
            };

            string serviceText = titleMappings.ContainsKey(fullPayment.Service)
                ? titleMappings[fullPayment.Service]
                : "عنوان نامشخص";

            // Data Row
            TableRowGroup dataGroup = new TableRowGroup();
            dataGroup.Rows.Add(CreateServiceRow(serviceText, fullPayment.PaymentDate.ToShortShamsiDate(),string.Format("{0:N0} ريال",fullPayment.Amount.Value), fullPayment.PaymentType));
            servicesTable.RowGroups.Add(dataGroup);

            mainContent.Blocks.Add(servicesTable);

            var lawyerFullName = "";
            if (AppSession.CurrentUser.Role == User.UserRole.Lawyer)
            {
                lawyerFullName = AppSession.CurrentUser.FullName ?? "نامشخص";
            }
            else if (AppSession.CurrentUser.Role == User.UserRole.Secretary)
            {
                lawyerFullName = AppSession.CurrentUser.FullNameLawyer ?? "نامشخص";
            }

            // جمع کل
            Paragraph totalSection = new Paragraph
            {
                TextAlignment = TextAlignment.Left,
                Margin = new Thickness(100, 10, 100, 10),
                Inlines =
        {
            new Run("جمع کل: ") { FontWeight = FontWeights.Bold, Foreground = (Brush)brushConverter.ConvertFromString("#1259D5")},
            new Run(string.Format("{0:N0} ريال",fullPayment.Amount.Value)),"\n\n",
            new Run("توضیحات: ") { FontWeight = FontWeights.Bold, Foreground = (Brush)brushConverter.ConvertFromString("#1259D5")},
            new Run(fullPayment.Description) {FontWeight = FontWeights.Bold, FontSize = 12},"\n\n",
            new Run("دفتر وکالت آقای/خانم: ") { FontWeight = FontWeights.Bold, Foreground = (Brush)brushConverter.ConvertFromString("#1259D5")},
            new Run(lawyerFullName),"\n\n",
            new Run("⚖ نرم افزار حقوقی هـــیام \n ارتباط با توسعه دهنده: 989023349043+  | محمدی نصیر") { FontWeight = FontWeights.Normal,FontSize = 12, Foreground = Brushes.Black },        
        }
            };
            mainContent.Blocks.Add(totalSection);
            // بسته‌بندی تمام محتوا داخل یک Border برای نمایش حاشیه
            BlockUIContainer borderedContainer = new BlockUIContainer
            {
                Child = new Border
                {
                    BorderBrush = Brushes.Gray,
                    BorderThickness = new Thickness(2),
                    CornerRadius = new CornerRadius(10),
                    Padding = new Thickness(10),
                    //Visibility = Visibility.Collapsed,
                    Child = new FlowDocumentScrollViewer
                    {
                        Document = new FlowDocument(mainContent)
                        {
                            PagePadding = new Thickness(10),
                            FontFamily = new FontFamily("IRANSansWeb(FaNum)"),
                            FontSize = 14
                        }
                    }
                }
            };
            doc.Blocks.Add(borderedContainer);
            // ست کردن به DocumentViewer
            InvoiceGenerate.Document = doc;
        }
        private TableRow CreateServiceRow(string service, string date, string cost, string paymenttype)
        {
            TableRow row = new TableRow();
            row.Cells.Add(new TableCell(new Paragraph(new Run(service))));
            row.Cells.Add(new TableCell(new Paragraph(new Run(date))));
            row.Cells.Add(new TableCell(new Paragraph(new Run(cost))));
            row.Cells.Add(new TableCell(new Paragraph(new Run(paymenttype))));
            return row;
        }
        #endregion

        #region Expense
        private void ExpenseAmountTB_TextChanged(object sender, TextChangedEventArgs e)
        {
            // اگر متن خالی است یا متن همان "ریال" است، ادامه نده
            if (string.IsNullOrWhiteSpace(ExpenseAmountTB.Text) || ExpenseAmountTB.Text == "ریال")
                return;

            // مکان‌نما را ذخیره می‌کنیم تا بتوانیم بعد از فرمت‌دهی آن را تنظیم کنیم
            int selectionStart = ExpenseAmountTB.SelectionStart;

            // حذف کاراکترهای غیر عددی (مثل کاما و ریال)
            string rawText = ExpenseAmountTB.Text.Replace(",", "").Replace(" ریال", "");

            // چک کردن برای اطمینان از اینکه متن فقط شامل اعداد است
            if (decimal.TryParse(rawText, out decimal amount))
            {
                // فرمت دادن به عدد به صورت سه‌رقمی و اضافه کردن "ریال" در انتها
                ExpenseAmountTB.Text = string.Format("{0:N0} ", amount);

                // بازگرداندن مکان‌نما به موقعیت قبلی
                ExpenseAmountTB.SelectionStart = ExpenseAmountTB.Text.Length - 1; // از انتهای متن 5 کاراکتر برای " ریال" کم می‌کنیم
            }
        }    
        private void ExpenseDescriptionBTN_Click(object sender, RoutedEventArgs e)
        {
            ExpenseDescription.Visibility = Visibility.Visible;
            MainGrid.Effect = new BlurEffect { Radius = 5 };            
        }
        private void ShowExpenseDescriptionBTN_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            if (btn?.Tag is int expenseId)
            {
                PopupExpenseDescriptionTB.Tag = expenseId;
                var _expense = lebll.GetById(expenseId);
                PopupExpenseDescriptionTB.Text = _expense.Description;

                PopupExpenseDescription.Visibility = Visibility.Visible;
                MainGrid.Effect = new BlurEffect { Radius = 5 };               
            }
        }   
        private void ExpenseDescriptionCloseBTN_Click(object sender, RoutedEventArgs e)
        {
            ExpenseDescription.Visibility = Visibility.Collapsed;
            MainGrid.Effect = null;
        }
        private void PopupExpenseDescriptionCloseBTN_Click(object sender, RoutedEventArgs e)
        {
            PopupExpenseDescription.Visibility = Visibility.Collapsed;
            MainGrid.Effect = null;
        }
        private void AddExpenseBTN_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(ExpenseTitleTB.Text) || string.IsNullOrWhiteSpace(ExpenseAmountTB.Text) || string.IsNullOrWhiteSpace(ExpenseDP.Text) || string.IsNullOrWhiteSpace(ExpenseTypeCB.Text))
            {
                ShowNotification("لطفاً فیلدهای ضروری را پر کنید", "warning");
                return;
            }
            var user = AppSession.CurrentUser;
            var newExpense = new LawyerExpense
            {
                UserId = user.Id,
                UserRole = (int)user.Role,
                Title = ExpenseTitleTB.Text,
                Amount = Convert.ToDecimal(ExpenseAmountTB.Text),
                ExpenseDate = ExpenseDP.SelectedDate.Value,
                ExpenseType = ExpenseTypeCB.Text,
                Description = ExpenseDescriptionTB.Text,
                CreatedAt = DateTime.Now
            };
            var result = lebll.Add(newExpense);
            ShowNotification(result, "success");
            RefreshExpense();
            RefreshExpenseReport();
            SaveChanges();
            ClearControls(ExpensePanel);
            ExpenseDescriptionTB.Text = "";
        }
        #endregion

        //گزارش گیری مالی      
        #region FinancialReporting  

        //درآمد و هزینه
        #region Expense&Payment Chart        
        private void TimeRangeCB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var reportBLL = new Report_BLL();
            List<ChartDataPoint> data = new List<ChartDataPoint>();
           
            string selected = (TimeRangeCB.SelectedItem as System.Windows.Controls.ComboBoxItem)?.Content?.ToString();

            switch (selected)
            {
                case "روزانه":
                    YearRangeWP.IsEnabled = false;
                    data = reportBLL.GetDailyChartData_CurrentMonth();
                    break;
                case "ماهانه":
                    if (YearRangeWP != null)
                    {
                        YearRangeWP.IsEnabled = false;
                    }                            
                    data = reportBLL.GetMonthlyData_CurrentYear();
                    break;                   
                case "سالانه":
                    YearRangeWP.IsEnabled = true;
                    data = reportBLL.GetYearlyChartData_Range(1400, 1410);
                    break;
            }          
            DrawChart(data);            
        }
        private void YearlyChartBTN_Click(object sender, RoutedEventArgs e)
        {
            if (YearFromCB.SelectedItem is int fromYear &&
            YearToCB.SelectedItem is int toYear)
            {
                if (fromYear > toYear)
                {
                    MessageBox.Show("سال شروع نباید بزرگ‌تر از سال پایان باشد.");
                    return;
                }

                var data = new Report_BLL().GetYearlyChartData_Range(fromYear, toYear);
                DrawChart(data);
            }
            else
            {
                MessageBox.Show("لطفاً سال‌های معتبر را انتخاب کنید.");
            }           
        }
        public static class ChartHelper
        {
            public static RenderTargetBitmap ExportToBitmap(FrameworkElement element)
            {
                if (element == null) return null;

                element.Measure(new Size(element.ActualWidth, element.ActualHeight));
                element.Arrange(new Rect(new Size(element.ActualWidth, element.ActualHeight)));

                RenderTargetBitmap rtb = new RenderTargetBitmap(
                    (int)element.ActualWidth,
                    (int)element.ActualHeight,
                    96, 96, PixelFormats.Pbgra32);

                rtb.Render(element);
                return rtb;
            }
        }
        public static class PdfExporter
        {
            public static void SaveChartToPdf(RenderTargetBitmap chartImage, string filePath)
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    BitmapEncoder encoder = new PngBitmapEncoder();
                    encoder.Frames.Add(BitmapFrame.Create(chartImage));
                    encoder.Save(ms);

                    PdfDocument pdf = new PdfDocument();
                    PdfPage page = pdf.AddPage();
                    XGraphics gfx = XGraphics.FromPdfPage(page);

                    XImage xImg = XImage.FromStream(ms);
                    page.Width = XUnit.FromPoint(900);
                    page.Height = XUnit.FromPoint(440);

                    gfx.DrawImage(xImg, 0, 0);
                    pdf.Save(filePath);
                }
            }
        }
        private void PrintChartReportBTN_Click(object sender, RoutedEventArgs e)
        {
            var bitmap = ChartHelper.ExportToBitmap(ReportChart);
            if (bitmap == null)
            {
                ShowNotification("نمودار برای ذخیره یافت نشد!", "error");               
                return;
            }

            var dialog = new Microsoft.Win32.SaveFileDialog
            {
                Filter = "PDF File|*.pdf",
                FileName = "گزارش نمودار"
            };

            if (dialog.ShowDialog() == true)
            {
                PdfExporter.SaveChartToPdf(bitmap, dialog.FileName);
                CustomMessageBox msgbox = new CustomMessageBox("نرم افزار هــــیام", "نمودار با موفقیت ذخیره شد.", false, CustomMessageBox.MessageType.Success);
                msgbox.ShowDialog();
            }
        }      
        #endregion

        //قراردادها
        #region Contracts
        private void PrintContractReportBTN_Click(object sender, RoutedEventArgs e)
        {
            var items = ReportContractsDGV.ItemsSource as IEnumerable<ContractDto>;
            if (items == null || !items.Any())
            {
                ShowNotification("رکوردی برای چاپ وجود ندارد!", "error");
                return;
            }
            var brushconverter = new System.Windows.Media.BrushConverter();
            FlowDocument doc = new FlowDocument
            {
                PageWidth = 900,
                PagePadding = new Thickness(30),
                ColumnWidth = double.PositiveInfinity,
                FontFamily = new FontFamily("IRANSansWeb(FaNum)"),
                FontSize = 12,
                TextAlignment = TextAlignment.Left,
                FlowDirection = FlowDirection.RightToLeft,
            };
            // محتوای اصلی قرارداد در یک StackPanel قرار می‌گیرد
            Section mainContent = new Section();
            // عنوان
            Paragraph title = new Paragraph(new Run("گزارش قراردادهای موکل ها"))
            {
                Foreground = (Brush)brushconverter.ConvertFromString("#1259D5"),
                FontSize = 20,
                FontWeight = FontWeights.Bold,
                TextAlignment = TextAlignment.Center,
                Margin = new Thickness(0, 0, 0, 20)
            };
            mainContent.Blocks.Add(title);

            // جدول قراردادها
            Table table = new Table
            {
                CellSpacing = 0,
                Padding = new Thickness(5),
                BorderBrush = Brushes.Black,
                BorderThickness = new Thickness(1.5),
            };

            // تعریف ستون‌ها - ستون اول عریض‌تر
            table.Columns.Add(new TableColumn { Width = new GridLength(175) }); // نام موکل
            table.Columns.Add(new TableColumn { Width = new GridLength(165) }); // شماره پرونده
            table.Columns.Add(new TableColumn { Width = new GridLength(135) }); // موضوع پرونده
            table.Columns.Add(new TableColumn { Width = new GridLength(90) }); // نوع قرارداد
            table.Columns.Add(new TableColumn { Width = new GridLength(145) }); // مبلغ
            table.Columns.Add(new TableColumn { Width = new GridLength(80) }); // تاریخ
                            
            // ردیف عنوان
            TableRowGroup headerGroup = new TableRowGroup();
            TableRow header = new TableRow();
            header.Cells.Add(new TableCell(new Paragraph(new Run("نام موکل"))) { FontWeight = FontWeights.Bold });
            header.Cells.Add(new TableCell(new Paragraph(new Run("شماره پرونده"))) { FontWeight = FontWeights.Bold });
            header.Cells.Add(new TableCell(new Paragraph(new Run("موضوع پرونده"))) { FontWeight = FontWeights.Bold });
            header.Cells.Add(new TableCell(new Paragraph(new Run("نوع قرارداد"))) { FontWeight = FontWeights.Bold });
            header.Cells.Add(new TableCell(new Paragraph(new Run("مبلغ"))) { FontWeight = FontWeights.Bold });
            header.Cells.Add(new TableCell(new Paragraph(new Run("تاریخ تنظیم"))) { FontWeight = FontWeights.Bold });
            headerGroup.Rows.Add(header);
            table.RowGroups.Add(headerGroup);

            // داده‌ها
            TableRowGroup dataGroup = new TableRowGroup();
            decimal totalAmount = 0; // جمع مبالغ

            foreach (var item in items)
            {
                TableRow row = new TableRow();
                row.Cells.Add(new TableCell(new Paragraph(new Run(item.ClientName))));
                row.Cells.Add(new TableCell(new Paragraph(new Run(item.CaseNumber))));
                row.Cells.Add(new TableCell(new Paragraph(new Run(item.CaseSubject))));
                row.Cells.Add(new TableCell(new Paragraph(new Run(item.ContractTypeText))));
                row.Cells.Add(new TableCell(new Paragraph(new Run(item.TotalAmountFormat))));
                row.Cells.Add(new TableCell(new Paragraph(new Run(item.SetDate.ToString("yyyy/MM/dd")))));
                dataGroup.Rows.Add(row);

                // جمع مبلغ در صورتی که مقدار داشته باشد
                if (item.TotalAmount.HasValue)
                    totalAmount += item.TotalAmount.Value;
            }

            table.RowGroups.Add(dataGroup); // اضافه کردن جدول به سند
            mainContent.Blocks.Add(table);
        
            // 👇 پاراگراف جمع کل بعد از جدول
            Paragraph totalAmountParagraph = new Paragraph
            {
                Margin = new Thickness(0, 20, 0, 0),
                FontWeight = FontWeights.Bold,
                FontSize = 14,
                TextAlignment = TextAlignment.Left,
            };
           
            var consultingContracts = items
            .Where(c => c.ContractTypeText != null && c.ContractTypeText.Contains("قرارداد مشاوره"))
            .ToList();

            var AttorneysfeesContracts = items
           .Where(c => c.ContractTypeText != null && c.ContractTypeText.Contains("قرارداد وکالت"))
           .ToList();

            int consultingrecordCount = consultingContracts.Count();
            decimal totalconsultingAmount = consultingContracts
                .Where(c => c.TotalAmount.HasValue)
                .Sum(c => c.TotalAmount.Value);

            int AttorneysfeesrecordCount = AttorneysfeesContracts.Count();
            decimal totalAttorneysAmount = AttorneysfeesContracts
                .Where(c => c.TotalAmount.HasValue)
                .Sum(c => c.TotalAmount.Value);

            Run dateReportText;
            if (!string.IsNullOrWhiteSpace(ContractStartDP.Text) && !string.IsNullOrWhiteSpace(ContractEndDP.Text))
            {
                dateReportText = new Run($"گزارش بازه زمانی {ContractStartDP.Text} الی {ContractEndDP.Text}\n")
                {
                    FontSize = 11,
                    Foreground = Brushes.Gray
                };
            }
            else
            {
                dateReportText = new Run("گزارش کلی بدون بازه زمانی\n")
                {
                    FontSize = 11,
                    Foreground = Brushes.Gray
                };
            }

            totalAmountParagraph.Inlines.Add(dateReportText);

            Run totalMoshavereText = new Run("تعداد قرارداد مشاوره: ")
            {
                Foreground = Brushes.Black
            };
            Run totalMoshavereValue = new Run($"{consultingrecordCount} \n")
            {
                Foreground = Brushes.DarkRed
            };

            Run totalAttorneysfeesText = new Run("تعداد قرارداد وکالت: ")
            {
                Foreground = Brushes.Black
            };
            Run totalAttorneysfeesValue = new Run($"{AttorneysfeesrecordCount} \n")
            {
                Foreground = Brushes.DarkRed
            };
            Run totalRecordsText = new Run("تعداد رکوردها: ")
            {
                Foreground = Brushes.Black
            };
            Run totalRecordsValue = new Run($"{items.Count()} \n")
            {
                Foreground = Brushes.DarkRed
            };
            Run totalMoshavereAmountText = new Run("جمع کل مبلغ های مشاوره: ")
            {
                Foreground = Brushes.Black
            };
            Run totalMoshavereAmountValue = new Run($"{totalconsultingAmount:N0} ریال \n")
            {
                Foreground = Brushes.DarkRed
            };
            Run totalAttorneysAmountText = new Run("جمع کل مبلغ های حق الوکاله: ")
            {
                Foreground = Brushes.Black
            };
            Run totalAttorneysAmountValue = new Run($"{totalAttorneysAmount:N0} ریال \n")
            {
                Foreground = Brushes.DarkRed
            };
            Run totalAmountText = new Run("جمع کل مبلغ ها: ")
            {
                Foreground = Brushes.Black
            };
            Run totalAmountValue = new Run($"{totalAmount:N0} ریال \n\n")
            {
                Foreground = Brushes.DarkRed
            };

            var userType = "";
            foreach (var item in items)
            {
                userType = item.UserTypeText;
            }

            Run UserTypeText = new Run("کاربر: ")
            {
                Foreground = Brushes.Black
            };
            Run UserTypeValue = new Run($"{userType} \n")
            {
                Foreground = (Brush)brushconverter.ConvertFromString("#1259D5")
            };

            var lawyername = "";
            if (AppSession.CurrentUser.Role == User.UserRole.Lawyer)
            {
                lawyername = AppSession.CurrentUser.FullName ?? "نامشخص";
            }
            else if (AppSession.CurrentUser.Role == User.UserRole.Secretary)
            {
                lawyername = AppSession.CurrentUser.FullNameLawyer ?? "نامشخص";
            }

            Run LawyerNameText = new Run("دفتر وکالت آقای/خانم: ")
            {
                Foreground = Brushes.Black
            };
            Run LawyerNameValue = new Run($"{lawyername} \n\n")
            {
                Foreground = (Brush)brushconverter.ConvertFromString("#1259D5")
            };

            Run DeveloperText = new Run("⚖ نرم افزار حقوقی هـــیام \n ارتباط با توسعه دهنده: 989023349043+  | محمدی نصیر")
            {
                FontWeight = FontWeights.Normal,
                FontSize = 12,
                Foreground = Brushes.Black
            };
         
            totalAmountParagraph.Inlines.Add(totalMoshavereText);
            totalAmountParagraph.Inlines.Add(totalMoshavereValue);
            totalAmountParagraph.Inlines.Add(totalAttorneysfeesText);
            totalAmountParagraph.Inlines.Add(totalAttorneysfeesValue);
            totalAmountParagraph.Inlines.Add(totalRecordsText);
            totalAmountParagraph.Inlines.Add(totalRecordsValue);
            totalAmountParagraph.Inlines.Add(totalMoshavereAmountText);
            totalAmountParagraph.Inlines.Add(totalMoshavereAmountValue);
            totalAmountParagraph.Inlines.Add(totalAttorneysAmountText);
            totalAmountParagraph.Inlines.Add(totalAttorneysAmountValue);
            totalAmountParagraph.Inlines.Add(totalAmountText);
            totalAmountParagraph.Inlines.Add(totalAmountValue);
            totalAmountParagraph.Inlines.Add(UserTypeText);
            totalAmountParagraph.Inlines.Add(UserTypeValue);
            totalAmountParagraph.Inlines.Add(LawyerNameText);
            totalAmountParagraph.Inlines.Add(LawyerNameValue);
            totalAmountParagraph.Inlines.Add(DeveloperText);
            mainContent.Blocks.Add(table);
            mainContent.Blocks.Add(totalAmountParagraph); // افزودن جمع کل به سند
            // بسته‌بندی تمام محتوا داخل یک Border برای نمایش حاشیه
            BlockUIContainer borderedContainer = new BlockUIContainer
            {
                Child = new Border
                {
                    BorderBrush = Brushes.Gray,
                    BorderThickness = new Thickness(2),
                    CornerRadius = new CornerRadius(3),
                    Padding = new Thickness(10),
                    Child = new FlowDocumentScrollViewer
                    {
                        Document = new FlowDocument(mainContent)
                        {
                            PagePadding = new Thickness(3),
                            FontFamily = new FontFamily("IRANSansWeb(FaNum)"),
                            FontSize = 14
                        }
                    }
                }
            };
            doc.Blocks.Add(borderedContainer);
            // نمایش در DocumentViewer یا پرینت
            PrintFlowDocumentContract(doc);
        }
        private void PrintFlowDocumentContract(FlowDocument doc)
        {
            PrintDialog printDialog = new PrintDialog();
            if (printDialog.ShowDialog() == true)
            {
                IDocumentPaginatorSource idpSource = doc;
                printDialog.PrintDocument(idpSource.DocumentPaginator, "گزارش قراردادها");
            }
        }
        private void ContractSearchClient_TextChanged(object sender, TextChangedEventArgs e)
        {
            string keyword = ContractSearchClient.Text.Trim();
            int typeFilter = 0;
            List<ContractDto> filtered;

            if (string.IsNullOrWhiteSpace(keyword))
            {
                ClientNamePlaceholderText.Visibility = Visibility.Visible;
                filtered = contract_bll.GetContractsForListView();
                ReportContractsDGV.ItemsSource = filtered;
            }
            else
            {
                filtered = contract_bll.SearchContractsForDGV(keyword, typeFilter);
                ReportContractsDGV.ItemsSource = filtered;
                ClientNamePlaceholderText.Visibility = Visibility.Hidden;
            }
        }       
        private void ContractStartDP_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(ContractStartDP.Text))
            {
                ContractStartPlaceholderText.Visibility = Visibility.Visible;
            }
            else
            {
                ContractStartPlaceholderText.Visibility = Visibility.Hidden;
            }        
        }
        private void ContractEndDP_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(ContractEndDP.Text))
            {
                ContractEndPlaceholderText.Visibility = Visibility.Visible;
            }
            else
            {
                ContractEndPlaceholderText.Visibility = Visibility.Hidden;
            }
         
        }
        private void DateFilterBTN_Click(object sender, RoutedEventArgs e)
        {
            // بررسی خالی نبودن تاریخ‌ها
            if (ContractStartDP.SelectedDate == null || ContractEndDP.SelectedDate == null)
            {
                RefreshContractsReport(); // اگر داری برای آمار یا بازنشانی
                return;
            }

            DateTime startDate = ContractStartDP.SelectedDate.Value.Date;
            DateTime endDate = ContractEndDP.SelectedDate.Value.Date;

            if (startDate > endDate)
            {
                ShowNotification("تاریخ شروع نباید بزرگ‌تر از تاریخ پایان باشد!", "error");
                return;
            }

            var filteredContracts = contract_bll.GetByDateRange(startDate, endDate); // متدی که در ادامه تعریف می‌کنیم
            ReportContractsDGV.ItemsSource = filteredContracts;

            // نمایش آمار
            TotalRecordsText.Text = filteredContracts.Count.ToString();
            TotalPaidText.Text = string.Format("{0:N0} ریال", contract_bll.GetTotalAmountByDateRange(startDate, endDate));           
        }
        #endregion

        //درآمدها
        #region Payments
        private void PaymentReportSearchClient_TextChanged(object sender, TextChangedEventArgs e)
        {
            string keyword = PaymentReportSearchClient.Text.Trim();
            int? statusFilter = null;
            List<PaymentDto> filtered;

            if (string.IsNullOrWhiteSpace(keyword))
            {
                PaymentReportClientNamePlaceholderText.Visibility = Visibility.Visible;
                filtered = pbll.GetPaymentsForListView(); // دریافت همه رکوردها بدون فیلتر
            }
            else
            {
                filtered = pbll.SearchPaymentsForDGV(keyword, statusFilter); // جستجو و فیلتر همزمان                
                PaymentReportClientNamePlaceholderText.Visibility = Visibility.Hidden;
            }
            PaymentReportDGV.ItemsSource = filtered;
        }    
        private void PaymentReportStatusCB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string searchTerm = PaymentReportSearchClient?.Text?.Trim() ?? string.Empty;
            int? statusFilter = null;
            if (int.TryParse(PaymentReportStatusCB.SelectedValue?.ToString(), out int parsedStatus))
            {
                statusFilter = parsedStatus;
            }
            var result = pbll.SearchPaymentsForDGV(searchTerm, statusFilter);
            //PaymentReportDGV.ItemsSource = null;
            //PaymentReportDGV.ItemsSource = result;      
        }
        private void PaymentStartDP_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(PaymentStartDP.Text))
            {
                PaymentStartPlaceholderText.Visibility = Visibility.Visible;
            }
            else
            {
                PaymentStartPlaceholderText.Visibility = Visibility.Hidden;
            }
        }
        private void PaymentEndDP_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(PaymentEndDP.Text))
            {
                PaymentEndPlaceholderText.Visibility = Visibility.Visible;
            }
            else
            {
                PaymentEndPlaceholderText.Visibility = Visibility.Hidden;
            }
        }
        private void PaymentDateFilterBTN_Click(object sender, RoutedEventArgs e)
        {
            // بررسی خالی نبودن تاریخ‌ها
            if (PaymentStartDP.SelectedDate == null || PaymentEndDP.SelectedDate == null)
            {
                RefreshPaymentsReport(); // اگر داری برای آمار یا بازنشانی
                return;
            }

            DateTime startDate = PaymentStartDP.SelectedDate.Value.Date;
            DateTime endDate = PaymentEndDP.SelectedDate.Value.Date;

            if (startDate > endDate)
            {
                ShowNotification("تاریخ شروع نباید بزرگ‌تر از تاریخ پایان باشد!", "error");
                return;
            }

            var filteredPayments = pbll.GetByDateRange(startDate, endDate);
            PaymentReportDGV.ItemsSource = filteredPayments;

            // نمایش آمار
            PaymentReportTotalRecordsText.Text = filteredPayments.Count.ToString();
            PaymentReportTotalPaidText.Text = string.Format("{0:N0} ریال", pbll.GetTotalAmountByDateRange(startDate, endDate));            
        }
        private void ShowReportPaymentsDetailsBTN_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            if (btn?.Tag is int paymentId)
            {
                PaymentDetailsPopup.Tag = paymentId;
                var _payment = pbll.GetPaymentId(paymentId);
                ClientCaseNumberText.Text = _payment.Case.CaseNumber;
                AmountText.Text = string.Format("{0:N0} ریال", _payment.Amount);
                PaymentDateText.Text = _payment.PaymentDate.ToShortShamsiDate();
                PaymentTypeText.Text = _payment.PaymentType;
                DescriptionText.Text = _payment.Description;              
                PopupReminderSetDP.SelectedDate = _payment.DueDate;
                LoadPaymentForEdit(_payment);

                //برای گزارش
                DescriptionText.IsReadOnly = true;
                PopupPaymentStatusCheckBox.IsEnabled = false;
                PopupPaymentStatusCheckBox.IsEnabled = false;
                PaymentDetailsPopupUpdateBTN.Visibility = Visibility.Hidden;

                PaymentDetailsPopup.Visibility = Visibility.Visible;
                MainGrid.Effect = new BlurEffect { Radius = 5 };
            }
        }
        private void PrintPaymentReportBTN_Click(object sender, RoutedEventArgs e)
        {
            var items = PaymentReportDGV.ItemsSource as IEnumerable<PaymentDto>;
            if (items == null || !items.Any())
            {
                MessageBox.Show("رکوردی برای چاپ وجود ندارد.");
                return;
            }

            FlowDocument doc = new FlowDocument
            {
                PageWidth = 975,
                PagePadding = new Thickness(30),
                ColumnWidth = double.PositiveInfinity,
                FontFamily = new FontFamily("IRANSansWeb(FaNum)"),
                FontSize = 10,
                TextAlignment = TextAlignment.Left,
                FlowDirection = FlowDirection.RightToLeft,
            };
            // محتوای اصلی درآمد در یک StackPanel قرار می‌گیرد
            Section mainContent = new Section();
            // عنوان
            Paragraph title = new Paragraph(new Run("گزارش درآمدها"))
            {
                Foreground = new SolidColorBrush(Colors.ForestGreen),
                FontSize = 20,
                FontWeight = FontWeights.Bold,
                TextAlignment = TextAlignment.Center,
                Margin = new Thickness(0, 0, 0, 20)
            };
            mainContent.Blocks.Add(title);

            // جدول پرداخت ها
            Table table = new Table
            {
                CellSpacing = 0,
                Padding = new Thickness(5),
                BorderBrush = Brushes.Black,
                BorderThickness = new Thickness(1.5),
            };

            // تعریف ستون‌ها - ستون اول عریض‌تر
            table.Columns.Add(new TableColumn { Width = new GridLength(130) }); // نام موکل
            table.Columns.Add(new TableColumn { Width = new GridLength(100) }); // تلفن همراه
            table.Columns.Add(new TableColumn { Width = new GridLength(155) }); // شماره پرونده
            table.Columns.Add(new TableColumn { Width = new GridLength(90) }); // موضوع پرونده          
            table.Columns.Add(new TableColumn { Width = new GridLength(65) }); // نوع خدمت
            table.Columns.Add(new TableColumn { Width = new GridLength(80) }); // وضعیت
            table.Columns.Add(new TableColumn { Width = new GridLength(90) }); // مبلغ پرداختی
            table.Columns.Add(new TableColumn { Width = new GridLength(85) }); // تاریخ پرداخت
            table.Columns.Add(new TableColumn { Width = new GridLength(80) }); // روش پرداخت

            // ردیف عنوان
            TableRowGroup headerGroup = new TableRowGroup();
            TableRow header = new TableRow();
            header.Cells.Add(new TableCell(new Paragraph(new Run("نام موکل"))) { FontWeight = FontWeights.Bold });
            header.Cells.Add(new TableCell(new Paragraph(new Run("تلفن همراه"))) { FontWeight = FontWeights.Bold });
            header.Cells.Add(new TableCell(new Paragraph(new Run("شماره پرونده"))) { FontWeight = FontWeights.Bold });
            header.Cells.Add(new TableCell(new Paragraph(new Run("موضوع پرونده"))) { FontWeight = FontWeights.Bold });         
            header.Cells.Add(new TableCell(new Paragraph(new Run("خدمات"))) { FontWeight = FontWeights.Bold });
            header.Cells.Add(new TableCell(new Paragraph(new Run("وضعیت"))) { FontWeight = FontWeights.Bold });
            header.Cells.Add(new TableCell(new Paragraph(new Run("مبلغ"))) { FontWeight = FontWeights.Bold });
            header.Cells.Add(new TableCell(new Paragraph(new Run("تاریخ پرداخت"))) { FontWeight = FontWeights.Bold });
            header.Cells.Add(new TableCell(new Paragraph(new Run("روش پرداخت"))) { FontWeight = FontWeights.Bold });
            headerGroup.Rows.Add(header);
            table.RowGroups.Add(headerGroup);

            // داده‌ها
            TableRowGroup dataGroup = new TableRowGroup();
            decimal totalAmount = 0; // جمع مبالغ
                                     // 
            foreach (var item in items)
            {
                Dictionary<int, string> titleMappings = new Dictionary<int, string>
                {
                { 0, "مشاوره" },
                { 1, "حق الوکاله" },
                };

                Dictionary<int, string> titleMappings1 = new Dictionary<int, string>
                {
                { 0, "پیش پرداخت" },
                { 1, "تسویه شده" },
                };
                string serviceText = titleMappings.ContainsKey(item.Service)
                      ? titleMappings[item.Service]
                      : "خدمات نامشخص";

                string serviceText1 = titleMappings1.ContainsKey(item.PaymentStatus)
                    ? titleMappings1[item.PaymentStatus]
                    : "وضعیت نامشخص";

                TableRow row = new TableRow();
                row.Cells.Add(new TableCell(new Paragraph(new Run(item.ClientName))));
                row.Cells.Add(new TableCell(new Paragraph(new Run(item.ClientPhoneNumber))));
                row.Cells.Add(new TableCell(new Paragraph(new Run(item.ClientCaseNumber))));
                row.Cells.Add(new TableCell(new Paragraph(new Run(item.ClientCaseSubject))));
                row.Cells.Add(new TableCell(new Paragraph(new Run(serviceText))));
                row.Cells.Add(new TableCell(new Paragraph(new Run(serviceText1))));
                row.Cells.Add(new TableCell(new Paragraph(new Run($"{item.Amount:N0} ريال"))));
                row.Cells.Add(new TableCell(new Paragraph(new Run(item.PaymentDate.ToString("yyyy/MM/dd")))));
                row.Cells.Add(new TableCell(new Paragraph(new Run(item.PaymentType))));
                dataGroup.Rows.Add(row);
                
                totalAmount = pbll.GetTotalAmount().Value;
            }
            table.RowGroups.Add(dataGroup); // اضافه کردن جدول به سند
            mainContent.Blocks.Add(table);

            // 👇 پاراگراف جمع کل بعد از جدول
            Paragraph totalAmountParagraph = new Paragraph
            {
                Margin = new Thickness(0, 20, 0, 0),
                FontWeight = FontWeights.Bold,
                FontSize = 12,
                TextAlignment = TextAlignment.Left,
            };

            var brushconverter = new System.Windows.Media.BrushConverter();

            var PrePaidPayments = items
            .Where(c => c.PaymentStatusTypeText != null && c.PaymentStatusTypeText.Contains("پیش پرداخت"))
            .ToList();

            var FullPaidPayments = items
            .Where(c => c.PaymentStatusTypeText != null && c.PaymentStatusTypeText.Contains("تسویه شده"))
            .ToList();

            int PrePaidRecordCount = PrePaidPayments.Count();          
            int FullPaidRecordCount = FullPaidPayments.Count();

            Run dateReportText;
            if (!string.IsNullOrWhiteSpace(PaymentStartDP.Text) && !string.IsNullOrWhiteSpace(PaymentEndDP.Text))
            {
                dateReportText = new Run($"گزارش بازه زمانی {PaymentStartDP.Text} الی {PaymentEndDP.Text}\n")
                {
                    FontSize = 11,
                    Foreground = Brushes.Gray
                };
            }
            else
            {
                dateReportText = new Run("گزارش کلی بدون بازه زمانی\n")
                {
                    FontSize = 11,
                    Foreground = Brushes.Gray
                };
            }
            totalAmountParagraph.Inlines.Add(dateReportText);

            Run totalPrePaidText = new Run("تعداد پیش پرداخت ها: ")
            {
                Foreground = Brushes.Black
            };
            Run totalPrePaidValue = new Run($"{PrePaidRecordCount} \n")
            {
                Foreground = Brushes.DarkRed
            };

            Run totalFullPaidText = new Run("تعداد تسویه شده ها: ")
            {
                Foreground = Brushes.Black
            };
            Run totalFullPaidValue = new Run($"{FullPaidRecordCount} \n")
            {
                Foreground = Brushes.DarkRed
            };

            Run totalRecordsText = new Run("تعداد رکوردها: ")
            {
                Foreground = Brushes.Black
            };
            Run totalRecordsValue = new Run($"{items.Count()} \n")
            {
                Foreground = Brushes.DarkRed
            };                
            Run totalAmountText = new Run("جمع کل مبلغ ها: ")
            {
                Foreground = Brushes.Black
            };
            Run totalAmountValue = new Run($"{totalAmount:N0} ریال \n\n")
            {
                Foreground = Brushes.DarkRed
            };

            var userType = "";
            foreach (var item in items)
            {
                userType = item.UserTypeText;
            }

            Run UserTypeText = new Run("کاربر: ")
            {
                Foreground = Brushes.Black
            };
            Run UserTypeValue = new Run($"{userType} \n")
            {
                Foreground = (Brush)brushconverter.ConvertFromString("#1259D5")
            };

            var lawyername = "";
            if (AppSession.CurrentUser.Role == User.UserRole.Lawyer)
            {
                lawyername = AppSession.CurrentUser.FullName ?? "نامشخص";
            }
            else if (AppSession.CurrentUser.Role == User.UserRole.Secretary)
            {
                lawyername = AppSession.CurrentUser.FullNameLawyer ?? "نامشخص";
            }

            Run LawyerNameText = new Run("دفتر وکالت آقای/خانم: ")
            {
                Foreground = Brushes.Black
            };
            Run LawyerNameValue = new Run($"{lawyername} \n\n")
            {
                Foreground = (Brush)brushconverter.ConvertFromString("#1259D5")
            };

            Run DeveloperText = new Run("⚖ نرم افزار حقوقی هـــیام \n ارتباط با توسعه دهنده: 989023349043+  | محمدی نصیر")
            {
                FontWeight = FontWeights.Normal,
                FontSize = 12,
                Foreground = Brushes.Black
            };
            totalAmountParagraph.Inlines.Add(totalPrePaidText);
            totalAmountParagraph.Inlines.Add(totalPrePaidValue);
            totalAmountParagraph.Inlines.Add(totalFullPaidText);
            totalAmountParagraph.Inlines.Add(totalFullPaidValue);
            totalAmountParagraph.Inlines.Add(totalRecordsText);
            totalAmountParagraph.Inlines.Add(totalRecordsValue);                
            totalAmountParagraph.Inlines.Add(totalAmountText);
            totalAmountParagraph.Inlines.Add(totalAmountValue);
            totalAmountParagraph.Inlines.Add(UserTypeText);
            totalAmountParagraph.Inlines.Add(UserTypeValue);
            totalAmountParagraph.Inlines.Add(LawyerNameText);
            totalAmountParagraph.Inlines.Add(LawyerNameValue);
            totalAmountParagraph.Inlines.Add(DeveloperText);
            mainContent.Blocks.Add(table);
            mainContent.Blocks.Add(totalAmountParagraph); // افزودن جمع کل به سند
                                                          // بسته‌بندی تمام محتوا داخل یک Border برای نمایش حاشیه
            BlockUIContainer borderedContainer = new BlockUIContainer
            {
                Child = new Border
                {
                    BorderBrush = Brushes.Gray,
                    BorderThickness = new Thickness(2),
                    CornerRadius = new CornerRadius(3),
                    Padding = new Thickness(5),
                    //Visibility = Visibility.Collapsed,
                    Child = new FlowDocumentScrollViewer
                    {
                        Document = new FlowDocument(mainContent)
                        {
                            PagePadding = new Thickness(3),
                            FontFamily = new FontFamily("IRANSansWeb(FaNum)"),
                            FontSize = 12
                        }
                    }
                }
            };
            doc.Blocks.Add(borderedContainer);
            // نمایش در DocumentViewer یا پرینت
            PrintFlowDocumentPayment(doc);         
        }
        private void PrintFlowDocumentPayment(FlowDocument doc)
        {
            PrintDialog printDialog = new PrintDialog();
            if (printDialog.ShowDialog() == true)
            {
                IDocumentPaginatorSource idpSource = doc;
                printDialog.PrintDocument(idpSource.DocumentPaginator, "گزارش درآمدها");
            }
        }
        #endregion

        //هزینه ها
        #region Expense
        private void ExpenseReportSearchClient_TextChanged(object sender, TextChangedEventArgs e)
        {
            string keyword = ExpenseReportSearchClient.Text.Trim();
            List<LawyerExpenseDto> filtered;

            if (string.IsNullOrWhiteSpace(keyword))
            {
                ExpenseReportClientNamePlaceholderText.Visibility = Visibility.Visible;
                filtered = lebll.GetAllExpenseForListView(); // دریافت همه رکوردها بدون فیلتر
            }
            else
            {
                filtered = lebll.SearchExpensesForListView(keyword); // جستجو و فیلتر همزمان                
                ExpenseReportClientNamePlaceholderText.Visibility = Visibility.Hidden;
            }
            ReportExpensesDGV.ItemsSource = filtered;
        }
        private void ExpenseStartDP_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(ExpenseStartDP.Text))
            {
                ExpenseStartPlaceholderText.Visibility = Visibility.Visible;
            }
            else
            {
                ExpenseStartPlaceholderText.Visibility = Visibility.Hidden;
            }
        }
        private void ExpenseEndDP_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(ExpenseEndDP.Text))
            {
                ExpenseEndPlaceholderText.Visibility = Visibility.Visible;
            }
            else
            {
                ExpenseEndPlaceholderText.Visibility = Visibility.Hidden;
            }
        }
        private void ExpenseDateFilterBTN_Click(object sender, RoutedEventArgs e)
        {
            // بررسی خالی نبودن تاریخ‌ها
            if (ExpenseStartDP.SelectedDate == null || ExpenseEndDP.SelectedDate == null)
            {
                RefreshExpenseReport(); // اگر داری برای آمار یا بازنشانی
                return;
            }

            DateTime startDate = ExpenseStartDP.SelectedDate.Value.Date;
            DateTime endDate = ExpenseEndDP.SelectedDate.Value.Date;

            if (startDate > endDate)
            {
                ShowNotification("تاریخ شروع نباید بزرگ‌تر از تاریخ پایان باشد!", "error");
                return;
            }

            var filteredExpenses = lebll.GetByDateRange(startDate, endDate);
            ReportExpensesDGV.ItemsSource = filteredExpenses;

            // نمایش آمار
            ReportExpenseTotalCountText.Text = filteredExpenses.Count.ToString();
            ReportExpenseTotalAmountText.Text = string.Format("{0:N0} ریال", lebll.GetTotalAmountByDateRange(startDate, endDate));
        }
        private void ShowReportExpenseDescriptionBTN_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            if (btn?.Tag is int expenseId)
            {
                PopupExpenseDescriptionTB.Tag = expenseId;
                var _expense = lebll.GetById(expenseId);
                PopupExpenseDescriptionTB.Text = _expense.Description;

                PopupExpenseDescription.Visibility = Visibility.Visible;
                MainGrid.Effect = new BlurEffect { Radius = 5 };
            }
        }
        private void ReportExpenseDescriptionPopupCloseBTN_Click(object sender, RoutedEventArgs e)
        {
            PopupExpenseDescription.Visibility = Visibility.Collapsed;
            MainGrid.Effect = null;
        }
        private void PrintExpenseReportBTN_Click(object sender, RoutedEventArgs e)
        {
            var items = ReportExpensesDGV.ItemsSource as IEnumerable<LawyerExpenseDto>;
            if (items == null || !items.Any())
            {
                MessageBox.Show("رکوردی برای چاپ وجود ندارد.");
                return;
            }

            FlowDocument doc = new FlowDocument
            {
                PageWidth = 780,
                PagePadding = new Thickness(30),
                ColumnWidth = double.PositiveInfinity,
                FontFamily = new FontFamily("IRANSansWeb(FaNum)"),
                FontSize = 12,
                TextAlignment = TextAlignment.Left,
                FlowDirection = FlowDirection.RightToLeft,
            };
            // محتوای اصلی درآمد در یک StackPanel قرار می‌گیرد
            Section mainContent = new Section();
            // عنوان
            Paragraph title = new Paragraph(new Run("گزارش هزینه ها"))
            {
                Foreground = new SolidColorBrush(Colors.Crimson),
                FontSize = 20,
                FontWeight = FontWeights.Bold,
                TextAlignment = TextAlignment.Center,
                Margin = new Thickness(0, 0, 0, 20)
            };
            mainContent.Blocks.Add(title);

            // جدول پرداخت ها
            Table table = new Table
            {
                CellSpacing = 0,
                Padding = new Thickness(5),
                BorderBrush = Brushes.Black,
                BorderThickness = new Thickness(1.5),
            };

            // تعریف ستون‌ها - ستون اول عریض‌تر
            table.Columns.Add(new TableColumn { Width = new GridLength(80) }); // عنوان هزینه
            table.Columns.Add(new TableColumn { Width = new GridLength(110) }); // مبلغ
            table.Columns.Add(new TableColumn { Width = new GridLength(80) }); // تاریخ
            table.Columns.Add(new TableColumn { Width = new GridLength(80) }); // نوع هزینه          
            table.Columns.Add(new TableColumn { Width = new GridLength(330)}); // توضیحات
           
            // ردیف عنوان
            TableRowGroup headerGroup = new TableRowGroup();
            TableRow header = new TableRow();
            header.Cells.Add(new TableCell(new Paragraph(new Run("عنوان هزینه"))) { FontWeight = FontWeights.Bold });
            header.Cells.Add(new TableCell(new Paragraph(new Run("مبلغ"))) { FontWeight = FontWeights.Bold });
            header.Cells.Add(new TableCell(new Paragraph(new Run("تاریخ"))) { FontWeight = FontWeights.Bold });
            header.Cells.Add(new TableCell(new Paragraph(new Run("نوع هزینه"))) { FontWeight = FontWeights.Bold });
            header.Cells.Add(new TableCell(new Paragraph(new Run("توضیحات"))) { FontWeight = FontWeights.Bold });          
            headerGroup.Rows.Add(header);
            table.RowGroups.Add(headerGroup);

            // داده‌ها
            TableRowGroup dataGroup = new TableRowGroup();
            decimal totalAmount = 0; // جمع مبالغ
                                     // 
            foreach (var item in items)
            {               
                TableRow row = new TableRow();
                row.Cells.Add(new TableCell(new Paragraph(new Run(item.Title))));
                row.Cells.Add(new TableCell(new Paragraph(new Run($"{item.Amount:N0} ريال"))));
                row.Cells.Add(new TableCell(new Paragraph(new Run(item.ExpenseDate.ToString("yyyy/MM/dd")))));
                row.Cells.Add(new TableCell(new Paragraph(new Run(item.ExpenseType))));
                var descriptionRun = new Run(item.Description)
                {
                    FontSize = 9 // یا هر سایز دلخواه مثلاً 12، 16 و ...
                };
                var paragraph = new Paragraph(descriptionRun);
                var cell = new TableCell(paragraph);
                row.Cells.Add(cell);
                
                dataGroup.Rows.Add(row);
                totalAmount = lebll.GetTotalAmount().Value;
            }
            table.RowGroups.Add(dataGroup); // اضافه کردن جدول به سند
            mainContent.Blocks.Add(table);

            // 👇 پاراگراف جمع کل بعد از جدول
            Paragraph totalAmountParagraph = new Paragraph
            {
                Margin = new Thickness(0, 20, 0, 0),
                FontWeight = FontWeights.Bold,
                FontSize = 12,
                TextAlignment = TextAlignment.Left,
            };

            var brushconverter = new System.Windows.Media.BrushConverter();

            Run dateReportText;
            if (!string.IsNullOrWhiteSpace(ExpenseStartDP.Text) && !string.IsNullOrWhiteSpace(ExpenseEndDP.Text))
            {
                dateReportText = new Run($"گزارش بازه زمانی {ExpenseStartDP.Text} الی {ExpenseEndDP.Text}\n")
                {
                    FontSize = 11,
                    Foreground = Brushes.Gray
                };
            }
            else
            {
                dateReportText = new Run("گزارش کلی بدون بازه زمانی\n")
                {
                    FontSize = 11,
                    Foreground = Brushes.Gray
                };
            }
            totalAmountParagraph.Inlines.Add(dateReportText);
            Run totalRecordsText = new Run("تعداد رکوردها: ")
            {
                Foreground = Brushes.Black
            };
            Run totalRecordsValue = new Run($"{items.Count()} \n")
            {
                Foreground = Brushes.DarkRed
            };
            Run totalAmountText = new Run("جمع کل هزینه ها: ")
            {
                Foreground = Brushes.Black
            };
            Run totalAmountValue = new Run($"{totalAmount:N0} ریال \n\n")
            {
                Foreground = Brushes.DarkRed
            };

            var userType = "";
            foreach (var item in items)
            {
                userType = item.UserTypeText;
            }

            Run UserTypeText = new Run("کاربر: ")
            {
                Foreground = Brushes.Black
            };
            Run UserTypeValue = new Run($"{userType} \n")
            {
                Foreground = (Brush)brushconverter.ConvertFromString("#1259D5")
            };

            var lawyername = "";
            if (AppSession.CurrentUser.Role == User.UserRole.Lawyer)
            {
                lawyername = AppSession.CurrentUser.FullName ?? "نامشخص";
            }
            else if (AppSession.CurrentUser.Role == User.UserRole.Secretary)
            {
                lawyername = AppSession.CurrentUser.FullNameLawyer ?? "نامشخص";
            }

            Run LawyerNameText = new Run("دفتر وکالت آقای/خانم: ")
            {
                Foreground = Brushes.Black
            };
            Run LawyerNameValue = new Run($"{lawyername} \n\n")
            {
                Foreground = (Brush)brushconverter.ConvertFromString("#1259D5")
            };

            Run DeveloperText = new Run("⚖ نرم افزار حقوقی هـــیام \n ارتباط با توسعه دهنده: 989023349043+  | محمدی نصیر")
            {
                FontWeight = FontWeights.Normal,
                FontSize = 12,
                Foreground = Brushes.Black
            };            
            totalAmountParagraph.Inlines.Add(totalRecordsText);
            totalAmountParagraph.Inlines.Add(totalRecordsValue);
            totalAmountParagraph.Inlines.Add(totalAmountText);
            totalAmountParagraph.Inlines.Add(totalAmountValue);
            totalAmountParagraph.Inlines.Add(UserTypeText);
            totalAmountParagraph.Inlines.Add(UserTypeValue);
            totalAmountParagraph.Inlines.Add(LawyerNameText);
            totalAmountParagraph.Inlines.Add(LawyerNameValue);
            totalAmountParagraph.Inlines.Add(DeveloperText);
            mainContent.Blocks.Add(table);
            mainContent.Blocks.Add(totalAmountParagraph); // افزودن جمع کل به سند
                                                          // بسته‌بندی تمام محتوا داخل یک Border برای نمایش حاشیه
            BlockUIContainer borderedContainer = new BlockUIContainer
            {
                Child = new Border
                {
                    BorderBrush = Brushes.Gray,
                    BorderThickness = new Thickness(2),
                    CornerRadius = new CornerRadius(3),
                    Padding = new Thickness(5),
                    //Visibility = Visibility.Collapsed,
                    Child = new FlowDocumentScrollViewer
                    {
                        Document = new FlowDocument(mainContent)
                        {
                            PagePadding = new Thickness(3),
                            FontFamily = new FontFamily("IRANSansWeb(FaNum)"),
                            FontSize = 12
                        }
                    }
                }
            };
            doc.Blocks.Add(borderedContainer);
            // نمایش در DocumentViewer یا پرینت
            PrintFlowDocumentExpense(doc);
        }
        private void PrintFlowDocumentExpense(FlowDocument doc)
        {
            PrintDialog printDialog = new PrintDialog();
            if (printDialog.ShowDialog() == true)
            {
                IDocumentPaginatorSource idpSource = doc;
                printDialog.PrintDocument(idpSource.DocumentPaginator, "گزارش هزینه ها");
            }
        }
        #endregion

        #endregion      
    }
}