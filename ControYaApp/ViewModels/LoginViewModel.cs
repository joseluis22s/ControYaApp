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
    class LoginViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        public ICommand GoToAsyncHome { get; private set; }

        public LoginViewModel()
        {
            GoToAsyncHome = new Command(() => {
                Shell.Current.GoToAsync("//home");
            });
        }

        public void OnPropertyChanged([CallerMemberName] string name = "") =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
