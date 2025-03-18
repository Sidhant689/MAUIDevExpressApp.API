using MAUIDevExpressApp.UI.ViewModels;

namespace MAUIDevExpressApp.UI.Views;

public partial class Page : ContentPage
{
    private readonly PageViewModel _pageViewModel;
    public Page(PageViewModel pageViewModel)
    {
        InitializeComponent();
        _pageViewModel = pageViewModel;
        BindingContext = _pageViewModel;
        
    }

    protected override void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);
        _pageViewModel.OnNavigatedToAsync();
    }
}