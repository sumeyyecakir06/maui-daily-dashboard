namespace odev3son.Services
{
    public static class HavaDurumuService
    {
        public static string NormalizeCityName(string? cityName)
        {
            if (string.IsNullOrWhiteSpace(cityName))
                return string.Empty;

            var normalized = cityName.ToUpperInvariant();

            // Türkçe karakter dönüşümü
            normalized = normalized
                .Replace("Ç", "C")
                .Replace("Ğ", "G")
                .Replace("İ", "I")
                .Replace("Ö", "O")
                .Replace("Ş", "S")
                .Replace("Ü", "U")
                .Replace("Â", "A")
                .Replace("Î", "I")
                .Replace("Û", "U");

            // Özel durumlar (MGM'nin kullandığı kodlar)
            if (normalized.Contains("KAHRAMANMARAS"))
                normalized = "K.MARAS";
            else if (normalized == "AFYON")
                normalized = "AFYONKARAHISAR";

            // Boşlukları kaldır
            normalized = normalized.Replace(" ", "");

            return normalized;
        }

        public static async Task<bool> ValidateCityName(string cityName)
        {
            try
            {
                var normalized = NormalizeCityName(cityName);
                var url = $"https://www.mgm.gov.tr/sunum/sondurum-show-2.aspx?m={normalized}&rC=111&rZ=fff";

                using var client = new HttpClient();
                client.Timeout = TimeSpan.FromSeconds(10);
                var response = await client.GetAsync(url);
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }

        public static List<string> GetPopularCities()
        {
            return new List<string>
            {
                "İstanbul",
                "Ankara",
                "İzmir",
                "Bursa",
                "Antalya",
                "Adana",
                "Konya",
                "Trabzon",
                "Samsun",
                "Erzurum"
            };
        }
    }
}