using MAUIDevExpressApp.UI.Interface_Services;
using MAUIDevExpressApp.UI.Services;
using MAUIDevExpressApp.UI.Services.Multiform;
using MAUIDevExpressApp.UI.ViewModels;
using MAUIDevExpressApp.UI.Views;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Syncfusion.Maui.Core.Hosting;
using System.Reflection;
using Page = MAUIDevExpressApp.UI.Views.Page;
namespace MAUIDevExpressApp.UI
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            var getAssembly = Assembly.GetExecutingAssembly();

            var resourceName = "MAUIDevExpressApp.UI.appsettings.json";
            using var stream = getAssembly.GetManifestResourceStream(resourceName)
                ?? throw new FileNotFoundException($"Embedded resource '{resourceName}' not found.");

            var config = new ConfigurationBuilder()
                .AddJsonStream(stream)
                .Build();

            builder.Configuration.AddConfiguration(config);


            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                })
                .ConfigureSyncfusionCore();

#if DEBUG
    		builder.Logging.AddDebug();
#endif

            // Register Services
            builder.Services.AddSingleton<HttpClient>();
            builder.Services.AddSingleton<IDialogService,DialogService>();
            builder.Services.AddSingleton<IAPIService, APIService>();
            builder.Services.AddSingleton<IAuthService, AuthService>();
            builder.Services.AddSingleton<INavigationService, NavigationService>();
            builder.Services.AddSingleton<IProductService, ProductService>();
            builder.Services.AddSingleton<IProductCategoryService, ProductCategoryService>();
            builder.Services.AddSingleton<IModuleService, ModuleService>();
            builder.Services.AddSingleton<IPageService, PageService>();
            builder.Services.AddSingleton<IRoleService, RoleService>();
            builder.Services.AddSingleton<IPermissionService, PermissionService>();
            builder.Services.AddSingleton<IRolePermissionService, RolePermissionService>();

            // Register ViewModels
            builder.Services.AddTransient<AppShellViewModel>();
            builder.Services.AddTransient<LoginViewModel>();
            builder.Services.AddTransient<RegisterViewModel>();
            builder.Services.AddTransient<MainViewModel>();
            builder.Services.AddTransient<ProductsCategoryViewModel>();
            builder.Services.AddTransient<ProductCategoryDetailViewModel>();
            builder.Services.AddTransient<ProductsViewModel>();
            builder.Services.AddTransient<ProductDetailViewModel>();
            builder.Services.AddTransient<ModulesViewModel>();
            builder.Services.AddTransient<PageViewModel>();
            builder.Services.AddTransient<ModuleDetailViewModel>();
            builder.Services.AddTransient<RolesViewModel>();
            builder.Services.AddTransient<RoleDetailViewModel>();
            builder.Services.AddTransient<MultiFormRolesViewModel>();
            builder.Services.AddTransient<PermissionViewModel>();
            builder.Services.AddTransient<PermissionFormViewModel>();
            builder.Services.AddTransient<RolePermissionViewModel>();


            // Register AppShell and App
            builder.Services.AddSingleton<AppShell>();  // Add this
            builder.Services.AddSingleton<App>();

            // Register Pages
            builder.Services.AddTransient<LoginPage>();
            builder.Services.AddTransient<RegisterPage>();
            builder.Services.AddTransient<MainPage>();
            builder.Services.AddTransient<ProductCategoriesPage>();
            builder.Services.AddTransient<ProductCategoryDetailPage>();
            builder.Services.AddTransient<ProductsPage>();
            builder.Services.AddTransient<ProductDetailPage>();
            builder.Services.AddTransient<ModulesPage>();
            builder.Services.AddTransient<ModuleDetailPage>();
            builder.Services.AddTransient<Page>();
            builder.Services.AddTransient<RolesPage>();
            builder.Services.AddTransient<RoleDetailPage>();
            builder.Services.AddTransient<MultiFormRolesPage>();
            builder.Services.AddTransient<PermissionPage>();
            builder.Services.AddTransient<PermissionDetailPage>();
            builder.Services.AddTransient<RolePermissionPage>();

            return builder.Build();
        }
    }
}
