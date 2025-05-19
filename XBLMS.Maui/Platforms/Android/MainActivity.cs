using Android.App;
using Android.Content.PM;
using Android.OS;
using Com.Igexin.Sdk;

namespace XBLMS.Maui
{
    [Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, LaunchMode = LaunchMode.SingleTop, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
    public class MainActivity : MauiAppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            PushManager.Instance?.PreInit(this);
            PushManager.Instance?.Initialize(this);
            var cid = PushManager.Instance?.GetClientid(this);
            System.Diagnostics.Debug.WriteLine($"cid:{cid}");
        }
    }
}
