using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ControYaApp.ViewModels
{
    class HomeViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        public ICommand GoToAsyncLogin { get; private set; }

        public HomeViewModel()
        {
            GoToAsyncLogin = new Command(() => {
                Shell.Current.GoToAsync("//login");
            });
        }

        public void OnPropertyChanged([CallerMemberName] string name = "") =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
