using AllDemo.Configuration;
using AllDemo.Data;
using AllDemo.Helper.Log;
using AllDemo.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ErrorLog>();
builder.Services.AddScoped<OTPService>();
builder.Services.AddScoped<EmailService>();

//Configure EF Core with SQL server
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DBCS")));

builder.Services.AddSession();//added session services to the application
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(options => //here the cookies services are added
{
    options.LoginPath = "/Account/Login";
    options.AccessDeniedPath = "/Account/AccessDenied";
    options.ExpireTimeSpan = TimeSpan.FromHours(1);
    options.SlidingExpiration = true;
});
builder.Services.AddAuthorization();//Authorization services are added
builder.Services.Configure<OTPSetting>(builder.Configuration.GetSection("OtpSettings"));//this is used because of get the otp digit number from the appsettings.json file
builder.Services.Configure<MailSetting>(builder.Configuration.GetSection("EmailSettings"));//EmailSettings: this is appsettings configuration name, and this is to get the value from the appsetting.json file and store in the  MailSetting class

var app = builder.Build();

app.UseSession();//by use this we are able to add session in out application
app.UseAuthentication();//authentication are added
app.UseAuthorization();//authorization are added

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}");

app.Run();
