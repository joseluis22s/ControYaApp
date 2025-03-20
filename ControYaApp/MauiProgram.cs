using CommunityToolkit.Maui;
using ControYaApp.Services.DI;
using ControYaApp.Services.LocalDatabase.Repositories;
using ControYaApp.Services.OrdenProduccionFilter;
using ControYaApp.Services.Pdf;
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
                .RegisterViewModels()
                .ConfigureMauiHandlers(handlers =>
                {
#if ANDROID
                    handlers.AddHandler(typeof(Picker), typeof(Platforms.Android.MyPickerHandler));
#endif
                });

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

            mauiAppBuilder.Services.AddTransient<LoadingPopUpViewModel>();
            mauiAppBuilder.Services.AddTransient<LoginViewModel>();
            mauiAppBuilder.Services.AddTransient<OrdenesViewModel>();
            mauiAppBuilder.Services.AddTransient<NotificarPtViewModel>();
            mauiAppBuilder.Services.AddTransient<ConfigViewModel>();
            mauiAppBuilder.Services.AddTransient<HomeViewModel>();
            mauiAppBuilder.Services.AddTransient<NotificarPmViewModel>();
            //mauiAppBuilder.Services.AddSingleton<HomeViewModel>();


            mauiAppBuilder.Services.AddSingleton<NotificarPtPdfViewModel>();

            return mauiAppBuilder;
        }
        public static MauiAppBuilder RegisterViews(this MauiAppBuilder mauiAppBuilder)
        {
            mauiAppBuilder.Services.AddSingleton<AppShell>();

            mauiAppBuilder.Services.AddTransient<LoadingPopUp>();
            mauiAppBuilder.Services.AddTransient<LoginPage>();
            mauiAppBuilder.Services.AddTransient<OrdenesPage>();
            mauiAppBuilder.Services.AddTransient<NotificarPtPage>();
            mauiAppBuilder.Services.AddTransient<ConfigPage>();
            mauiAppBuilder.Services.AddTransient<HomePage>();
            mauiAppBuilder.Services.AddTransient<NotificarPmPage>();
            //mauiAppBuilder.Services.AddSingleton<HomePage>();
            mauiAppBuilder.Services.AddTransient<NotificarPtPdfPage>();

            return mauiAppBuilder;
        }
        public static MauiAppBuilder RegisterAppServices(this MauiAppBuilder mauiAppBuilder)
        {
            mauiAppBuilder.Services.AddSingleton<RestService>();
            mauiAppBuilder.Services.AddSingleton<EmpleadosRepo>();
            mauiAppBuilder.Services.AddSingleton<DataConfigRepo>();
            mauiAppBuilder.Services.AddSingleton<OrdenProduccionMpRepo>();
            mauiAppBuilder.Services.AddSingleton<OrdenProduccionPtRepo>();
            mauiAppBuilder.Services.AddSingleton<OrdenProduccionRepo>();
            mauiAppBuilder.Services.AddSingleton<PeriodoRepo>();
            mauiAppBuilder.Services.AddSingleton<PtNotificadoRepo>();
            mauiAppBuilder.Services.AddSingleton<PmNotificadoRepo>();
            mauiAppBuilder.Services.AddSingleton<UsuarioRepo>();
            mauiAppBuilder.Services.AddSingleton<LocalRepoService>();
            mauiAppBuilder.Services.AddSingleton<ISharedData, SharedData>();
            mauiAppBuilder.Services.AddSingleton<OrdenProduccionFilter>();
            mauiAppBuilder.Services.AddSingleton<OrdenProduccionMpFilter>();

            mauiAppBuilder.Services.AddTransient<PdfService>();

            return mauiAppBuilder;
        }

        public static void ConfigureMauiHandlers()
        {

        }
    }

}
