using CommunityToolkit.Mvvm.ComponentModel;
using MAUIDevExpressApp.Shared.DTOs;
using MAUIDevExpressApp.UI.Interface_Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MAUIDevExpressApp.UI.Services.Multiform
{
    public partial class MultiFormManager : ObservableObject
    {
        private readonly IDialogService _dialogService;

        [ObservableProperty]
        private ObservableCollection<FormSession> _activeForms;

        [ObservableProperty]
        private FormSession _currentForm;

        // Add a computed property for the count
        public int FormCount => ActiveForms?.Count ?? 0;

        public MultiFormManager(IDialogService dialogService)
        {
            _dialogService = dialogService;
            ActiveForms = new ObservableCollection<FormSession>();

            // Add a collection changed handler to notify when the count changes
            ActiveForms.CollectionChanged += (s, e) => OnPropertyChanged(nameof(FormCount));
        }

        public FormSession CreateNewSession(RoleDTO role = null, string title = null)
        {
            try
            {
                var session = new FormSession
                {
                    Id = Guid.NewGuid().ToString(),
                    Role = role ?? new RoleDTO(),
                    IsEditing = role != null,
                    Title = title ?? (role != null ? "Edit Role" : "New Role"),
                    IsBusy = false,
                    IsMinimized = false
                };

                ActiveForms.Add(session);
                CurrentForm = session;

                // Explicitly notify that FormCount has changed
                OnPropertyChanged(nameof(FormCount));
                return session;
            }
            catch (Exception ex)
            {
                _dialogService.ShowErrorAsync("Error", ex.Message);
                throw;
            }
        }

        public void CloseSession(string sessionId)
        {
            try
            {
                var session = ActiveForms.FirstOrDefault(s => s.Id == sessionId);
                if (session != null)
                {
                    ActiveForms.Remove(session);
                    if (CurrentForm?.Id == sessionId && ActiveForms.Count > 0)
                    {
                        CurrentForm = ActiveForms[0];
                    }
                    else if (ActiveForms.Count == 0)
                    {
                        CurrentForm = null;
                    }

                    // Explicitly notify that FormCount has changed
                    OnPropertyChanged(nameof(FormCount));
                }
            }
            catch (Exception ex)
            {
                _dialogService.ShowErrorAsync("Error", ex.Message);
            }
            
        }

        public void MinimizeSession(string sessionId, bool minimize)
        {
            var session = ActiveForms.FirstOrDefault(s => s.Id == sessionId);
            if (session != null)
            {
                session.IsMinimized = minimize;
                if(minimize && CurrentForm?.Id == sessionId && ActiveForms.Count > 0)
                {
                    // sitch to anaother minimized form which is not closed
                    var nextForm = ActiveForms.FirstOrDefault(s => s.Id != sessionId && !s.IsMinimized);
                    if (nextForm != null)
                    {
                        CurrentForm = nextForm;
                    }
                }
             
            }
        }
    }
}
