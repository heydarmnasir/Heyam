using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace PL
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public void ChangeLanguage(string lang)
        {
            ResourceDictionary dict = new ResourceDictionary();
            switch (lang)
            {
                case "fa":
                    dict.Source = new Uri("Resources/Languages/Lang.fa.xaml", UriKind.Relative);                    
                    break;
                default:
                    dict.Source = new Uri("Resources/Languages/Lang.en.xaml", UriKind.Relative);
                    break;
            }

            // جایگزینی دیکشنری زبان
            //Application.Current.Resources.MergedDictionaries.Clear();
            Application.Current.Resources.MergedDictionaries.Add(dict);

            // بروزرسانی متن کنترل‌ها
            foreach (Window window in Application.Current.Windows)
            {
                foreach (var control in FindVisualChildren<TextBlock>(window))
                {
                    if (control.Tag != null)
                        control.Text = (string)Application.Current.Resources[control.Tag.ToString()];
                }
            }

            // ذخیره زبان انتخاب‌شده
            Heyam.Properties.Settings.Default.Language = lang;
            Heyam.Properties.Settings.Default.Save();
        }

        // متد کمکی برای یافتن کنترل‌ها در کل فرم
        private static IEnumerable<T> FindVisualChildren<T>(DependencyObject depObj) where T : DependencyObject
        {
            if (depObj != null)
            {
                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
                {
                    DependencyObject child = VisualTreeHelper.GetChild(depObj, i);
                    if (child != null && child is T t)
                    {
                        yield return t;
                    }

                    foreach (T childOfChild in FindVisualChildren<T>(child))
                    {
                        yield return childOfChild;
                    }
                }
            }
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            string savedLanguage = Heyam.Properties.Settings.Default.Language;
            ChangeLanguage(string.IsNullOrEmpty(savedLanguage) ? "fa" : savedLanguage);
        }      
    }
}
