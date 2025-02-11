using AspNetCoreIdentityApp.Web.Models;
using AspNetCoreIdentityApp.Web.Validators;

namespace AspNetCoreIdentityApp.Web.Extensions
{
    public static class StartupExtension
    {
        public static void AddCustomIdentity(this IServiceCollection services)
        {
            services.AddIdentity<AppUser, AppRole>(options =>
                {
                    options.SignIn.RequireConfirmedAccount = true;
                    options.Password.RequiredLength = 6;
                    options.Password.RequireNonAlphanumeric = true;
                    options.Password.RequireLowercase = true;
                    options.Password.RequireUppercase = true;
                })
                .AddPasswordValidator<PasswordValidator>().AddUserValidator<UserValidator>().AddEntityFrameworkStores<AppDbContext>();
        }
    }
}
