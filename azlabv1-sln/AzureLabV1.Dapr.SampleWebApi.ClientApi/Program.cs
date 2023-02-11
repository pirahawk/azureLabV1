using AzureLabV1.Dapr.SampleWebApi.ClientApi.Controllers;
using Microsoft.IdentityModel.Logging;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddUserSecrets(Assembly.GetExecutingAssembly(), true);
builder.Configuration.AddEnvironmentVariables();

IdentityModelEventSource.ShowPII = true;

// Add services to the container.

builder.Services.AddControllers().AddDapr();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpClient("daprServiceInvocation", (serviceProvider, httpClient) =>
{
    var logger = serviceProvider.GetService<ILogger<IHttpClientBuilder>>();
    var configurationRoot = serviceProvider.GetService<IConfiguration>();
    var daprServicePort = configurationRoot?.GetValue<int?>("DAPR_HTTP_PORT");
    var daprAppId = configurationRoot?.GetValue<string?>("DAPR_INVOKE_TARGET_APP_ID");

    if (!daprServicePort.HasValue || string.IsNullOrWhiteSpace(daprAppId))
    {
        logger?.LogError($"daprServiceInvocation: Unable to bind to configuration setting: DAPR_HTTP_PORT");
        return;
    }

    var daprInvokeUrl = $"http://localhost:{daprServicePort}";

    logger?.LogInformation($"daprServiceInvocation: created url: {daprInvokeUrl} for dapr app id: {daprAppId}");

    httpClient.BaseAddress = new Uri(daprInvokeUrl);
    httpClient.DefaultRequestHeaders.Add("dapr-app-id", daprAppId);
    httpClient.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

});

var app = builder.Build();

// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
app.UseSwagger();
app.UseSwaggerUI();
//}

//app.UseHttpsRedirection();

//app.UseAuthorization();

// Dapr will send serialized event object vs. being raw CloudEvent
app.UseCloudEvents();

// needed for Dapr pub/sub routing
app.MapSubscribeHandler();

app.MapControllers();

app.Run();

