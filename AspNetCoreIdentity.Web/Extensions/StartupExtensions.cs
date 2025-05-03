using AspNetCoreIdentity.Web.CustomValidations;
using AspNetCoreIdentity.Web.Localization;
using AspNetCoreIdentity.Web.Models;

namespace AspNetCoreIdentity.Web.Extensions
{
    public static class StartupExtensions
    {
        public static void AddIdentityWithIndex(this IServiceCollection services)
        {
            services.AddIdentity<AppUser, AppRole>(options =>
            {
                options.User.RequireUniqueEmail = true;  // E-posta adresinin benzersiz olmasını zorunlu kıl
                options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._";

                options.Password.RequiredLength = 6;

                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = false;

                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireDigit = false;

            }).AddPasswordValidator<PasswordValidator>()  //Özel şifre doğrulayıcıyı 
              .AddUserValidator<UserValidator>()          //Özel kullanıcı doğrulayıcıyı
              .AddErrorDescriber<LocalizationIdentityErrorDescriber>() //Hata mesajlarını özelleştirir
              .AddEntityFrameworkStores<AppDbcontext>();
        }
    }
}
