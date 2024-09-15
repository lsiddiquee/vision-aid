namespace VisionAid.MobileApp;

internal static class Configuration
{
    public const string ApiUrl = "https://app-service-vision-aid.azurewebsites.net/";
    public const string AzureAdClientId = "032004cb-0ab1-45fd-88d8-64f24f2d7889";
    public static readonly string AzureAdRedirectUri = $"msal{AzureAdClientId}://auth";
    public const string VisionAidApiScope = "api://032004cb-0ab1-45fd-88d8-64f24f2d7889/access_as_user";
}
