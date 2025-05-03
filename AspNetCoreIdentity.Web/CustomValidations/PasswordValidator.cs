using AspNetCoreIdentity.Web.Models;
using Microsoft.AspNetCore.Identity;

namespace AspNetCoreIdentity.Web.CustomValidations
{
    public class PasswordValidator : IPasswordValidator<AppUser>
    {
        public Task<IdentityResult> ValidateAsync(UserManager<AppUser> manager, AppUser user, string? password)
        {
            var errors = new List<IdentityError>();

            //Ünlem işaretleri değerlerin null gelmeyeceğini derleyiciye belirtir, koyulmasa da olur
            if (password!.ToLower().Contains(user.UserName!.ToLower()))  
            {
                errors.Add(new IdentityError
                {
                    Code = "PasswordContainsUserName",
                    Description = "Şifre, kullanıcı adını içeremez."
                });
            }

            if (password!.Length < 6)
            {
                errors.Add(new IdentityError
                {
                    Code = "PasswordTooShort",
                    Description = "Şifre en az 6 karakter olmalıdır."
                });
            }

            if (!errors.Any())
            {
                return Task.FromResult(IdentityResult.Success);
            }
            else
            {
                return Task.FromResult(IdentityResult.Failed(errors.ToArray()));
            }
        }
    }
}
