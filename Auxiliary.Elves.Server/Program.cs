using Auxiliary.Elves.Api.ApiService;
using Auxiliary.Elves.Api.IApiService;
using Auxiliary.Elves.Domain;
using Auxiliary.Elves.Infrastructure.Config;
using Auxiliary.Elves.Server.Exceptions;
using Microsoft.EntityFrameworkCore;
using NLog.Web;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString(SystemConstant.DefaultConnection);

builder.Services.AddScoped<ILoginApiService, LoginApiService>();

// Add services to the container.
builder.Services.AddHttpContextAccessor();

builder.Services.AddDbContext<AuxiliaryDbContext>((options) =>
{
    options.UseMySql(connectionString, new MySqlServerVersion(new Version(8, 0, 36)));
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpClient();

builder.Services.AddMemoryCache();
builder.Host.UseNLog();

builder.WebHost.UseUrls($"http://0.0.0.0:{GetPort()}");
var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseException();
app.MapControllers();

app.Run();


string GetPort()
{
    var configBuilder = new ConfigurationBuilder()
   .SetBasePath(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location))
   .AddJsonFile(SystemConstant.HostFileName)
   .Build();
    var port = configBuilder.GetSection(SystemConstant.HostPort).Value;
    return port;
}