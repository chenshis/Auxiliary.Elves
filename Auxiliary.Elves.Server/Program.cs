using Auxiliary.Elves.Api.ApiService;
using Auxiliary.Elves.Api.IApiService;
using Auxiliary.Elves.Domain;
using Auxiliary.Elves.Infrastructure.Config;
using Auxiliary.Elves.Server.Exceptions;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using NLog.Web;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString(SystemConstant.DefaultConnection);

// ��ȡ����Ŀ¼
var baseDic = AppDomain.CurrentDomain.BaseDirectory;
var videoDirectory = Path.Combine(baseDic, "videos");
if (!Directory.Exists(videoDirectory))
{
    Directory.CreateDirectory(videoDirectory);
}

builder.Services.AddScoped<ILoginApiService, LoginApiService>();
builder.Services.AddScoped<IPointsApiService, PointsApiService>();
builder.Services.AddScoped<IAnnouncementApiService, AnnouncementApiService>();
builder.Services.AddScoped<ISystemSettingApiService, SystemSettingApiService>();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy
            .AllowAnyOrigin()   // ����������Դ
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});


builder.WebHost.ConfigureKestrel(options =>
{
    options.Limits.MaxRequestBodySize = 314572800; // 300 MB
});

// Add services to the container.
builder.Services.AddHttpContextAccessor();

builder.Services.AddDbContext<AuxiliaryDbContext>((options) =>
{
    options.UseMySql(connectionString, new MySqlServerVersion(new Version(8, 0, 36)));
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    // ��ȡ��Ŀ�� xml ע���ļ�·��
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);

    // ����ע��
    c.IncludeXmlComments(xmlPath, includeControllerXmlComments: true);
});
builder.Services.AddHttpClient();

builder.Services.AddMemoryCache();
builder.Host.UseNLog();

builder.WebHost.UseUrls($"http://0.0.0.0:{GetPort()}");
var app = builder.Build();

// ���þ�̬�ļ�����
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(videoDirectory),
    RequestPath = "",
    ServeUnknownFileTypes = true,
    OnPrepareResponse = ctx =>
    {
        var fileExtension = Path.GetExtension(ctx.File.Name).ToLower();
        ctx.Context.Response.Headers["Content-Type"] = GetMimeType(fileExtension);
        ctx.Context.Response.Headers["Access-Control-Allow-Origin"] = "*";
        ctx.Context.Response.Headers["Cache-Control"] = "public, max-age=3600";
    }
});

// ��ѡ����Ŀ¼���ʷ�����ʾ
app.MapGet("/", () => "Video server is running...");

app.UseCors("AllowAll");  // ���� CORS

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


 string GetMimeType(string fileExtension)
{
    return fileExtension.ToLower() switch
    {
        ".mp4" => "video/mp4",
        ".webm" => "video/webm",
        ".ogg" => "video/ogg",
        ".avi" => "video/x-msvideo",
        ".mov" => "video/quicktime",
        ".wmv" => "video/x-ms-wmv",
        ".mkv" => "video/x-matroska",
        _ => "application/octet-stream"
    };
}