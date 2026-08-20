using JwtDayMusic.WebApi.Enums;

namespace JwtDayMusic.WebApi.Helpers
{
    public static class TierHelper
    {
        public static int ToTierValue(string roleName)
        {
            return Enum.TryParse<MembershipTier>(roleName, out var tier) ? (int)tier : 0;
        }
    }
}