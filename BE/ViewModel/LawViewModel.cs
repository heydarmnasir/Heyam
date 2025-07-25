using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.IO;
using static BE.ViewModel.ArticleDto;

namespace BE.ViewModel
{
    public class LawViewModel : INotifyPropertyChanged
    {

        #region Law_Articles
        public ObservableCollection<ArticleDto> AllArticles { get; set; } = new ObservableCollection<ArticleDto>();
        private ObservableCollection<ArticleDto> _filteredArticles = new ObservableCollection<ArticleDto>();
        public ObservableCollection<PdfFile> PdfFiles { get; set; } = new ObservableCollection<PdfFile>();
        public ObservableCollection<PdfFile> ArticlePdfFiles { get; set; } = new ObservableCollection<PdfFile>();

        public ObservableCollection<ArticleDto> FilteredArticles
        {
            get => _filteredArticles;
            set
            {
                _filteredArticles = value;
                OnPropertyChanged(nameof(FilteredArticles));
            }
        }

        private string _searchText;
        public string SearchText
        {
            get => _searchText;
            set
            {
                if (_searchText != value)
                {
                    _searchText = value;
                    OnPropertyChanged(nameof(SearchText));
                    SearchArticles();
                }
            }
        }

        private ArticleDto _selectedArticle;
        public ArticleDto SelectedArticle
        {
            get => _selectedArticle;
            set
            {
                _selectedArticle = value;
                if (_selectedArticle != null)
                {
                    // فرض کنیم SelectedLawInfo.Name نام قانون رو مشخص می‌کنه
                    LoadUserNotes(SelectedLawInfo.Name, _selectedArticle);
                }
                OnPropertyChanged(nameof(SelectedArticle));
                OnPropertyChanged(nameof(Title));
                OnPropertyChanged(nameof(Text));
                OnPropertyChanged(nameof(NotesText));
                OnPropertyChanged(nameof(UserNotesText));
            }
        }

        // خروجی‌های متن ماده و یادداشت
        public string Title => SelectedArticle?.Title ?? "";
        public string Text => SelectedArticle?.Text ?? "";
        public string NotesText => SelectedArticle != null ? ConvertNotesToText(SelectedArticle.Notes) : "";
        public string UserNotesText => SelectedArticle?.UserNotesText ?? "";

        // اطلاعات کلی قانون (نام، تاریخ تصویب و نوع سند)
        private LawInfo _selectedLawInfo = new LawInfo();
        public LawInfo SelectedLawInfo
        {
            get => _selectedLawInfo;
            set
            {
                _selectedLawInfo = value;
                OnPropertyChanged(nameof(SelectedLawInfo));
            }
        }

        public void LoadArticlesFromJson(string path)
        {
            var json = File.ReadAllText(path);
            var items = JsonConvert.DeserializeObject<List<ArticleDto>>(json);
            AllArticles.Clear();
            foreach (var a in items)
                AllArticles.Add(a);

            // وقتی قانون جدید لود شد، سرچ ریست میشه
            SearchArticles();
            // وقتی قانون عوض میشه، انتخاب ماده هم پاک شه
            SelectedArticle = null;
        }

        // تنظیم مشخصات قانون هنگام تغییر نوع قانون
        public void SetLawInfo(int lawTypeIndex)
        {
            switch (lawTypeIndex)
            {
                case 0:
                    SelectedLawInfo.Name = "قانون اساسی";
                    SelectedLawInfo.ApprovalDate = "۱۳۵۸/۰۹/۱۲ بازنگری شده: ۱۳۶۸/۰۵/۰۶";
                    SelectedLawInfo.DocumentType = "قانون";
                    break;
                case 1:
                    SelectedLawInfo.Name = "قانون مدنی";
                    SelectedLawInfo.ApprovalDate = "جلد اول: 1307/02/18 - جلد دوم: ۱۳۱۴/۰۱/۲۰ - جلد سوم: 1314";
                    SelectedLawInfo.DocumentType = "قانون";
                    break;
                case 2:
                    SelectedLawInfo.Name = "آیین دادرسی مدنی";
                    SelectedLawInfo.ApprovalDate = "۱۳۷۹/۰۱/۲۱ آخرین اصلاحات: ۱۳۹۴/۱۲/۱۸";
                    SelectedLawInfo.DocumentType = "قانون";
                    break;
                case 3:
                    SelectedLawInfo.Name = "قانون اجرای احکام مدنی";
                    SelectedLawInfo.ApprovalDate = "۱۳۵۶/۰۸/۰۱ آخرین اصلاحات تا تاریخ ۱۳۹۴/۱۱/۱۲";
                    SelectedLawInfo.DocumentType = "قانون";
                    break;
                case 4:
                    SelectedLawInfo.Name = "قانون تجارت";
                    SelectedLawInfo.ApprovalDate = "۱۳ اردیبهشت ماه ۱۳۱۱ شمسی (کمیسیون قوانین عدلیه) به روز رسانی در ۱۴۰۱/۰۲/۰۲";
                    SelectedLawInfo.DocumentType = "قانون";
                    break;
                case 5:
                    SelectedLawInfo.Name = "آیین دادرسی کیفری";
                    SelectedLawInfo.ApprovalDate = "۱۳۹۲/۱۲/۴ با آخرین اصلاحات تا تاریخ ۱۳۹۵/۱۱/۱۰";
                    SelectedLawInfo.DocumentType = "قانون";
                    break;
                case 6:
                    SelectedLawInfo.Name = "قانون مجازات اسلامی مصوب 1375";
                    SelectedLawInfo.ApprovalDate = "۱۳۷۵/۰۳/۰۲ با اصلاحات تا ۱۴۰۱/۱۲/۱۳";
                    SelectedLawInfo.DocumentType = "قانون";
                    break;
                case 7:
                    SelectedLawInfo.Name = "قانون مجازات اسلامی مصوب 1392";
                    SelectedLawInfo.ApprovalDate = "۱۳۹۲/۰۲/۰۱";
                    SelectedLawInfo.DocumentType = "قانون";
                    break;
                case 8:
                    SelectedLawInfo.Name = "قانون کاهش مجازات حبس تعزیری";
                    SelectedLawInfo.ApprovalDate = "۱۳۹۹/۲/۲۳";
                    SelectedLawInfo.DocumentType = "قانون";
                    break;
                case 9:
                    SelectedLawInfo.Name = "قانون مبارزه با قاچاق کالا و ارز";
                    SelectedLawInfo.ApprovalDate = "۱۴۰۰/۱۱/۱۰";
                    SelectedLawInfo.DocumentType = "قانون";
                    break;
                case 10:
                    SelectedLawInfo.Name = "قانون مجازات اسید پاشی";
                    SelectedLawInfo.ApprovalDate = "۱۳۳۷/۱۱/۱۹";
                    SelectedLawInfo.DocumentType = "قانون";
                    break;
                case 11:
                    SelectedLawInfo.Name = "قانون حمایت خانواده";
                    SelectedLawInfo.ApprovalDate = "۱۳۹۱/۱۲/۱";
                    SelectedLawInfo.DocumentType = "قانون";
                    break;
                case 12:
                    SelectedLawInfo.Name = "قانون جرم سیاسی";
                    SelectedLawInfo.ApprovalDate = "۱۳۹۵/۰۲/۲۰";
                    SelectedLawInfo.DocumentType = "قانون";
                    break;
                case 13:
                    SelectedLawInfo.Name = "قانون دفتر اسناد";
                    SelectedLawInfo.ApprovalDate = "۱۳۵۴/۰۴/۲۵ با آخرین اصلاحات تا تاریخ ۱۳۷۵/۰۳/۰۶";
                    SelectedLawInfo.DocumentType = "قانون";
                    break;
                case 14:
                    SelectedLawInfo.Name = "قانون کار";
                    SelectedLawInfo.ApprovalDate = "۱۳۶۹/۰۸/۲۹";
                    SelectedLawInfo.DocumentType = "قانون";
                    break;
                case 15:
                    SelectedLawInfo.Name = "قانون بیمه";
                    SelectedLawInfo.ApprovalDate = "۱۳۱۶/۰۲/۰۷";
                    SelectedLawInfo.DocumentType = "قانون";
                    break;
                case 16:
                    SelectedLawInfo.Name = "قانون صدور چک";
                    SelectedLawInfo.ApprovalDate = "۱۳۵۵/۰۴/۱۶ با آخرین اصلاحات و الحاقات تا تاریخ ۱۴۰۰/۰۱/۲۹";
                    SelectedLawInfo.DocumentType = "قانون";
                    break;
                case 17:
                    SelectedLawInfo.Name = "قانون چک تضمین شده";
                    SelectedLawInfo.ApprovalDate = "۱۳۳۷/۴/۲۲";
                    SelectedLawInfo.DocumentType = "قانون";
                    break;
                case 18:
                    SelectedLawInfo.Name = "قانون ثبت احوال";
                    SelectedLawInfo.ApprovalDate = "۱۳۵۵/۴/۱۶ مجلس شورای ملی با آخرین اصلاحات تا تاریخ ۱۴۰۱/۸/۱۰";
                    SelectedLawInfo.DocumentType = "قانون";
                    break;
                case 19:
                    SelectedLawInfo.Name = "قانون شوراهای حل اختلاف";
                    SelectedLawInfo.ApprovalDate = "۱۴۰۲/۶/۲۲";
                    SelectedLawInfo.DocumentType = "قانون";
                    break;
                case 20:
                    SelectedLawInfo.Name = "قانون وکالت";
                    SelectedLawInfo.ApprovalDate = "۱۳۱۵/۱۱/۲۵";
                    SelectedLawInfo.DocumentType = "قانون";
                    break;
                case 21:
                    SelectedLawInfo.Name = "قانون تملک آپارتمان";
                    SelectedLawInfo.ApprovalDate = "۱۳۴۳/۱۲/۱۶ با آخرین اصلاحات تا تاریخ ۱۳۷۶/۳/۱۱";
                    SelectedLawInfo.DocumentType = "قانون";
                    break;
                case 22:
                    SelectedLawInfo.Name = "قانون پیش فروش ساختمان";
                    SelectedLawInfo.ApprovalDate = "۱۳۸۹/۱۰/۱۲";
                    SelectedLawInfo.DocumentType = "قانون";
                    break;
                case 23:
                    SelectedLawInfo.Name = "قانون مؤجر و مستأجر";
                    SelectedLawInfo.ApprovalDate = "۱۳۷۶";
                    SelectedLawInfo.DocumentType = "قانون";
                    break;
                case 24:
                    SelectedLawInfo.Name = "قانون تصدیق انحصار وراثت";
                    SelectedLawInfo.ApprovalDate = "۱۳۰۹/۷/۱۴ با آخرین اصلاحات تا تاریخ ۱۳۱۳/۹/۱";
                    SelectedLawInfo.DocumentType = "قانون";
                    break;
                case 25:
                    SelectedLawInfo.Name = "قانون نقل و انتقال قضات";
                    SelectedLawInfo.ApprovalDate = "۱۳۷۵/۱۱/۲۸ با آخرین اصلاحات تا تاریخ ۱۳۷۹/۱۲/۰۲";
                    SelectedLawInfo.DocumentType = "قانون";
                    break;
                case 26:
                    SelectedLawInfo.Name = "قانون ورود و اقامت اتباع خارجه";
                    SelectedLawInfo.ApprovalDate = "۱۳۱۰/۲/۱۹";
                    SelectedLawInfo.DocumentType = "قانون";
                    break;
                case 27:
                    SelectedLawInfo.Name = "قانون اموال غیر منقول اتباع خارجه";
                    SelectedLawInfo.ApprovalDate = "۱۳۱۰/۳/۱۶";
                    SelectedLawInfo.DocumentType = "قانون";
                    break;
                case 28:
                    SelectedLawInfo.Name = "قانون الزام به ثبت رسمی معاملات اموال غیر منقول";
                    SelectedLawInfo.ApprovalDate = "مصوب 1401/9/6 مجلس و 1403/2/26 مجمع تشخیص مصلحت نظام";
                    SelectedLawInfo.DocumentType = "قانون";
                    break;
                case 29:
                    SelectedLawInfo.Name = "قانون مجازات انتقال مال غیر";
                    SelectedLawInfo.ApprovalDate = "۱۳۰۸/۰۱/۰۵";
                    SelectedLawInfo.DocumentType = "قانون";
                    break;
                case 30:
                    SelectedLawInfo.Name = "قانون مجازات استفاده غیر مجاز آب، برق و گاز";
                    SelectedLawInfo.ApprovalDate = "۱۳۹۶/۳/۱۰";
                    SelectedLawInfo.DocumentType = "قانون";
                    break;
                case 31:
                    SelectedLawInfo.Name = "قانون نظارت بر رفتار قضات";
                    SelectedLawInfo.ApprovalDate = "۱۳۹۰/۰۷/۱۷";
                    SelectedLawInfo.DocumentType = "قانون";
                    break;
                case 32:
                    SelectedLawInfo.Name = "قانون ثبت شرکت";
                    SelectedLawInfo.ApprovalDate = "۱۳۱۰/۰۳/۰۲";
                    SelectedLawInfo.DocumentType = "قانون";
                    break;
                case 33:
                    SelectedLawInfo.Name = "قانون دریایی";
                    SelectedLawInfo.ApprovalDate = "۱۳۴۳/۶/۲۹ با اصلاحات و الحاقات ۱۳۹۱/۸/۱۶";
                    SelectedLawInfo.DocumentType = "قانون";
                    break;
                case 34:
                    SelectedLawInfo.Name = "قانون صید و شکار";
                    SelectedLawInfo.ApprovalDate = "۱۳۴۶/۰۳/۱۶ با آخرین اصلاحات تا ۱۳۹۷/۲/۱۶";
                    SelectedLawInfo.DocumentType = "قانون";
                    break;
                case 35:
                    SelectedLawInfo.Name = "قانون امور گمرکی";
                    SelectedLawInfo.ApprovalDate = "۱۳۹۰/۰۸/۲۲ با آخرین اصلاحات تا تاریخ ۱۴۰۱/۲/۱۱";
                    SelectedLawInfo.DocumentType = "قانون";
                    break;
                case 36:
                    SelectedLawInfo.Name = "قانون صادرات و واردات";
                    SelectedLawInfo.ApprovalDate = "مصوب ۱۳۷۲/۰۷/۰۴ با آخرین اصلاحات تا تاریخ ۱۳۸۷/۰۴/۱۲";
                    SelectedLawInfo.DocumentType = "قانون";
                    break;
                case 37:
                    SelectedLawInfo.Name = "اصلاح آیین دادرسی کیفری";
                    SelectedLawInfo.ApprovalDate = "۱۳۹۴";
                    SelectedLawInfo.DocumentType = "قانون";
                    break;
                case 38:
                    SelectedLawInfo.Name = "اصلاح صدور چک";
                    SelectedLawInfo.ApprovalDate = "۱۴۰۰/۰۱/۲۹";
                    SelectedLawInfo.DocumentType = "قانون";
                    break;
                case 39:
                    SelectedLawInfo.Name = "اصلاح قانون مجازات قاچاق اسلحه";
                    SelectedLawInfo.ApprovalDate = "۱۴۰۳/۱۲/۲۲";
                    SelectedLawInfo.DocumentType = "قانون";
                    break;
                case 40:
                    SelectedLawInfo.Name = "اصلاح قانون نظام صنفی";
                    SelectedLawInfo.ApprovalDate = "۱۳۸۲/۱۲/۲۴";
                    SelectedLawInfo.DocumentType = "قانون";
                    break;
                case 41:
                    SelectedLawInfo.Name = "اصلاح مبارزه با پولشویی";
                    SelectedLawInfo.ApprovalDate = "۱۳۹۷/۷/۳ مجلس شورای اسلامی و ۱۳۹۷/۱۰/۱۵ سوی مجمع تشخیص مصلحت نظام";
                    SelectedLawInfo.DocumentType = "قانون";
                    break;
                case 42:
                    SelectedLawInfo.Name = "اصلاح ماده 104 قانون مجازات اسلامی";
                    SelectedLawInfo.ApprovalDate = "۱۴۰۳/۲/۲۵";
                    SelectedLawInfo.DocumentType = "قانون";
                    break;
                default:
                    SelectedLawInfo.Name = "";
                    SelectedLawInfo.ApprovalDate = "";
                    SelectedLawInfo.DocumentType = "";
                    break;
            }
        }

        // تبدیل Notes به متن قابل خواندن
        private string ConvertNotesToText(object notes)
        {
            if (notes == null)
                return "";

            if (notes is JArray jArray)
            {
                return string.Join(Environment.NewLine, jArray.ToObject<List<string>>());
            }
            else if (notes is JObject jObject)
            {
                return string.Join(Environment.NewLine, jObject.Properties().Select(p => $"{p.Name}: {p.Value}"));
            }
            else if (notes is string s)
            {
                return s;
            }

            return notes.ToString();
        }

        //یادداشت های شخصی 
        private string GetNotesDirectory(string lawName)
        {
            var folder = Path.Combine("D:\\Privet\\Projects\\Heyam\\PL\\Resources\\Laws_Articles\\ArticlePersonalNotes", lawName);
            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder);
            return folder;
        }
        private string GetNotesFilePath(string lawName, decimal articleId)
        {
            var folder = GetNotesDirectory(lawName);
            return Path.Combine(folder, $"Article_{articleId}.json");
        }
        public void LoadUserNotes(string lawName, ArticleDto article)
        {
            if (article != null)
            {
                var path = GetNotesFilePath(lawName, article.Id);
                if (File.Exists(path))
                {
                    var json = File.ReadAllText(path);
                    article.UserNotes = JsonConvert.DeserializeObject<List<string>>(json);
                }
                else
                {
                    article.UserNotes = new List<string>();
                }
            }
        }
        public string SaveUserNotes(string lawName, ArticleDto article)
        {
            try
            {
                var path = GetNotesFilePath(lawName, article.Id);
                var json = JsonConvert.SerializeObject(article.UserNotes ?? new List<string>(), Formatting.Indented);
                File.WriteAllText(path, json);
            }
            catch (Exception ex)
            {
                return ($"خطا در ذخیره یادداشت: {ex.Message}");
            }
            return "یادداشت ذخیره شد";
        }
        public string UpdateUserNotes(string newNotesText)
        {
            if (SelectedArticle == null) return "ماده ای انتخاب کنید!";

            SelectedArticle.UserNotes = newNotesText
                .Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries)
                .ToList();

            SaveUserNotes(SelectedLawInfo.Name, SelectedArticle);
            OnPropertyChanged(nameof(UserNotesText));
            return "یادداشت ویرایش شد";
        }

        //تاریخچه
        public ObservableCollection<HistoryItem> BrowseHistory { get; set; } = new ObservableCollection<HistoryItem>();

        private HistoryItem _selectedHistoryItem;
        public HistoryItem SelectedHistoryItem
        {
            get => _selectedHistoryItem;
            set
            {
                if (_selectedHistoryItem != value)
                {
                    _selectedHistoryItem = value;
                    OnPropertyChanged(nameof(SelectedHistoryItem));
                }
            }
        }
        public void AddToHistory(string lawTitle, string articleTitle)
        {
            var item = new HistoryItem
            {
                LawTitle = lawTitle,
                ArticleTitle = articleTitle,
            };

            BrowseHistory.Insert(0, item);  // اضافه شدن به ابتدای لیست
            if (BrowseHistory.Count > 5)   // محدود کردن تاریخچه به ۲۰ مورد آخر
                BrowseHistory.RemoveAt(BrowseHistory.Count - 1);
        }

        //جستجو       
        private void SearchArticles()
        {
            if (string.IsNullOrWhiteSpace(SearchText))
            {
                FilteredArticles = new ObservableCollection<ArticleDto>(AllArticles);
            }
            else
            {
                var lowerSearch = SearchText.ToLower();
                var filtered = AllArticles.Where(a =>
                    (!string.IsNullOrWhiteSpace(a.Title) && a.Title.ToLower().Contains(lowerSearch)) ||
                    (!string.IsNullOrWhiteSpace(a.Text) && a.Text.ToLower().Contains(lowerSearch))
                );

                FilteredArticles = new ObservableCollection<ArticleDto>(filtered);
            }
        }
        #endregion

        #region PdfFiles
        public void LoadPdfFiles()
        {
            #region فایل های حقوقی
            string[] pdfNames = { "قانون اساسی جمهوری اسلامی ایران.pdf", "قانون مدنی جمهوری اسلامی ایران.pdf", "قانون اجرای احکام مدنی.pdf", "قانون تجارت.pdf", "قانون تجارت الکترونیکی.pdf", "قانون آیین دادرسی کیفری.pdf", "قانون کار.pdf", "آیین دادرسی کار.pdf", "آيين‌نامه اجرايی قانون بازنشستگی پيش از موعد كاركنان دولت.pdf", "قانون اتباع.pdf", "قانون اداره تصفیه امور ورشکستگی.pdf", "‌قانون اراضی مستحدث و ساحلی.pdf", "‌قانون اراضي شهري.pdf", "قانون اصلاح قانون صدور چک.pdf", "قانون اصلاح قانون ممنوعیت بکارگیری بازنشستگان.pdf", "قانون امور حسبی.pdf", "قانون آیین دادرسی جرائم نیروهای مسلح و دادرسی الکترونیکی.pdf", "قانون آیین دادرسی دادگاههای عمومی و انقلاب ‌در امور مدنی.pdf", "قانون برنامه و بودجه کشور.pdf", "قانون بیمه اجباری شخص ثالث مصوب 1395 همراه با آیین نامه های اجرایی.pdf", "قانون بيمه بيكاري ‌.pdf", "قانون پیشگیری و مقابله با تقلب در تهیه آثار علمی.pdf", "قانون پيش ‌فروش ساختمان.pdf", "قانون تأسيس و اداره مدارس، مراكز آموزشي و مراكز پرورشي غيردولتی.pdf", "‌قانون تأمین اجتماعی.pdf", "قانون تسهيل تسويه بدهي بدهكاران شبكه بانكي كشور.pdf", "قانون تسهيل تنظيم اسناد در دفاتر اسناد رسمي.pdf", "قانون تشدید مجازات اسیدپاشی.pdf", "قانون تشويق و حمايت سرمايه‌ گذاري خارجي.pdf", "قانون تصدیق انحصار وراثت.pdf", "قانون تعیین تکلیف تابعیت فرزندان حاصل از ازدواج زنان ایرانی با مردان غیرایرانی.pdf", "قانون تفسیر چگونگی امهال وام.pdf", "‌قانون ثبت احوال.pdf", "قانون ثبت اسناد و املاک.pdf", "قانون جامع كنترل و مبارزه ملي با دخانيات.pdf", "قانون جرایم رایانه ای.pdf", "قانون جرم سیاسی.pdf", "قانون چک‌های تضمین‌شده.pdf", "قانون حفاظت از خاک.pdf", "قانون حفظ كاربري اراضي زراعي و باغها.pdf", "قانون حمایت از حقوق پدیدآورندگان نرم‌افزارهای رایانه‌ای.pdf", "قانون حمایت از مصرف کنندگان خودرو.pdf", "قانون حمایت خانواده.pdf", "قانون حمايت از توسعه و ايجاد اشتغال پايدار در مناطق روستايي و عشايري.pdf", "قانون حمايت از حقوق مصرف كنندگان.pdf", "قانون حمايت از حقوق معلولان.pdf", "قانون دریایی.pdf", "قانون دیوان عدالت اداری.pdf", "قانون راجع به اموال غیر منقول اتباع خارجی.pdf", "قانون رسیدگی به تخلفات رانندگی.pdf", "‌قانون رسيدگي به تخلفات اداري.pdf", "قانون زمين شهري.pdf", "قانون شرایط انتخاب قضات دادگستری.pdf", "قانون شکار و صید.pdf", "قانون شوراهاي حل اختلاف.pdf", "قانون شهرداری.pdf", "قانون صدور چک.pdf", "قانون گذرنامه.pdf", "قانون کیفیت اخذ پروانه وکالت دادگستری.pdf", "‌قانون كانون كارشناسان رسمي دادگستري.pdf", "قانون ماليات بر ارزش افزوده.pdf", "قانون مبارزه با پولشویی.pdf", "قانون مبارزه با تأمين مالي تروريسم.pdf", "قانون مبارزه با قاچاق انسان.pdf", "قانون مبارزه با قاچاق کالا و ارز با اصلاحات 1394.pdf", "قانون مجازات اخلالگران در نظام اقتصادی کشور.pdf", "قانون مجازات استفاده غیرمجاز از عناوین علمی.pdf", "قانون مجازات استفاده كنندگان غيرمجاز از آب، برق، تلفن، فاضلاب و گاز.pdf", "قانون مجازات اسلامی مصوب 1392.pdf", "قانون مجازات اسلامی.pdf", "قانون مجازات راجع به انتقال مال غیر.pdf", "قانون مجازات قاچاق اسلحه و مهمات و دارندگان سلاح و مهمات غیرمجاز.pdf", "قانون مدیریت خدمات کشوری.pdf", "قانون معافیت وزارتخانه‌ها و مؤسسات دولتی از پرداخت هزینه‌های ثبتی.pdf", "‌قانون مقررات صادرات و واردات.pdf", "قانون نحوه اجرای محکومیت مالی.pdf", "قانون نحوه اهداء جنين به زوجين نابارور.pdf", "قانون نحوه مجازات اشخاصی که در امور  سمعی و بصری فعالیت های غیر مجاز میکنند.pdf", "قانون نحوه واگذاري مالكيت و اداره امور شهرك هاي صنعتی.pdf", "قانون نظارت بر رفتار قضات.pdf", "‌قانون نقل و انتقال دوره‌ای قضات.pdf", "قانون نظام صنفی.pdf", "قانون وظایف و اختیارات رئیس قوه قضائیه.pdf", "قانون هوای پاک.pdf", "قوانین و مقررات دادرسی کیفری در دادگاههای عمومی و انقلاب  .pdf", "مجموعه قوانین خاص حقوقی و کیفری.pdf" }; // لیست فایل‌های PDF داخل Resources
            #endregion
            PdfFiles.Clear();

            foreach (var pdf in pdfNames)
            {
                string resourcePath = $"Heyam.Resources.Legal_Files.Laws.{pdf}";
                PdfFiles.Add(new PdfFile { FileName = pdf, FilePath = resourcePath });
            }
        }                    
        public void LoadArticlePdfFiles()
        {
            #region فایل های حقوقی
            string[] pdfNames = { "ادله پیش زمینه در حقوق ایران و انگلیس.pdf", "بحث تطبیقی سرقت در حقوق ایران آمریکا و کانادا.pdf", "بررسی علم قاضی.pdf", "تعارض تعهدات اصلی و فرعی در قرار داد.pdf", "تعلیق مجازات در حقوق جزا و رویه محاکم ایران.pdf", "چگونه می توانید دکتری و کارشناسی ارشد بگیرید.pdf", "حقوق بشر و تناقض با سیاست های آمریکا.pdf", "حقوق و بالزاک.pdf", "دانلود کتابچه خانواده،جنسیت و حقوق در دانمارک.pdf", "دیوان کیفری بین المللی ؛ پایان یلدای مصونیت بی کیفرمانی.pdf", "رکن مادی کلاهبرداری.pdf", "‌قواعد حاکم بر استجواب کیفری.pdf", "‌ماهیت حقوقی وصیت.pdf", "مبانی نظری تعهدات طبیعی.pdf", "نقد و بررسی دادرسی فوری در حقوق انگلیس.pdf", "نقش قرار داد های خصوصی در نکاح.pdf" }; // لیست فایل‌های PDF داخل Resources
            #endregion
            ArticlePdfFiles.Clear();

            foreach (var pdf in pdfNames)
            {
                string resourcePath = $"Heyam.Resources.Legal_Files.Articles.{pdf}";
                ArticlePdfFiles.Add(new PdfFile { FileName = pdf, FilePath = resourcePath });
            }
        }    
        public class PdfFile
        {
            public string FileName { get; set; }
            public string FilePath { get; set; }
        }        
        public int GetPdfFileCount()
        {
            return PdfFiles.Count;
        }
        public int GetArticlePdfFileCount()
        {
            return ArticlePdfFiles.Count;
        }
        #endregion

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string name) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}