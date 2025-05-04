using AspNetCoreIdentity.Web.CustomValidations;
using AspNetCoreIdentity.Web.Localization;
using AspNetCoreIdentity.Web.Models;
using Microsoft.AspNetCore.Identity;

namespace AspNetCoreIdentity.Web.Extensions
{
    public static class StartupExtensions
    {
        public static void AddIdentityWithIndex(this IServiceCollection services)
        {
            services.Configure<DataProtectionTokenProviderOptions>(opt =>
            {
                opt.TokenLifespan = TimeSpan.FromHours(2); //Token süresi 2 saat
            });

            services.AddIdentity<AppUser, AppRole>(options =>
            {
                options.User.RequireUniqueEmail = true;  // E-posta adresinin benzersiz olmasını zorunlu kıl
                options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._";

                options.Password.RequiredLength = 6;

                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = false;

                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireDigit = false;

                options.Lockout.MaxFailedAccessAttempts = 3; // 3 kez yanlış giriş yapıldığında kilitlenecek
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(3); // 3 dakika boyunca kilitli kalacak

            }).AddPasswordValidator<PasswordValidator>()  //Özel şifre doğrulayıcıyı 
              .AddUserValidator<UserValidator>()          //Özel kullanıcı doğrulayıcıyı
              .AddErrorDescriber<LocalizationIdentityErrorDescriber>() //Hata mesajlarını özelleştirir
              .AddDefaultTokenProviders()                 //Token oluşturma işlemleri için gerekli olan default providerları ekler
              .AddEntityFrameworkStores<AppDbcontext>();
        }
    }
}
