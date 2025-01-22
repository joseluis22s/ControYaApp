using ControYaApp.ViewModels;

namespace ControYaApp
{
    public partial class AppShell : Shell
    {
        private AppShellViewModel _vm;
        public AppShell()
        {
            InitializeComponent();
            BindingContext = _vm = new AppShellViewModel();
        }
    }
}
