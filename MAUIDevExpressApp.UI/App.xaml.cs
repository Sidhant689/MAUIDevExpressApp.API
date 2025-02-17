namespace MAUIDevExpressApp.UI
{
    public partial class App : Application
    {
        public App(AppShell appShell)
        {
            InitializeComponent();

            MainPage = appShell;

            // Navigate to Login page on startup
            Shell.Current.GoToAsync("//LoginPage");

        }
    }
}
