using Android.App;
using Android.Runtime;

namespace ControYaApp
{
    [Application]
    public class MainApplication : MauiApplication
    {
        public MainApplication(IntPtr handle, JniHandleOwnership ownership)
            : base(handle, ownership)
        {
        }

        protected override MauiApp CreateMauiApp()
        {
            Microsoft.Maui.Handlers.EntryHandler.Mapper.AppendToMapping(nameof(Entry), (handler, view) =>
            {
                handler.PlatformView.SetBackgroundColor(Android.Graphics.Color.Transparent);
            });
            return MauiProgram.CreateMauiApp();
        }
    }
}
