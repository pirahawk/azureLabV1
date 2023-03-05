using AzureLabV1.Dapr.Common;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddActors(services =>
{
    services.Actors.RegisterActor<OrderActor>();
});
builder.Services.AddControllers()
    .AddDapr();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.MapActorsHandlers();



app.Run();
