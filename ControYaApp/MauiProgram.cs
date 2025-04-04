using CommunityToolkit.Maui;
using ControYaApp.Services.AppLocalDatabase;
using ControYaApp.Services.Dialog;
using ControYaApp.Services.LocalDatabase;
using ControYaApp.Services.LocalDatabase.Repositories;
using ControYaApp.Services.Navigation;
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
                .ConfigureFonts(RegisterAppFonts)
                .UseMauiCommunityToolkit()
                .RegisterApp()
                .RegisterProduccionModule()
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

        public static void RegisterAppFonts(IFontCollection fonts)
        {
            fonts.AddFont("Aileron-Regular", "AileronRegular");
            fonts.AddFont("Aileron-Semibol", "OpenSansRegular");
            fonts.AddMaterialSymbolsFonts();
        }

        // AddSingleton: Una sola instancia para la App. Debe durar. Generlamente a servicios
        // AddTransient: Cada vez que se solicita una instancia, se crea una nueva. Generalmente a Views y ViewModels
        public static MauiAppBuilder RegisterApp(this MauiAppBuilder mauiAppBuilder)
        {
            // TODO: Probar si se puede eliminar la siguiente línea.
            mauiAppBuilder.Services.AddSingleton<AppShell>();

            mauiAppBuilder.Services.AddSingleton<AppShellViewModel>();

            mauiAppBuilder.RegisterAppServices();

            return mauiAppBuilder;
        }

        public static MauiAppBuilder RegisterAppServices(this MauiAppBuilder mauiAppBuilder)
        {
            mauiAppBuilder.Services.AddSingleton<INavigationService, MauiNavigationService>();
            mauiAppBuilder.Services.AddSingleton<IDialogService, DialogService>();
            mauiAppBuilder.Services.AddSingleton<ISharedData, SharedData>();

            mauiAppBuilder.Services.AddSingleton<RestService>();
            mauiAppBuilder.Services.AddTransient<PdfService>();

            mauiAppBuilder.Services.AddSingleton<AppDbReposService>(
                serviceProvider =>
                {
                    var dataConfigRepo = serviceProvider.GetRequiredService<DataConfigRepo>();
                    var empleadosRepo = serviceProvider.GetRequiredService<EmpleadosRepo>();
                    var periodoRepo = serviceProvider.GetRequiredService<PeriodoRepo>();
                    var usuarioRepo = serviceProvider.GetRequiredService<UsuarioRepo>();

                    return new AppDbReposService(dataConfigRepo, empleadosRepo,
                        periodoRepo, usuarioRepo);
                }
            );
            return mauiAppBuilder;
        }

        public static MauiAppBuilder RegisterProduccionModule(this MauiAppBuilder mauiAppBuilder)
        {
            mauiAppBuilder.RegisterProduccionViews();
            mauiAppBuilder.RegisterProduccionViewModels();
            mauiAppBuilder.RegisterProduccionServices();

            return mauiAppBuilder;
        }

        public static MauiAppBuilder RegisterProduccionViews(this MauiAppBuilder mauiAppBuilder)
        {

            mauiAppBuilder.Services.AddTransient<LoadingPopUp>();
            mauiAppBuilder.Services.AddTransient<LoginPage>();
            mauiAppBuilder.Services.AddTransient<OrdenesPage>();
            mauiAppBuilder.Services.AddTransient<NotificarPtPage>();
            mauiAppBuilder.Services.AddTransient<ConfigPage>();
            mauiAppBuilder.Services.AddTransient<HomePage>();
            mauiAppBuilder.Services.AddTransient<NotificarPmPage>();
            mauiAppBuilder.Services.AddTransient<NotificarPtPdfPage>();
            mauiAppBuilder.Services.AddTransient<AprobarOrdenesPrdPage>();
            mauiAppBuilder.Services.AddTransient<AprobarOrdenesPrdInvPage>();

            return mauiAppBuilder;
        }

        public static MauiAppBuilder RegisterProduccionViewModels(this MauiAppBuilder mauiAppBuilder)
        {
            mauiAppBuilder.Services.AddTransient<LoadingPopUpViewModel>();
            mauiAppBuilder.Services.AddTransient<LoginViewModel>();
            mauiAppBuilder.Services.AddTransient<OrdenesViewModel>();
            mauiAppBuilder.Services.AddTransient<NotificarPtViewModel>();
            mauiAppBuilder.Services.AddTransient<ConfigViewModel>();
            mauiAppBuilder.Services.AddTransient<HomeViewModel>();
            mauiAppBuilder.Services.AddTransient<NotificarPmViewModel>();
            mauiAppBuilder.Services.AddTransient<AprobarOrdenesPrdViewModel>();
            mauiAppBuilder.Services.AddTransient<AprobarOrdenesPrdInvViewModel>();

            mauiAppBuilder.Services.AddTransient<NotificarPtPdfViewModel>();

            return mauiAppBuilder;
        }

        public static MauiAppBuilder RegisterProduccionServices(this MauiAppBuilder mauiAppBuilder)
        {

            mauiAppBuilder.Services.AddSingleton<PrdDbReposService>(
                serviceProvider =>
                {
                    var ordenProduccionRepo = serviceProvider.GetRequiredService<OrdenProduccionRepo>();
                    var ordenProduccionPtRepo = serviceProvider.GetRequiredService<OrdenProduccionPtRepo>();
                    var ordenProduccionMpRepo = serviceProvider.GetRequiredService<OrdenProduccionMpRepo>();
                    var ptNotificadoRepo = serviceProvider.GetRequiredService<PtNotificadoRepo>();
                    var mpNotificadoRepo = serviceProvider.GetRequiredService<MpNotificadoRepo>();

                    return new PrdDbReposService(ordenProduccionRepo,
                        ordenProduccionPtRepo, ordenProduccionMpRepo,
                        ptNotificadoRepo, mpNotificadoRepo);
                }
            );



            mauiAppBuilder.Services.AddSingleton<OrdenProduccionFilter>();

            mauiAppBuilder.Services.AddSingleton<OrdenProduccionMpFilter>();


            return mauiAppBuilder;
        }

        //public static void ConfigureMauiHandlers()
        //{

        //}
    }

}
