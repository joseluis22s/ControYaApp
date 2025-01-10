using CommunityToolkit.Maui;
using ControYaApp.ViewModels;
using ControYaApp.Views;
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
                }).UseMauiCommunityToolkit();

            builder.Services.AddSingleton<LoginPage>();
            builder.Services.AddSingleton<LoginViewModel>();
            builder.Services.AddSingleton<HomePage>();
            builder.Services.AddSingleton<HomeViewModel>();
#if DEBUG
            builder.Logging.AddDebug();
#endif
            return builder.Build();
        }
    }
}
