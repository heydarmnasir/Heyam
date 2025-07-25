using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace BE.ViewModel
{   
    public class FlexibleNotesConverter : JsonConverter<List<string>>
    {
        public override List<string> ReadJson(JsonReader reader, Type objectType, List<string> existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            var result = new List<string>();

            var token = JToken.Load(reader);

            switch (token.Type)
            {
                case JTokenType.String:
                    result.Add(token.ToString());
                    break;

                case JTokenType.Array:
                    foreach (var item in token)
                    {
                        result.Add(item.ToString());
                    }
                    break;

                case JTokenType.Object:
                    var obj = token.ToObject<Dictionary<string, string>>();
                    foreach (var kvp in obj)
                    {
                        result.Add($"{kvp.Key}: {kvp.Value}");
                    }
                    break;
            }

            return result;
        }
        public override void WriteJson(JsonWriter writer, List<string> value, JsonSerializer serializer)
        {
            serializer.Serialize(writer, value);
        }
    }
   
    public class ArticleDto
    {
        public decimal Id { get; set; }
        public string Title { get; set; }
        public string Text { get; set; }
       
        [JsonConverter(typeof(FlexibleNotesConverter))]     
        public List<string> Notes { get; set; }
        // این پراپرتی فقط برای نمایش        
        public string NotesText => Notes != null ? string.Join("\n", Notes) : string.Empty;

        // این پراپرتی برای یادداشت‌های شخصی کاربر است
        public List<string> UserNotes { get; set; } = new List<string>();      
        public string UserNotesText => UserNotes != null ? string.Join("\n", UserNotes) : string.Empty;

        public class HistoryItem
        {
            public string LawTitle { get; set; }   // عنوان قانون (مثلاً قانون مدنی)
            public string ArticleTitle { get; set; } // عنوان ماده (مثلاً ماده ۱۰)
            public string ArticleText { get; set; }  // متن کامل ماده

            public override string ToString()
            {
                return $"{LawTitle} - {ArticleTitle}";  // نمایش در ListBox
            }
        }
    }
}