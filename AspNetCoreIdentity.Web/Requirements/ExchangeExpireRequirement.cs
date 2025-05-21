using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace AspNetCoreIdentity.Web.Requirements
{
    public class ExchangeExpireRequirement : IAuthorizationRequirement
    {
    }

    public class ExchangeExpireRequirementHandler : AuthorizationHandler<ExchangeExpireRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, ExchangeExpireRequirement requirement)
        {
            // Kullanıcıda ExchangeExpireDate claim'i var mı?
            if (!context.User.HasClaim(x => x.Type == "ExchangeExpireDate"))
            {
                // Eğer yoksa yetkilendirme başarısız.
                context.Fail();
                return Task.CompletedTask;
            }

            // Claim'i alıyoruz.
            Claim exchangeExpireDate = context.User.FindFirst("ExchangeExpireDate")!;

            // Claim'in değeri DateTime türüne çevriliyor.
            if (DateTime.Now > Convert.ToDateTime(exchangeExpireDate.Value))
            {
                // Eğer claim'in süresi dolmuşsa, yetkilendirme başarısız.
                context.Fail();
                return Task.CompletedTask;
            }

            // Eğer claim'in süresi dolmadıysa, yetkilendirme başarılı.
            context.Succeed(requirement);
            return Task.CompletedTask;
        }
    }
}
