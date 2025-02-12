using AspNetCoreIdentityApp.Web.Models;
using AspNetCoreIdentityApp.Web.Validators;

namespace AspNetCoreIdentityApp.Web.Extensions
{
    public static class StartupExtension
    {
        // Bu metod, özel kimlik doğrulama ayarlarını ve konfigürasyonlarını eklemek için kullanılır.
        public static void AddCustomIdentity(this IServiceCollection services)
        {
            // Identity hizmetlerini ekliyoruz ve özel ayarları sağlıyoruz.
            services.AddIdentity<AppUser, AppRole>(options =>
            {
                // Hesap onayı gerektirilmesi gerektiğini belirliyoruz (yani kullanıcı e-posta onayı yapmalı).
                options.SignIn.RequireConfirmedAccount = true;

                // Şifre uzunluğunun en az 6 karakter olması gerektiğini belirtiyoruz.
                options.Password.RequiredLength = 6;

                // Şifrede alfanümerik olmayan karakterlerin bulunması gerektiğini belirliyoruz.
                options.Password.RequireNonAlphanumeric = true;

                // Şifrede küçük harf bulunması gerektiğini belirtiyoruz.
                options.Password.RequireLowercase = true;

                // Şifrede büyük harf bulunması gerektiğini belirtiyoruz.
                options.Password.RequireUppercase = true;

                // Hesap kilitleme süresi 2 dakika olarak ayarlanıyor.
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(2);

                // Yanlış giriş denemelerinin sayısı 3 ile sınırlandırılıyor (max 3 deneme).
                options.Lockout.MaxFailedAccessAttempts = 3;
            })
                // Şifre doğrulama için özel bir PasswordValidator ekliyoruz.
                .AddPasswordValidator<PasswordValidator>()
                // Kullanıcı doğrulaması için özel bir UserValidator ekliyoruz.
                .AddUserValidator<UserValidator>()
                // Identity için gerekli veritabanı sağlayıcısını (EntityFramework) ekliyoruz.
                .AddEntityFrameworkStores<AppDbContext>();

            // Uygulamanın çerez ayarlarını yapılandırıyoruz.
            services.ConfigureApplicationCookie(opt =>
            {
                // Çerez adı 'AppCookie' olarak belirleniyor.
                var cookieBuilder = new CookieBuilder();
                cookieBuilder.Name = "AppCookie";

                // Kullanıcı giriş yolunu "/Home/Signin" olarak ayarlıyoruz.
                opt.LoginPath = new PathString("/Home/Signin");
                opt.LogoutPath = new PathString("/Member/Logout");

                // Çerez ile ilgili ayarları yapılandırıyoruz.
                opt.Cookie = cookieBuilder;
                // Çerezin geçerliliğinin 30 gün olduğunu belirtiyoruz.
                opt.ExpireTimeSpan = TimeSpan.FromDays(30);
                // Çerezin geçerliliği sona erdikten sonra yenilenmesini sağlıyoruz.
                opt.SlidingExpiration = true;
            });
        }
    }
}
