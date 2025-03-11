using ControYaApp.ViewModels;

namespace ControYaApp
{
    public partial class App : Application
    {
        private readonly IServiceProvider _provider;

        public App(IServiceProvider provider)
        {
            _provider = provider;
            InitializeComponent();
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            var viewModel = _provider.GetRequiredService<AppShellViewModel>();
            return new Window(new AppShell(viewModel));
            //return new Window(new PruebaPDfgGenerar());
        }
    }
}