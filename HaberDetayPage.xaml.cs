using System;
using System.Threading.Tasks;
using Microsoft.Maui.ApplicationModel.DataTransfer;
using Microsoft.Maui.Controls;
using Microsoft.Maui.ApplicationModel;
using odev3son.Models;

namespace odev3son
{

    /// Haber detay sayfasý - Seçilen haberin tüm bilgilerini gösterir
 
    public partial class HaberDetayPage : ContentPage
    {
        private HaberItem _haber;

        public HaberDetayPage(HaberItem haber)
        {
            InitializeComponent();
            _haber = haber ?? throw new ArgumentNullException(nameof(haber));
            HaberiGoster();
        }

        
        /// Haber bilgilerini arayüze yükler
       
        private void HaberiGoster()
        {
            // Baþlýk
            baslikLabel.Text = _haber.Title;
            Title = _haber.Title;

            // Tarih
            tarihLabel.Text = _haber.FormattedDate;

            // Kategori
            kategoriLabel.Text = _haber.Category;

            // Ýçerik - HTML decode ve temizleme
            var icerik = System.Net.WebUtility.HtmlDecode(_haber.Description);
            icerik = System.Text.RegularExpressions.Regex.Replace(icerik, "<.*?>", string.Empty);
            icerikLabel.Text = string.IsNullOrEmpty(icerik)
                ? "Ýçerik mevcut deðil"
                : icerik;

            // Link
            linkLabel.Text = _haber.Link;

            // Görsel kontrolü
            if (_haber.HasImage)
            {
                resimImage.Source = _haber.ImageUrl;
                resimFrame.IsVisible = true;
                placeholderFrame.IsVisible = false;
            }
            else
            {
                resimFrame.IsVisible = false;
                placeholderFrame.IsVisible = true;
            }
        }

      
        /// Haber paylaþma iþlemi
        
        private async void PaylasClicked(object sender, EventArgs e)
        {
            try
            {
                await Share.Default.RequestAsync(new ShareTextRequest
                {
                    Uri = _haber.Link,
                    Title = _haber.Title,
                    Text = $"{_haber.Title}\n\n{_haber.ShortDescription}\n\n{_haber.Link}"
                });
            }
            catch (Exception ex)
            {
                await DisplayAlert("Hata",
                    $"Paylaþým sýrasýnda bir hata oluþtu:\n{ex.Message}",
                    "Tamam");
            }
        }

       
        /// Linke týklandýðýnda tarayýcýda aç
  
        private async void LinkTapped(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(_haber.Link))
                {
                    await DisplayAlert("Uyarý", "Haber linki bulunamadý", "Tamam");
                    return;
                }

                if (!Uri.IsWellFormedUriString(_haber.Link, UriKind.Absolute))
                {
                    await DisplayAlert("Uyarý", "Geçersiz link formatý", "Tamam");
                    return;
                }

                // Tarayýcýda aç
                await Launcher.OpenAsync(new Uri(_haber.Link));
            }
            catch (Exception ex)
            {
                await DisplayAlert("Hata",
                    $"Link açýlamadý:\n{ex.Message}",
                    "Tamam");
            }
        }
    }
}