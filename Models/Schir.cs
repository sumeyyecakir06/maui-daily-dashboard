using System.ComponentModel;
using System.Runtime.CompilerServices;
using odev3son.Services;

namespace odev3son.Models
{
    public class Schir : INotifyPropertyChanged
    {
        private string _schirAdi;

        public string SchirAdi
        {
            get => _schirAdi;
            set
            {
                if (_schirAdi != value)
                {
                    _schirAdi = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(BugunUrl));
                    OnPropertyChanged(nameof(BesGunUrl));
                }
            }
        }
        //------------------
        public string BugunUrl
        {
            get
            {
                if (string.IsNullOrEmpty(SchirAdi))
                    return string.Empty;

                var normalized = HavaDurumuService.NormalizeCityName(SchirAdi);
                return $"https://www.mgm.gov.tr/sunum/sondurum-show-2.aspx?m={normalized}&rC=111&rZ=fff";
            }
        }

        public string BesGunUrl
        {
            get
            {
                if (string.IsNullOrEmpty(SchirAdi))
                    return string.Empty;

                var normalized = HavaDurumuService.NormalizeCityName(SchirAdi);
                return $"https://www.mgm.gov.tr/sunum/tahmin-show-2.aspx?m={normalized}&basla=1&bitir=5&rC=111&rZ=fff";
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}