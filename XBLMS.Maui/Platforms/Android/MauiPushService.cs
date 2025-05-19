using Android.App;

namespace XBLMS.Maui
{
    [Service(Process = ":pushservice", Exported = false)]
    public class MauiPushService : Com.Igexin.Sdk.PushService
    {
    }
}
