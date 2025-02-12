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

        private readonly UserManager<AppUser> _userManager; //User olu�turma S�gnUp

        private readonly SignInManager<AppUser> _signInManager; //S�gnIn k�sm� i�in Cookie olu�turmak i�in

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
            // E�er returnUrl bo� ise, varsay�lan olarak Home controller'daki Index action'�na y�nlendir.
            returnUrl = returnUrl ?? Url.Action("Index", "Home");

            // Girilen e-posta adresine ait kullan�c�y� veritaban�ndan buluyoruz.
            // ��nk� SignInManager, kullan�c�y� ve �ifreyi e�le�tirerek do�rulama yap�yor.
            var hasUser = await _userManager.FindByEmailAsync(request.Email);

            // E�er e-posta adresine ait kullan�c� bulunamazsa,
            // ModelState'e hata mesaj� ekleyip, kullan�c�ya yanl�� email veya �ifre oldu�unu bildiriyoruz.
            if (hasUser == null)
            {
                ModelState.AddModelError(string.Empty, " Wrong email or password!");
                return View();
            }

            // Kullan�c�n�n �ifresi ve "hat�rla beni" se�ene�iyle giri� yapmay� deniyoruz.
            // true parametresi, kullan�c� yanl�� giri� yaparsa hesap kilitleme (lockout) �zelli�ini aktif eder.
            var result = await _signInManager.PasswordSignInAsync(hasUser, request.Password, request.RememberMe, true);

            // E�er giri� i�lemi ba�ar�l� ise, belirtilen returnUrl'e y�nlendiriyoruz.
            if (result.Succeeded)
            {
                return Redirect(returnUrl);
            }

            if (result.IsLockedOut)
            {
                // E�er hesap kilitlenmi�se, kullan�c�ya hata mesaj� g�steriyoruz.
                ModelState.AddModelError(string.Empty, "Your account has been locked out. Please try again later.");
                return View();
            }


            // Giri� i�lemi ba�ar�s�zsa, ModelState'e hata mesaj� ekliyoruz.
            ModelState.AddModelErrorList(new List<string>() { "Wrong email or password!" });

            // Hatal� giri� durumunda, tekrar giri� sayfas�n� g�r�nt�l�yoruz.
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
            // Model do�rulamas�: E�er g�nderilen veriler modelin gerektirdi�i kurallara uymuyorsa, tekrar ayn� view'e d�n.
            if (!ModelState.IsValid)
            {
                return View();
            }

            // Yeni bir kullan�c� olu�turuluyor.
            // _userManager.CreateAsync metodu, kullan�c�n�n temel bilgilerini (UserName, PhoneNumber, Email) ve �ifresini alarak veritaban�na eklemeye �al���r.
            // EmailConfirmed = true, kullan�c�n�n email adresinin onayland���n� varsayar.
            var identityResult = await _userManager.CreateAsync(new()
            {
                UserName = request.UserName,   // Kullan�c� ad�
                PhoneNumber = request.Phone,     // Telefon numaras�
                Email = request.Email,           // Email adresi
                EmailConfirmed = true            // Email do�rulamas� ba�ar�l� kabul ediliyor
            }, request.ConfirmPassword);         // Kullan�c�n�n �ifresi (onaylanan �ifre)

            // E�er kullan�c� olu�turma i�lemi ba�ar�l� ise...
            if (identityResult.Succeeded)
            {
                // TempData kullan�larak ba�ar� mesaj� olu�turuluyor.
                TempData["Message"] = "The user has been created successfully. \n Welcome " + request.UserName;
                // Ba�ar�l� i�lem sonras�, SignUp sayfas�na y�nlendiriliyor.
                return RedirectToAction(nameof(HomeController.SignUp));
            }

            // E�er kullan�c� olu�turma i�lemi s�ras�nda hatalar varsa, bu hatalar ModelState'e ekleniyor.
            // Bu hatalar view �zerinde kullan�c�ya g�sterilebilir.
            ModelState.AddModelErrorList(identityResult.Errors.Select(x => x.Description).ToList());

            // Hata durumunda, tekrar ayn� view'e geri d�n�l�yor.
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
