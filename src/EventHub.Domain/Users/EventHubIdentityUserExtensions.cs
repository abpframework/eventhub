using Volo.Abp.Identity;

namespace EventHub.Users
{
    public static class EventHubIdentityUserExtensions
    {
        public static string GetFullNameOrUsername(this IdentityUser user)
        {
            if (!string.IsNullOrWhiteSpace(user.Name) && !string.IsNullOrWhiteSpace(user.Surname))
            {
                return user.Name + " " + user.Surname;
            }

            if (!string.IsNullOrWhiteSpace(user.Name))
            {
                return user.Name;
            }

            return user.UserName;
        }
    }
}