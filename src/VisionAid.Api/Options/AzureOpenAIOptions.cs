namespace VisionAid.Api.Options
{
    public class AzureOpenAIOptions
    {
        public required string ChatDeploymentName { get; set; }
        public required string Endpoint { get; set; }
        public required string ApiKey { get; set; }
    }
}
