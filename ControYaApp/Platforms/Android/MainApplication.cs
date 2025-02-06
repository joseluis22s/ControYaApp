using Android.App;
using Android.Content.Res;
using Android.Runtime;

namespace ControYaApp
{
    // TODO: VErificar que el servidor soporte SSL/HTTPS y verificar esto al implementar la aplicacion
    //#if DEBUG                                  
    //    [Application(UsesCleartextTraffic = true)]
    //#else
    //    [Application]
    //#endif
    [Application(UsesCleartextTraffic = true)]
    public class MainApplication : MauiApplication
    {
        public MainApplication(IntPtr handle, JniHandleOwnership ownership)
            : base(handle, ownership)
        {
            Microsoft.Maui.Handlers.EntryHandler.Mapper.AppendToMapping(nameof(Entry), (handler, view) =>
            {
                if (view is Entry)
                {
                    // Remove underline
                    handler.PlatformView.BackgroundTintList = ColorStateList.ValueOf(Android.Graphics.Color.Transparent);
                }
            });

            Microsoft.Maui.Handlers.PickerHandler.Mapper.AppendToMapping(nameof(Picker), (handler, view) =>
            {
                if (view is Picker)
                {
                    // Remove underline
                    handler.PlatformView.BackgroundTintList = ColorStateList.ValueOf(Android.Graphics.Color.Transparent);
                }
            });


            Microsoft.Maui.Handlers.DatePickerHandler.Mapper.AppendToMapping(nameof(DatePicker), (handler, view) =>
            {
                if (view is DatePicker)
                {
                    // Remove underline
                    handler.PlatformView.BackgroundTintList = ColorStateList.ValueOf(Android.Graphics.Color.Transparent);
                }
            });

        }

        protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();
    }
}
