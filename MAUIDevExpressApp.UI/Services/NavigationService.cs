﻿using MAUIDevExpressApp.UI.Interface_Services;

public class NavigationService : INavigationService
{

    private readonly IAuthService _authService;

    public NavigationService(IAuthService authService)
    {
        _authService = authService;
    }

    public async Task NavigateToAsync(string route, Dictionary<string, object>? parameters = null)
    {
        
        if (parameters != null)
        {
            await Shell.Current.GoToAsync(route, parameters);
        }
        else
        {
            await Shell.Current.GoToAsync(route);
        }

    }

    public async Task GoBackAsync()
    {
        await Shell.Current.GoToAsync("..");
    }
}
