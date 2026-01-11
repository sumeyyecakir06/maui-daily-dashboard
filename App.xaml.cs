using Microsoft.Maui.Controls;

namespace odev3son
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            // Başlangıç temasını yükle
            var isDarkMode = Preferences.Get("IsDarkMode", false);
            Application.Current!.UserAppTheme = isDarkMode ? AppTheme.Dark : AppTheme.Light;

            MainPage = new NavigationPage(new LoginPage());
        }

        // Tema değişikliğini dinle
        protected override void OnStart()
        {
            // Tema değişikliği için event ekleyebilirsiniz
        }
    }
}