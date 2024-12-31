namespace ControYaApp
{
    public partial class App : Application
    {
        public App(IServiceProvider provider)
        {
            InitializeComponent();
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            return new Window(new AppShell());
        }
    }
}