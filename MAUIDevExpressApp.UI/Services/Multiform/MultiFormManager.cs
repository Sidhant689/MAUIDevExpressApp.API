using CommunityToolkit.Mvvm.ComponentModel;
using MAUIDevExpressApp.UI.Interface_Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace MAUIDevExpressApp.UI.Services.Multiform
{
    public partial class MultiFormManager<T> : ObservableObject where T : class, new()
    {
        private readonly IDialogService _dialogService;

        [ObservableProperty]
        private ObservableCollection<FormSession<T>> _activeForms;

        [ObservableProperty]
        private FormSession<T> _currentForm;

        // Add a computed property for the count
        public int FormCount => ActiveForms?.Count ?? 0;

        public MultiFormManager(IDialogService dialogService)
        {
            _dialogService = dialogService;
            ActiveForms = new ObservableCollection<FormSession<T>>();
            // Add a collection changed handler to notify when the count changes
            ActiveForms.CollectionChanged += (s, e) => OnPropertyChanged(nameof(FormCount));
        }

        public FormSession<T> CreateNewSession(T entity = null, string title = null)
        {
            try
            {
                var session = new FormSession<T>
                {
                    Id = Guid.NewGuid().ToString(),
                    Entity = entity ?? new T(),
                    IsEditing = entity != null,
                    Title = title ?? (entity != null ? "Edit Item" : "New Item"),
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
                if (minimize && CurrentForm?.Id == sessionId && ActiveForms.Count > 0)
                {
                    // Switch to another non-minimized form
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