using Microsoft.AspNetCore.Identity;

namespace AspNetCoreIdentity.Web.Localization
{
    public class LocalizationIdentityErrorDescriber :IdentityErrorDescriber
    {
        public override IdentityError DuplicateUserName(string role)
        {
            return new()
            {
                Code = nameof(DuplicateUserName),
                Description = $"\"{role}\" kullanıcı adı daha önce alınmış."
            };
        }

        public override IdentityError DuplicateEmail(string email)
        {
            return new()
            {
                Code = "DublicateEmail",
                Description = $"\"{email}\" email daha önce alınmış."
            };
        }
    }
}
