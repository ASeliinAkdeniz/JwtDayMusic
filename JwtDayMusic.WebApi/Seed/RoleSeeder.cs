using Microsoft.AspNetCore.Identity;

namespace JwtDayMusic.WebApi.Seed
{
    public static class RoleSeeder
    {
        // Case'in dört paket kademesi. Enum (MembershipTier) ile aynı isimler.
        private static readonly string[] TierRoles = { "Basic", "Gold", "Premium", "Elit" };

        public static async Task SeedAsync(RoleManager<IdentityRole> roleManager)
        {
            foreach (var roleName in TierRoles)
            {
                // Rol zaten varsa tekrar oluşturmuyoruz; her açılışta güvenle çalışır.
                if (!await roleManager.RoleExistsAsync(roleName))
                {
                    await roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }
        }
    }
}