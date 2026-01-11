namespace odev3son
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

       
            Routing.RegisterRoute(nameof(AddToDoPage), typeof(AddToDoPage));
            Routing.RegisterRoute(nameof(LoginPage), typeof(LoginPage));
            Routing.RegisterRoute(nameof(RegisterPage), typeof(RegisterPage));
        }
    }
}