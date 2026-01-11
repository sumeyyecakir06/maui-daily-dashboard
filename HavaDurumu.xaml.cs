using System.Collections.ObjectModel;
using System.Text.Json;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using odev3son.Models;
using odev3son.Services;

namespace odev3son
{
    public partial class HavaDurumu : ContentPage, INotifyPropertyChanged
    {
        private ObservableCollection<Schir> _schirler = new();
        private const string SEHIRLER_JSON = "schirler.json";

        public ObservableCollection<Schir> Schirler
        {
            get => _schirler;
            set
            {
                _schirler = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public HavaDurumu()
        {
            InitializeComponent();
            BindingContext = this;
            _ = LoadSchirlerAsync();
        }

        // Şehirleri yükle
        private async Task LoadSchirlerAsync()
        {
            try
            {
                var loaded = await LoadFromFileAsync();

                if (loaded != null && loaded.Count > 0)
                {
                    Schirler = loaded;
                }
                else
                {
                    // Varsayılan şehirler
                    Schirler = new ObservableCollection<Schir>
                    {
                        new Schir { SchirAdi = "İstanbul" },
                        new Schir { SchirAdi = "Ankara" },
                        new Schir { SchirAdi = "İzmir" }
                    };
                    await SaveToFileAsync();
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Hata", $"Yükleme hatası: {ex.Message}", "Tamam");
            }
        }

        // Dosyadan oku
        private async Task<ObservableCollection<Schir>> LoadFromFileAsync()
        {
            try
            {
                string filePath = Path.Combine(FileSystem.AppDataDirectory, SEHIRLER_JSON);

                if (File.Exists(filePath))
                {
                    var json = await File.ReadAllTextAsync(filePath);
                    return JsonSerializer.Deserialize<ObservableCollection<Schir>>(json);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Dosya okuma hatası: {ex.Message}");
            }
            return null;
        }

        // Dosyaya kaydet
        private async Task SaveToFileAsync()
        {
            try
            {
                string filePath = Path.Combine(FileSystem.AppDataDirectory, SEHIRLER_JSON);
                var options = new JsonSerializerOptions { WriteIndented = true };
                var json = JsonSerializer.Serialize(Schirler, options);
                await File.WriteAllTextAsync(filePath, json);
                Console.WriteLine($"✅ {Schirler.Count} şehir kaydedildi");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Kayıt hatası: {ex.Message}");
            }
        }

        // Şehir ekle
        private async void AddSchir_Clicked(object sender, EventArgs e)
        {
            try
            {
                var schirAdi = await DisplayPromptAsync(
                    "Şehir Ekle",
                    "Şehir adını girin:",
                    "Ekle",
                    "İptal",
                    "Örn: Samsun",
                    maxLength: 50);

                if (string.IsNullOrWhiteSpace(schirAdi))
                    return;

                schirAdi = schirAdi.Trim();

                // Aynı şehir kontrolü
                if (Schirler.Any(s => s.SchirAdi?.Equals(schirAdi, StringComparison.OrdinalIgnoreCase) == true))
                {
                    await DisplayAlert("Uyarı", $"{schirAdi} zaten listede", "Tamam");
                    return;
                }

                // Şehir doğrula
                bool isValid = await HavaDurumuService.ValidateCityName(schirAdi);

                if (!isValid)
                {
                    await DisplayAlert("Hata", $"{schirAdi} bulunamadı", "Tamam");
                    return;
                }

                // Ekle ve kaydet
                Schirler.Add(new Schir { SchirAdi = schirAdi });
                await SaveToFileAsync();

                await DisplayAlert("Başarılı", $"{schirAdi} eklendi", "Tamam");
            }
            catch (Exception ex)
            {
                await DisplayAlert("Hata", ex.Message, "Tamam");
            }
        }

        // Şehir sil 
        private async void Remove_Clicked(object sender, EventArgs e)
        {
            try
            {
                // Button'dan CommandParameter'ı al
                if (sender is Button button && button.CommandParameter is Schir schir)
                {
                    bool answer = await DisplayAlert(
                        "Sil",
                        $"{schir.SchirAdi} silinsin mi?",
                        "Evet",
                        "Hayır");

                    if (answer)
                    {
                        Schirler.Remove(schir);
                        await SaveToFileAsync();
                        await DisplayAlert("Başarılı", $"{schir.SchirAdi} silindi", "Tamam");
                    }
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Hata", ex.Message, "Tamam");
            }
        }

        // Yenile 
        private async void Update_Clicked(object sender, EventArgs e)
        {
            try
            {
                // Button'dan CommandParameter'ı al
                if (sender is Button button && button.CommandParameter is Schir schir)
                {
                    var index = Schirler.IndexOf(schir);

                    if (index >= 0)
                    {
                        // WebView'ı yenilemek için şehri yeniden ekle
                        var yeniSchir = new Schir { SchirAdi = schir.SchirAdi };
                        Schirler.RemoveAt(index);
                        Schirler.Insert(index, yeniSchir);

                        await DisplayAlert("Yenilendi", $"{schir.SchirAdi} güncellendi", "Tamam");
                    }
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Hata", ex.Message, "Tamam");
            }
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            _ = SaveToFileAsync();
        }
    }
}