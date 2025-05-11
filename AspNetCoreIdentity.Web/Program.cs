using AspNetCoreIdentity.Web.Extensions;
using AspNetCoreIdentity.Web.Models;
using AspNetCoreIdentity.Web.OptionModels;
using AspNetCoreIdentity.Web.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using System;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<AppDbcontext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")
    )
);

//Email datalarýný settings jsondan alýyoruz.
builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));

builder.Services.AddScoped<IEmailService, EmailService>();

builder.Services.Configure<SecurityStampValidatorOptions>(opt => { 
    opt.ValidationInterval = TimeSpan.FromMinutes(30); //30 dakikada bir security stamp kontrolü yapar. Varsayýlan deðerde 30 dk dýr.
});

//FÝLE PROVÝDER
builder.Services.AddSingleton<IFileProvider>(new PhysicalFileProvider(Directory.GetCurrentDirectory()));

builder.Services.AddIdentityWithIndex();

builder.Services.ConfigureApplicationCookie(opt =>
{
    var cookieBuilder = new CookieBuilder();

    cookieBuilder.Name = "FkAppCookie";
    
    opt.LoginPath = new PathString("/Home/SignIn"); 
    opt.LogoutPath = new PathString("/Member/Logout");
    opt.Cookie = cookieBuilder;
    opt.ExpireTimeSpan = TimeSpan.FromDays(30);  //Cookie süresi 30 gün
    opt.SlidingExpiration = true;                //Het giriþ yapýldýðýnda 30 günün tazelenmesine yarar

});



var app = builder.Build();

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

app.UseAuthentication();
app.UseAuthorization();

//Area için eriþim tanýmlamasý
app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
