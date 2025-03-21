using System.ComponentModel;

namespace MAUIDevExpressApp.Shared.DTOs
{
    public class PermissionWithSelectionDTO : PermissionDTO, INotifyPropertyChanged
    {
        private bool _isSelected;

        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                if (_isSelected != value)
                {
                    _isSelected = value;
                    OnPropertyChanged(nameof(IsSelected));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public PermissionWithSelectionDTO(PermissionDTO permission, bool isSelected)
        {
            Id = permission.Id;
            Name = permission.Name;
            Description = permission.Description;
            PageId = permission.PageId;
            Action = permission.Action;
            IsActive = permission.IsActive;
            CreatedAt = permission.CreatedAt;
            UpdatedAt = permission.UpdatedAt;
            Page = permission.Page;
            PageObj = permission.PageObj;
            IsSelected = isSelected;
        }

    }
}

