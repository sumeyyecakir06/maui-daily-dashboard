using odev3son.Models;
using odev3son.Services;
using System.Collections.ObjectModel;

namespace odev3son
{
    public partial class DovizKurlari : ContentPage
    {
        public ObservableCollection<Currency> Dovizler { get; set; } = new ObservableCollection<Currency>();
        public ObservableCollection<Currency> Altinlar { get; set; } = new ObservableCollection<Currency>();
        public ObservableCollection<Currency> Kriptolar { get; set; } = new ObservableCollection<Currency>();

        public DovizKurlari()
        {
            InitializeComponent();

            
            DovizList.ItemsSource = Dovizler;
            AltinList.ItemsSource = Altinlar;
            KriptoList.ItemsSource = Kriptolar;

            LoadData();
        }

        private async void LoadData()
        {
            if (loadingIndicator.IsRunning) return;

            try
            {
                //------------------------------------------
                loadingIndicator.IsVisible = true;
                loadingIndicator.IsRunning = true;
                lblDate.Text = "Güncelleniyor...";

                var result = await KurService.GetKurlarAsync();

                if (result.Kurlar.Count > 0)
                {
                    Dovizler.Clear();
                    Altinlar.Clear();
                    Kriptolar.Clear();

                    foreach (var kur in result.Kurlar)
                    {
                        if (kur.Kategori == "DOVIZ") Dovizler.Add(kur);
                        else if (kur.Kategori == "ALTIN" || kur.Kategori == "GUMUS") Altinlar.Add(kur);
                        else if (kur.Kategori == "KRIPTO" || kur.Kategori == "BIST") Kriptolar.Add(kur);
                    }

                    lblDate.Text = $"Son Güncelleme: {result.Tarih}";
                }
                else
                {
                    lblDate.Text = "Veri alınamadı.";
                    await DisplayAlert("Hata", "Veriler çekilemedi.", "Tamam");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Hata", ex.Message, "Tamam");
            }
            finally
            {
                loadingIndicator.IsRunning = false;
                loadingIndicator.IsVisible = false;
            }
        }

        private void BtnYenile_Clicked(object sender, EventArgs e)
        {
            LoadData();
        }
    }
}