using Microsoft.Maui.Controls;
using Microsoft.Maui.Storage;
using odev3son.Services;

namespace odev3son
{
    public partial class SettingsPage : ContentPage
    {
        public SettingsPage()
        {
            InitializeComponent();

            // Mevcut tema ayarını yükle
            var isDarkMode = Preferences.Get("IsDarkMode", false);
            ThemeSwitch.IsToggled = isDarkMode;

            // Temayı uygula
            Application.Current!.UserAppTheme = isDarkMode ? AppTheme.Dark : AppTheme.Light;
        }

        private void OnThemeToggled(object sender, ToggledEventArgs e)
        {
            // Tema değiştir - AppThemeBinding otomatik güncellenir
            Application.Current!.UserAppTheme = e.Value ? AppTheme.Dark : AppTheme.Light;

            // Tercihi kaydet
            Preferences.Set("IsDarkMode", e.Value);
        }

        private async void OnLogoutClicked(object sender, EventArgs e)
        {
            bool confirm = await DisplayAlert("Çıkış Yap",
                "Uygulamadan çıkış yapmak istediğinizden emin misiniz?",
                "Evet", "Hayır");

            if (confirm)
            {
                // Firebase oturum bilgilerini temizle
                FirebaseService.Logout();

                // Oturum bilgilerini temizle
                Preferences.Remove("IsLoggedIn");
                Preferences.Remove("UserEmail");
                Preferences.Remove("UserId");

                // Login sayfasına dön
                Application.Current.MainPage = new NavigationPage(new LoginPage());
            }
        }
    }
}