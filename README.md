# How to setup the configurations

```console
cd src/VisionAid.Api
dotnet user-secrets init
dotnet user-secrets set "AzureOpenAI:ChatDeploymentName" "gpt-4"
dotnet user-secrets set "AzureOpenAI:Endpoint" "https://YOUR_ENDPOINT.openai.azure.com/"
dotnet user-secrets set "AzureOpenAI:ApiKey" "YOUR_API_KEY"
dotnet user-secrets set "AzureMaps:SubscriptionKey" "Your Map Subscription key"
```
