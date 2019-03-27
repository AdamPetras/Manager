using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Manager.Annotations;
using Manager.Views;
using Xamarin.Forms;

namespace Manager.ViewModels
{
    public class MainPageVm: INotifyPropertyChanged
    {
        private string _name;
        public string Name { get => _name;
            set
            {
                _name = value;
                OnPropertyChanged(nameof(Name));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}