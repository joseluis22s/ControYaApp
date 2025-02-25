using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Views;
using Android.Views.InputMethods;
using Android.Widget;

namespace ControYaApp
{
    [Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, LaunchMode = LaunchMode.SingleTop, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
    public class MainActivity : MauiAppCompatActivity
    {
        public override bool DispatchTouchEvent(MotionEvent? e)
        {
            if (e!.Action == MotionEventActions.Down)
            {
                var focusedElement = CurrentFocus;
                if (focusedElement is EditText editText)
                {
                    var editTextLocation = new int[2];
                    editText.GetLocationOnScreen(editTextLocation);
                    var clearTextButtonWidth = 100; // syncfusion clear button at the end of the control
                    var editTextRect = new Rect(editTextLocation[0], editTextLocation[1], editText.Width + clearTextButtonWidth, editText.Height);
                    //var editTextRect = editText.GetGlobalVisibleRect(editTextRect);  //not working in MAUI, always returns 0,0,0,0
                    var touchPosX = (int)e.RawX;
                    var touchPosY = (int)e.RawY;
                    if (!editTextRect.Contains(touchPosX, touchPosY))
                    {
                        editText.ClearFocus();
                        var inputService = GetSystemService(Context.InputMethodService) as InputMethodManager;
                        inputService?.HideSoftInputFromWindow(editText.WindowToken, 0);
                    }
                }
            }
            return base.DispatchTouchEvent(e);
        }
    }
}
