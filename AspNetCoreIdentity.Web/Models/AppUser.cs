using Microsoft.AspNetCore.Identity;

namespace AspNetCoreIdentity.Web.Models
{
    public class AppUser : IdentityUser
    {
		//Kullanıcı hakkında tutmak istediğimiz bilgileri ekleyebiliriz

		public string? City { get; set; }
		public string? Picture { get; set; }
		public DateTime? BirthDate { get; set; }
		public Gender? Gender { get; set; }
	}
}
