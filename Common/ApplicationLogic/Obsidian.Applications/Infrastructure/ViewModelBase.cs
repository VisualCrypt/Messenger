using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using Obsidian.Applications.Mvvm.Commands;
using Obsidian.Applications.Services.Interfaces;
using Obsidian.Language.Strings;

namespace Obsidian.Applications.Infrastructure
{
    [DataContract]
    public abstract class ViewModelBase : NotifyPropertyChangedBase
    {
        Dictionary<string, DelegateCommandBase> AllCommands => _allCommands
            ?? (_allCommands = new Dictionary<string, DelegateCommandBase>());
        Dictionary<string, DelegateCommandBase> _allCommands;

        Dictionary<string, object> AllFields => _allFields
            ?? (_allFields = new Dictionary<string, object>());
        Dictionary<string, object> _allFields;

        readonly ResourceWrapper _r;
        protected readonly Container _container;
        protected ViewModelBase(Container container)
        {
            _container = container;
            _r = _container.Get<ResourceWrapper>();
            _r.Info.CultureChanged += (sender, args) => { OnPropertyChangedExplicit(null); };
        }

        protected ViewModelBase()
        {
            
        }

        public ResourceWrapper R => _r;

        

        protected T Get<T>([CallerMemberName] string propertyName = null)
        {
            if (!AllFields.ContainsKey(propertyName))
                return default(T);

            return (T)_allFields[propertyName];
        }

        /// <summary>
        /// Raises All PropertyChanged and CanExecuteChanged events in this class and
        /// sets the backing field.
        /// </summary>
        protected void SetChanged(object value, [CallerMemberName] string propertyName = null)
        {
            AllFields[propertyName] = value;

            OnPropertyChangedExplicit(null);
            AllCanExecuteChanged();
        }

        protected void SetChanged(object value, bool avoidStackoverflow, [CallerMemberName] string propertyName = null)
        {
            AllFields[propertyName] = value;
            OnPropertyChangedExplicit(propertyName);
        }

        protected void AllCanExecuteChanged()
        {
            foreach (var c in AllCommands)
                c.Value.RaiseCanExecuteChanged();
        }

        protected DelegateCommand CreateCommand(Action execute, Func<bool> canExecute, [CallerMemberName] string commandName = null)
        {
            if (!AllCommands.ContainsKey(commandName))
            {
                AllCommands.Add(commandName, new DelegateCommand(execute, canExecute));
            }
            return (DelegateCommand)AllCommands[commandName];
        }

        protected DelegateCommand<T> CreateCommand<T>(Action<T> execute, Func<T, bool> canExecute, [CallerMemberName] string commandName = null)
        {
            if (!AllCommands.ContainsKey(commandName))
            {
                AllCommands.Add(commandName, new DelegateCommand<T>(execute, canExecute));
            }
            return (DelegateCommand<T>)AllCommands[commandName];
        }

        public virtual async Task OnViewDidLoad()
        {
            
        }

    }
}
