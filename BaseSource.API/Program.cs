using BaseSouce.Data.EF;
using BaseSouce.Data.Entities;
using BaseSource.Shared.Constants;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using BaseSource.API.Cofigurations;
using EFCore.UnitOfWork;

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

//add autoMapper
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

// Add services to the container.

builder.Services.AddControllers();
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

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseAuthentication();
app.UseRouting();

app.UseAuthorization();

app.MapControllers();

app.Run();
