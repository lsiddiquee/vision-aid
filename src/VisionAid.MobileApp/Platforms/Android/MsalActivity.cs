using Android.App;
using Android.Content;
using Microsoft.Identity.Client;

namespace VisionAid.MobileApp.Platforms.Android;

[Activity(Exported = true)]
[IntentFilter(new[] { Intent.ActionView },
    Categories = new[] { Intent.CategoryBrowsable, Intent.CategoryDefault },
    DataHost = "auth",
    DataScheme = "msal432b6202-439d-48a2-b714-2d2becc9af9c")]
public class MsalActivity : BrowserTabActivity
{
}