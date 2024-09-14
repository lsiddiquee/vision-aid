# How to setup the configurations

```console
dotnet user-secrets init
dotnet user-secrets set "AzureOpenAI:ChatDeploymentName" "gpt-4"
dotnet user-secrets set "AzureOpenAI:Endpoint" "https://YOUR_ENDPOINT.openai.azure.com/"
dotnet user-secrets set "AzureOpenAI:ApiKey" "YOUR_API_KEY"
```
