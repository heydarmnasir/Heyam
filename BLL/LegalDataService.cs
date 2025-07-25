using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text.Json;

namespace BLL
{
    public class LegalDataService
    {
        public ObservableCollection<BE.ViewModel.ArticleDto> LoadArticlesFromJson(string filePath)
        {
            var json = File.ReadAllText(filePath);
            var articles = JsonSerializer.Deserialize<List<BE.ViewModel.ArticleDto>>(json);
            return new ObservableCollection<BE.ViewModel.ArticleDto>(articles);
        }
    }
}