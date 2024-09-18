using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.AzureOpenAI;
using Microsoft.Extensions.Options;
using VisionAid.Api.Options;
using VisionAid.Api.Services;
using Azure.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Identity.Web;
using Microsoft.Extensions.Configuration;
using Microsoft.OpenApi.Models;
using VisionAid.Api.Filters;

namespace VisionAid.Api;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        builder.Services.AddCors();

        builder.Configuration.AddAzureKeyVault(
            new Uri($"https://{builder.Configuration["KeyVaultName"]}.vault.azure.net/"),
            new DefaultAzureCredential()
        );

        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(c =>
        {
            c.UseInlineDefinitionsForEnums();

            c.SwaggerDoc("v1", new OpenApiInfo { Title = "VisionAid API", Version = "v1" });

            // Define the OAuth2.0 scheme that's in use (i.e. Implicit Flow)
            c.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
            {
                Type = SecuritySchemeType.OAuth2,
                Flows = new OpenApiOAuthFlows
                {
                    Implicit = new OpenApiOAuthFlow
                    {
                        AuthorizationUrl = new Uri($"{builder.Configuration["AzureAd:Instance"]}{builder.Configuration["AzureAd:TenantId"]}/oauth2/v2.0/authorize"),
                        TokenUrl = new Uri($"{builder.Configuration["AzureAd:Instance"]}{builder.Configuration["AzureAd:TenantId"]}/oauth2/v2.0/token"),
                        Scopes = new Dictionary<string, string>
                        {
                            { builder.Configuration["AzureAd:Scope"]!, "Access API as a user" }
                        }
                    }
                }
            });

            c.OperationFilter<AuthorizeCheckOperationFilter>();
        });
        builder.Services.AddOptions<AzureOpenAIOptions>()
             .Bind(builder.Configuration.GetSection(nameof(AzureOpenAIOptions)))
             .ValidateDataAnnotations()
             .ValidateOnStart();

        builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddMicrosoftIdentityWebApi(builder.Configuration.GetSection("AzureAd"));

        builder.Services.AddSingleton<IChatCompletionService>(sp =>
        {
            AzureOpenAIOptions options = sp.GetRequiredService<IOptions<AzureOpenAIOptions>>().Value;

            return new AzureOpenAIChatCompletionService(options.ChatDeploymentName, options.Endpoint, options.ApiKey);
        });

        builder.Services.AddTransient<ChatService>();
        
        var app = builder.Build();

        // Configure the HTTP request pipeline.
        //if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "VisionAid API V1");

                c.OAuthClientId(builder.Configuration["AzureAd:ClientId"]);
                c.OAuthUsePkce();
            });
        }

        app.UseHttpsRedirection();

        app.UseAuthentication();
        app.UseAuthorization();

        // disable CORS
        app.UseCors(builder => builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());

        app.MapControllers();

        app.Run();
    }
}
