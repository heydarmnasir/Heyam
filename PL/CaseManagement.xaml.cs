using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Effects;
using BLL;
using BE;
using System.IO;
using System.Diagnostics;
using BE.ViewModel;
using PL;
using IPE.SmsIrClient.Models.Requests;
using IPE.SmsIrClient;

namespace Heyam
{  
    public partial class CaseManagement : Window
    {
        private MainWindow _mainWindow;

        public CaseManagement(MainWindow mainWindow)
        {
            InitializeComponent();
            RefreshCases();
            RefreshCorrespondence();
            _mainWindow = mainWindow;

            #region Clients FullNames In SelectClient ComboBox
            var clients = cbll.GetAllClients(); // از BLL           
            SelectClientNameCB.ItemsSource = clients;
            SelectClientNameCB.DisplayMemberPath = "FullName"; // فرض بر اینکه پراپرتی FullName داری
            SelectClientNameCB.SelectedValuePath = "Id";
            SelectClientCB.ItemsSource = clients;
            SelectClientCB.DisplayMemberPath = "FullName"; // فرض بر اینکه پراپرتی FullName داری
            SelectClientCB.SelectedValuePath = "Id";
            #endregion          
        }

        Client_BLL cbll = new Client_BLL();
        Case_BLL case_bll = new Case_BLL();
        Correspondence_BLL correspondence_BLL = new Correspondence_BLL();
        Dashboard_BLL dbll = new Dashboard_BLL();
        Reminder_BLL rbll = new Reminder_BLL();

        private Case _case;
        private Correspondence _correspondence;

        #region CaseAttachmentList
        private List<CaseAttachment> Caseattachments = new List<CaseAttachment>();
        private List<CaseAttachment> Existattachments = new List<CaseAttachment>();
        private List<CaseAttachment> deletedAttachments = new List<CaseAttachment>();
        #endregion
        #region CorrespondenceAttachmentList
        private List<CorrespondenceAttachment> correspondenceattachments = new List<CorrespondenceAttachment>();
        private List<CorrespondenceAttachment> Existcorrespondenceattachments = new List<CorrespondenceAttachment>();
        private List<CorrespondenceAttachment> deletedcorrespondenceAttachments = new List<CorrespondenceAttachment>();
        #endregion

        #region Methods
        public void OpenBlurWindow(Window window)
        {
            BlurEffect blurEffect = new BlurEffect();
            this.Effect = blurEffect;
            blurEffect.Radius = 10;
            window.ShowDialog();
            this.Effect = null;
        }
        private void SaveChanges()
        {
            _mainWindow.RefreshPage(); // صدا زدن متد از MainWindow
        }
        public void RefreshCases()
        {
            CasesDGV.ItemsSource = null;            
            CasesDGV.ItemsSource = case_bll.GetCasesForListView();
            CasesCountLabel.Text = dbll.TotalCasesCount();
        }
        public void RefreshCorrespondence()
        {
            CorrespondencesDGV.ItemsSource = null;
            CorrespondencesDGV.ItemsSource = correspondence_BLL.GetCorrespondencesForListView();
            CorrespondenceCountLabel.Text = dbll.CorrespondenceCount();
        }
        private void RefreshFilesList()
        {
            FilesListBox.ItemsSource = null;
            FilesListBox.ItemsSource = Caseattachments;      
        }
        private void RefreshPopupFilesList()
        {
            PopupFilesListBox.ItemsSource = null;
            PopupFilesListBox.ItemsSource = Existattachments;
        }
        private void RefreshCorrespondenceFilesList()
        {
            CorrespondenceFilesListBox.ItemsSource = null;
            CorrespondenceFilesListBox.ItemsSource = correspondenceattachments;
        }
        private void RefreshPopupCorrespondenceFilesList()
        {
            PopupCorrespondenceFilesListBox.ItemsSource = null;
            PopupCorrespondenceFilesListBox.ItemsSource = Existcorrespondenceattachments;
        }
        private void LoadCaseForEdit(Case caseToEdit)
        {
            _case = caseToEdit;
            Existattachments = caseToEdit.CaseAttachments?.ToList() ?? new List<CaseAttachment>();
            RefreshPopupFilesList();
        }
        private void LoadCorrespondenceForEdit(Correspondence correspondenceToEdit)
        {
            _correspondence = correspondenceToEdit;
            Existcorrespondenceattachments = correspondenceToEdit.CorrespondenceAttachments?.ToList() ?? new List<CorrespondenceAttachment>();
            RefreshPopupCorrespondenceFilesList();
        }
        private void ClearControls(DependencyObject parent)
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);

                // پاک کردن تمام آیتم‌های لیست باکس پیوست‌ها
                Caseattachments.Clear();
                correspondenceattachments.Clear();
               
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
        #endregion

        #region Case
        private void DescriptionTB_TextChanged(object sender, TextChangedEventArgs e)
        {
            CharCountLabel.Text = $"{DescriptionTB.Text.Length} کاراکتر";
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
                    if (!Caseattachments.Any(a => a.FilePath == file))
                    {
                        Caseattachments.Add(new CaseAttachment
                        {
                            FileName = Path.GetFileName(file),
                            FilePath = file // مسیر اصلی فعلاً
                        });
                    }
                }
                RefreshFilesList();
            }
            ShowDocBTN.IsEnabled = FilesListBox.SelectedItem != null;
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

                    if (!Caseattachments.Any(a => a.FileName == fileName && a.FilePath == path))
                    {
                        Caseattachments.Add(new CaseAttachment
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
            if (button?.Tag is BE.CaseAttachment attachmentToDelete)
            {
                try
                {
                    // حذف از لیست
                    Caseattachments.Remove(attachmentToDelete);
                    // رفرش مجدد لیست
                    FilesListBox.ItemsSource = null;
                    FilesListBox.ItemsSource = Caseattachments;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"خطا در حذف فایل: {ex.Message}", "خطا", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
        private void ShowDocBTN_Click(object sender, RoutedEventArgs e)
        {
            if (FilesListBox.SelectedItem is BE.CaseAttachment selectedAttachment)
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
        private void FilesListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ShowDocBTN.IsEnabled = FilesListBox.SelectedItem != null; // دکمه فقط وقتی فعال شود که فایلی انتخاب شده باشد
        }
        private void ShowCaseRegisterBTN_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            ShowCaseRegisterBTN.Background = new SolidColorBrush(Colors.DodgerBlue);
            ShowCaseRegisterBTN.Foreground = new SolidColorBrush(Colors.White);
        }
        private void ShowCaseRegisterBTN_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            ShowCaseRegisterBTN.Background = new SolidColorBrush(Colors.White);
            ShowCaseRegisterBTN.Foreground = new SolidColorBrush(Colors.Black);
        }
        private void ShowCaseRegisterBTN_Click(object sender, RoutedEventArgs e)
        {
            CaseRegisterPopup.Visibility = Visibility.Visible;
            MainGrid.Effect = new BlurEffect() { Radius = 5 };
        }
        private void SearchCases_TextChanged(object sender, TextChangedEventArgs e)
        {
            string keyword = SearchCases.Text.Trim();
            int? statusFilter = null;
            List<CaseDto> filtered;

            if (string.IsNullOrWhiteSpace(keyword))
            {
                CasesPlaceholderText.Visibility = Visibility.Visible;
                filtered = case_bll.GetCasesForListView();
                CasesDGV.ItemsSource = filtered;
            }
            else
            {
                filtered = case_bll.SearchCasesForDGV(keyword, statusFilter);
                CasesDGV.ItemsSource = filtered;
                CasesPlaceholderText.Visibility = Visibility.Hidden;
            }
        }                             
        private void DescriptionPopupTB_TextChanged(object sender, TextChangedEventArgs e)
        {
            PopupCharCountLabel.Text = $"{DescriptionPopupTB.Text.Length} کاراکتر";
        }
        private void PopupShowDocBTN_Click(object sender, RoutedEventArgs e)
        {
            if (PopupFilesListBox.SelectedItem is CaseAttachment selectedAttachment)
            {
                try
                {
                    var fileName = selectedAttachment.FileName;
                    var fullPath = System.IO.Path.Combine(
                        Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                        "Heyam", "CaseAttachments", fileName);

                    if (File.Exists(fullPath))
                    {
                        Process.Start(new ProcessStartInfo
                        {
                            FileName = fullPath,
                            UseShellExecute = true
                        });
                    }
                    else if (!File.Exists(fullPath))
                    {
                        if (PopupFilesListBox.SelectedItem is BE.CaseAttachment selectedPopupAttachment)
                        {
                            try
                            {
                                Process.Start(new ProcessStartInfo
                                {
                                    FileName = selectedPopupAttachment.FilePath,
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
                    else
                    {
                        MessageBox.Show("فایل مورد نظر در مسیر مشخص ‌شده وجود ندارد.");
                    }
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
        private void CaseStatusPopupCB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CaseStatusPopupCB.SelectedIndex == 1) //مختومه
            {
                ClosingDatePopupDP.IsEnabled = true;
            }
            else
            {
                ClosingDatePopupDP.Text = null;
                ClosingDatePopupDP.IsEnabled = false;
            }
        }     
        private void CaseDetailsPopupCloseBTN_Click(object sender, RoutedEventArgs e)
        {
            CaseDetailsPopup.Visibility = Visibility.Collapsed;
            MainGrid.Effect = null;
        }
        private void PopupDropBoxDocuments_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                PopupDropBoxDocuments.Background = new SolidColorBrush(Color.FromRgb(220, 240, 255)); // تغییر رنگ پس‌زمینه هنگام ورود فایل
                e.Effects = DragDropEffects.Copy;
            }
            else
            {
                e.Effects = DragDropEffects.None;
            }
        }
        private void PopupDropBoxDocuments_DragLeave(object sender, DragEventArgs e)
        {
            PopupDropBoxDocuments.Background = new SolidColorBrush(Color.FromRgb(230, 244, 255)); // بازگرداندن رنگ اولیه
        }
        private void PopupDropBoxDocuments_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

                foreach (var file in files)
                {
                    // بررسی تکراری نبودن فایل بر اساس مسیر فایل اصلی
                    if (!Caseattachments.Any(a => a.FilePath == file))
                    {
                        Caseattachments.Add(new CaseAttachment
                        {
                            FileName = Path.GetFileName(file),
                            FilePath = file // مسیر اصلی فعلاً
                        });
                    }
                }
                RefreshPopupFilesList();
            }
            PopupShowDocBTN.IsEnabled = PopupFilesListBox.SelectedItem != null;
        }
        private void PopupFilesListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            PopupShowDocBTN.IsEnabled = PopupFilesListBox.SelectedItem != null; // دکمه فقط وقتی فعال شود که فایلی انتخاب شده باشد
        }
        private void PopupAddDocBTN_Click(object sender, RoutedEventArgs e)
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

                    if (!Existattachments.Any(a => a.FileName == fileName && a.FilePath == path))
                    {
                        Existattachments.Add(new CaseAttachment
                        {
                            FileName = fileName,
                            FilePath = path, // ⛔ فعلاً فایل رو جابجا نکن، فقط آدرس موقتی نگه دار
                            UploadedAt = DateTime.Now
                        });
                    }
                }
                RefreshPopupFilesList();
            }           
        }
        private void CancelBTN_Click(object sender, RoutedEventArgs e)
        {
            CaseRegisterPopup.Visibility = Visibility.Collapsed;
            MainGrid.Effect = null;
            ClearControls(CaseRegisterPopup);
            ShowDocBTN.IsEnabled = false;
        }
        private void PopupDeleteFile_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (button?.Tag is CaseAttachment selectedAttachment)
            {
                Caseattachments.Remove(selectedAttachment);                
                if (selectedAttachment.Id != 0) // یعنی قبلاً ذخیره شده
                {
                    deletedAttachments.Add(selectedAttachment);
                }
                RefreshPopupFilesList();
            }
        }
        private void StatusFilterCB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string searchTerm = SearchCases?.Text?.Trim() ?? string.Empty;
            int? statusFilter = null;
            if (int.TryParse(StatusFilterCB.SelectedValue?.ToString(), out int parsedStatus))
            {
                statusFilter = parsedStatus;
            }
            var result = case_bll.SearchCasesForDGV(searchTerm, statusFilter);
            CasesDGV.ItemsSource = null;
            CasesDGV.ItemsSource = result;
        }     
        private void CaseRegisterBTN_Click(object sender, RoutedEventArgs e)
        {
            string input = CaseNumberTB.Text.Trim();
            if (string.IsNullOrWhiteSpace(SelectClientNameCB.Text) || string.IsNullOrWhiteSpace(CaseNumberTB.Text) || string.IsNullOrWhiteSpace(CaseTitleCB.Text) || string.IsNullOrWhiteSpace(CaseSubjectTB.Text) || string.IsNullOrWhiteSpace(SetDateDP.Text) || string.IsNullOrWhiteSpace(CaseStatusCB.Text))
            {
                ShowNotification("اطلاعات ضروری را وارد کنید!", "error");
            }
            else if (input.All(char.IsDigit) && input.Length >= 16 && input.Length <= 18)
            {
                // بررسی وجود موکل در ComboBox
                if (SelectClientNameCB.SelectedItem == null)
                {
                    ShowNotification("موکل در سیستم موجود نمیباشد!", "error");
                    return;
                }                
                try
                {
                    // دریافت نام موکل و شماره پرونده
                    string clientName = SelectClientNameCB.Text.Trim(); // یا هر فیلدی که نمایش نام موکل را دارد
                    string caseNumber = CaseNumberTB.Text.Trim();

                    foreach (var attachment in Caseattachments)
                    {
                        string documentsPath = Path.Combine(
                            Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                            "Heyam", "CaseAttachments", clientName, caseNumber);

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
                    var newCase = new Case
                    {
                        UserId = user.Id,
                        UserRole = (int)user.Role,
                        ClientId = (int)SelectClientNameCB.SelectedValue,
                        OpponentPerson = OpponentPersonTB.Text,
                        CaseNumber = CaseNumberTB.Text,
                        CaseArchiveNumber = CaseArchiveNumberTB.Text,
                        SetDate = SetDateDP.SelectedDate.Value,
                        CaseTitle = CaseTitleCB.Text,
                        CaseSubject = CaseSubjectTB.Text,
                        ProcessingBranch = ProcessingBranchTB.Text,
                        CaseStatus = CaseStatusCB.SelectedIndex,
                        Description = DescriptionTB.Text,
                        CaseAttachments = Caseattachments.ToList(),
                        CreatedAt = DateTime.Now
                    };
                    if (case_bll.IsExist(newCase) == false)
                    {
                        ShowNotification(case_bll.Create(newCase), "success");
                        RefreshCases();
                        SaveChanges();
                        ClearControls(CaseRegisterPopup);
                        CaseRegisterPopup.Visibility = Visibility.Collapsed;
                        MainGrid.Effect = null;
                    }
                    else
                    {
                        ShowNotification("پرونده در سیستم موجود میباشد!", "error");
                    }
                }
                catch (Exception)
                {
                    ShowNotification("خطا در ثبت پرونده!", "error");
                }
            }
            else
            {
                ShowNotification("شماره پرونده باید فقط شامل عدد و بین ۱۶ تا ۱۸ رقم باشد!", "error");
            }
        }
        private void ShowCaseDetailsBTN_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            if (btn?.Tag is int caseId)
            {
                CaseDetailsPopup.Tag = caseId;
                var _case = case_bll.GetCaseById(caseId);
                // پر کردن جزئیات در پاپ‌آپ               
                OpponentPersonPopupTB.Text = _case.OpponentPerson;
                CaseArchiveNumberPopupTB.Text = _case.CaseArchiveNumber;
                ProcessingBranchPopupTB.Text = _case.ProcessingBranch;
                CaseStatusPopupCB.SelectedIndex = _case.CaseStatus;
                ClosingDatePopupDP.Text = _case.ClosingDate;
                DescriptionPopupTB.Text = _case.Description;
                LoadCaseForEdit(_case);
                CaseDetailsPopup.Visibility = Visibility.Visible;
                MainGrid.Effect = new BlurEffect() { Radius = 5 };
            }
        }
        private void CaseDetailsPopupUpdateBTN_Click(object sender, RoutedEventArgs e)
        {
            if (!(CaseDetailsPopup.Tag is int caseId))
            {
                ShowNotification("آیدی پرونده مشخص نیست!", "error");
                return;
            }
            var _case = case_bll.GetCaseById(caseId);
            if (_case == null)
            {
                ShowNotification("پرونده موردنظر یافت نشد!", "error");
                return;
            }
            if (string.IsNullOrWhiteSpace(OpponentPersonPopupTB.Text) || string.IsNullOrWhiteSpace(CaseArchiveNumberPopupTB.Text) || string.IsNullOrWhiteSpace(ProcessingBranchPopupTB.Text) || string.IsNullOrWhiteSpace(CaseStatusPopupCB.Text))
            {
                ShowNotification("اطلاعات ضروری را وارد کنید!", "error");
            }
            else
            {             
                foreach (var attachment in Existattachments)
                {
                    string documentsPath = Path.Combine(
                        Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                        "Heyam", "CaseAttachments", _case.Client.FullName, _case.CaseNumber);

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

                _case.OpponentPerson = OpponentPersonPopupTB.Text;
                _case.CaseArchiveNumber = CaseArchiveNumberPopupTB.Text;
                _case.ProcessingBranch = ProcessingBranchPopupTB.Text;
                _case.CaseStatus = CaseStatusPopupCB.SelectedIndex;
                _case.Description = DescriptionPopupTB.Text;

                _case.ClosingDate = (CaseStatusPopupCB.SelectedIndex == 1)
                                    ? ClosingDatePopupDP.Text
                                    : null;
                // افزودن پیوست‌های ویرایش‌شده
                _case.CaseAttachments = Existattachments;
                bool result = case_bll.UpdateCase(_case, deletedAttachments);
                ShowNotification($"ویرایش اطلاعات با موفقیت انجام شد", "success");
                RefreshCases();
                SaveChanges();
                CaseDetailsPopup.Visibility = Visibility.Collapsed;
                MainGrid.Effect = null;
            }
        }      
        #endregion

        #region Correspondence
        private void CorrespondenceBTN_Click(object sender, RoutedEventArgs e)
        {
            CorrespondenceWindow.Visibility = Visibility.Visible;
            MainGrid.Visibility = Visibility.Collapsed;
        }
        private void CorrespondenceBTN_MouseEnter(object sender, MouseEventArgs e)
        {
            CorrespondenceBTN.Background = new SolidColorBrush(Colors.DodgerBlue);
            CorrespondenceBTN.Foreground = new SolidColorBrush(Colors.White);
        }
        private void CorrespondenceBTN_MouseLeave(object sender, MouseEventArgs e)
        {
            CorrespondenceBTN.Background = new SolidColorBrush(Colors.White);
            CorrespondenceBTN.Foreground = new SolidColorBrush(Colors.Black);
        }
        private void AddCorrespondenceBTN_Click(object sender, RoutedEventArgs e)
        {
            AddCorrespondenceWindow.Visibility = Visibility.Visible;
            CorrespondenceWindow.Effect = new BlurEffect() { Radius = 5 };
        }
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
        private void CorrespondenceDescriptionTB_TextChanged(object sender, TextChangedEventArgs e)
        {
            CorrespondenceCharCountLabel.Text = $"{CorrespondenceDescriptionTB.Text.Length} کاراکتر";
        }     
        private void CorrespondenceDropBoxDocuments_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

                foreach (var file in files)
                {
                    // بررسی تکراری نبودن فایل بر اساس مسیر فایل اصلی
                    if (!correspondenceattachments.Any(a => a.FilePath == file))
                    {
                        correspondenceattachments.Add(new CorrespondenceAttachment
                        {
                            FileName = Path.GetFileName(file),
                            FilePath = file // مسیر اصلی فعلاً
                        });
                    }
                }
                RefreshCorrespondenceFilesList();
            }
            CorrespondenceShowDocBTN.IsEnabled = CorrespondenceFilesListBox.SelectedItem != null;           
        }
        private void CorrespondenceDropBoxDocuments_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                CorrespondenceDropBoxDocuments.Background = new SolidColorBrush(Color.FromRgb(220, 240, 255)); // تغییر رنگ پس‌زمینه هنگام ورود فایل
                e.Effects = DragDropEffects.Copy;
            }
            else
            {
                e.Effects = DragDropEffects.None;
            }
        }
        private void CorrespondenceDropBoxDocuments_DragLeave(object sender, DragEventArgs e)
        {
            CorrespondenceDropBoxDocuments.Background = new SolidColorBrush(Color.FromRgb(230, 244, 255)); // بازگرداندن رنگ اولیه
        }
        private void CorrespondenceFilesListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CorrespondenceShowDocBTN.IsEnabled = CorrespondenceFilesListBox.SelectedItem != null; // دکمه فقط وقتی فعال شود که فایلی انتخاب شده باشد
        }
        private void CorrespondenceDeleteFile_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (button?.Tag is BE.CorrespondenceAttachment correspondenceattachmentToDelete)
            {
                try
                {
                    // حذف از لیست
                    correspondenceattachments.Remove(correspondenceattachmentToDelete);
            
                    // رفرش مجدد لیست
                    CorrespondenceFilesListBox.ItemsSource = null;
                    CorrespondenceFilesListBox.ItemsSource = correspondenceattachments;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"خطا در حذف فایل: {ex.Message}", "خطا", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
        private void CorrespondenceAddDocBTN_Click(object sender, RoutedEventArgs e)
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

                    if (!correspondenceattachments.Any(a => a.FileName == fileName && a.FilePath == path)) // FilePath فعلاً مسیر موقت
                    {
                        correspondenceattachments.Add(new CorrespondenceAttachment
                        {
                            FileName = fileName,
                            FilePath = path, // ⛔ فعلاً فایل رو جابجا نکن، فقط آدرس موقتی نگه دار
                            UploadedAt = DateTime.Now
                        });
                    }
                }
                RefreshCorrespondenceFilesList();
            }           
        }
        private void CorrespondenceShowDocBTN_Click(object sender, RoutedEventArgs e)
        {
            if (CorrespondenceFilesListBox.SelectedItem is BE.CorrespondenceAttachment selectedAttachment)
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
        private void CorrespondenceCancelBTN_Click(object sender, RoutedEventArgs e)
        {
            AddCorrespondenceWindow.Visibility = Visibility.Collapsed;
            ClearControls(AddCorrespondenceWindow);
            CaseSubjectLabel.Text = "";          
            CorrespondenceShowDocBTN.IsEnabled = false;
            CorrespondenceWindow.Effect = null;
        }
        private void SearchCorrespondence_TextChanged(object sender, TextChangedEventArgs e)
        {
            string keyword = SearchCorrespondence.Text.Trim();
            int? statusFilter = null;
            List<CorrespondenceDto> filtered;

            if (string.IsNullOrWhiteSpace(keyword))
            {
                CorrespondencePlaceholderText.Visibility = Visibility.Visible;
                filtered = correspondence_BLL.GetCorrespondencesForListView();
                CorrespondencesDGV.ItemsSource = filtered;
            }
            else
            {
                filtered = correspondence_BLL.SearchCorrespondenceForDGV(keyword, statusFilter);
                CorrespondencesDGV.ItemsSource = filtered;
                CorrespondencePlaceholderText.Visibility = Visibility.Hidden;
            }           
        }
        private void CloseCorrespondenceWindowBTN_Click(object sender, RoutedEventArgs e)
        {
            CorrespondenceWindow.Visibility = Visibility.Collapsed;
            MainGrid.Visibility = Visibility.Visible;
        }       
        private void CorrespondenceDescriptionPopupTB_TextChanged(object sender, TextChangedEventArgs e)
        {
            PopupCorrespondenceCharCountLabel.Text = $"{CorrespondenceDescriptionPopupTB.Text.Length} کاراکتر";
        }
        private void PopupCorrespondenceDropBoxDocuments_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

                foreach (var file in files)
                {
                    // بررسی تکراری نبودن فایل بر اساس مسیر فایل اصلی
                    if (!correspondenceattachments.Any(a => a.FilePath == file))
                    {
                        correspondenceattachments.Add(new CorrespondenceAttachment
                        {
                            FileName = Path.GetFileName(file),
                            FilePath = file // مسیر اصلی فعلاً
                        });
                    }
                }
                RefreshPopupCorrespondenceFilesList();
            }
            PopupCorrespondenceShowDocBTN.IsEnabled = PopupCorrespondenceFilesListBox.SelectedItem != null;
        }
        private void PopupCorrespondenceDropBoxDocuments_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                PopupCorrespondenceDropBoxDocuments.Background = new SolidColorBrush(Color.FromRgb(220, 240, 255)); // تغییر رنگ پس‌زمینه هنگام ورود فایل
                e.Effects = DragDropEffects.Copy;
            }
            else
            {
                e.Effects = DragDropEffects.None;
            }
        }
        private void PopupCorrespondenceDropBoxDocuments_DragLeave(object sender, DragEventArgs e)
        {
            PopupCorrespondenceDropBoxDocuments.Background = new SolidColorBrush(Color.FromRgb(230, 244, 255)); // بازگرداندن رنگ اولیه
        }
        private void PopupCorrespondenceFilesListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            PopupCorrespondenceShowDocBTN.IsEnabled = PopupCorrespondenceFilesListBox.SelectedItem != null; // دکمه فقط وقتی فعال شود که فایلی انتخاب شده باشد
        }
        private void PopupCorrespondenceDeleteFile_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (button?.Tag is CorrespondenceAttachment selectedAttachment)
            {
                correspondenceattachments.Remove(selectedAttachment);                
                if (selectedAttachment.Id != 0) // یعنی قبلاً ذخیره شده
                {
                    deletedcorrespondenceAttachments.Add(selectedAttachment);
                }
                RefreshPopupCorrespondenceFilesList();
            }
        }
        private void PopupCorrespondenceAddDocBTN_Click(object sender, RoutedEventArgs e)
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

                    if (!Existcorrespondenceattachments.Any(a => a.FilePath == path)) // FilePath فعلاً مسیر موقت
                    {
                        Existcorrespondenceattachments.Add(new CorrespondenceAttachment
                        {
                            FileName = fileName,
                            FilePath = path, // ⛔ فعلاً فایل رو جابجا نکن، فقط آدرس موقتی نگه دار
                            UploadedAt = DateTime.Now
                        });
                    }
                }
                RefreshPopupCorrespondenceFilesList();
            }
        }       
        private void PopupCorrespondenceShowDocBTN_Click(object sender, RoutedEventArgs e)
        {
            if (PopupCorrespondenceFilesListBox.SelectedItem is CorrespondenceAttachment selectedAttachment)
            {
                try
                {
                    var fileName = selectedAttachment.FileName;
                    var fullPath = System.IO.Path.Combine(
                        Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                        "Heyam", "CorrespondenceAttachments", fileName);

                    if (File.Exists(fullPath))
                    {
                        Process.Start(new ProcessStartInfo
                        {
                            FileName = fullPath,
                            UseShellExecute = true
                        });
                    }
                    else if (!File.Exists(fullPath))
                    {
                        if (PopupCorrespondenceFilesListBox.SelectedItem is BE.CorrespondenceAttachment selectedPopupAttachment)
                        {
                            try
                            {
                                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                                {
                                    FileName = selectedPopupAttachment.FilePath,
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
                    else
                    {
                        MessageBox.Show("فایل مورد نظر در مسیر مشخص ‌شده وجود ندارد.");
                    }
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
        private void CorrespondenceDetailsPopupCloseBTN_Click(object sender, RoutedEventArgs e)
        {
            CorrepondenceDetailsPopup.Visibility = Visibility.Collapsed;
            CorrespondenceWindow.Effect = null;
        }
        private void CorrespondenceReminderCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            CorrespondenceReminderDate.IsEnabled = true;
        }
        private void CorrespondenceReminderCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            CorrespondenceReminderDate.IsEnabled = false;
            CorrespondenceReminderDate.SelectedDate = null;
        }
        private void CorrespondenceReminderCheckBoxPopup_Checked(object sender, RoutedEventArgs e)
        {
            CorrespondenceReminderDatePopup.IsEnabled = true;
        }
        private void CorrespondenceReminderCheckBoxPopup_Unchecked(object sender, RoutedEventArgs e)
        {
            CorrespondenceReminderDatePopup.IsEnabled = false;
            CorrespondenceReminderDatePopup.SelectedDate = null;
        }
        private void CorrespondenceStatusFilterCB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string searchTerm = SearchCorrespondence?.Text?.Trim() ?? string.Empty;
            int? statusFilter = null;
            if (int.TryParse(CorrespondenceStatusFilterCB.SelectedValue?.ToString(), out int parsedStatus))
            {
                statusFilter = parsedStatus;
            }
            var result = correspondence_BLL.SearchCorrespondenceForDGV(searchTerm, statusFilter);
            CorrespondencesDGV.ItemsSource = null;
            CorrespondencesDGV.ItemsSource = result;
        }      
        private void CorrespondenceRegisterBTN_Click(object sender, RoutedEventArgs e)
        {
            string input = SelectCaseCB.Text.Trim();

            if (string.IsNullOrWhiteSpace(SelectCaseCB.Text) || string.IsNullOrWhiteSpace(CorrespondenceTitleTB.Text) || string.IsNullOrWhiteSpace(CorrespondenceTypeCB.Text) || string.IsNullOrWhiteSpace(CorrespondenceSenderCB.Text) || string.IsNullOrWhiteSpace(CorrespondenceReceiverCB.Text) || string.IsNullOrWhiteSpace(CorrespondenceSetDateDP.Text) || string.IsNullOrWhiteSpace(CorrespondenceStatusCB.Text))
            {
                ShowNotification("اطلاعات ضروری را وارد کنید!", "error");
            }
            else if (input.All(char.IsDigit) && input.Length >= 16 && input.Length <= 18)
            {
                try
                {
                    // بررسی وجود پرونده در ComboBox
                    if (SelectCaseCB.SelectedItem == null)
                    {
                        ShowNotification("پرونده در سیستم موجود نمیباشد!", "error");
                        return;
                    }

                    foreach (var attachment in correspondenceattachments)
                    {
                        // دریافت نام موکل و شماره پرونده
                        string CaseNumber = SelectCaseCB.Text.Trim(); // یا هر فیلدی که نمایش نام موکل را دارد
                        string Title = CorrespondenceTitleTB.Text.Trim();

                        string documentsPath = Path.Combine(
                            Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                            "Heyam", "CorrespondenceAttachments", CaseNumber, Title);

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
                    var newcorrespondence = new Correspondence
                    {
                        UserId = user.Id,
                        UserRole = (int)user.Role,
                        CaseId = (int)SelectCaseCB.SelectedValue,
                        Title = CorrespondenceTitleTB.Text,
                        Subject = CorrespondenceSubjectTB.Text,
                        CorrespondenceNumber = CorrespondenceNumberTB.Text,
                        Type = CorrespondenceTypeCB.Text,
                        Sender = CorrespondenceSenderCB.Text,
                        Receiver = CorrespondenceReceiverCB.Text,
                        CorrespondenceDate = CorrespondenceSetDateDP.Text,
                        Status = CorrespondenceStatusCB.SelectedIndex,
                        CorrespondenceDescription = CorrespondenceDescriptionTB.Text,
                        CorrespondenceAttachments = correspondenceattachments.ToList(),
                        IsReminderSet = CorrespondenceReminderCheckBox.IsChecked == true,
                        ReminderDate = CorrespondenceReminderCheckBox.IsChecked == true ? CorrespondenceReminderDate.SelectedDate : null,
                        IsReminderDone = false,
                        CreatedAt = DateTime.Now
                    };
                    int correpondenceId = correspondence_BLL.AddCorrespondence(newcorrespondence);
                    if (correpondenceId > 0)
                    {
                        ShowNotification("مکاتبه با موفقیت ثبت شد", "success");

                        if (CorrespondenceReminderCheckBox.IsChecked == true && CorrespondenceReminderDate.SelectedDate.Value != null)
                        {
                            var reminder = new Reminder
                            {
                                Title = $"یادآور مکاتبه برای شماره پرونده: {SelectCaseCB.Text}",
                                Description = CorrespondenceSubjectTB.Text,
                                ReminderDate = CorrespondenceReminderDate.SelectedDate.Value,
                                UserId = user.Id,
                                CorrespondenceId = correpondenceId
                            };
                            rbll.Create(reminder);

                            SmsIr smsIr = new SmsIr("XwrI6iBRvxaWJ4AVcqaQddyRF5Ux3iHesb7XuQGbcSVag4Ut");
                            var bulkSendResult = smsIr.BulkSendAsync(30007732011420, $"یادآوری مکاتبه برای: {SelectCaseCB.Text}\nعنوان: {CorrespondenceTitleTB.Text}\nتاریخ یادآوری: {CorrespondenceReminderDate.SelectedDate.Value.ToString("yyyy/MM/dd")}\n\nنرم افزار حقوقی هیام", new string[] { user.PhoneNumber });
                            var verificationSendResult = smsIr.VerifySendAsync(user.PhoneNumber, 100000, new VerifySendParameter[] { new VerifySendParameter("Code", "12345") });
                        }
                        RefreshCorrespondence();
                        SaveChanges();
                        ClearControls(AddCorrespondenceWindow);
                        CaseSubjectLabel.Text = "";
                        AddCorrespondenceWindow.Visibility = Visibility.Collapsed;
                        CorrespondenceWindow.Effect = null;
                    }
                    else
                    {
                        ShowNotification("خطا در ثبت مکاتبه!", "error");
                    }                   
                }
                catch (Exception)
                {
                    ShowNotification("خطا در ثبت مکاتبه!", "error");
                }
            }
            else
            {
                ShowNotification("شماره پرونده باید فقط شامل عدد و بین ۱۶ تا ۱۸ رقم باشد!", "error");
            }
        }
        private void ShowCorrespondenceDetailsBTN_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            if (btn?.Tag is int correspondenceId)
            {
                CorrepondenceDetailsPopup.Tag = correspondenceId;
                var _correspondence = correspondence_BLL.GetCorrespondenceById(correspondenceId);
                // پر کردن جزئیات در پاپ‌آپ               
                CorrepondenceNumberPopupTB.Text = _correspondence.CorrespondenceNumber;
                SubjectPopupTB.Text = _correspondence.Subject;
                SenderPopupTB.Text = _correspondence.Sender;
                ReceiverPopupTB.Text = _correspondence.Receiver;
                CorrespondenceReminderCheckBoxPopup.IsChecked = _correspondence.IsReminderSet;
                CorrespondenceDatePopupDP.Text = _correspondence.CorrespondenceDate;
                CorrespondenceReminderDatePopup.SelectedDate = _correspondence.ReminderDate;
                CorrespondenceDescriptionPopupTB.Text = _correspondence.CorrespondenceDescription;
                CorrespondenceStatusPopupCB.SelectedIndex = _correspondence.Status;
                LoadCorrespondenceForEdit(_correspondence);
                PopupCorrespondenceFilesListBox.ItemsSource = _correspondence.CorrespondenceAttachments?.ToList();
                CorrepondenceDetailsPopup.Visibility = Visibility.Visible;
                CorrespondenceWindow.Effect = new BlurEffect() { Radius = 5 };
            }
        }
        private void CorrespondenceDetailsPopupUpdateBTN_Click(object sender, RoutedEventArgs e)
        {
            if (!(CorrepondenceDetailsPopup.Tag is int caseId))
            {
                ShowNotification("آیدی مکاتبه مشخص نیست!", "error");
                return;
            }
            var _correpondence = correspondence_BLL.GetCorrespondenceById(caseId);
            if (_correpondence == null)
            {
                ShowNotification("مکاتبه موردنظر یافت نشد!", "error");
                return;
            }

            foreach (var attachment in Existcorrespondenceattachments)
            {
                string documentsPath = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                    "Heyam", "CorrespondenceAttachments", _correpondence.Case.CaseNumber, _correpondence.Title);

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

            _correpondence.CorrespondenceNumber = CorrepondenceNumberPopupTB.Text;
            _correpondence.Subject = SubjectPopupTB.Text;
            _correpondence.IsReminderSet = CorrespondenceReminderCheckBoxPopup.IsChecked == true;
            _correpondence.ReminderDate = CorrespondenceReminderCheckBoxPopup.IsChecked == true ? CorrespondenceReminderDatePopup.SelectedDate : null;
            _correpondence.IsReminderDone = false;
            _correpondence.CorrespondenceDescription = CorrespondenceDescriptionPopupTB.Text;
            _correpondence.Status = CorrespondenceStatusPopupCB.SelectedIndex;
            // افزودن پیوست‌های ویرایش‌شده
            _correpondence.CorrespondenceAttachments = Existcorrespondenceattachments;
            bool result = correspondence_BLL.UpdateCorrespondence(_correpondence, deletedcorrespondenceAttachments);
            ShowNotification($"ویرایش اطلاعات با موفقیت انجام شد", "success");
            RefreshCorrespondence();
            SaveChanges();
            CorrepondenceDetailsPopup.Visibility = Visibility.Collapsed;
            CorrespondenceWindow.Effect = null;
        }
        #endregion
        private void CloseBTN_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }        
    }
}