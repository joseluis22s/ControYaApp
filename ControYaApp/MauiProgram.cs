using CommunityToolkit.Maui;
using ControYaApp.Services.Database;
using ControYaApp.ViewModels;
using Microsoft.Extensions.Logging;

namespace ControYaApp
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            MauiAppBuilder builder = MauiApp.CreateBuilder();
            _ = builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                    fonts.AddFont("Aileron-Regular", "AileronRegular");
                    fonts.AddFont("Aileron-Semibol", "AileronSemibold");
                })
                .UseMauiCommunityToolkit()
                .RegistrarViewModels()
                .RegistrarServiciosDatabase();

#if DEBUG
            builder.Logging.AddDebug();
#endif
            return builder.Build();
        }

        private static MauiAppBuilder RegistrarViewModels(this MauiAppBuilder mauiAppBuilder)
        {
            _ = mauiAppBuilder.Services.AddTransient<LoginViewModel>();
            _ = mauiAppBuilder.Services.AddTransient<HomeViewModel>();

            return mauiAppBuilder;
        }

        private static MauiAppBuilder RegistrarServiciosDatabase(this MauiAppBuilder mauiAppBuilder)
        {
            _ = mauiAppBuilder.Services.AddSingleton<DatabaseConnection>();

            return mauiAppBuilder;
        }
    }
}
