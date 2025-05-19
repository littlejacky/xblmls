namespace XBLMS.Maui
{
    public partial class MainPage : ContentPage
    {
        int count = 0;

        public MainPage()
        {
            InitializeComponent();
        }

        private void WebView_Navigated(object sender, WebNavigatedEventArgs e)
        {
#if ANDROID
            Com.Igexin.Sdk.PushManager.Instance?.Initialize(Platform.CurrentActivity);
            var cid = Com.Igexin.Sdk.PushManager.Instance?.GetClientid(Platform.CurrentActivity);
#endif
        }
    }

}
