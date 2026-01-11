namespace odev3son
{
    public partial class HomePage : ContentPage
    {
        public HomePage()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            Preferences.Set("LastVisit", DateTime.Now.ToString("dd.MM.yyyy HH:mm"));
        }
    }
}