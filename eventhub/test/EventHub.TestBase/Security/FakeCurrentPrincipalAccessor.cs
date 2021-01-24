using System.Collections.Generic;
using System.Security.Claims;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Security.Claims;

namespace EventHub.Security
{
    [Dependency(ReplaceServices = true)]
    public class FakeCurrentPrincipalAccessor : ThreadCurrentPrincipalAccessor
    {
        private readonly EventHubTestData _eventHubTestData;

        public FakeCurrentPrincipalAccessor(EventHubTestData eventHubTestData)
        {
            _eventHubTestData = eventHubTestData;
        }

        protected override ClaimsPrincipal GetClaimsPrincipal()
        {
            return GetPrincipal();
        }

        private ClaimsPrincipal GetPrincipal()
        {
            return new ClaimsPrincipal(
                new ClaimsIdentity(
                    new List<Claim>
                    {
                        new Claim(AbpClaimTypes.UserId,_eventHubTestData.UserAdminId.ToString()),
                        new Claim(AbpClaimTypes.UserName,_eventHubTestData.UserAdminUserName),
                        new Claim(AbpClaimTypes.Email,"admin@abp.io")
                    }
                )
            );
        }
    }
}
