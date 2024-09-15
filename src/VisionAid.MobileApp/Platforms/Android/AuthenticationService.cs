using Microsoft.Identity.Client;

namespace VisionAid.MobileApp.Services;

public partial class AuthenticationService
{
    private partial Task RegisterMsalCacheAsync(ITokenCache tokenCache)
    {
        return Task.CompletedTask;
    }

    private partial PublicClientApplicationBuilder AddPlatformConfiguration(PublicClientApplicationBuilder builder)
    {
        return builder.WithParentActivityOrWindow(() => Platform.CurrentActivity);
    }
}
