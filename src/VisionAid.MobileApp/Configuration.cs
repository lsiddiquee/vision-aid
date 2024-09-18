namespace VisionAid.MobileApp;

internal static class Configuration
{
    public const string ApiUrl = "https://app-service-vision-aid.azurewebsites.net/";
    public const string AzureAdTenantId = "14bea8dc-84c9-40e7-85b6-9983e122c43d";
    public const string AzureAdClientId = "432b6202-439d-48a2-b714-2d2becc9af9c";
    public static readonly string AzureAdRedirectUri = $"msal{AzureAdClientId}://auth";
    public static readonly string VisionAidApiScope = $"api://{AzureAdClientId}/access_as_user";
    public const int Image_Buffer_Size = 5;
}
