using CommunityToolkit.Mvvm.ComponentModel;
using MAUIDevExpressApp.Shared.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MAUIDevExpressApp.UI.Services.Multiform
{
    public partial class FormSession : ObservableObject
    {
        public string Id { get; set; }

        [ObservableProperty]
        private RoleDTO _role;

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
