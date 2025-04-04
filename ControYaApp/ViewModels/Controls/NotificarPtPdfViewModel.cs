using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using ControYaApp.Models;
using ControYaApp.Services.Navigation;
using ControYaApp.Services.Pdf;
using ControYaApp.ViewModels.Base;

namespace ControYaApp.ViewModels.Controls
{
    [QueryProperty(nameof(OrdenProduccion), "orden")]
    [QueryProperty(nameof(Serie), "serie")]
    [QueryProperty(nameof(Empleado), "empleado")]
    public partial class NotificarPtPdfViewModel : BaseViewModel
    {

        private readonly PdfService _pdfService;



        private string _webViewPdfSource;
        public string WebViewPdfSource
        {
            get => _webViewPdfSource;
            set => SetProperty(ref _webViewPdfSource, value);
        }


        private OrdenProduccion? _ordenProduccion;
        public OrdenProduccion? OrdenProduccion
        {
            get => _ordenProduccion;
            set => SetProperty(ref _ordenProduccion, value);
        }


        private string? _serie;
        public string? Serie
        {
            get => _serie;
            set => SetProperty(ref _serie, value);
        }


        private string? _empleado;
        public string? Empleado
        {
            get => _empleado;
            set => SetProperty(ref _empleado, value);
        }


        private bool _isVisible = true;
        public bool IsVisible
        {
            get => _isVisible;
            set => SetProperty(ref _isVisible, value);
        }


        public ICommand GoBackCommand { get; }

        public ICommand GenerarPdfCommand { get; }


        public NotificarPtPdfViewModel(INavigationService navigationService, PdfService pdfService) : base(navigationService)
        {

            _pdfService = pdfService;

            GoBackCommand = new RelayCommand(GoBackAsync);
            GenerarPdfCommand = new RelayCommand(GenerarPdf);

        }


        private async void GoBackAsync()
        {
            await NavigationService.GoBackAsync();
        }


        private async void GenerarPdf()
        {
            //try
            //{
            //    var view = Shell.Current.CurrentPage as ContentPage;
            //    var path = _pdfService.GeneratePdf(view.Content, OrdenProduccion.CodigoProduccion, OrdenProduccion.Orden, OrdenProduccion.CodigoMaterial);
            //    if (string.IsNullOrEmpty(path))
            //    {
            //        await Toast.Make("No se genero PDF").Show();
            //    }
            //    else
            //    {
            //        //WebViewPdfSource = path;
            //        await Toast.Make($"PDF generado en: {path}").Show();
            //    }
            //}
            //catch (Exception ex)
            //{
            //    await Toast.Make(ex.Message).Show();
            //}
        }


        internal void Loaded()
        {
            //GenerarPdf();
        }
    }
}
