using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.PlatformConfiguration.WindowsSpecific;
using odev3son.Models;
using odev3son.Services;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace odev3son
{
    
    /// Haberler sayfasý - RSS haberlerini listeler
    
    public partial class NewsPage : ContentPage
    {
        private ObservableCollection<HaberItem> _haberler;
        private bool _ilkYukleme = true;

        public NewsPage()
        {
            InitializeComponent();

            // Observable collection oluþtur
            _haberler = new ObservableCollection<HaberItem>();
            haberlerCollectionView.ItemsSource = _haberler;

            // Kategorileri yükle
            YukleniyorGoster(true);
            KategorileriYukle();
        }

       
        /// Kategorileri picker'a yükler

        private void KategorileriYukle()
        {
            try
            {
                var kategoriler = HaberService.GetKategoriler();
                kategoriPicker.ItemsSource = kategoriler;

                // Ýlk kategoriyi seç (Manþet)
                if (kategoriler.Count > 0)
                {
                    kategoriPicker.SelectedIndex = 0;
                }
            }
            catch (Exception ex)
            {
                DisplayAlert("Hata", $"Kategoriler yüklenemedi: {ex.Message}", "Tamam");
                YukleniyorGoster(false);
            }
        }

      
        /// Sayfa görünür olduðunda
      
        protected override void OnAppearing()
        {
            base.OnAppearing();

            // Ýlk yükleme deðilse ve kategori seçiliyse haberleri yenile
            if (!_ilkYukleme && kategoriPicker.SelectedItem != null)
            {
                _ = HaberleriYukle(kategoriPicker.SelectedItem.ToString());
            }
        }

        /// <summary>
        /// Kategori deðiþtiðinde
        /// </summary>
        private async void KategoriPicker_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (kategoriPicker.SelectedItem is string kategori)
            {
                await HaberleriYukle(kategori);
                _ilkYukleme = false;
            }
        }

        /// Haberleri yükler
        
        private async Task HaberleriYukle(string kategori)
        {
            try
            {
                YukleniyorGoster(true);

                // Haberleri getir
                var haberler = await HaberService.GetHaberler(kategori);

                // Listeyi güncelle
                _haberler.Clear();
                foreach (var haber in haberler)
                {
                    _haberler.Add(haber);
                }

                YukleniyorGoster(false);
            }
            catch (Exception ex)
            {
                YukleniyorGoster(false);
                await DisplayAlert("Hata",
                    $"Haberler yüklenirken bir hata oluþtu:\n{ex.Message}",
                    "Tamam");
            }
        }

        /// Yenileme iþlemi
        
        private async void RefreshView_Refreshing(object sender, EventArgs e)
        {
            try
            {
                //-------------------------
                if (kategoriPicker.SelectedItem is string kategori)
                {
                    var haberler = await HaberService.GetHaberler(kategori);

                    _haberler.Clear();
                    foreach (var haber in haberler)
                    {
                        _haberler.Add(haber);
                    }
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Hata", $"Yenileme hatasý: {ex.Message}", "Tamam");
            }
            finally
            {
                refreshView.IsRefreshing = false;
            }
        }

        /// Haber seçildiðinde detay sayfasýna git
     
        private async void HaberSecildi(object sender, SelectionChangedEventArgs e)
        {
            if (e.CurrentSelection.FirstOrDefault() is HaberItem secilenHaber)
            {
                try
                {
                    // Haber detay sayfasýný aç
                    await Navigation.PushAsync(new HaberDetayPage(secilenHaber));
                }
                catch (Exception ex)
                {
                    await DisplayAlert("Hata", $"Sayfa açýlamadý: {ex.Message}", "Tamam");
                }
                finally
                {
                    // Seçimi temizle
                    haberlerCollectionView.SelectedItem = null;
                }
            }
        }

        
        /// Yüklenme göstergesini göster/gizle
        
        private void YukleniyorGoster(bool goster)
        {
            loadingOverlay.IsVisible = goster;
            mainContent.IsVisible = !goster;
        }
    }
}