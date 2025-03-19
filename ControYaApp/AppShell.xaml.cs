using ControYaApp.ViewModels;
using ControYaApp.Views;
using ControYaApp.Views.Controls;

namespace ControYaApp
{
    public partial class AppShell : Shell
    {

        private readonly AppShellViewModel _vm;

        public AppShell(AppShellViewModel vm)
        {
            InitializeComponent();

            BindingContext = _vm = vm;

            Routing.RegisterRoute("notificarPt", typeof(NotificarPtPage));
            Routing.RegisterRoute("notificarPm", typeof(NotificarPmPage));
            Routing.RegisterRoute("config", typeof(ConfigPage));
            Routing.RegisterRoute("notificarPtPdf", typeof(NotificarPtPdfPage));
        }


    }
}
