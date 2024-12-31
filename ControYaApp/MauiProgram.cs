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
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                    fonts.AddFont("Aileron-Regular", "AileronRegular");
                    fonts.AddFont("Aileron-Semibol", "AileronSemibold");
                })
                .UseMauiCommunityToolkit()
                .RegisterViewModels();

            builder.Services.AddSingleton<DatabaseConnection>();

#if DEBUG
            builder.Logging.AddDebug();
#endif
            return builder.Build();
        }

        private static MauiAppBuilder RegisterViewModels(this MauiAppBuilder mauiAppBuilder)
        {
            _ = mauiAppBuilder.Services.AddTransient<LoginViewModel>();
            _ = mauiAppBuilder.Services.AddTransient<HomeViewModel>();

            return mauiAppBuilder;
        }
    }
}
