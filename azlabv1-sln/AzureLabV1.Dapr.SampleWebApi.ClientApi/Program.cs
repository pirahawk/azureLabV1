using Dapr.Actors.Client;
using Microsoft.IdentityModel.Logging;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddUserSecrets(Assembly.GetExecutingAssembly(), true);
builder.Configuration.AddEnvironmentVariables();

IdentityModelEventSource.ShowPII = true;

// Add services to the container.

builder.Services
    .AddControllers()
    .AddDapr();

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

builder.Services.AddTransient<IActorProxyFactory>(serviceProvider =>
{
    var logger = serviceProvider.GetService<ILogger<IActorProxyFactory>>();
    var configurationRoot = serviceProvider.GetService<IConfiguration>();
    // Note: For the Actors Proxy to work, you need to know the URL (mainly the Dapr Sidecar PORT number)
    // of the target Dapr Sidecar that hosts the app that contains the Actors defined within.
    // You must always point to the Dapr sidecar in the url, not the Apps hosted url:port.

    var daprActorUrl = configurationRoot?.GetValue<string?>("DAPR_ACTOR_URL");

    if (string.IsNullOrWhiteSpace(daprActorUrl))
    {
        var message = $"IActorProxyFactoryInvocation: Unable to bind to configuration setting: DAPR_ACTOR_URL";
        logger?.LogError(message);
        throw new ArgumentException(message);
    }

    logger?.LogInformation($"IActorProxyFactoryInvocation: Actor proxy URL created for {daprActorUrl}");
    var proxyOptions = new ActorProxyOptions
    {
        HttpEndpoint = daprActorUrl,
    };

    return new ActorProxyFactory(proxyOptions);
});

builder.Services.AddActors(configure =>
{


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

