using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.AzureOpenAI;
using Microsoft.Extensions.Options;
using VisionAid.Api.Options;
using VisionAid.Api.Services;
using Azure.Identity;

namespace VisionAid.Api;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Configuration.AddAzureKeyVault(
            new Uri($"https://{builder.Configuration["KeyVaultName"]}.vault.azure.net/"),
            new DefaultAzureCredential()
        );

        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        builder.Services.AddOptions<AzureOpenAIOptions>()
            .Bind(builder.Configuration.GetSection(nameof(AzureOpenAIOptions)))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        builder.Services.AddSingleton<IChatCompletionService>(sp =>
        {
            AzureOpenAIOptions options = sp.GetRequiredService<IOptions<AzureOpenAIOptions>>().Value;

            return new AzureOpenAIChatCompletionService(options.ChatDeploymentName, options.Endpoint, options.ApiKey);
        });

        builder.Services.AddTransient<ChatService>();
        
        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

        app.UseAuthorization();


        app.MapControllers();

        app.Run();
    }
}
