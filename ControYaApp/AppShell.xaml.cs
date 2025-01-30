using ControYaApp.ViewModels;
using ControYaApp.Views;

namespace ControYaApp
{
    public partial class AppShell : Shell
    {
        private AppShellViewModel _vm;
        public AppShell()
        {
            InitializeComponent();
            BindingContext = _vm = new AppShellViewModel();
            Routing.RegisterRoute("notificarPt", typeof(NotificarPtPage));
        }
    }
}
