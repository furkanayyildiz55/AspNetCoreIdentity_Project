using AspNetCoreIdentity.Web.Models;
using Microsoft.AspNetCore.Identity;

namespace AspNetCoreIdentity.Web.CustomValidations
{
    public class UserValidator : IUserValidator<AppUser>
    {
        public Task<IdentityResult> ValidateAsync(UserManager<AppUser> manager, AppUser user)
        {
            var errors = new List<IdentityError>();

            var isNumaric = int.TryParse(user.UserName, out _);

            if (isNumaric)
            {
                errors.Add(new IdentityError
                {
                    Code = "UserNameIsNumeric",
                    Description = "Kullanıcı adı sayılardan oluşamaz."
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
