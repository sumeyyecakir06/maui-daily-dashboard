using Newtonsoft.Json;
using odev3son.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace odev3son.Services
{
    
    /// Haber servisi - TRT Haber RSS kaynaklarından haberleri çeker
   
    public static class HaberService
    {
        private static readonly HttpClient _httpClient = new HttpClient
        {
            Timeout = TimeSpan.FromSeconds(15) // Timeout ayarı
        };
        //--------------------
        // TRT Haber RSS kategorileri
        private static readonly Dictionary<string, string> _rssUrls = new()
        {
            { "Manşet", "https://www.trthaber.com/manset_articles.rss" },
            { "Son Dakika", "https://www.trthaber.com/sondakika_articles.rss" },
            { "Gündem", "https://www.trthaber.com/gundem_articles.rss" },
            { "Ekonomi", "https://www.trthaber.com/ekonomi_articles.rss" },
            { "Spor", "https://www.trthaber.com/spor_articles.rss" },
            { "Bilim/Teknoloji", "https://www.trthaber.com/bilim_teknoloji_articles.rss" },
            { "Güncel", "https://www.trthaber.com/guncel_articles.rss" }
        };

        
        /// Mevcut kategorileri döndürür
        
        public static List<string> GetKategoriler()
        {
            return _rssUrls.Keys.ToList();
        }

        
        /// Belirtilen kategoriden haberleri çeker
       
        /// <param name="kategori">Haber kategorisi</param>
        /// <returns>Haber listesi</returns>
        public static async Task<List<HaberItem>> GetHaberler(string kategori)
        {
            try
            {
                if (string.IsNullOrEmpty(kategori) || !_rssUrls.ContainsKey(kategori))
                {
                    throw new ArgumentException("Geçersiz kategori");
                }

                var rssUrl = _rssUrls[kategori];
                var apiUrl = $"https://api.rss2json.com/v1/api.json?rss_url={Uri.EscapeDataString(rssUrl)}";

                var response = await _httpClient.GetStringAsync(apiUrl);
                var result = JsonConvert.DeserializeObject<RssResult>(response);

                if (result?.Status != "ok")
                {
                    throw new Exception("RSS servisi hatası");
                }

                if (result?.Items == null || result.Items.Count == 0)
                {
                    return new List<HaberItem>();
                }

                var haberler = new List<HaberItem>();
                foreach (var item in result.Items.Take(20)) // İlk 20 haberi al
                {
                    haberler.Add(new HaberItem
                    {
                        Title = System.Net.WebUtility.HtmlDecode(item.Title ?? "Başlık yok"),
                        Description = item.Description ?? "",
                        Link = item.Link ?? "",
                        PubDate = item.PubDate,
                        Category = kategori,
                        ImageUrl = item.Thumbnail ?? ""
                    });
                }

                return haberler;
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"İnternet bağlantı hatası: {ex.Message}");
                throw new Exception("İnternet bağlantısı kontrol edilemedi");
            }
            catch (TaskCanceledException)
            {
                Console.WriteLine("İstek zaman aşımına uğradı");
                throw new Exception("İstek zaman aşımına uğradı");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Haber yükleme hatası: {ex.Message}");
                throw;
            }
        }

        #region RSS JSON Modelleri
        private class RssResult
        {
            [JsonProperty("status")]
            public string Status { get; set; }

            [JsonProperty("items")]
            public List<RssItem> Items { get; set; }
        }

        private class RssItem
        {
            [JsonProperty("title")]
            public string Title { get; set; }

            [JsonProperty("description")]
            public string Description { get; set; }

            [JsonProperty("link")]
            public string Link { get; set; }

            [JsonProperty("pubDate")]
            public DateTime PubDate { get; set; }

            [JsonProperty("thumbnail")]
            public string Thumbnail { get; set; }
        }
        #endregion
    }
}