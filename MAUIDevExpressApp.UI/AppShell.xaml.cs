using MAUIDevExpressApp.UI.ViewModels;
using MAUIDevExpressApp.UI.Views;

namespace MAUIDevExpressApp.UI
{
    public partial class AppShell : Shell
    {
        public AppShell(AppShellViewModel appShellViewModel)
        {
            InitializeComponent();
            BindingContext = appShellViewModel;


            // Register routes for navigation
            Routing.RegisterRoute("ProductCategoryDetailPage", typeof(ProductCategoryDetailPage));
        }
    }
}
