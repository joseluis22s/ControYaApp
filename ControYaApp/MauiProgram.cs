using CommunityToolkit.Maui;
using ControYaApp.Services.Database;
using ControYaApp.Services.DI;
using ControYaApp.Services.LocalDatabase.Repositories;
using ControYaApp.Services.SharedData;
using ControYaApp.Services.WebService;
using ControYaApp.ViewModels;
using ControYaApp.ViewModels.Controls;
using ControYaApp.Views;
using ControYaApp.Views.Controls;
using Microsoft.Extensions.Logging;
using UraniumUI;

namespace ControYaApp
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>().UseUraniumUI()
                .UseUraniumUIMaterial()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("Aileron-Regular", "AileronRegular");
                    fonts.AddFont("Aileron-Semibol", "OpenSansRegular");
                    fonts.AddMaterialSymbolsFonts();

                }).UseMauiCommunityToolkit()
                .RegisterAppServices()
                .RegisterViews()
                .RegisterViewModels();

#if DEBUG
            builder.Logging.AddDebug();
#endif
            return builder.Build();
        }

        // AddSingleton: Una sola instancia para la App. Debe durar. Generlamente a servicios
        // AddTransient: Cada vez que se solicita una instancia, se crea una nueva. Generalmente a Views y ViewModels
        public static MauiAppBuilder RegisterViewModels(this MauiAppBuilder mauiAppBuilder)
        {
            mauiAppBuilder.Services.AddSingleton<AppShellViewModel>();

            //mauiAppBuilder.Services.AddSingleton<LoadingPopUpViewModel>();
            //mauiAppBuilder.Services.AddSingleton<LoginViewModel>();
            //mauiAppBuilder.Services.AddSingleton<OrdenesViewModel>();
            //mauiAppBuilder.Services.AddSingleton<NotificarPtViewModel>();
            //mauiAppBuilder.Services.AddSingleton<ConfigViewModel>();
            //mauiAppBuilder.Services.AddTransient<HomeViewModel>();

            mauiAppBuilder.Services.AddTransient<LoadingPopUpViewModel>();
            mauiAppBuilder.Services.AddTransient<LoginViewModel>();
            mauiAppBuilder.Services.AddTransient<OrdenesViewModel>();
            mauiAppBuilder.Services.AddTransient<NotificarPtViewModel>();
            mauiAppBuilder.Services.AddTransient<ConfigViewModel>();
            mauiAppBuilder.Services.AddTransient<HomeViewModel>();

            return mauiAppBuilder;
        }
        public static MauiAppBuilder RegisterViews(this MauiAppBuilder mauiAppBuilder)
        {
            mauiAppBuilder.Services.AddSingleton<AppShell>();

            //mauiAppBuilder.Services.AddSingleton<LoadingPopUp>();
            //mauiAppBuilder.Services.AddSingleton<LoginPage>();
            //mauiAppBuilder.Services.AddSingleton<OrdenesPage>();
            //mauiAppBuilder.Services.AddSingleton<NotificarPtPage>();
            //mauiAppBuilder.Services.AddSingleton<ConfigPage>();
            //mauiAppBuilder.Services.AddSingleton<HomePage>();

            mauiAppBuilder.Services.AddTransient<LoadingPopUp>();
            mauiAppBuilder.Services.AddTransient<LoginPage>();
            mauiAppBuilder.Services.AddTransient<OrdenesPage>();
            mauiAppBuilder.Services.AddTransient<NotificarPtPage>();
            mauiAppBuilder.Services.AddTransient<ConfigPage>();
            mauiAppBuilder.Services.AddTransient<HomePage>();

            return mauiAppBuilder;
        }
        public static MauiAppBuilder RegisterAppServices(this MauiAppBuilder mauiAppBuilder)
        {
            mauiAppBuilder.Services.AddSingleton<DatabaseConnection>();
            mauiAppBuilder.Services.AddSingleton<RestService>();
            mauiAppBuilder.Services.AddSingleton<OrdenRepo>();
            mauiAppBuilder.Services.AddSingleton<EmpleadosRepo>();
            mauiAppBuilder.Services.AddSingleton<MaterialEgresadoRepo>();
            mauiAppBuilder.Services.AddSingleton<ProductoTerminadoRepo>();
            mauiAppBuilder.Services.AddSingleton<PeriodoRepo>();
            mauiAppBuilder.Services.AddSingleton<UsuarioRepo>();
            mauiAppBuilder.Services.AddSingleton<PtNotificadoRepo>();
            mauiAppBuilder.Services.AddSingleton<LocalRepoService>();
            mauiAppBuilder.Services.AddSingleton<IpServidorRepo>();
            mauiAppBuilder.Services.AddSingleton<ISharedData, SharedData>();

            return mauiAppBuilder;
        }
    }

}
