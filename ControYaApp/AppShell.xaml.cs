using ControYaApp.ViewModels;
using ControYaApp.Views;

namespace ControYaApp
{
    public partial class AppShell : Shell
    {

        public AppShell(AppShellViewModel vm)
        {
            InitializeComponent();
            BindingContext = vm;
            Routing.RegisterRoute("notificarPt", typeof(NotificarPtPage));
            Routing.RegisterRoute("config", typeof(ConfigPage));
        }
    }
}
