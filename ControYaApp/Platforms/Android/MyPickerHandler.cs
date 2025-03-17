using Android.Graphics.Drawables;
using Microsoft.Maui.Controls.Platform;
using Microsoft.Maui.Handlers;
using Microsoft.Maui.Platform;

namespace ControYaApp.Platforms.Android
{
    public class MyPickerHandler : PickerHandler
    {
        protected override void ConnectHandler(MauiPicker platformView)
        {
            base.ConnectHandler(platformView);
            GradientDrawable gd = new();
            gd.SetColor(global::Android.Graphics.Color.Transparent);
            platformView.SetBackground(gd);

        }
    }
}
