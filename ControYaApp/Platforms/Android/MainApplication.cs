using Android.App;
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

        }

        protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();
    }
}
