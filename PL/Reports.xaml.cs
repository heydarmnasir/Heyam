using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using LiveCharts.Wpf;
using LiveCharts;
using System.Windows.Media.Imaging;
using System.Linq;
using BLL;
using BE.ViewModel;
using BE;
using System.Globalization;
using PdfSharp.Pdf;
using PdfSharp.Drawing;

namespace Heyam
{  
    public partial class Reports : Window
    {
        Payment_BLL pbll = new Payment_BLL();
        Client_BLL cbll = new Client_BLL();
        Case_BLL case_bll = new Case_BLL();
        Contract_BLL contract_bll = new Contract_BLL();
        LawyerExpense_BLL lebll = new LawyerExpense_BLL();
        Dashboard_BLL dbll = new Dashboard_BLL();
        public Reports()
        {
            InitializeComponent();
            RefreshCases();

            RefreshContractsReport();
            RefreshPaymentsReport();
            RefreshExpenseReport();
            PopulateYears();
        }

        #region Methods
        private bool TryConvertPersianToMiladi(string persianDate, out DateTime miladiDate)
        {
            miladiDate = DateTime.MinValue;
            try
            {
                var parts = persianDate.Split('/');
                if (parts.Length == 3 &&
                    int.TryParse(parts[0], out int year) &&
                    int.TryParse(parts[1], out int month) &&
                    int.TryParse(parts[2], out int day))
                {
                    var pc = new PersianCalendar();
                    miladiDate = pc.ToDateTime(year, month, day, 0, 0, 0, 0);
                    return true;
                }
            }
            catch { }

            return false;
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
        public void RefreshCases()
        {
            ReportCasesDGV.ItemsSource = null;
            ReportCasesDGV.ItemsSource = case_bll.GetCasesForListView();
            Case_AmountCalculate.Text = dbll.ActiveCasesCount();
            ClosedCase_AmountCalculate.Text = dbll.ClosedCasesCount();
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

        //گزارش گیری مالی      
        #region FinancialReporting  
        private void ReportTabs_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CasesTab != null && CasesTab.IsSelected)
            {
                CasesItemsReportWP.Visibility = Visibility.Visible;
            }
            else if (FinancialTab.IsSelected)
            {
                CasesItemsReportWP.Visibility = Visibility.Collapsed;
            }
        }
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
                MessageBox.Show("رکوردی برای چاپ وجود ندارد.");
                return;
            }
            var brushconverter = new System.Windows.Media.BrushConverter();
            FlowDocument doc = new FlowDocument
            {
                PageWidth = 845,
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
            table.Columns.Add(new TableColumn { Width = new GridLength(160) }); // نام موکل
            table.Columns.Add(new TableColumn { Width = new GridLength(155) }); // شماره پرونده
            table.Columns.Add(new TableColumn { Width = new GridLength(120) }); // موضوع پرونده
            table.Columns.Add(new TableColumn { Width = new GridLength(90) }); // نوع قرارداد
            table.Columns.Add(new TableColumn { Width = new GridLength(135) }); // مبلغ
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
                    //Visibility = Visibility.Collapsed,
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
        private void ReportContractTypeCB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string searchTerm = ContractSearchClient?.Text?.Trim() ?? string.Empty;
            int? typeFilter = null;
            //if (int.TryParse(ReportContractTypeCB.SelectedValue?.ToString(), out int parsedType))
            //{
            //    typeFilter = parsedType;
            //}
            var result = contract_bll.SearchContractsForDGV(searchTerm, typeFilter);
            //ReportContractsDGV.ItemsSource = null;
            //ReportContractsDGV.ItemsSource = result;
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
            table.Columns.Add(new TableColumn { Width = new GridLength(330) }); // توضیحات

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

        //گزارش گیری پرونده ها
        #region CasesReporting
        private void CasesSearchClientTB_TextChanged(object sender, TextChangedEventArgs e)
        {
            string keyword = CasesSearchClientTB.Text.Trim();
            int typeFilter = 0;
            List<CaseDto> filtered;

            if (string.IsNullOrWhiteSpace(keyword))
            {
                CasesClientNamePlaceholderText.Visibility = Visibility.Visible;
                filtered = case_bll.GetCasesForListView();
                ReportCasesDGV.ItemsSource = filtered;
            }
            else
            {
                filtered = case_bll.SearchCasesForDGV(keyword, typeFilter);
                ReportCasesDGV.ItemsSource = filtered;
                CasesClientNamePlaceholderText.Visibility = Visibility.Hidden;
            }
        }
        private void CasesStartDateDP_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(CasesStartDateDP.Text))
            {
                CasesStartDatePlaceholderText.Visibility = Visibility.Visible;
            }
            else
            {
                CasesStartDatePlaceholderText.Visibility = Visibility.Hidden;
            }
        }
        private void CasesEndDateDP_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(CasesEndDateDP.Text))
            {
                CasesEndDatePlaceholderText.Visibility = Visibility.Visible;
            }
            else
            {
                CasesEndDatePlaceholderText.Visibility = Visibility.Hidden;
            }
        }
        private void ApplyFiltersBTN_Click(object sender, RoutedEventArgs e)
        {
            // بررسی خالی نبودن تاریخ‌ها
            if (CasesStartDateDP.SelectedDate == null || CasesEndDateDP.SelectedDate == null)
            {
                RefreshCases(); // اگر داری برای آمار یا بازنشانی
                return;
            }

            DateTime startDate = CasesStartDateDP.SelectedDate.Value.Date;
            DateTime endDate = CasesEndDateDP.SelectedDate.Value.Date;

            if (startDate > endDate)
            {
                ShowNotification("تاریخ شروع نباید بزرگ‌تر از تاریخ پایان باشد!", "error");
                return;
            }

            var filteredCases = case_bll.GetByDateRange(startDate, endDate); // متدی که در ادامه تعریف می‌کنیم
            ReportCasesDGV.ItemsSource = filteredCases;
        }
        private void PrintCasesReportBTN_Click(object sender, RoutedEventArgs e)
        {
            var items = ReportCasesDGV.ItemsSource as IEnumerable<CaseDto>;
            if (items == null || !items.Any())
            {
                ShowNotification("رکوردی برای چاپ وجود ندارد!", "error");
                return;
            }
            var brushconverter = new System.Windows.Media.BrushConverter();
            FlowDocument doc = new FlowDocument
            {
                PageWidth = 1195,
                PagePadding = new Thickness(30),
                ColumnWidth = double.PositiveInfinity,
                FontFamily = new FontFamily("IRANSansWeb(FaNum)"),
                FontSize = 12,
                TextAlignment = TextAlignment.Left,
                FlowDirection = FlowDirection.RightToLeft,
            };
            Section mainContent = new Section();
            // عنوان
            Paragraph title = new Paragraph(new Run("گزارش پرونده ها"))
            {
                Foreground = (Brush)brushconverter.ConvertFromString("#1259D5"),
                FontSize = 20,
                FontWeight = FontWeights.Bold,
                TextAlignment = TextAlignment.Center,
                Margin = new Thickness(0, 0, 0, 20)
            };
            mainContent.Blocks.Add(title);

            // جدول پرونده ها
            Table table = new Table
            {
                CellSpacing = 0,
                Padding = new Thickness(5),
                BorderBrush = Brushes.Black,
                BorderThickness = new Thickness(1.5),
            };

            // تعریف ستون‌ها - ستون اول عریض‌تر
            table.Columns.Add(new TableColumn { Width = new GridLength(160) }); // شماره پرونده
            table.Columns.Add(new TableColumn { Width = new GridLength(140) }); // نام موکل
            table.Columns.Add(new TableColumn { Width = new GridLength(80) }); // تاریخ
            table.Columns.Add(new TableColumn { Width = new GridLength(100) }); // عنوان پرونده
            table.Columns.Add(new TableColumn { Width = new GridLength(100) }); // موضوع پرونده
            table.Columns.Add(new TableColumn { Width = new GridLength(90) }); // وضعیت
            table.Columns.Add(new TableColumn { Width = new GridLength(140) }); // خوانده(طرف مقابل)
            table.Columns.Add(new TableColumn { Width = new GridLength(160) }); // شماره بایگانی
            table.Columns.Add(new TableColumn { Width = new GridLength(120) }); // شعبه رسیدگی کننده

            // ردیف عنوان
            TableRowGroup headerGroup = new TableRowGroup();
            TableRow header = new TableRow();
            header.Cells.Add(new TableCell(new Paragraph(new Run("شماره پرونده"))) { FontWeight = FontWeights.Bold });
            header.Cells.Add(new TableCell(new Paragraph(new Run("نام موکل"))) { FontWeight = FontWeights.Bold });
            header.Cells.Add(new TableCell(new Paragraph(new Run("تاریخ تنظیم"))) { FontWeight = FontWeights.Bold });
            header.Cells.Add(new TableCell(new Paragraph(new Run("عنوان"))) { FontWeight = FontWeights.Bold });
            header.Cells.Add(new TableCell(new Paragraph(new Run("موضوع"))) { FontWeight = FontWeights.Bold });
            header.Cells.Add(new TableCell(new Paragraph(new Run("وضعیت"))) { FontWeight = FontWeights.Bold });
            header.Cells.Add(new TableCell(new Paragraph(new Run("خوانده(طرف مقابل)"))) { FontWeight = FontWeights.Bold });
            header.Cells.Add(new TableCell(new Paragraph(new Run("شماره بایگانی"))) { FontWeight = FontWeights.Bold });
            header.Cells.Add(new TableCell(new Paragraph(new Run("شعبه رسیدگی کننده"))) { FontWeight = FontWeights.Bold });
            headerGroup.Rows.Add(header);
            table.RowGroups.Add(headerGroup);

            // داده‌ها
            TableRowGroup dataGroup = new TableRowGroup();

            foreach (var item in items)
            {
                Dictionary<int, string> titleMappings = new Dictionary<int, string>
                {
                { 0, "جاری" },
                { 1, "مختومه" },
                { 2, "معلق"}
                };

                string serviceText = titleMappings.ContainsKey(item.CaseStatus)
                      ? titleMappings[item.CaseStatus]
                      : "خدمات نامشخص";

                TableRow row = new TableRow();
                row.Cells.Add(new TableCell(new Paragraph(new Run(item.CaseNumber))));
                row.Cells.Add(new TableCell(new Paragraph(new Run(item.ClientName))));
                row.Cells.Add(new TableCell(new Paragraph(new Run(item.SetDate))));
                row.Cells.Add(new TableCell(new Paragraph(new Run(item.CaseTitle))));
                row.Cells.Add(new TableCell(new Paragraph(new Run(item.CaseSubject))));
                row.Cells.Add(new TableCell(new Paragraph(new Run(serviceText))));
                row.Cells.Add(new TableCell(new Paragraph(new Run(item.OpponentPerson))));
                row.Cells.Add(new TableCell(new Paragraph(new Run(item.CaseArchiveNumber))));
                row.Cells.Add(new TableCell(new Paragraph(new Run(item.ProcessingBranch))));
                dataGroup.Rows.Add(row);
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

            var ActiveCases = items
            .Where(c => c.StatusText.Contains("در حال رسیدگی"))
            .ToList();

            var ClosedCases = items
           .Where(c => c.StatusText.Contains("مختومه"))
           .ToList();

            var SuspendCases = items
           .Where(c => c.StatusText.Contains("معلق"))
           .ToList();

            int ActiveCasesRecordCount = ActiveCases.Count();
            int ClosedCasesRecordCount = ClosedCases.Count();
            int SuspendCasesRecordCount = SuspendCases.Count();

            Run dateReportText;
            if (!string.IsNullOrWhiteSpace(CasesStartDateDP.Text) && !string.IsNullOrWhiteSpace(CasesEndDateDP.Text))
            {
                dateReportText = new Run($"گزارش بازه زمانی {CasesStartDateDP.Text} الی {CasesEndDateDP.Text}\n")
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

            Run totalActiveCasesText = new Run("تعداد پرونده های جاری: ")
            {
                Foreground = Brushes.Black
            };
            Run totalActiveCasesValue = new Run($"{ActiveCasesRecordCount} \n")
            {
                Foreground = Brushes.DarkRed
            };

            Run totalClosedCasesText = new Run("تعداد پرونده های مختومه: ")
            {
                Foreground = Brushes.Black
            };
            Run totalClosedCasesValue = new Run($"{ClosedCasesRecordCount} \n")
            {
                Foreground = Brushes.DarkRed
            };

            Run totalSuspendCasesText = new Run("تعداد پرونده های معلق: ")
            {
                Foreground = Brushes.Black
            };
            Run totalSuspendCasesValue = new Run($"{SuspendCasesRecordCount} \n")
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

            totalAmountParagraph.Inlines.Add(totalActiveCasesText);
            totalAmountParagraph.Inlines.Add(totalActiveCasesValue);
            totalAmountParagraph.Inlines.Add(totalClosedCasesText);
            totalAmountParagraph.Inlines.Add(totalClosedCasesValue);
            totalAmountParagraph.Inlines.Add(totalSuspendCasesText);
            totalAmountParagraph.Inlines.Add(totalSuspendCasesValue);
            totalAmountParagraph.Inlines.Add(totalRecordsText);
            totalAmountParagraph.Inlines.Add(totalRecordsValue);
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
                    //Visibility = Visibility.Collapsed,
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
            PrintFlowDocumentCase(doc);
        }
        private void PrintFlowDocumentCase(FlowDocument doc)
        {
            PrintDialog printDialog = new PrintDialog();
            if (printDialog.ShowDialog() == true)
            {
                IDocumentPaginatorSource idpSource = doc;
                printDialog.PrintDocument(idpSource.DocumentPaginator, "گزارش پرونده ها");
            }
        }
        #endregion

        private void CloseBTN_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }        
    }  
}