using MAUIDevExpressApp.UI.Interface_Services;
using MAUIDevExpressApp.UI.ViewModels;
using MAUIDevExpressApp.UI.Views;
using Microsoft.Extensions.DependencyInjection;


namespace MAUIDevExpressApp.UI
{
    public partial class App : Application
    {
        private readonly IServiceProvider _serviceProvider;

        public App(AppShell appShell, IServiceProvider serviceProvider)
        {
            InitializeComponent();

            _serviceProvider = serviceProvider;
            MainPage = appShell;

            // Navigate to Login page on startup
            Shell.Current.GoToAsync("//LoginPage");
        }
    }
}
