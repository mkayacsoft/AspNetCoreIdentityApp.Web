using System.Diagnostics;
using AspNetCoreIdentityApp.Web.Extensions;
using AspNetCoreIdentityApp.Web.Models;
using AspNetCoreIdentityApp.Web.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace AspNetCoreIdentityApp.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        private readonly UserManager<AppUser> _userManager; //User olu�turma S�gnUp

        private readonly SignInManager<AppUser> _signInManager; //S�gnIn k�sm� i�in Cookie olu�turmak i�in

        public HomeController(ILogger<HomeController> logger, UserManager<AppUser> userManager, SignInManager<AppUser> signInManager)
        {
            _logger = logger;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult SignUp()
        {
            return View();
        } 
        public IActionResult SignIn()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> SignIn(SignInViewModel request,string? returnUrl=null)
        { 
            returnUrl = returnUrl ?? Url.Action("Index","Home");

            var hasUser =await _userManager.FindByEmailAsync(request.Email); //Bu emaile sahip user'� al�yoruz ��nk� S�gnInManager User ve Password e�lemesi yap�yor.

            if (hasUser==null)
            {
                ModelState.AddModelError(string.Empty, "Wrong email or password!");
                return View();
            }

            var result = await _signInManager.PasswordSignInAsync(hasUser, request.Password, request.RememberMe, false); //user veriyoruz,password� veriyorz,hat�rla beni i�aretli mi, ve son olarak e�er kullan�c� ir ka� kere yanl�� girerse zaman a��m�na u�rat�yoruz.

            if (result.Succeeded)
            {
                return Redirect(returnUrl);
            }

            ModelState.AddModelErrorList(new List<string>(){ "Wrong email or password!" });


            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SignUp(SignUpViewModel request)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            var identityResult = await _userManager.CreateAsync(new()
                { UserName = request.UserName, PhoneNumber = request.Phone, Email = request.Email,EmailConfirmed = true},request.ConfirmPassword);

         

            if (identityResult.Succeeded)
           {
                TempData["Message"] = "The user has been created successfully. \n Welcome "+ request.UserName;
               return RedirectToAction(nameof(HomeController.SignUp));
           }

            ModelState.AddModelErrorList(identityResult.Errors.Select(x=>x.Description).ToList());

            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
