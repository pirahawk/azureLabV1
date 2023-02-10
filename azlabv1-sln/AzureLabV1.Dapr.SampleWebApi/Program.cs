using Dapr;
using Microsoft.IdentityModel.Logging;
using System.Reflection;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddUserSecrets(Assembly.GetExecutingAssembly(), true);
builder.Configuration.AddEnvironmentVariables();

IdentityModelEventSource.ShowPII = true;

// Add services to the container.

builder.Services.AddControllers()
    .AddDapr(); // NEED THIS FOR DAPR to bind to Controller methods


// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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

//app.MapPost("/orders", [Topic("orderpubsub", "orders")] (Order order) => {
//    Console.WriteLine("Subscriber received : " + order);
//    return Results.Ok(order);
//});

app.Run();