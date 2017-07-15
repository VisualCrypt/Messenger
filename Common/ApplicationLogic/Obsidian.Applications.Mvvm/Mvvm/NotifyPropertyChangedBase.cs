using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Obsidian.Applications.Mvvm.Mvvm
{
    public class NotifyPropertyChangedBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
       
        protected void OnPropertyChanged([CallerMemberName]string propertyName = null)
        {
            var eventHandler = PropertyChanged;
            eventHandler?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
      
        protected void OnPropertyChangedExplicit(string propertyName)
        {
            var eventHandler = PropertyChanged;
            eventHandler?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        
    }
}
