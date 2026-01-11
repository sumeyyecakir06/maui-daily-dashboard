using System;

namespace odev3son.Models
{
    
    /// Haber öğesi modeli - RSS'den gelen haberleri temsil eder
   
    public class HaberItem
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string Link { get; set; }
        public DateTime PubDate { get; set; }
        public string Category { get; set; }
        public string ImageUrl { get; set; }

   
        public string ShortDescription
        {
            get
            {
                if (string.IsNullOrEmpty(Description))
                    return "Açıklama mevcut değil";

                var plainText = System.Net.WebUtility.HtmlDecode(Description);
                // HTML etiketlerini temizle
                plainText = System.Text.RegularExpressions.Regex.Replace(plainText, "<.*?>", string.Empty);

                return plainText.Length > 150
                    ? plainText.Substring(0, 150) + "..."
                    : plainText;
            }
        }

        
        /// Tarih formatı - dd.MM.yyyy HH:mm
        
        public string FormattedDate => PubDate.ToString("dd.MM.yyyy HH:mm");

       
        /// Görsel var mı kontrolü
       
        public bool HasImage => !string.IsNullOrEmpty(ImageUrl);
    }
}