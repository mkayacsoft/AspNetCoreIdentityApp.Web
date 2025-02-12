using System.Diagnostics;
using AspNetCoreIdentityApp.Web.Extensions;
using AspNetCoreIdentityApp.Web.Models;
using AspNetCoreIdentityApp.Web.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace AspNetCoreIdentityApp.Web.Controllers
{
    /// <summary>
    /// Controller for handling home page and user authentication actions.
    /// </summary>
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        private readonly UserManager<AppUser> _userManager; //User oluþturma SýgnUp

        private readonly SignInManager<AppUser> _signInManager; //SýgnIn kýsmý için Cookie oluþturmak için

        /// <summary>
        /// Initializes a new instance of the <see cref="HomeController"/> class.
        /// </summary>
        /// <param name="logger">The logger instance.</param>
        /// <param name="userManager">The user manager instance.</param>
        /// <param name="signInManager">The sign-in manager instance.</param>
        public HomeController(ILogger<HomeController> logger, UserManager<AppUser> userManager, SignInManager<AppUser> signInManager)
        {
            _logger = logger;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        /// <summary>
        /// Displays the index view.
        /// </summary>
        /// <returns>The index view.</returns>
        public IActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Displays the privacy policy view.
        /// </summary>
        /// <returns>The privacy view.</returns>
        public IActionResult Privacy()
        {
            return View();
        }

        /// <summary>
        /// Displays the sign-up view.
        /// </summary>
        /// <returns>The sign-up view.</returns>
        public IActionResult SignUp()
        {
            return View();
        }

        /// <summary>
        /// Displays the sign-in view.
        /// </summary>
        /// <returns>The sign-in view.</returns>
        public IActionResult SignIn()
        {
            return View();
        }

        /// <summary>
        /// Handles the sign-in process.
        /// </summary>
        /// <param name="request">The sign-in view model.</param>
        /// <param name="returnUrl">The return URL after sign-in.</param>
        /// <returns>The result of the sign-in process.</returns>
        [HttpPost]
        public async Task<IActionResult> SignIn(SignInViewModel request, string? returnUrl = null)
        {
            // Eðer returnUrl boþ ise, varsayýlan olarak Home controller'daki Index action'ýna yönlendir.
            returnUrl = returnUrl ?? Url.Action("Index", "Home");

            // Girilen e-posta adresine ait kullanýcýyý veritabanýndan buluyoruz.
            // Çünkü SignInManager, kullanýcýyý ve þifreyi eþleþtirerek doðrulama yapýyor.
            var hasUser = await _userManager.FindByEmailAsync(request.Email);

            // Eðer e-posta adresine ait kullanýcý bulunamazsa,
            // ModelState'e hata mesajý ekleyip, kullanýcýya yanlýþ email veya þifre olduðunu bildiriyoruz.
            if (hasUser == null)
            {
                ModelState.AddModelError(string.Empty, " Wrong email or password!");
                return View();
            }

            // Kullanýcýnýn þifresi ve "hatýrla beni" seçeneðiyle giriþ yapmayý deniyoruz.
            // true parametresi, kullanýcý yanlýþ giriþ yaparsa hesap kilitleme (lockout) özelliðini aktif eder.
            var result = await _signInManager.PasswordSignInAsync(hasUser, request.Password, request.RememberMe, true);

            // Eðer giriþ iþlemi baþarýlý ise, belirtilen returnUrl'e yönlendiriyoruz.
            if (result.Succeeded)
            {
                return Redirect(returnUrl);
            }

            if (result.IsLockedOut)
            {
                // Eðer hesap kilitlenmiþse, kullanýcýya hata mesajý gösteriyoruz.
                ModelState.AddModelError(string.Empty, "Your account has been locked out. Please try again later.");
                return View();
            }


            // Giriþ iþlemi baþarýsýzsa, ModelState'e hata mesajý ekliyoruz.
            ModelState.AddModelErrorList(new List<string>() { "Wrong email or password!" });

            // Hatalý giriþ durumunda, tekrar giriþ sayfasýný görüntülüyoruz.
            return View();
        }


        /// <summary>
        /// Handles the sign-up process.
        /// </summary>
        /// <param name="request">The sign-up view model.</param>
        /// <returns>The result of the sign-up process.</returns>
        [HttpPost]
        public async Task<IActionResult> SignUp(SignUpViewModel request)
        {
            // Model doðrulamasý: Eðer gönderilen veriler modelin gerektirdiði kurallara uymuyorsa, tekrar ayný view'e dön.
            if (!ModelState.IsValid)
            {
                return View();
            }

            // Yeni bir kullanýcý oluþturuluyor.
            // _userManager.CreateAsync metodu, kullanýcýnýn temel bilgilerini (UserName, PhoneNumber, Email) ve þifresini alarak veritabanýna eklemeye çalýþýr.
            // EmailConfirmed = true, kullanýcýnýn email adresinin onaylandýðýný varsayar.
            var identityResult = await _userManager.CreateAsync(new()
            {
                UserName = request.UserName,   // Kullanýcý adý
                PhoneNumber = request.Phone,     // Telefon numarasý
                Email = request.Email,           // Email adresi
                EmailConfirmed = true            // Email doðrulamasý baþarýlý kabul ediliyor
            }, request.ConfirmPassword);         // Kullanýcýnýn þifresi (onaylanan þifre)

            // Eðer kullanýcý oluþturma iþlemi baþarýlý ise...
            if (identityResult.Succeeded)
            {
                // TempData kullanýlarak baþarý mesajý oluþturuluyor.
                TempData["Message"] = "The user has been created successfully. \n Welcome " + request.UserName;
                // Baþarýlý iþlem sonrasý, SignUp sayfasýna yönlendiriliyor.
                return RedirectToAction(nameof(HomeController.SignUp));
            }

            // Eðer kullanýcý oluþturma iþlemi sýrasýnda hatalar varsa, bu hatalar ModelState'e ekleniyor.
            // Bu hatalar view üzerinde kullanýcýya gösterilebilir.
            ModelState.AddModelErrorList(identityResult.Errors.Select(x => x.Description).ToList());

            // Hata durumunda, tekrar ayný view'e geri dönülüyor.
            return View();
        }


        /// <summary>
        /// Displays the error view.
        /// </summary>
        /// <returns>The error view.</returns>
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
