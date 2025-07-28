using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Effects;
using BE.ViewModel;
using BE;
using BLL;
using PL;
using IPE.SmsIrClient;

namespace Heyam
{   
    public partial class LegalAutomation : Window
    {
        private MainWindow _mainWindow;

        Contract_BLL contract_bll = new Contract_BLL();
        Client_BLL cbll = new Client_BLL();
        Case_BLL case_bll = new Case_BLL();
        LegalBrief_BLL legalBrief_bll = new LegalBrief_BLL();
        Reminder_BLL rbll = new Reminder_BLL();

        public LegalAutomation(MainWindow mainWindow)
        {
            InitializeComponent();
            RefreshContract();
            RefreshLegalBrief();
            LoadTemplates();
            _mainWindow = mainWindow;
            
            #region Fill ClientName & CaseNumber Field ComboBox
            var clients = cbll.GetAllClients(); // از BLL           
            SelectClientNameCB.ItemsSource = clients;
            SelectClientForLegalBriefCB.ItemsSource = clients;
            SelectClientNameCB.DisplayMemberPath = "FullName"; // فرض بر اینکه پراپرتی FullName داری
            SelectClientNameCB.SelectedValuePath = "Id";
            SelectClientForLegalBriefCB.DisplayMemberPath = "FullName"; // فرض بر اینکه پراپرتی FullName داری
            SelectClientForLegalBriefCB.SelectedValuePath = "Id";          
            #endregion
        }
        SmsIr smsIr = new SmsIr("XwrI6iBRvxaWJ4AVcqaQddyRF5Ux3iHesb7XuQGbcSVag4Ut");
   
        #region Dashboard
        private Button lastSelectedButton;
        private void TanzimContract_Click(object sender, RoutedEventArgs e)
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

            DynamicTopLabel.Visibility = Visibility.Visible;
            DashboardGrid.Visibility = Visibility.Collapsed;      
            TanzimContractGrid.Visibility = Visibility.Visible;                      
            LegalBriefGrid.Visibility = Visibility.Collapsed;
        }
        private void TanzimLayehe_Click(object sender, RoutedEventArgs e)
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

            LegalBriefGrid.Visibility = Visibility.Visible;
            DynamicTopLabel.Visibility = Visibility.Collapsed;         
            DashboardGrid.Visibility = Visibility.Collapsed;
            TanzimContractGrid.Visibility = Visibility.Collapsed;                    
              
            LegalBriefDGV.Visibility = Visibility.Collapsed;
            SearchLegalBriefBox.Visibility = Visibility.Collapsed;
            SearchLegalBriefPlaceholderText.Visibility = Visibility.Collapsed;          
            TemplateCB.SelectedValue = "";                      
        }      
        private void AdlIran_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("https://adliran.ir/");
        }
        private void TasmimSite_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("https://zaman.behzisti.net/");
        }
        private void ParvandeIjra_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("https://iehraz.adliran.ir/Login/Authenticate?ReturnUrl=https://resultcase.adliran.ir/ResultCase/CaseListForLawyer&SystemName=ResultCaseService&isSelectNaturalPerson=True&isSelectNaturalForigenPerson=False&isSelectLegalPerson=False&isSelectJudPerson=False&LoginTitle=%d8%b3%d8%a7%d9%85%d8%a7%d9%86%d9%87%20%d8%a7%d8%b7%d9%84%d8%a7%d8%b9%20%d8%b1%d8%b3%d8%a7%d9%86%db%8c%20%d9%be%d8%b1%d9%88%d9%86%d8%af%d9%87%20%d9%88%da%a9%d9%84%d8%a7");
        }
        private void ArayeGhazaee_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("https://ara.jri.ac.ir/");
        }
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
        private void ClearControls(DependencyObject parent)
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
             
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
        public void RefreshContract()
        {
            ContractsDGV.ItemsSource = null;
            ContractsDGV.ItemsSource = contract_bll.GetContractsForListView();
        }
        public void RefreshLegalBrief()
        {
            LegalBriefDGV.ItemsSource = null;
            LegalBriefDGV.ItemsSource = legalBrief_bll.GetLegalBriefsForListView();
        }        
        #endregion

        #region Contract
        private void GenerateContract()
        {
            // Create FlowDocument
            FlowDocument doc = new FlowDocument
            {
                PagePadding = new Thickness(20),
                ColumnWidth = 800,
                FontFamily = new FontFamily("Sahel"),
                FontSize = 12,
                TextAlignment = TextAlignment.Right,
            };
           
            // ایجاد Border به عنوان حاشیه صفحه
            Border pageBorder = new Border
            {
                BorderBrush = Brushes.Black,
                BorderThickness = new Thickness(2),
                CornerRadius = new CornerRadius(10),
                Padding = new Thickness(20),
                Margin = new Thickness(0),
                Child = new StackPanel
                {
                    Children =
                    {
                        new TextBlock
                        {
                            Text = "\t\t\t\tبسمه تعالی\t\t\t⚖ قرارداد",
                            FontSize = 18,
                            FontWeight = FontWeights.Bold,
                            TextAlignment = TextAlignment.Center
                        },
                        new TextBlock
                        {
                            Text = ContractContentTB.Text,
                            TextAlignment = TextAlignment.Justify,
                            Margin = new Thickness(0, 5, 0, 0),
                            TextWrapping = TextWrapping.Wrap,
                        }
                    }
                }
            };

            // افزودن Border به FlowDocument
            doc.Blocks.Add(new BlockUIContainer(pageBorder));

            // Assign Document to Viewer
            ContractPreview.Document = doc;
        }
        private void GeneratePopupContract()
        {
            // Create FlowDocument
            FlowDocument doc = new FlowDocument
            {
                PagePadding = new Thickness(20),
                ColumnWidth = 800,
                FontFamily = new FontFamily("Sahel"),
                FontSize = 12,
                TextAlignment = TextAlignment.Right,
            };

            // ایجاد Border به عنوان حاشیه صفحه
            Border pageBorder = new Border
            {
                BorderBrush = Brushes.Black,
                BorderThickness = new Thickness(2),
                CornerRadius = new CornerRadius(10),
                Padding = new Thickness(20),
                Margin = new Thickness(0),
                Child = new StackPanel
                {
                    Children =
                    {
                        new TextBlock
                        {
                            Text = "\t\t\t\tبسمه تعالی\t\t\t⚖ قرارداد",
                            FontSize = 18,
                            FontWeight = FontWeights.Bold,
                            TextAlignment = TextAlignment.Center
                        },
                        new TextBlock
                        {
                            Text = PopupContractContentTB.Text,
                            TextAlignment = TextAlignment.Justify,
                            Margin = new Thickness(0, 5, 0, 0),
                            TextWrapping = TextWrapping.Wrap,
                        }
                    }
                }
            };

            // افزودن Border به FlowDocument
            doc.Blocks.Add(new BlockUIContainer(pageBorder));

            // Assign Document to Viewer
            ContractPreview.Document = doc;
        }
        private void ContractTypeCB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ContractTypeCB.SelectedItem is ComboBoxItem selectedItem)
            {
                string contractType = selectedItem.Content.ToString();

                // متن قالب قراردادها
                string contractTemplate = string.Empty;
                switch (contractType)
                {
                    case "قرارداد مشاوره":
                        contractTemplate = "ماده 1- طرفین قرارداد                                            \r\nاین قرارداد در تاریخ ………...................... بین شرکت ……………...........….. به مدیریت خانم /آقای ………................…………. تماس ثابت ………...........…….. همراه ………...................…….. دورنگار ………….. به آدرس …………............................................………………….. که من بعد «کارفرما» نامیده می شود از یک طرف و خانم/آقای …………..........……….. تماس ثابت …………........….. همراه …...............………….. دورنگار ………….. به آدرس ……………............................................……………….. به عنوان «مشاور» منعقد می گردد.\r\n\r\nماده 2- موضوع قرارداد\r\nارائه خدمات مشاوره ای در زمینه امور حقوقی و قراردادها برای کارفرما توسط مشاور فوق الذکر.\r\n\r\nماده 3- مبلغ قرارداد\r\nمبلغ قرارداد با توافق طرفین از قرار ماهیانه به میزان …………...............……. ریال معادل ……..................………… تومان تعیین می گردد که در آخر هر ماه توسط کارفرما به مشاور پرداخت خواهد شد. ضمناً مبلغ کل قرارداد برای یک سال تمام شمسی به میزان ……………............……. ریال معادل ………..............…….. تومان می باشد.\r\n\r\nماده 4- مدت قرارداد\r\nمدت قرارداد یک سال تمام شمسی از تاریخ امضاء قرارداد توسط طرفین قرارداد می باشد.\r\n\r\nماده 5- تاریخ قرارداد\r\nاین قرارداد در تاریخ …......................……. توسط طرفین امضاء شده و تاریخ مذکور، تاریخ انعقاد قرارداد تلقی و دارای آثار حقوقی می باشد.\r\n\r\nماده 6- تعهدات طرفین\r\n1-6- مشاور مذکور موظف است هر زمانی که کارفرما نیاز به مشاوره در زمینه مسائل حقوقی و قراردادها داشته باشد، در اسرع وقت نسبت به ارائه مشاوره لازم در محل شرکت به آدرس فوق و یا هر طریق مقتضی که نیاز شرکت مزبور را مرتفع نماید، اقدام نماید.\r\n2-6- مشاور موظف است کلیه آمار، اطلاعات و اسناد و مدارک مربوط به کارفرما را که به لحاظ اجرای مفاد این قرارداد به دست می آورد، محرمانه تلقی نموده و از افشای آن نزد هر شخص یا مقام یا مرجع، بدون اجازه کارفرما خودداری نماید. لازم به ذکر است در صورت عدم رعایت مفاد این ماده، با مشاور مطابق موازین قانونی جاری کشور رفتار خواهد شد.\r\n\r\nماده 7- مرجع حل اختلاف\r\nدر صورت بروز اختلاف ما بین طرفین قرارداد ابتدائاً سعی بر آن باشد که از طریق مذاکره بینابین حل و فصل گردد و در صورت حل نشدن اختلاف موضوع به داوری و در پایان به مراجع صالح قانونی مراجعه گردد.\r\n\r\nماده 8- اقامتگاه طرفین\r\nنشانی های مندرج در قرارداد تا هنگامی که تغییر آن ها کتباً به طرف مقابل اعلام نشده باشد اقامتگاه قانونی طرفین قرارداد محسوب می شود. هرگونه مکاتبات ارسالی به نشانی های مندرج در قرارداد و یا نشانی هایی که کتباً به طرف دیگر اعلام شده باشد واصل شده تلقی می گردد.\r\n\r\nماده 9- نُسخه های قرارداد\r\nاین قرارداد مشتمل بر نه ماده در تاریخ ……….......................... به تعداد ….... نسخه متحد المتن دارای اعتبار حقوقی واحد تنظیم و طرفین نسبت به صحت، اعتبار و لازم الاجراء بودن آن اقرار نموده و نُسخه های قرارداد مزبور ما بین طرفین مبادله گردید.\r\n\r\n\r\n        امضاء کارفرما                                                امضاء مشاور حقوقی\r\n\r\n ";
                        break;
                    case "قرارداد وکالت":
                        contractTemplate = "ماده 1- طرفین قرارداد : \r\nپیرو تنظیم وکالت نامه شماره ………. مورخ ……………….. قرارداد حق‌الوکاله به شرح زیر فی مابین خانم/آقای: ……………………….. فرزند ………………… شماره شناسنامه ………….. کدملی ………………… صادره از ……….. شماره تماس ………………. ساکن ……………………………………………………………………......................................................................................................  به عنوان موکل و\r\nآقا/خانم ………………………. فرزند …………………. شماره شناسنامه …………… کدملی ………………… صادره از ……………… شماره تماس ………………… ساکن …………………………………………………........………………….................................... به عنوان وکیل تنظیم گردیده که برای طرفین لازم‌الاجرا می‌باشد.\r\n\r\nماده 2- موضوع وکالت: موضوع وکالت عبارت است از ……………………………………….........................\r\n\r\nماده 3- مبلغ قرارداد (حق الوکاله): حق الوکاله طبق توافق طرفین مبلغ …………………..........….. ریال معادل …………................……. تومان تعیین گردید.\r\nتبصره: ما بین طرفین مقرر گردید که مبلغ ………….............………….. ریال معادل ………….................……… تومان نقداً و مابقی پس از اخذ حکم به نفع موکل دریافت گردد.\r\n\r\nماده 4- شرایط قرارداد:\r\n1-تعهدات وکیل در اجرای این قرارداد، موکول به وصول مبلغ اولیه حق‌الوکاله خواهد بود.\r\n2-وکیل فوق‌الذکر موظف است در راستای احقاق حقوق موکل یا موکلین تمامی تلاش و کوشش خود را مبذول داشته و از هر طریقی که لازم بداند، اقدامات مقتضی را جهت مصلحت موکل به انجام رساند.\r\n3- موکل با اطلاع از اثر عدم پیشرفت کار و عواقب احتمالی آن قرارداد را امضاء نمود؛ بنابراین عدم پیشرفت کار برائت موکل را از پرداخت مبالغ مندرج در ماده 3 قرارداد حاصل نمی‌نماید.\r\n4- در صورت عزل وکیل یا ضم وکیل به وکیل، صلح و سازش طرفین دعوا با مداخله وکیل یا بدون مداخله وکیل، انصراف موکل از تعقیب موضوع در هر مرحله که باشد، استرداد دعوی توسط موکل یا طرف دعوی، وضع مقررات جدید یا به هر علتی که موضوع دعوا فیصله یابد، موکل متعهد است در تمام موارد فوق‌الذکر حق‌الوکاله مندرج در ماده 3 قرارداد را نقداً به صورت یکجا در مقابل اخذ رسید به وکیل پرداخت نماید.\r\n5- چنانچه موضوع وکالت ولو با ارسال اظهارنامه یا مذاکره شفاهی یا مصالحه به نتیجه برسد، موکل ملزم به پرداخت تمام حق الوکاله است.\r\n6- موکل در مورد حق‌الوکاله و کلیه اقدامات وکیل در جریان دادرسی هیچگونه اعتراض و ادعایی اعم از کیفری، حقوقی و انتظامی نداشته و به موجب این مقرره با رضایت کامل، تمامی اختلافات و دعاوی احتمالی آتی را با وکیل انتخابی خویش به صلح خاتمه می‌دهد.\r\n7- چنانچه پس از امضاء قرارداد و قبل از هرگونه اقدامی از جانب وکیل، موکل اقدام به عزل وکیل نماید، در این صورت موکل موظف است نیمی از مبلغ پیش پرداخت را به وکیل بپردازد.\r\n8- تعیین اوقات دادرسی با محکمه بوده و وکیل هیچگونه مسئولیتی در کندی و تسریع دادرسی ندارد.\r\n9- تعهد وکیل در مقابل موکل فقط دفاع از حقوق موکل در حد توانایی علمی و فنی خویش و با توجه به قوانین و مقررات مربوطه است و تصمیم گیرنده در خصوص دعوا قاضی است و وکیل هیچ تسلطی بر وی ندارد.\r\n10- این قرارداد شامل مرحله اجرای حکم نخواهد بود و چنانچه موکل تمایل به پیگیری مرحله اجرائی حکم توسط وکیل داشته باشد، می‌بایستی با توافق طرفین حق‌الوکاله آن مرحله نیز مشخص و نقداً به وکیل نیز پرداخت گردد؛ در غیر این صورت وکیل مزبور هیچ مسئولیتی در پیگیری مراحل اجرائی حکم نخواهد داشت.\r\n11- تهیه وسایل اجرای قرارهای دادگاه، معرفی شهود و مطلعین، احضار و جلب متهم به عهده وکیل نبوده و موکل موظف است نسبت به این امور اقدام کند.\r\n12- پرداخت هزینه‌های دادرسی و سایر هزینه‌ها از قبیل الصاق و ابطال تمبر، دستمزد کارشناس منتخب دادگاه، داور مورد رضایت طرفین، هزینه درج آگهی، هزینه مسافرت و غیره که برای رسیدگی و پیگیری پرونده مطروحه و احقاق حقوق موکل لازم باشد؛ به عهده موکل می‌باشد که در هر مرحله که لازم باشد، موکل موظف است نسبت به پرداخت نقدی هزینه موارد مذکور اقدام نماید؛ در غیر این صورت وکیل هیچ مسئولیتی به عهده نخواهد داشت.\r\n13- پرداخت مالیات و سهم کانون وکلاء به عهده وکیل خواهد بود.\r\n\r\nماده 5- مرجع حل اختلاف: در صورت بروز اختلاف ما بین طرفین به ترتیب از طریق مذاکره بین‌الطرفینی و سپس داوری و در نهایت مرجع صالح قانونی اقدام خواهد شد.\r\n\r\nماده 6- نُسخ قرارداد: این قرارداد مشتمل بر هفت ماده و یک تبصره در تاریخ ………........……….. به تعداد …….. نسخه با اعتبار واحد تنظیم، امضاء و بین طرفین جهت اجراء مبادله گردید.\r\n\r\n           امضاء وکیل                                                 امضاء موکل \r\n\r\n";
                        break;
                    default:
                        contractTemplate = string.Empty;
                        break;
                }
                // نمایش قالب قرارداد در TextBox
                ContractContentTB.IsEnabled = true;
                ContractContentTB.Text = contractTemplate;
            }
        }       
        private void ExportContractBTN_Click(object sender, RoutedEventArgs e)
        {
            PrintDialog printDialog = new PrintDialog();
            if (printDialog.ShowDialog() == true)
            {
                GenerateContract();

                IDocumentPaginatorSource document = ContractPreview.Document;
                printDialog.PrintDocument(document.DocumentPaginator, "قرارداد");
            }
        }    
        private void ShowContractListBTN_Click(object sender, RoutedEventArgs e)
        {
            if (ContractsDGV.Visibility == Visibility.Collapsed && SearchContractBox.Visibility == Visibility.Collapsed && SearchContractPlaceholderText.Visibility == Visibility.Collapsed && ContractTypeFilterTextBlock.Visibility == Visibility.Collapsed && ContractTypeFilterCB.Visibility == Visibility.Collapsed)
            {
                // نمایش جدول و پنهان‌سازی فرم
                ContractsDGV.Visibility = Visibility.Visible;
                ContractTypeFilterTextBlock.Visibility = Visibility.Visible;
                ContractTypeFilterCB.Visibility = Visibility.Visible;
                SearchContractBox.Visibility = Visibility.Visible;
                SearchContractPlaceholderText.Visibility = Visibility.Visible;
                RegContractPanel.Visibility = Visibility.Collapsed;
                DynamicTopLabel.Text = "🧾 لیست قرارداد ها";
            }
            else
            {
                // پنهان‌سازی جدول و نمایش فرم
                ContractsDGV.Visibility = Visibility.Collapsed;
                ContractTypeFilterTextBlock.Visibility = Visibility.Collapsed;
                ContractTypeFilterCB.Visibility = Visibility.Collapsed;
                SearchContractBox.Visibility = Visibility.Collapsed;
                SearchContractPlaceholderText.Visibility = Visibility.Collapsed;
                RegContractPanel.Visibility = Visibility.Visible;
                DynamicTopLabel.Visibility = Visibility.Visible;
                DynamicTopLabel.Text = "📄 اطلاعات قرارداد را وارد کنید";
            }
        }       
        private void SearchContractBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string keyword = SearchContractBox?.Text?.Trim() ?? string.Empty;
            int? typeFilter = null;
            List<ContractDto> filtered;

            if (string.IsNullOrWhiteSpace(SearchContractBox.Text.Trim()))
            {
                SearchContractPlaceholderText.Visibility = Visibility.Visible;
                filtered = contract_bll.GetContractsForListView();
                ContractsDGV.ItemsSource = contract_bll.GetContractsForListView();
            }
            else
            {
                ContractsDGV.ItemsSource = contract_bll.SearchContractsForDGV(keyword, typeFilter);
                SearchContractPlaceholderText.Visibility = Visibility.Hidden;
            }

            if (string.IsNullOrWhiteSpace(SearchContractBox.Text))
            {
                SearchContractPlaceholderText.Visibility = Visibility.Visible;
            }
            else
            {
                SearchContractPlaceholderText.Visibility = Visibility.Hidden;
            }
        }
        private void ContractTypeFilterCB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string searchTerm = SearchContractBox?.Text?.Trim() ?? string.Empty;
            int? typeFilter = null;
            if (int.TryParse(ContractTypeFilterCB.SelectedValue?.ToString(), out int parsedType))
            {
                typeFilter = parsedType;
            }
            var result = contract_bll.SearchContractsForDGV(searchTerm, typeFilter);
            ContractsDGV.ItemsSource = null;
            ContractsDGV.ItemsSource = result;
        }
        private void TotalAmountTB_TextChanged(object sender, TextChangedEventArgs e)
        {
            // اگر متن خالی است یا متن همان "ریال" است، ادامه نده
            if (string.IsNullOrWhiteSpace(TotalAmountTB.Text) || TotalAmountTB.Text == "ریال")
                return;

            // مکان‌نما را ذخیره می‌کنیم تا بتوانیم بعد از فرمت‌دهی آن را تنظیم کنیم
            int selectionStart = TotalAmountTB.SelectionStart;

            // حذف کاراکترهای غیر عددی (مثل کاما و ریال)
            string rawText = TotalAmountTB.Text.Replace(",", "").Replace(" ریال", "");

            // چک کردن برای اطمینان از اینکه متن فقط شامل اعداد است
            if (decimal.TryParse(rawText, out decimal amount))
            {
                // فرمت دادن به عدد به صورت سه‌رقمی و اضافه کردن "ریال" در انتها
                TotalAmountTB.Text = string.Format("{0:N0} ", amount);

                // بازگرداندن مکان‌نما به موقعیت قبلی
                TotalAmountTB.SelectionStart = TotalAmountTB.Text.Length - 1; // از انتهای متن 5 کاراکتر برای " ریال" کم می‌کنیم
            }
        }
        private void SelectClientNameCB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (SelectClientNameCB.SelectedItem is Client selectedClient)
            {
                // به شرط اینکه Case شامل Client باشد
                SelectCaseNumberCB.ItemsSource = selectedClient.Cases;
                SelectCaseNumberCB.DisplayMemberPath = "CaseNumber";
                SelectCaseNumberCB.SelectedValuePath = "Id";
                CaseSubjectLabel.Text = "";
            }
        }
        private void SelectCaseNumberCB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (SelectCaseNumberCB.SelectedItem is Case selectedCase)
            {
                // به شرط اینکه Case شامل Client باشد
                CaseSubjectLabel.Text = $"موضوع پرونده: {selectedCase.CaseSubject ?? "ناشناخته"}";
            }
        }
        private void ShowContractsDetailsBTN_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            if (btn?.Tag is int contractId)
            {
                ContractDetailsPopup.Tag = contractId;
                var _contract = contract_bll.GetContractId(contractId);
                PopupContractContentTB.Text = _contract.ContractContent;                            
                ContractDetailsPopup.Visibility = Visibility.Visible;
                MainGrid.Effect = new BlurEffect() { Radius = 5 };
            }
        }             
        private void ContractDetailsPopupCloseBTN_Click(object sender, RoutedEventArgs e)
        {
            ContractDetailsPopup.Visibility = Visibility.Collapsed;
            MainGrid.Effect = null;
        }
        private void PopupContractContentTB_TextChanged(object sender, TextChangedEventArgs e)
        {
            PopupCharCountLabel.Text = $"{PopupContractContentTB.Text.Length} کاراکتر";
        }
        private void PopupExportContractBTN_Click(object sender, RoutedEventArgs e)
        {
            PrintDialog printDialog = new PrintDialog();
            if (printDialog.ShowDialog() == true)
            {
                GeneratePopupContract();

                IDocumentPaginatorSource document = ContractPreview.Document;
                printDialog.PrintDocument(document.DocumentPaginator, "قرارداد");
            }
        }
        private void ContractSubmitBTN_Click(object sender, RoutedEventArgs e)
        {
            // بررسی اینکه آیا قالب تکمیل شده است
            if (string.IsNullOrWhiteSpace(SelectClientNameCB.Text) || string.IsNullOrWhiteSpace(ContractTypeCB.Text) || string.IsNullOrWhiteSpace(TotalAmountTB.Text) || string.IsNullOrWhiteSpace(SetDateDP.Text) || string.IsNullOrWhiteSpace(ContractContentTB.Text))
            {
                ShowNotification("لطفاً فیلدهای ضروری را پر کنید", "warning");
                return;
            }
            // بررسی وجود موکل در ComboBox
            if (SelectClientNameCB.SelectedItem == null)
            {
                ShowNotification("موکل در سیستم موجود نمیباشد!", "error");
                return;
            }
            var user = AppSession.CurrentUser;
            var newcontract = new Contract
            {
                UserId = user.Id,
                UserRole = (int)user.Role,
                ClientId = (int)SelectClientNameCB.SelectedValue,
                CaseId = SelectCaseNumberCB.SelectedValue as int?,
                ContractType = ContractTypeCB.SelectedIndex,
                TotalAmount = Convert.ToDecimal(TotalAmountTB.Text),
                SetDate = SetDateDP.SelectedDate.Value,
                ContractContent = ContractContentTB.Text,
                CreatedAt = DateTime.Now
            };
            var result = contract_bll.Create(newcontract);
            ShowNotification(result, "success");
            RefreshContract();
            SaveChanges();
            ClearControls(RegContractPanel);
            CaseSubjectLabel.Text = "";
        }
        #endregion

        #region LegalBrief
        private void GenerateLegalBrief()
        {
            // Create FlowDocument
            FlowDocument doc = new FlowDocument
            {
                PagePadding = new Thickness(20),
                ColumnWidth = 800,
                FontFamily = new FontFamily("Sahel"),
                FontSize = 12,
                TextAlignment = TextAlignment.Right,
            };

            // ایجاد Border به عنوان حاشیه صفحه
            Border pageBorder = new Border
            {
                BorderBrush = Brushes.Black,
                BorderThickness = new Thickness(2),
                CornerRadius = new CornerRadius(10),
                Padding = new Thickness(20),
                Margin = new Thickness(0),
                Child = new StackPanel
                {
                    Children =
                    {
                        new TextBlock
                        {
                            Text = "\t\t\t\tبسمه تعالی\t\t\t⚖ لایحه",
                            FontSize = 18,
                            FontWeight = FontWeights.Bold,
                            TextAlignment = TextAlignment.Center
                        },
                        new TextBlock
                        {
                            Text = LegalBriefContentTB.Text,
                            TextAlignment = TextAlignment.Justify,
                            Margin = new Thickness(0, 5, 0, 0),
                            TextWrapping = TextWrapping.Wrap,
                        }
                    }
                }
            };

            // افزودن Border به FlowDocument
            doc.Blocks.Add(new BlockUIContainer(pageBorder));

            // Assign Document to Viewer
            ContractPreview.Document = doc;
        }
        private void GeneratePopupLegalBrief()
        {
            // Create FlowDocument
            FlowDocument doc = new FlowDocument
            {
                PagePadding = new Thickness(20),
                ColumnWidth = 800,
                FontFamily = new FontFamily("Sahel"),
                FontSize = 12,
                TextAlignment = TextAlignment.Right,
            };

            // ایجاد Border به عنوان حاشیه صفحه
            Border pageBorder = new Border
            {
                BorderBrush = Brushes.Black,
                BorderThickness = new Thickness(2),
                CornerRadius = new CornerRadius(10),
                Padding = new Thickness(20),
                Margin = new Thickness(0),
                Child = new StackPanel
                {
                    Children =
                    {
                        new TextBlock
                        {
                            Text = "\t\t\t\tبسمه تعالی\t\t\t⚖ لایحه",
                            FontSize = 18,
                            FontWeight = FontWeights.Bold,
                            TextAlignment = TextAlignment.Center
                        },
                        new TextBlock
                        {
                            Text = PopupLegalBriefContentTB.Text,
                            TextAlignment = TextAlignment.Justify,
                            Margin = new Thickness(0, 5, 0, 0),
                            TextWrapping = TextWrapping.Wrap,
                        }
                    }
                }
            };

            // افزودن Border به FlowDocument
            doc.Blocks.Add(new BlockUIContainer(pageBorder));

            // Assign Document to Viewer
            ContractPreview.Document = doc;
        }
        private List<BriefTemplate> templates;
        private void SelectClientForLegalBriefCB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (SelectClientForLegalBriefCB.SelectedItem is Client selectedClient)
            {
                // به شرط اینکه Case شامل Client باشد
                SelectCaseNumberForLegalBriefCB.ItemsSource = selectedClient.Cases;
                SelectCaseNumberForLegalBriefCB.DisplayMemberPath = "CaseNumber";
                SelectCaseNumberForLegalBriefCB.SelectedValuePath = "Id";              
            }
        }    
        private void LoadTemplates()
        {
            templates = new List<BriefTemplate>
            {
                new BriefTemplate { Id = 1, Title = "درخواست مطالعه پرونده", Content ="بسمه تعالی\r\nریاست محترم شعبه … دادگاه عمومی/کیفری/اجرای احکام …\r\nبا سلام و احترام\r\n\r\nاینجانب )نام وکیل(، وکیل پایه یک دادگستری به وکالت از )نام موکل(، در پرونده شماره )شماره پرونده( مطروحه در آن مرجع محترم، به استحضار می‌رساند:\r\n\r\nنظر به ضرورت بررسی مستندات و مدارک موجود در پرونده و دفاع مؤثر از حقوق موکل، ، خواهشمند است دستور فرمایید امکان مطالعه پرونده و اخذ کپی و رونوشت از اوراق پرونده، از جمله )مشخص کردن مدارک خاص در صورت نیاز، مانند دادخواست، لوایح، صورت جلسات، گزارش کارشناسی و غیره(، برای اینجانب فراهم گردد.\r\n\r\nپیشاپیش از حسن توجه و مساعدت حضرتعالی کمال تشکر را دارم.\r\n\r\nبا احترام\r\n)نام وکیل(\r\nوکیل خواهان/ خوانده /شاکی/ متهم\r\nتاریخ: …\r\nامضاء: …"},
                new BriefTemplate { Id = 2, Title = "استرداد اعتراض به نظریه کارشناسی", Content ="بسمه‌تعالی\r\n\r\nریاست محترم شعبه ……… دادگاه ………\r\nبا سلام و احترام\r\n\r\nاینجانب )نام وکیل( به وکالت از )نام موکل( در پرونده کلاسه )شماره پرونده(، موضوع )موضوع پرونده(، در خصوص اعتراض به نظریه کارشناسی که پیش‌تر تقدیم دادگاه محترم گردیده، به استحضار می‌رساند:\r\n\r\nبا توجه به بررسی‌های مجدد صورت گرفته و در راستای احترام به نظر کارشناسی ارائه‌شده، بدین‌وسیله اعتراض خود را مسترد نموده و آمادگی موکل را جهت اجرای مفاد نظریه کارشناسی اعلام می‌دارم.\r\n\r\nلذا از دادگاه محترم تقاضا دارم با توجه به استرداد اعتراض، ادامه رسیدگی را بر اساس نظریه کارشناسی ارائه‌شده انجام داده و تصمیم مقتضی را صادر فرماید.\r\n\r\nبا احترام\r\nنام وکیل\r\nامضاء و مهر"},
                new BriefTemplate { Id = 3, Title = "استرداد دادخواست", Content ="بسمه تعالی\r\nریاست محترم شعبه … دادگاه عمومی حقوقی …\r\nبا سلام و احترام\r\n\r\nاینجانب )نام وکیل(، وکیل پایه یک دادگستری و به وکالت از )نام موکل(، در پرونده شماره )شماره پرونده(، با موضوع )موضوع دعوا( که در آن مرجع محترم مطرح گردیده است، به استحضار می‌رساند:\r\n\r\nبا توجه به )دلایل استرداد دادخواست مانند: حصول توافق با طرف دعوی، رفع اختلاف، تغییر استراتژی حقوقی، نقص مدارک و نیاز به تکمیل، یا هر دلیل دیگر(، و مستند به ماده ۱۰۷ قانون آیین دادرسی مدنی، تقاضای استرداد دادخواست تقدیمی را قبل از صدور حکم از آن مقام محترم دارم.\r\n\r\nپیشاپیش از حسن نظر و مساعدت حضرتعالی سپاسگزارم.\r\n\r\nبا احترام\r\n)نام وکیل(\r\nوکیل پایه یک دادگستری\r\nتاریخ: …\r\nامضاء: …"},
                new BriefTemplate { Id = 4, Title = "اسقاط حق تجدیدنظرخواهی یا فرجام‌خواهی", Content ="بسمه‌تعالی\r\n\r\nریاست محترم شعبه ……… دادگاه ………\r\nبا سلام و احترام\r\n\r\nاینجانب )نام وکیل( به وکالت از )نام موکل( در پرونده کلاسه )شماره پرونده(، موضوع )عنوان پرونده(، بدین‌وسیله اعلام می‌دارم که موکل اینجانب با علم و آگاهی کامل از حقوق قانونی خود، حق تجدیدنظرخواهی / فرجام‌خواهی نسبت به رأی صادره در این پرونده را اسقاط نموده و هیچ‌گونه ادعایی در این خصوص نخواهد داشت.\r\n\r\n۱. موکل پس از بررسی رأی صادره، آن را عادلانه و منطبق با قانون تشخیص داده و نیازی به اعتراض یا تجدیدنظر در آن نمی‌بیند.\r\n۲. این تصمیم با اختیار و اراده کامل اتخاذ شده و هیچ‌گونه اجبار یا اکراهی در آن وجود ندارد.\r\n۳. درخواست ثبت اسقاط حق تجدیدنظرخواهی / فرجام‌خواهی و قطعیت رأی صادره مطابق  قانون آیین دادرسی مدنی تقدیم می‌گردد.\r\n\r\nبا عنایت به موارد فوق، از دادگاه محترم تقاضا دارم مراتب اسقاط حق تجدیدنظرخواهی / فرجام‌خواهی را در پرونده ثبت نموده و اقدامات مقتضی را در خصوص اجرای رأی صادره مبذول فرمایید.\r\n\r\nبا احترام\r\nنام وکیل\r\nامضاء و مهر"},              
                new BriefTemplate { Id = 5, Title = "اعتراض به بهای خواسته", Content ="بسمه‌تعالی\r\n\r\nریاست محترم شعبه ……… دادگاه ………\r\nبا سلام و احترام\r\n\r\nاینجانب )نام وکیل( به وکالت از )نام موکل( در پرونده کلاسه )شماره پرونده(، موضوع )عنوان دعوی(، بدین‌وسیله اعتراض خود را نسبت به بهای خواسته تعیین‌شده توسط خواهان اعلام می‌دارم.\r\n\r\n۱. مغایرت بهای خواسته با ارزش واقعی مورد دعوی: خواهان در دادخواست تقدیمی، بهای خواسته را مبلغ )مبلغ اعلام‌شده( تعیین نموده که این مبلغ با ارزش واقعی موضوع دعوا تطابق ندارد.\r\n۲. تأثیر نادرست بهای خواسته بر هزینه دادرسی و صلاحیت دادگاه: تعیین نادرست بهای خواسته می‌تواند موجب تغییر در میزان هزینه دادرسی و حتی مرجع صالح رسیدگی گردد که این امر برخلاف موازین قانونی است.\r\n۳. لزوم ارجاع به کارشناس رسمی دادگستری: جهت تعیین دقیق ارزش واقعی موضوع دعوی، پیشنهاد می‌شود که موضوع به کارشناسی ارجاع شود تا از تضییع حقوق موکل جلوگیری گردد.\r\n\r\nبا عنایت به موارد فوق، از دادگاه محترم تقاضا دارم:\r\n۱. نسبت به اصلاح بهای خواسته بر اساس ارزیابی دقیق و واقعی اقدام گردد.\r\n۲. در صورت لزوم، موضوع به کارشناس رسمی دادگستری ارجاع شود.\r\n۳. هزینه دادرسی متناسب با بهای واقعی خواسته تعیین گردد.\r\n\r\nبا احترام\r\nنام وکیل\r\nامضاء و مهر"},
                new BriefTemplate { Id = 6, Title = "اظهار به تقدیم دادخواست جلب ثالث", Content = "بسمه‌تعالی\r\n\r\nریاست محترم شعبه ……… دادگاه ………\r\nبا سلام و احترام\r\n\r\nاینجانب )نام وکیل( به وکالت از )نام موکل( در پرونده کلاسه )شماره پرونده(، موضوع )عنوان پرونده(، بدین‌وسیله مراتب تقدیم دادخواست جلب ثالث را به استحضار دادگاه محترم می‌رسانم.\r\n\r\nنظر به اینکه در جریان رسیدگی به پرونده حاضر، شخص )نام شخص ثالث( به عنوان فردی که حقوق یا تعهدات وی در نتیجه رأی صادره تحت تأثیر قرار می‌گیرد، در موضوع اختلاف نقش دارد، موکل اینجانب به موجب )مواد ۱۳۵ و ۱۳۷ قانون آیین دادرسی مدنی( دادخواست جلب ثالث را بنا به دلایل زیر تنظیم و به دفتر دادگاه محترم تقدیم نموده است:\r\n\r\n۱. نقش مؤثر شخص ثالث در موضوع دعوی و ارتباط مستقیم حقوق یا تعهدات وی با نتیجه پرونده.\r\n۲. پیشگیری از تضییع حقوق موکل و جلوگیری از صدور رأیی که بدون لحاظ شدن حقوق شخص ثالث ممکن است ناقص یا ناعادلانه باشد.\r\n۳. تقدیم دادخواست جلب ثالث در مهلت قانونی و درخواست رسیدگی هم‌زمان با پرونده اصلی.\r\n\r\nبا توجه به مراتب فوق، از دادگاه محترم تقاضا دارم مراتب تقدیم دادخواست جلب ثالث را در پرونده منعکس نموده و وفق مقررات، اقدامات لازم را جهت رسیدگی هم‌زمان به دعوای اصلی و جلب ثالث به عمل آورید.\r\n\r\nبا احترام\r\nنام وکیل\r\nامضاء و مهر"},
                new BriefTemplate { Id = 7, Title = "لایحه دفاعیه (در دعاوی حقوقی)", Content = "ریاست محترم شعبه }شماره شعبه{ دادگاه محترم }نوع دادگاه{ شهرستان }نام شهرستان{\r\n\r\nبا سلام و احترام\r\nاینجانب }نام موکل{ با وکالت اینجانب در پرونده کلاسه }کلاسه پرونده{ به خواسته }خواسته خواهان{ بدین‌وسیله لایحه دفاعیه خود را به شرح زیر تقدیم می‌نمایم:\r\n\r\n.۱ در خصوص ادعای خواهان مبنی بر }خلاصه خواسته یا ادعا{ لازم به ذکر است که...\r\n\r\n.۲ با توجه به مستندات پیوست )شامل }اسناد مورد اشاره{( و سوابق موضوع به استحضار می‌رساند...\r\n\r\n.۳ از آن مقام محترم تقاضای صدور حکم بر رد دعوای خواهان و احقاق حقوق موکل را دارم.\r\n\r\nبا تجدید احترام\r\nوکیل پایه یک دادگستری  \r\n}نام وکیل{  \r\nتاریخ: }تاریخ روز{\r\n"},
                new BriefTemplate { Id = 8, Title = "اعتراض به رأی بدوی", Content = "ریاست محترم شعبه }شماره شعبه{ دادگاه تجدیدنظر استان }نام استان{\r\n\r\nموضوع: اعتراض به رأی صادره در پرونده کلاسه }کلاسه پرونده{\r\n\r\nبا سلام و احترام  \r\nبدین‌وسیله نسبت به رأی صادره به شماره دادنامه }شماره دادنامه{ مورخ }تاریخ صدور رأی{ از شعبه }شماره شعبه{ دادگاه بدوی، به وکالت از }نام موکل{، اعتراض می‌نمایم و دلایل اعتراض به شرح زیر تقدیم حضور می‌گردد:\r\n\r\n.۱ اولاً،...\r\n\r\n.۲ ثانیاً،...\r\n\r\nبا توجه به مراتب فوق و مستندات پیوست، از آن مرجع محترم تجدیدنظر تقاضای نقض دادنامه معترضٌ‌عنه و صدور رأی شایسته را دارم.\r\nبا احترام  \r\nوکیل معترض  \r\n}نام وکیل{  \r\nتاریخ: }تاریخ{\r\n" },
                new BriefTemplate { Id = 9, Title = "دفاعیه در دعاوی کیفری", Content = "ریاست محترم شعبه }شماره شعبه{ دادگاه تجدیدنظر استان }نام استانریاست محترم شعبه }شماره شعبه{ دادگاه کیفری }نوع دادگاه{ شهرستان }نام شهرستان{\r\n\r\nبا سلام و احترام  \r\nاینجانب، وکیل مدافع آقای/خانم }نام موکل{ در پرونده کلاسه }کلاسه پرونده{ اتهام وارده مبنی بر }عنوان اتهام{ را به دلایل زیر رد می‌نمایم:\r\n\r\n.۱ موکل در تاریخ مذکور در محل وقوع جرم حضور نداشته و بر اساس }مدرک/شاهد/...{، بی‌گناهی ایشان محرز است.\r\n\r\n.۲ ...\r\n\r\nلذا با عنایت به دلایل فوق و مستندات تقدیمی، تقاضای صدور حکم بر برائت موکل را دارم.\r\n\r\nبا احترام  \r\nوکیل پایه یک دادگستری  \r\n}نام وکیل{  \r\nتاریخ: }تاریخ{\r\n" },
                new BriefTemplate { Id = 10, Title = "درخواست استمهال (تمدید مهلت)", Content = "ریاست محترم شعبه }شماره شعبه{ دادگاه محترم }نوع دادگاه{ شهرستان }نام شهرستان{\r\n\r\nبا سلام و احترام  \r\nاحتراماً در خصوص پرونده کلاسه }کلاسه پرونده{ به طرفیت }نام طرف مقابل{ به وکالت از }نام موکل{، نظر به شرایط خاص و محدودیت‌های پیش‌آمده، بدین‌وسیله تقاضای استمهال }تمدید مهلت{ جهت }شرح موضوع، مثلاً: تقدیم لایحه دفاعیه / ارائه اسناد{ به مدت }مدت مورد نیاز{ روز را دارم.\r\n\r\nبا تجدید احترام  \r\nوکیل پایه یک دادگستری  \r\n}نام وکیل{  \r\nتاریخ: }تاریخ{\r\n" },
                new BriefTemplate { Id = 11, Title = "درخواست اجرای حکم", Content = "ریاست محترم شعبه اجرای احکام مدنی شهرستان }نام شهرستان{\r\n\r\nبا سلام و احترام  \r\nاحتراماً به استحضار می‌رساند اینجانب، به وکالت از }نام موکل{ در پرونده کلاسه }کلاسه پرونده{ موفق به اخذ حکم قطعی به شماره دادنامه }شماره دادنامه{ مورخ }تاریخ{ از شعبه }شماره شعبه{ دادگاه }نوع دادگاه{ شده‌ام.\r\n\r\nبا عنایت به قطعیت حکم صادره، خواهشمند است دستور اجرای آن را صادر فرمایید.\r\n\r\nبا احترام  \r\nوکیل پایه یک دادگستری  \r\n}نام وکیل{  \r\nتاریخ: }تاریخ{\r\n" },

                // ✅ لایحه جدید سفارشی توسط وکیل
                new BriefTemplate
                {
                    Id = 0, // آی‌دی صفر برای تشخیص ویژه
                    Title = "✍️ لایحه جدید (سفارشی)",
                    Content = "لطفاً متن لایحه موردنظر خود را در این قسمت وارد نمایید..."
                }
            };
            TemplateCB.ItemsSource = templates;
        }
        private void SelectLayeheCB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (TemplateCB.SelectedItem is BriefTemplate selectedTemplate)
            {
                LegalBriefContentTB.Text = selectedTemplate.Content;
                LegalBriefContentTB.IsReadOnly = false;
            }
        }
        private void SearchLegalBriefBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string keyword = SearchLegalBriefBox.Text.Trim();                    
            List<LegalBriefDto> filtered;

            if (string.IsNullOrWhiteSpace(keyword))
            {
                SearchLegalBriefPlaceholderText.Visibility = Visibility.Visible;
                filtered = legalBrief_bll.GetLegalBriefsForListView();              
                LegalBriefDGV.ItemsSource = filtered;
            }
            else
            {
                filtered = legalBrief_bll.SearchLegalBriefsForDGV(keyword);             
                LegalBriefDGV.ItemsSource = filtered;
                SearchLegalBriefPlaceholderText.Visibility = Visibility.Hidden;
            }

            if (string.IsNullOrWhiteSpace(SearchLegalBriefBox.Text))
            {
                SearchLegalBriefPlaceholderText.Visibility = Visibility.Visible;
            }
            else
            {
                SearchLegalBriefPlaceholderText.Visibility = Visibility.Hidden;
            }
        }       
        private void ShowLegalBriefsListBTN_Click(object sender, RoutedEventArgs e)
        {
            if (LegalBriefDGV.Visibility == Visibility.Collapsed && SearchLegalBriefBox.Visibility == Visibility.Collapsed && SearchLegalBriefPlaceholderText.Visibility == Visibility.Collapsed)
            {
                // نمایش جدول و پنهان‌سازی فرم
                LegalBriefDGV.Visibility = Visibility.Visible;               
                SearchLegalBriefBox.Visibility = Visibility.Visible;
                SearchLegalBriefPlaceholderText.Visibility = Visibility.Visible;
                LegalBriefSubmitBTN.Visibility = Visibility.Collapsed;
                RegBriefPanel.Visibility = Visibility.Collapsed;
                DeliveryCheckBox.Visibility = Visibility.Collapsed;
                DeliveryDateDP.Visibility = Visibility.Collapsed;
                ExportLegalBriefBTN.Visibility = Visibility.Collapsed;    
                ListLayeheDynamicText.Visibility = Visibility.Visible;
            }
            else
            {
                // پنهان‌سازی جدول و نمایش فرم
                LegalBriefDGV.Visibility = Visibility.Collapsed;              
                SearchLegalBriefBox.Visibility = Visibility.Collapsed;
                SearchLegalBriefPlaceholderText.Visibility = Visibility.Collapsed;
                LegalBriefSubmitBTN.Visibility = Visibility.Visible;
                RegBriefPanel.Visibility = Visibility.Visible;
                DeliveryCheckBox.Visibility = Visibility.Visible;
                DeliveryDateDP.Visibility = Visibility.Visible;
                ExportLegalBriefBTN.Visibility = Visibility.Visible;
                ListLayeheDynamicText.Visibility = Visibility.Collapsed;
            }
        }
        private void DeliveryCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            DeliveryDateDP.IsEnabled = true;
        }
        private void DeliveryCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            DeliveryDateDP.IsEnabled = false;
            DeliveryDateDP.SelectedDate = null;
        }
        private void ShowLegalBriefContentBTN_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            if (btn?.Tag is int legalbriefId)
            {
                LegalBriefDetailsPopup.Tag = legalbriefId;
                var _legalbrief = legalBrief_bll.GetBriefById(legalbriefId);
                PopupLegalBriefContentTB.Text = _legalbrief.Content;
                LegalBriefDetailsPopup.Visibility = Visibility.Visible;
                MainGrid.Effect = new BlurEffect() { Radius = 5 };
                LegalBriefGrid.Effect = new BlurEffect() { Radius = 5 };
            }            
        }
        private void LegalBriefDetailsPopupCloseBTN_Click(object sender, RoutedEventArgs e)
        {
            LegalBriefDetailsPopup.Visibility = Visibility.Collapsed;
            MainGrid.Effect = null;
            LegalBriefGrid.Effect = null;
        }
        private void LegalBriefSubmitBTN_Click(object sender, RoutedEventArgs e)
        {
            // بررسی اینکه آیا قالب تکمیل شده است
            if (string.IsNullOrWhiteSpace(SelectClientForLegalBriefCB.Text) || string.IsNullOrWhiteSpace(SelectCaseNumberForLegalBriefCB.Text) || string.IsNullOrWhiteSpace(TemplateCB.Text) || string.IsNullOrWhiteSpace(SetDateDForLegalBriefDP.Text) || string.IsNullOrWhiteSpace(LegalBriefContentTB.Text))
            {                
                ShowNotification("لطفاً تمام فیلدها را پر کنید", "warning");
                return;
            }
            // بررسی وجود موکل در ComboBox
            if (SelectClientForLegalBriefCB.SelectedItem == null)
            {
                ShowNotification("موکل در سیستم موجود نمیباشد!", "error");
                return;
            }
            var user = AppSession.CurrentUser;
            var newlegalbrief = new LegalBrief
            {
                UserId = user.Id,
                UserRole = (int)user.Role,
                ClientId = (int)SelectClientForLegalBriefCB.SelectedValue,
                CaseId = SelectCaseNumberForLegalBriefCB.SelectedValue as int?,
                Title = TemplateCB.SelectedIndex,
                SetDate = SetDateDForLegalBriefDP.SelectedDate.Value,
                Content = LegalBriefContentTB.Text,
                IsDeliverySet = DeliveryCheckBox.IsChecked == true,
                DeliveryDate = DeliveryCheckBox.IsChecked == true ? DeliveryDateDP.SelectedDate : null,
                IsDeliveryDone = false,
                CreatedAt = DateTime.Now
            };
            if (DeliveryCheckBox.IsChecked == true && DeliveryDateDP.Text == "")
            {
                ShowNotification("تاریخ را وارد کنید!","error");
            }
            else
            {
                int legalbriefId = legalBrief_bll.AddBrief(newlegalbrief);
                if (legalbriefId > 0)
                {
                    ShowNotification("لایحه با موفقیت ثبت شد", "success");

                    if (DeliveryCheckBox.IsChecked == true && DeliveryDateDP.SelectedDate.Value != null)
                    {
                        var reminder = new Reminder
                        {
                            Title = $"یادآور لایحه برای {SelectClientForLegalBriefCB.Text}",
                            //Description = RichEditBox.Document.Text,
                            ReminderDate = DeliveryDateDP.SelectedDate.Value,
                            UserId = user.Id,
                            LegalBriefId = legalbriefId
                        };
                        rbll.Create(reminder);
                        var bulkSendResult = smsIr.BulkSendAsync(30007732011420, $"یادآوری لایحه برای موکل: {SelectClientForLegalBriefCB.Text}\n پرونده: {SelectCaseNumberForLegalBriefCB.Text}\n عنوان: {TemplateCB.Text} \nتاریخ یادآوری: {DeliveryDateDP.SelectedDate.Value.ToString("yyyy/MM/dd")}\n\n نرم افزار حقوقی هیام", new string[] { user.PhoneNumber });
                    }
                    RefreshLegalBrief();
                    SaveChanges();
                    ClearControls(RegBriefPanel);
                    DeliveryCheckBox.IsChecked = false;
                    DeliveryDateDP.SelectedDate = null;
                    LegalBriefContentTB.IsReadOnly = true;
                }
                else
                {
                    ShowNotification("خطا در ثبت لایحه!", "error");
                }            
            }           
        }
        private void ExportLegalBriefBTN_Click(object sender, RoutedEventArgs e)
        {
            PrintDialog printDialog = new PrintDialog();
            if (printDialog.ShowDialog() == true)
            {
                GenerateLegalBrief();

                IDocumentPaginatorSource document = ContractPreview.Document;
                printDialog.PrintDocument(document.DocumentPaginator, "لایحه");
            }
        }
        private void PopupExportLegalBriefBTN_Click(object sender, RoutedEventArgs e)
        {
            PrintDialog printDialog = new PrintDialog();
            if (printDialog.ShowDialog() == true)
            {
                GeneratePopupLegalBrief();

                IDocumentPaginatorSource document = ContractPreview.Document;
                printDialog.PrintDocument(document.DocumentPaginator, "لایحه");
            }
        }
        #endregion

        private void CloseBTN_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }       
    }
}