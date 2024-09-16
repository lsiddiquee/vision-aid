namespace VisionAid.MobileApp;

internal static class Configuration
{
    public const string ApiUrl = "https://app-service-vision-aid.azurewebsites.net/";
    public const string AzureAdTenantId = "16b3c013-d300-468d-ac64-7eda0820b6d3";
    public const string AzureAdClientId = "032004cb-0ab1-45fd-88d8-64f24f2d7889";
    public static readonly string AzureAdRedirectUri = $"msal{AzureAdClientId}://auth";
    public const string VisionAidApiScope = "api://032004cb-0ab1-45fd-88d8-64f24f2d7889/access_as_user";
}
