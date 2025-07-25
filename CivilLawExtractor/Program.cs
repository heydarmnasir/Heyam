using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using HtmlAgilityPack;
using Newtonsoft.Json;

namespace CivilLawExtractor
{
    public class LegalArticle
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Text { get; set; }
    }
    class Program
    {
        static void Main()
        {
            var url = "https://vakil.net/متن-کامل-قانون-مدنی/";
            var web = new HtmlWeb();
            var doc = web.Load(url);

            var articles = new List<LegalArticle>();
            LegalArticle current = null;

            foreach (var p in doc.DocumentNode.SelectNodes("//p"))
            {
                var txt = p.InnerText.Trim();
                var m = Regex.Match(txt, @"^ماده\s+(\d+)\s*[–\-]?\s*(.*)");
                if (m.Success)
                {
                    if (current != null) articles.Add(current);
                    int id = int.Parse(m.Groups[1].Value);
                    current = new LegalArticle
                    {
                        Id = id,
                        Title = $"ماده {id}",
                        Text = m.Groups[2].Value.Trim()
                    };
                }
                else if (current != null && !string.IsNullOrWhiteSpace(txt))
                {
                    current.Text += "\n" + txt;
                }
            }

            if (current != null) articles.Add(current);

            File.WriteAllText("civil_article.json", JsonConvert.SerializeObject(articles, Formatting.Indented));
            Console.WriteLine($"✅ Done: Extracted {articles.Count} مواد قانونی!");
        }
    }
}
