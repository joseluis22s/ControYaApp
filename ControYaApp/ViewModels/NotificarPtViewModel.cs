using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using ControYaApp.Models;

namespace ControYaApp.ViewModels
{
    [QueryProperty(nameof(OrdenProduccion), "orden")]
    public class NotificarPtViewModel : ViewModelBase
    {

        private OrdenProduccion? _ordenProduccion;


        public OrdenProduccion? OrdenProduccion
        {
            get => _ordenProduccion;
            set => SetProperty(ref _ordenProduccion, value);
        }


        public ICommand GoBackCommand { get; }


        public NotificarPtViewModel()
        {
            GoBackCommand = new AsyncRelayCommand(GoBackAsync);
        }


        private async Task GoBackAsync()
        {
            await Shell.Current.GoToAsync("..");
        }
    }
}
