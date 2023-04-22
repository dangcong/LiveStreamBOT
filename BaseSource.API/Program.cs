using BaseSource.API;
using BaseSource.API.Extensions;
using System.Text.Json.Serialization;

using BaseSouce.Data.EF;
using BaseSouce.Data.Entities;
using BaseSource.Shared.Constants;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using BaseSource.API.Cofigurations;
using EFCore.UnitOfWork;
using BaseSource.Shared.Helpers;
using BaseSouce.Services.HubServices;
using BaseSouce.Services.BackgroundServices;

var builder = WebApplication.CreateBuilder(args);

//add logging
builder.Host.ConfigureLogging(logging =>
{
    logging.ClearProviders();
    logging.AddFilter("Microsoft.EntityFrameworkCore", LogLevel.Warning);
    logging.AddLog4Net();
    // logging.AddFilter("Microsoft.EntityFrameworkCore", LogLevel.Information);

});

//add db context and unitOfwork
builder.Services.AddDbContext<BaseSourceDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString(SystemConstants.MainConnectionString)))
                .AddUnitOfWork<BaseSourceDbContext>();

builder.Services.AddIdentity<AppUser, AppRole>()
                .AddEntityFrameworkStores<BaseSourceDbContext>()
                .AddDefaultTokenProviders();

builder.Services.Configure<IdentityOptions>(options =>
{
    options.Password.RequireDigit = false;
    options.Password.RequiredLength = 6;
    options.Password.RequireLowercase = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireNonAlphanumeric = false;
});
//AddAuthorization
builder.Services.AuthorizationPolyConfiguration();
//Declare DI
builder.Services.DIConfiguration();



// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddHostedService<CheckConnectBotService>();
//add autoMapper
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
// Register the Swagger generator, defining 1 or more Swagger documents
builder.Services.SwaggerConfiguration(builder.Configuration);


builder.Services.AddSignalR().AddJsonProtocol(configure => configure.PayloadSerializerOptions.NumberHandling = JsonNumberHandling.AllowNamedFloatingPointLiterals);

builder.AddTelegram();




var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseAuthentication();
app.UseRouting();

app.UseAuthorization();

app.MapControllers();

app.UseEndpoints(endpoints =>
{
    endpoints.MapHub<WebHub>($"/WebHub");
    endpoints.MapHub<BotHub>("/BotHub");
});

app.Run();
