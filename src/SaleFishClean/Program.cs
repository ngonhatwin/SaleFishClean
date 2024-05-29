using Arch.EntityFrameworkCore.UnitOfWork;
using Contract.Helper;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SaleFishClean.Application;
using SaleFishClean.Infrastructure;
using SaleFishClean.Infrastructure.Data;
using SaleFishClean.Web;
using SaleFishClean.Web.BackgroundServices;
using SaleFishClean.Web.Middleware;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
#region BackgroundServices
builder.Services.AddHostedService<DeleteExpiredOrderService>();
#endregion
builder.Services.AddControllersWithViews();
builder.Services.AddSignalR();
builder.Services.AddLogging();
builder.Services.AddDbContext<SaleFishProjectContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("MyDb")))
                .AddUnitOfWork<SaleFishProjectContext>();
builder.Services.AddInfrastructureService();
builder.Services.AddApplicationServices();
builder.Services.ConfigureRedis(builder.Configuration);
builder.Services.Configure<AppSettings>(builder.Configuration.GetSection("AppSettings"));
builder.Services.Configure<MailSettings>(builder.Configuration.GetSection("MailSettings"));
var secretKey = builder.Configuration["AppSettings:SecretKey"];
var secretKeyBytes = Encoding.ASCII.GetBytes(secretKey!);
builder.Services.AddAuthentication()
    .AddJwtBearer(opt =>
    {
        opt.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(secretKeyBytes),
            ClockSkew = TimeSpan.Zero,
        };
    });
builder.Services.AddHttpContextAccessor();
builder.Services.AddSession();
builder.Services.AddAuthorization();
//Đăng ký dịch vụ quản lý các controller trong ứng dụng.
//Middleware này giúp xác định và quản lý các controller trong ứng dụng ASP.NET Core.
builder.Services.AddControllers();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}

app.UseStaticFiles();
app.UseRouting();
app.UseSession();
app.UseMiddleware<CheckTokenMiddleware>();
app.UseMiddleware<AddTokenToHeaderMiddleware>();
app.UseAuthentication();
app.UseAuthorization();
app.MapHub<ChatHub>("/chatHub");
app.MapHub<CommentHub>("/commentHub");
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=WebApp}/{action=Index}/{id?}");

app.Run();
