using MAUIDevExpressApp.UI.ViewModels;
using Syncfusion.TreeView.Engine;

namespace MAUIDevExpressApp.UI.Views;

public partial class RolePermissionPage : ContentPage
{
    private readonly RolePermissionViewModel _rolePermissionViewModel;
    public RolePermissionPage(RolePermissionViewModel rolePermissionViewModel)
    {
        InitializeComponent();
        _rolePermissionViewModel = rolePermissionViewModel;
        BindingContext = _rolePermissionViewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        _rolePermissionViewModel.OnNavigatedToAsync();
    }

    
}
