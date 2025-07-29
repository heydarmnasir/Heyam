using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using BE.ViewModel;

namespace Heyam
{
    public class NotesToBackgroundConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {          
            var notesText = value as string;
            return string.IsNullOrWhiteSpace(notesText) ? Brushes.Transparent : Brushes.LightBlue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public partial class LegalInfoBank : Window
    {
        private bool isMenuOpenA = false;
        private bool isMenuOpenB = false;
        private LawViewModel _viewModel;
        public LegalInfoBank()
        {
            InitializeComponent();           
            DataContext = _viewModel;
            string filePath = _lawFilePaths[0];
            if (_viewModel != null)
            {
                _viewModel.LoadArticlesFromJson(filePath);
            }          
            _viewModel = new LawViewModel();
            DataContext = _viewModel;
            _viewModel.LoadArticlesFromJson("C:\\Program Files (x86)\\Heyamcompany\\Heyam\\PL\\Resources\\Laws_Articles\\Asasi_Law.json");
            _viewModel.SetLawInfo(SelectLawTypeCB.SelectedIndex);
            LoadAenLetters("Asasi_Law");                   
        }   

        #region Method
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
        private void CloseMenuForDropdownApprovalsListMenu()
        {
            DoubleAnimation animation = new DoubleAnimation
            {
                To = 0,
                Duration = TimeSpan.FromMilliseconds(300),
                EasingFunction = new QuadraticEase()
            };
            DropdownApprovalsListMenu.BeginAnimation(FrameworkElement.HeightProperty, animation);
            isMenuOpenA = false;
        }
        private void CloseMenuForDropdownRelatedJobsMenu()
        {
            DoubleAnimation animation = new DoubleAnimation
            {
                To = 0,
                Duration = TimeSpan.FromMilliseconds(300),
                EasingFunction = new QuadraticEase()
            };
            DropdownRelatedJobsMenu.BeginAnimation(FrameworkElement.HeightProperty, animation);
            isMenuOpenB = false;
        }
        #endregion

        #region MenuItems
        private Button lastSelectedButton;
        private void LawsBTN_Click(object sender, RoutedEventArgs e)
        {
            if (lastSelectedButton != null)
            {
                lastSelectedButton.Foreground = Brushes.White;
                lastSelectedButton.BorderBrush = Brushes.Transparent;
            }
            Button clickedButton = sender as Button;
            clickedButton.Foreground = Brushes.Yellow;
            clickedButton.BorderBrush = Brushes.Red;
            clickedButton.BorderThickness = new Thickness(0, 0, 0, 3); // خط زیر دکمه

            lastSelectedButton = clickedButton;

            LawGrid.Visibility = Visibility.Visible;
            LawFilesGrid.Visibility = Visibility.Collapsed;
            ArticleLawGrid.Visibility = Visibility.Collapsed;
            TotalExistFilesWP.Visibility = Visibility.Collapsed;
            DashboardGrid.Visibility = Visibility.Collapsed;
        }
        private void LawsFilesBTN_Click(object sender, RoutedEventArgs e)
        {
            if (lastSelectedButton != null)
            {
                lastSelectedButton.Foreground = Brushes.White;
                lastSelectedButton.BorderBrush = Brushes.Transparent;
            }
            Button clickedButton = sender as Button;
            clickedButton.Foreground = Brushes.Yellow;
            clickedButton.BorderBrush = Brushes.Red;
            clickedButton.BorderThickness = new Thickness(0, 0, 0, 3); // خط زیر دکمه

            lastSelectedButton = clickedButton;

            LawFilesGrid.Visibility = Visibility.Visible;
            ArticleLawGrid.Visibility = Visibility.Collapsed;
            LawGrid.Visibility = Visibility.Collapsed;
            TotalExistFilesWP.Visibility = Visibility.Visible;
            DashboardGrid.Visibility = Visibility.Collapsed;
            _viewModel.LoadPdfFiles();
            DynamicCountText.Text = _viewModel.GetPdfFileCount().ToString();           
        }
        private void ApprovalsListBTN_Click(object sender, RoutedEventArgs e)
        {
            double targetHeight = isMenuOpenA ? 0 : 150; // ارتفاع لیست (مثلاً 100 پیکسل)
            ApprovalsListBTN.Content = "⚖ لیست مصوبات 🔻";
            DoubleAnimation animation = new DoubleAnimation
            {
                To = targetHeight,
                Duration = TimeSpan.FromMilliseconds(300),
                EasingFunction = new QuadraticEase() // انیمیشن نرم‌تر               
            };
            DropdownApprovalsListMenu.BeginAnimation(FrameworkElement.HeightProperty, animation);

            isMenuOpenA = !isMenuOpenA;
            if (targetHeight == 0)
            {
                ApprovalsListBTN.Content = "⚖ لیست مصوبات 🔺";
            }
        }
        private void ArticlesBTN_Click(object sender, RoutedEventArgs e)
        {
            if (lastSelectedButton != null)
            {
                lastSelectedButton.Foreground = Brushes.White;
                lastSelectedButton.BorderBrush = Brushes.Transparent;
            }
            Button clickedButton = sender as Button;
            clickedButton.Foreground = Brushes.Yellow;
            clickedButton.BorderBrush = Brushes.Red;
            clickedButton.BorderThickness = new Thickness(0, 0, 0, 3); // خط زیر دکمه

            lastSelectedButton = clickedButton;

            ArticleLawGrid.Visibility = Visibility.Visible;
            LawFilesGrid.Visibility = Visibility.Collapsed;
            LawGrid.Visibility = Visibility.Collapsed;
            TotalExistFilesWP.Visibility = Visibility.Visible;
            DashboardGrid.Visibility = Visibility.Collapsed;
            _viewModel.LoadArticlePdfFiles();
            DynamicCountText.Text = _viewModel.GetArticlePdfFileCount().ToString();
        }
        private void RelatedJobsBTN_Click(object sender, RoutedEventArgs e)
        {
            double targetHeight = isMenuOpenB ? 0 : 50; // ارتفاع لیست (مثلاً 100 پیکسل)
            RelatedJobsBTN.Content = "♻ اطلاعات مشاغل مرتبط 🔻";
            DoubleAnimation animation = new DoubleAnimation
            {
                To = targetHeight,
                Duration = TimeSpan.FromMilliseconds(300),
                EasingFunction = new QuadraticEase() // انیمیشن نرم‌تر               
            };
            DropdownRelatedJobsMenu.BeginAnimation(FrameworkElement.HeightProperty, animation);

            isMenuOpenB = !isMenuOpenB;
            if (targetHeight == 0)
            {
                RelatedJobsBTN.Content = "♻ اطلاعات مشاغل مرتبط 🔺";
            }
        }
        #endregion

        #region Law       
        private readonly List<string> _lawFilePaths = new List<string>
        {
            "C:\\Program Files (x86)\\Heyamcompany\\Heyam\\PL\\Resources\\Laws_Articles\\Asasi_Law.json",
            "C:\\Program Files (x86)\\Heyamcompany\\Heyam\\PL\\Resources\\Laws_Articles\\Madani_Law.json",
            "C:\\Program Files (x86)\\Heyamcompany\\Heyam\\PL\\Resources\\Laws_Articles\\AenDadrasiDadgahomomi&enghlab(Madani)_Law.json",
            "C:\\Program Files (x86)\\Heyamcompany\\Heyam\\PL\\Resources\\Laws_Articles\\EjrayeAhkamMadani_Law.json",
            "C:\\Program Files (x86)\\Heyamcompany\\Heyam\\PL\\Resources\\Laws_Articles\\Tejarat_Law.json",
            "C:\\Program Files (x86)\\Heyamcompany\\Heyam\\PL\\Resources\\Laws_Articles\\AenDadrasiKeyfari_Law.json",
            "C:\\Program Files (x86)\\Heyamcompany\\Heyam\\PL\\Resources\\Laws_Articles\\MojazatEslami1375_Law.json",
            "C:\\Program Files (x86)\\Heyamcompany\\Heyam\\PL\\Resources\\Laws_Articles\\MojazatEslami1392_Law.json",
            "C:\\Program Files (x86)\\Heyamcompany\\Heyam\\PL\\Resources\\Laws_Articles\\KaheshMojazatHabsTaaziri_Law.json",
            "C:\\Program Files (x86)\\Heyamcompany\\Heyam\\PL\\Resources\\Laws_Articles\\MobarezeBaGhachaghKala&Arz_Law.json",
            "C:\\Program Files (x86)\\Heyamcompany\\Heyam\\PL\\Resources\\Laws_Articles\\MojazatAsidPashi_Law.json",
            "C:\\Program Files (x86)\\Heyamcompany\\Heyam\\PL\\Resources\\Laws_Articles\\HemayatFamily_Law.json",
            "C:\\Program Files (x86)\\Heyamcompany\\Heyam\\PL\\Resources\\Laws_Articles\\JormSeyasi_Law.json",
            "C:\\Program Files (x86)\\Heyamcompany\\Heyam\\PL\\Resources\\Laws_Articles\\DaftarAsnad_Law.json",
            "C:\\Program Files (x86)\\Heyamcompany\\Heyam\\PL\\Resources\\Laws_Articles\\Work_Law.json",
            "C:\\Program Files (x86)\\Heyamcompany\\Heyam\\PL\\Resources\\Laws_Articles\\Bime_Law.json",
            "C:\\Program Files (x86)\\Heyamcompany\\Heyam\\PL\\Resources\\Laws_Articles\\SodorCheck_Law.json",
            "C:\\Program Files (x86)\\Heyamcompany\\Heyam\\PL\\Resources\\Laws_Articles\\CheckTazminShode_Law.json",
            "C:\\Program Files (x86)\\Heyamcompany\\Heyam\\PL\\Resources\\Laws_Articles\\SabtAhval_Law.json",
            "C:\\Program Files (x86)\\Heyamcompany\\Heyam\\PL\\Resources\\Laws_Articles\\ShorahayeHalEkhtelaf_Law.json",
            "C:\\Program Files (x86)\\Heyamcompany\\Heyam\\PL\\Resources\\Laws_Articles\\Vekalat_Law.json",
            "C:\\Program Files (x86)\\Heyamcompany\\Heyam\\PL\\Resources\\Laws_Articles\\TamalokAparteman_Law.json",
            "C:\\Program Files (x86)\\Heyamcompany\\Heyam\\PL\\Resources\\Laws_Articles\\PreSealSakhteman_Law.json",
            "C:\\Program Files (x86)\\Heyamcompany\\Heyam\\PL\\Resources\\Laws_Articles\\Moajer&Mostaajer_Law.json",
            "C:\\Program Files (x86)\\Heyamcompany\\Heyam\\PL\\Resources\\Laws_Articles\\TasdighEnhesarVerasat_Law.json",
            "C:\\Program Files (x86)\\Heyamcompany\\Heyam\\PL\\Resources\\Laws_Articles\\TransferJudge_Law.json",
            "C:\\Program Files (x86)\\Heyamcompany\\Heyam\\PL\\Resources\\Laws_Articles\\Vorod&EghamatAtbaKhareje_Law.json",
            "C:\\Program Files (x86)\\Heyamcompany\\Heyam\\PL\\Resources\\Laws_Articles\\AmvalGheyrManghoolAtbaKhareje_Law.json",
            "C:\\Program Files (x86)\\Heyamcompany\\Heyam\\PL\\Resources\\Laws_Articles\\ElzamBeSabtRasmiMoamelatAmvalGheyrManghool_Law.json",
            "C:\\Program Files (x86)\\Heyamcompany\\Heyam\\PL\\Resources\\Laws_Articles\\MojazatEnteghalMaalGheyr_Law.json",
            "C:\\Program Files (x86)\\Heyamcompany\\Heyam\\PL\\Resources\\Laws_Articles\\MojazatEstefadeGheyrmojazAAbBargh_Law.json",
            "C:\\Program Files (x86)\\Heyamcompany\\Heyam\\PL\\Resources\\Laws_Articles\\NezaratBarRaftarGhozat_Law.json",
            "C:\\Program Files (x86)\\Heyamcompany\\Heyam\\PL\\Resources\\Laws_Articles\\SabtSherkat_Law.json",
            "C:\\Program Files (x86)\\Heyamcompany\\Heyam\\PL\\Resources\\Laws_Articles\\Sea_Law.json",
            "C:\\Program Files (x86)\\Heyamcompany\\Heyam\\PL\\Resources\\Laws_Articles\\Seyd&Shekar_Law.json",
            "C:\\Program Files (x86)\\Heyamcompany\\Heyam\\PL\\Resources\\Laws_Articles\\OmorGomroki_Law.json",
            "C:\\Program Files (x86)\\Heyamcompany\\Heyam\\PL\\Resources\\Laws_Articles\\Saderat&Varedat_Law.json",
            "C:\\Program Files (x86)\\Heyamcompany\\Heyam\\PL\\Resources\\Laws_Articles\\EslahAenDadrasiKeyfari_Law.json",
            "C:\\Program Files (x86)\\Heyamcompany\\Heyam\\PL\\Resources\\Laws_Articles\\EslahSodorCheck_Law.json",
            "C:\\Program Files (x86)\\Heyamcompany\\Heyam\\PL\\Resources\\Laws_Articles\\EslahGhanonMojazatghachaghAslahe_Law.json",
            "C:\\Program Files (x86)\\Heyamcompany\\Heyam\\PL\\Resources\\Laws_Articles\\EslahGhanoonNezamSenfi_Law.json",
            "C:\\Program Files (x86)\\Heyamcompany\\Heyam\\PL\\Resources\\Laws_Articles\\EslahMobarezeBaPoolShoe_Law.json",
            "C:\\Program Files (x86)\\Heyamcompany\\Heyam\\PL\\Resources\\Laws_Articles\\EslahMaddeh104GhanoonMojazatEslami_Law.json"
        };
        private void SelectLawTypeCB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (SelectLawTypeCB.SelectedIndex >= 0 && SelectLawTypeCB.SelectedIndex < _lawFilePaths.Count)
            {
                if (_viewModel != null)
                {
                    string selectedFilePath = _lawFilePaths[SelectLawTypeCB.SelectedIndex];
                    _viewModel.LoadArticlesFromJson(selectedFilePath);
                    _viewModel.SetLawInfo(SelectLawTypeCB.SelectedIndex);                  
                }            
            }

            if (SelectLawTypeCB.SelectedIndex == 0)
                LoadAenLetters("Asasi_Law");
            else if (SelectLawTypeCB.SelectedIndex == 1)
                LoadAenLetters("Madani_Law");
            else if (SelectLawTypeCB.SelectedIndex == 4)
                LoadAenLetters("Tejarat_Law");
            else if (SelectLawTypeCB.SelectedIndex == 5)
                LoadAenLetters("AenDadrasiKeyfari_Law");
            else if (SelectLawTypeCB.SelectedIndex == 14)
                LoadAenLetters("Work_Law");
            else if (SelectLawTypeCB.SelectedIndex == 15)
                LoadAenLetters("Bime_Law");
            else
            {
                LoadAenLetters("");
            }
        }
        private void TreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {             
            if (DataContext is LawViewModel vm && e.NewValue is ArticleDto selected)
            {
                var selectedItem = SelectLawTypeCB.SelectedItem as ComboBoxItem;
                string lawType = selectedItem?.Content.ToString() ?? "نامشخص";

                vm.SelectedArticle = selected;
                vm.AddToHistory(lawType, selected.Title);
            }
        }
        private void SavePersonalNotesBTN_Click(object sender, RoutedEventArgs e)
        {
            if (_viewModel.SelectedArticle == null)
                return;

            // محتوای TextBox رو به لیست تبدیل کن و ذخیره کن
            var userNotes = UserNotesTB.Text
                .Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries)
                .ToList();

            _viewModel.SelectedArticle.UserNotes = userNotes;
            ShowNotification(_viewModel.SaveUserNotes(_viewModel.SelectedLawInfo.Name, _viewModel.SelectedArticle), "Success");            
        }
        private void LoadAenLetters(string lawFolderName)
        {
            if (AenLettersLB != null)
            {         
                AenLettersLB.Items.Clear(); // لیست رو پاک کن
                string folderPath = $"C:\\Program Files(x86)\\Heyamcompany\\Heyam\\PL\\Resources\\Legal_Files\\AenLetters\\{lawFolderName}";
             
                var files = Directory.GetFiles(folderPath, "*.pdf");
                foreach (var filePath in files)
                {
                    string fileName = System.IO.Path.GetFileName(filePath);

                    // ایجاد Hyperlink
                    var hyperlink = new Hyperlink
                    {
                        NavigateUri = new Uri(filePath),
                    };
                    hyperlink.Inlines.Add(fileName);
                    hyperlink.RequestNavigate += Hyperlink_RequestNavigate;

                    // اضافه کردن به TextBlock
                    var textBlock = new TextBlock();
                    textBlock.Inlines.Add(hyperlink);

                    // اضافه کردن به ListBoxItem
                    var listBoxItem = new ListBoxItem();
                    listBoxItem.Content = textBlock;

                    // اضافه کردن به ListBox
                    AenLettersLB.Items.Add(listBoxItem);
                }
            }
        }
        private void Hyperlink_RequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
        {
            try
            {
                // گرفتن مسیر کامل محلی
                string filePath = e.Uri.LocalPath;

                if (!File.Exists(filePath))
                {
                    MessageBox.Show($"فایل پیدا نشد:\n{filePath}");
                    return;
                }

                // باز کردن فایل با برنامه پیش‌فرض
                System.Diagnostics.Process.Start(new ProcessStartInfo(filePath) { UseShellExecute = true });

                e.Handled = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"خطا در باز کردن فایل:\n{ex.Message}");
            }
        }
        private void AllNazareyatMashverati_Link_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Process.Start("https://www.ekhtebar.ir/category/%D9%82%D9%88%D8%A7%D9%86%DB%8C%D9%86-%D9%88-%D9%85%D8%B5%D9%88%D8%A8%D8%A7%D8%AA/%D9%86%D8%B8%D8%B1%DB%8C%D9%87-%D9%87%D8%A7%DB%8C-%D9%85%D8%B4%D9%88%D8%B1%D8%AA%DB%8C/");
        }
        private void LawSearchBar_TextChanged(object sender, TextChangedEventArgs e)
        {            
            // کنترل نمایش Placeholder
            LawPlaceholderText.Visibility = string.IsNullOrWhiteSpace(LawSearchBar.Text.Trim())
                ? Visibility.Visible
                : Visibility.Hidden;
        }
        #endregion

        #region PdfFilesList
        private void OpenPdfExternally(string filePath)
        {
            try
            {
                if (File.Exists(filePath))
                {
                    Process.Start(new ProcessStartInfo
                    {
                        FileName = filePath,
                        UseShellExecute = true // برای اجرای فایل با برنامه پیش‌فرض سیستم
                    });
                }
                else
                {
                    MessageBox.Show("فایل موردنظر یافت نشد!", "خطا", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"خطا در باز کردن فایل PDF: {ex.Message}", "خطا", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void btnViewPdf_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is string resourcePath)
            {
                string tempFilePath = ExtractEmbeddedPdf(resourcePath);

                if (!string.IsNullOrEmpty(tempFilePath) && File.Exists(tempFilePath))
                {
                    OpenPdfExternally(tempFilePath);                  
                }
                else
                {
                    MessageBox.Show("فایل PDF موردنظر یافت نشد!", "خطا", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
        private void btnViewArticlePdf_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is string resourcePath)
            {
                string tempFilePath = ExtractEmbeddedPdf(resourcePath);

                if (!string.IsNullOrEmpty(tempFilePath) && File.Exists(tempFilePath))
                {
                    OpenPdfExternally(tempFilePath);
                }
                else
                {
                    MessageBox.Show("فایل PDF موردنظر یافت نشد!", "خطا", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
        private string ExtractEmbeddedPdf(string fileName)
        {
            try
            {
                string tempPath = System.IO.Path.Combine(System.IO.Path.GetTempPath(), fileName);

                // اگر فایل قبلاً ایجاد شده باشد، همان مسیر را برمی‌گرداند
                if (File.Exists(tempPath)) return tempPath;

                Assembly assembly = Assembly.GetExecutingAssembly();
                string resourceName = assembly.GetManifestResourceNames().FirstOrDefault(r => r.EndsWith(fileName));

                if (resourceName == null)
                {
                    MessageBox.Show($"فایل '{fileName}' در Resources یافت نشد!", "خطا", MessageBoxButton.OK, MessageBoxImage.Error);
                    return string.Empty;
                }

                using (Stream resourceStream = assembly.GetManifestResourceStream(resourceName))
                {
                    if (resourceStream == null)
                    {
                        MessageBox.Show($"بارگذاری فایل '{fileName}' از Resources ناموفق بود!", "خطا", MessageBoxButton.OK, MessageBoxImage.Error);
                        return string.Empty;
                    }

                    using (FileStream outputFileStream = new FileStream(tempPath, FileMode.Create, FileAccess.Write))
                    {
                        resourceStream.CopyTo(outputFileStream);
                    }
                }

                return tempPath;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"خطا در بارگذاری فایل PDF: {ex.Message}", "خطا", MessageBoxButton.OK, MessageBoxImage.Error);
                return string.Empty;
            }
        }
        #endregion

        #region Links
        private void OfficialExpertsText_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Process.Start("https://hub.23055.ir/search-official-expert");
        }
        private void DocumentRegistrationOfficesText_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Process.Start("https://adliran.ir/Home/GetPolice");
        }
        private void AraVahdatRaveyeText_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Process.Start("https://www.ekhtebar.ir/category/%d9%82%d9%88%d8%a7%d9%86%db%8c%d9%86-%d9%88-%d9%85%d8%b5%d9%88%d8%a8%d8%a7%d8%aa/%d8%a2%d8%b1%d8%a7-%d9%88%d8%ad%d8%af%d8%aa-%d8%b1%d9%88%db%8c%d9%87/");
        }
        private void BakhshnamehaText_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Process.Start("https://www.ekhtebar.ir/category/%d9%82%d9%88%d8%a7%d9%86%db%8c%d9%86-%d9%88-%d9%85%d8%b5%d9%88%d8%a8%d8%a7%d8%aa/%d8%a8%d8%ae%d8%b4%d9%86%d8%a7%d9%85%d9%87-%d9%87%d8%a7/");
        }
        private void RaveyeGhazaeeText_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Process.Start("https://www.ekhtebar.ir/category/%d9%82%d9%88%d8%a7%d9%86%db%8c%d9%86-%d9%88-%d9%85%d8%b5%d9%88%d8%a8%d8%a7%d8%aa/%d8%b1%d9%88%db%8c%d9%87-%d9%82%d8%b6%d8%a7%db%8c%db%8c/");
        }
        private void TarhLayeheText_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Process.Start("https://www.ekhtebar.ir/category/%d9%82%d9%88%d8%a7%d9%86%db%8c%d9%86-%d9%88-%d9%85%d8%b5%d9%88%d8%a8%d8%a7%d8%aa/%d8%b7%d8%b1%d8%ad-%d9%88-%d9%84%d8%a7%db%8c%d8%ad%d9%87/");
        }
        private void RaayEsrariText_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Process.Start("https://www.ekhtebar.ir/category/%d9%82%d9%88%d8%a7%d9%86%db%8c%d9%86-%d9%88-%d9%85%d8%b5%d9%88%d8%a8%d8%a7%d8%aa/%d8%b1%d8%a7%db%8c-%d8%a7%d8%b5%d8%b1%d8%a7%d8%b1%db%8c/");
        }
        private void AraDivanEdalatAdariText_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Process.Start("https://www.ekhtebar.ir/category/%d9%82%d9%88%d8%a7%d9%86%db%8c%d9%86-%d9%88-%d9%85%d8%b5%d9%88%d8%a8%d8%a7%d8%aa/%d8%b1%d8%a7%db%8c-%d9%87%db%8c%d8%a7%d8%aa-%d8%b9%d9%85%d9%88%d9%85%db%8c-%d8%af%db%8c%d9%88%d8%a7%d9%86-%d8%b9%d8%af%d8%a7%d9%84%d8%aa-%d8%a7%d8%af%d8%a7%d8%b1%db%8c/");
        }
        #endregion

        private void CloseBTN_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}