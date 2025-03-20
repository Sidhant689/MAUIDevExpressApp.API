using MAUIDevExpressApp.Shared.DTOs;
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

    private async void TreeCheckBox_StateChanged(object sender, Syncfusion.Maui.Buttons.StateChangedEventArgs e)
    {
        if (sender is Syncfusion.Maui.Buttons.SfCheckBox checkBox)
        {
            if (checkBox.BindingContext is TreeViewNode node)
            {
                var viewModel = (RolePermissionViewModel)BindingContext;

                // Explicitly set the SelectedTreeItem to this node
                viewModel.SelectedTreeItem = node;

                if (node.Content is ModuleDTO module)
                {
                    if (e.IsChecked == true)
                    {
                        await viewModel.LoadModulePermissionsAsync(module);
                    }
                    else
                    {
                        viewModel.RemoveModulePermissions(module);
                    }
                }
                else if (node.Content is PageDTO page)
                {
                    if (e.IsChecked == true)
                    {
                        await viewModel.LoadPagePermissionsAsync(page);
                    }
                    else
                    {
                        await viewModel.RemovePagePermissions(page);
                    }
                }
            }
        }
    }
}
