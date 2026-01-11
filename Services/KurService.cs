using Newtonsoft.Json;
using odev3son.Models;
using System.Globalization;

namespace odev3son.Services
{
    public static class KurService
    {
        private const string TRUNCGIL_API = "https://finans.truncgil.com/today.json";
        private const string BINANCE_API = "https://api.binance.com/api/v3/ticker/price";

        // Truncgil Yanıt Modeli (Genişletildi)
        private class TruncgilResponse
        {
            [JsonProperty("Update_Date")] public string Update_Date { get; set; }

            // ANA DÖVİZLER
            [JsonProperty("USD")] public CurrencyItem USD { get; set; }
            [JsonProperty("EUR")] public CurrencyItem EUR { get; set; }
            [JsonProperty("GBP")] public CurrencyItem GBP { get; set; }
            [JsonProperty("CHF")] public CurrencyItem CHF { get; set; } // İsviçre Frangı
            [JsonProperty("CAD")] public CurrencyItem CAD { get; set; } // Kanada Doları
            [JsonProperty("JPY")] public CurrencyItem JPY { get; set; } // Japon Yeni
            [JsonProperty("RUB")] public CurrencyItem RUB { get; set; } // Rus Rublesi
            [JsonProperty("AUD")] public CurrencyItem AUD { get; set; } // Avustralya Doları

            // ALTIN TÜRLERİ
            [JsonProperty("gram-altin")] public CurrencyItem gram_altin { get; set; }
            [JsonProperty("ceyrek-altin")] public CurrencyItem ceyrek_altin { get; set; }
            [JsonProperty("yarim-altin")] public CurrencyItem yarim_altin { get; set; }
            [JsonProperty("tam-altin")] public CurrencyItem tam_altin { get; set; }
            [JsonProperty("cumhuriyet-altini")] public CurrencyItem cumhuriyet_altini { get; set; }
            [JsonProperty("22-ayar-bilezik")] public CurrencyItem bilezik_22 { get; set; }
            [JsonProperty("gumus")] public CurrencyItem gumus { get; set; }
        }

        private class CurrencyItem
        {
            [JsonProperty("Alış")] public string Buying { get; set; }
            [JsonProperty("Satış")] public string Selling { get; set; }
            [JsonProperty("Değişim")] public string Change { get; set; }
        }

        private class BinancePrice
        {
            public string symbol { get; set; }
            public string price { get; set; }
        }

        public static async Task<(string Tarih, List<Currency> Kurlar)> GetKurlarAsync()
        {
            var list = new List<Currency>();
            string tarih = DateTime.Now.ToString("dd.MM.yyyy HH:mm");
            double usdRate = 34.0;

            using var client = new HttpClient();
            client.Timeout = TimeSpan.FromSeconds(10);
            client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0");
            //------------------------------------------------
            // 1. TRUNCGIL VERİLERİ
            try
            {
                var json = await client.GetStringAsync(TRUNCGIL_API);
                var data = JsonConvert.DeserializeObject<TruncgilResponse>(json);

                if (data != null)
                {
                    tarih = data.Update_Date ?? tarih;

                    // USD Kurunu al
                    if (data.USD != null)
                        double.TryParse(data.USD.Selling.Replace(".", "").Replace(",", "."), NumberStyles.Any, CultureInfo.InvariantCulture, out usdRate);

                    // DÖVİZLERİ EKLE
                    AddCurrency(list, data.USD, "USD", "ABD DOLARI");
                    AddCurrency(list, data.EUR, "EUR", "EURO");
                    AddCurrency(list, data.GBP, "GBP", "İNGİLİZ STERLİNİ");
                    AddCurrency(list, data.CHF, "CHF", "İSVİÇRE FRANGI");
                    AddCurrency(list, data.CAD, "CAD", "KANADA DOLARI");
                    AddCurrency(list, data.JPY, "JPY", "JAPON YENİ");
                    AddCurrency(list, data.AUD, "AUD", "AVUSTRALYA DOLARI");
                    AddCurrency(list, data.RUB, "RUB", "RUS RUBLESİ");

                    // ALTINLARI EKLE
                    AddCurrency(list, data.gram_altin, "GAU", "GRAM ALTIN");
                    AddCurrency(list, data.ceyrek_altin, "CAU", "ÇEYREK ALTIN");
                    AddCurrency(list, data.yarim_altin, "YAU", "YARIM ALTIN");
                    AddCurrency(list, data.tam_altin, "TAU", "TAM ALTIN");
                    AddCurrency(list, data.cumhuriyet_altini, "CUM", "CUMHURİYET ALTINI");
                    AddCurrency(list, data.bilezik_22, "22A", "22 AYAR BİLEZİK");
                    AddCurrency(list, data.gumus, "GUM", "GÜMÜŞ");
                }
            }
            catch (Exception ex) { Console.WriteLine($"Truncgil Hatası: {ex.Message}"); }

            // 2. KRİPTO VERİLERİ (BINANCE)
            try
            {
                // BTC
                var btcJson = await client.GetStringAsync($"{BINANCE_API}?symbol=BTCUSDT");
                var btcData = JsonConvert.DeserializeObject<BinancePrice>(btcJson);
                if (btcData != null)
                {
                    double priceUsd = double.Parse(btcData.price, CultureInfo.InvariantCulture);
                    double priceTry = priceUsd * usdRate;

                    list.Add(new Currency
                    {
                        Kod = "BTC",
                        Isim = "BITCOIN",
                        ForexBuying = priceTry.ToString("N2", new CultureInfo("tr-TR")),
                        ForexSelling = priceTry.ToString("N2", new CultureInfo("tr-TR")),
                        Degisim = "%0.00"
                    });
                }

                // ETH
                var ethJson = await client.GetStringAsync($"{BINANCE_API}?symbol=ETHUSDT");
                var ethData = JsonConvert.DeserializeObject<BinancePrice>(ethJson);
                if (ethData != null)
                {
                    double priceUsd = double.Parse(ethData.price, CultureInfo.InvariantCulture);
                    double priceTry = priceUsd * usdRate;

                    list.Add(new Currency
                    {
                        Kod = "ETH",
                        Isim = "ETHEREUM",
                        ForexBuying = priceTry.ToString("N2", new CultureInfo("tr-TR")),
                        ForexSelling = priceTry.ToString("N2", new CultureInfo("tr-TR")),
                        Degisim = "%0.00"
                    });
                }
            }
            catch (Exception ex) { Console.WriteLine($"Binance Hatası: {ex.Message}"); }

            return (tarih, list);
        }

        private static void AddCurrency(List<Currency> list, CurrencyItem item, string kod, string isim)
        {
            if (item != null)
            {
                list.Add(new Currency
                {
                    Kod = kod,
                    Isim = isim,
                    ForexBuying = item.Buying,
                    ForexSelling = item.Selling,
                    Degisim = item.Change
                });
            }
        }
    }
}