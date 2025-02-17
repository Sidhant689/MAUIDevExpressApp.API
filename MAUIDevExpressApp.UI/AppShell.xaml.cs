using MAUIDevExpressApp.UI.ViewModels;

namespace MAUIDevExpressApp.UI
{
    public partial class AppShell : Shell
    {
        public AppShell(AppShellViewModel appShellViewModel)
        {
            InitializeComponent();
            BindingContext = appShellViewModel;

        }
    }
}
