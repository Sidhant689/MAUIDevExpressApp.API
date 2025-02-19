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

        //protected override async void OnStart()
        //{
        //    var authService = (IAuthService)_serviceProvider.GetService(typeof(IAuthService));
        //    bool isSessionValid = await authService.IsSessionValidAsync();

        //    if (isSessionValid)
        //    {
        //        var mainViewModel = (MainViewModel)_serviceProvider.GetService(typeof(MainViewModel));
        //        MainPage = new MainPage(mainViewModel); // Navigate directly to main page
        //    }
        //    else
        //    {
        //        var loginViewModel = (LoginViewModel)_serviceProvider.GetService(typeof(LoginViewModel));
        //        MainPage = new LoginPage(loginViewModel);
        //    }
        //}
    }
}
