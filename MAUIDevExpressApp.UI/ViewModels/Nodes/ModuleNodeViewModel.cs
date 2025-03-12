using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MAUIDevExpressApp.UI.ViewModels.Nodes
{
    public partial class ModuleNodeViewModel : ObservableObject
    {
        [ObservableProperty]
        private int _id;

        [ObservableProperty]
        private string _name;

        [ObservableProperty]
        private string _description;

        [ObservableProperty]
        private string _action;

        [ObservableProperty]
        private bool _isModule;

        [ObservableProperty]
        private bool _isSelected;

        [ObservableProperty]
        private bool _isIndeterminate;

        [ObservableProperty]
        private ObservableCollection<ModuleNodeViewModel> _permissions;
    }
}
