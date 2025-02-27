using CommunityToolkit.Mvvm.ComponentModel;

namespace MAUIDevExpressApp.UI.Services.Multiform
{
    public partial class FormSession<T> : ObservableObject where T : class
    {
        public string Id { get; set; }

        [ObservableProperty]
        private T _entity;

        [ObservableProperty]
        private bool _isEditing;

        [ObservableProperty]
        private string _title;

        [ObservableProperty]
        private bool _isBusy;

        [ObservableProperty]
        private bool _isMinimized;

        [ObservableProperty]
        private bool _hasUnsavedChanges;
    }
}