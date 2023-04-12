using BaseSource.API;
using BaseSource.API.Extensions;
using BaseSource.API.Helpers;
using BaseSource.API.Services.SignalRService;
using BaseSource.API.Singletons;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSignalR().AddJsonProtocol(configure => configure.PayloadSerializerOptions.NumberHandling = JsonNumberHandling.AllowNamedFloatingPointLiterals);
builder.Services.AddSingleton<IBotDataCache, BotDataCache>();
builder.Services.AddTransient<IRenderArgumentGenerateHelper, RenderArgumentGenerateHelper>();
builder.AddTelegram();




var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthorization();

app.MapControllers();

app.UseEndpoints(endpoints =>
{
    endpoints.MapHub<WebHub>($"/WebHub");
    endpoints.MapHub<BotHub>("/BotHub");
});

app.Run();
