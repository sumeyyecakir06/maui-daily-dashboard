using System.Globalization;

namespace odev3son.Models
{
    public class Currency
    {
        public string Kod { get; set; }
        public string Isim { get; set; }
        public string CurrencyName { get; set; }
        public string ForexBuying { get; set; }
        public string ForexSelling { get; set; }

        // API'den gelen değişim oranı (%0,54 gibi)
        public string Degisim { get; set; }
        //------------------------------------------
        public string DisplayForexBuying => FormatPrice(ForexBuying);
        public string DisplayForexSelling => FormatPrice(ForexSelling);
        public string DisplayName => !string.IsNullOrEmpty(Isim) ? Isim : (CurrencyName ?? Kod);

        // Fark artık doğrudan API'den gelen değişim verisi
        public string Fark => Degisim;

        public string Yon
        {
            get
            {
                if (string.IsNullOrEmpty(Degisim)) return "→";
                if (Degisim.Contains("-")) return "↓";
                return "↑";
            }
        }

        public string YonColor
        {
            get
            {
                if (string.IsNullOrEmpty(Degisim)) return "#FF9800"; // Turuncu
                if (Degisim.Contains("-")) return "#F44336"; // Kırmızı
                return "#4CAF50"; // Yeşil
            }
        }

        public string Kategori
        {
            get
            {
                var name = (DisplayName ?? "").ToUpperInvariant();
                var kod = (Kod ?? "").ToUpperInvariant();

                if (name.Contains("ALTIN") || name.Contains("GRAM") || name.Contains("ÇEYREK") || kod == "GAU" || kod == "CAU")
                    return "ALTIN";
                if (name.Contains("GÜMÜŞ") || kod == "GUM")
                    return "GUMUS";
                if (name.Contains("BITCOIN") || name.Contains("ETHEREUM") || kod == "BTC" || kod == "ETH")
                    return "KRIPTO";
                if (name.Contains("BIST") || kod == "XU100")
                    return "BIST";
                return "DOVIZ";
            }
        }

        private double ParseDouble(string value)
        {
            if (string.IsNullOrWhiteSpace(value) || value == "-") return 0;
            try
            {
                string cleanValue = value.Replace(".", "").Replace(",", ".");
                if (double.TryParse(cleanValue, NumberStyles.Any, CultureInfo.InvariantCulture, out double result))
                    return result;
                return 0;
            }
            catch { return 0; }
        }

        private string FormatPrice(string price)
        {
            var val = ParseDouble(price);
            if (val == 0) return "-";
            if (val > 1000) return val.ToString("N2", new CultureInfo("tr-TR"));
            return val.ToString("N4", new CultureInfo("tr-TR"));
        }
    }
}