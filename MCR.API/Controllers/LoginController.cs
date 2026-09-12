using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MCR.API.Entities;
using MCR.API.Models.ViewModels;

namespace MCR.API.Controllers
{
    public class LoginController : Controller
    {
        private readonly SignInManager<UsuarioEntity> _signInManager;
        private readonly UserManager<UsuarioEntity> _userManager;
        private readonly ILogger<LoginController> _logger;

        public LoginController(
            SignInManager<UsuarioEntity> signInManager,
            UserManager<UsuarioEntity> userManager,
            ILogger<LoginController> logger)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return RedirectToAction("Authentication");
        }

        [HttpGet]
        public IActionResult Authentication(string returnUrl = null)
        {
            if (User.Identity.IsAuthenticated)
                return RedirectToAction("Index", "Dashboard");

            return View(new AuthenticationModel { ReturnUrl = returnUrl });
        }

        [HttpPost]
        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> Authentication(AuthenticationModel model)
        {
            _logger.LogWarning("=== LOGIN POST RECEBIDO ===");
            _logger.LogWarning("Email: '{Email}' | Password: '{Pass}' | HttpMethod: {Method}", 
                model?.Email, model?.Password ?? "NULL", HttpContext.Request.Method);
            _logger.LogWarning("ContentType: {CT}", HttpContext.Request.ContentType);
            _logger.LogWarning("Form keys: {Keys}", string.Join(", ", HttpContext.Request.Form?.Keys ?? new Microsoft.Extensions.Primitives.StringValues()));
            _logger.LogWarning("===========================");

            if (string.IsNullOrWhiteSpace(model?.Email) || string.IsNullOrWhiteSpace(model?.Password))
            {
                _logger.LogWarning("Empty credentials");
                model.Mensagem = "Preencha todos os campos obrigatórios.";
                return View(model);
            }

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                _logger.LogWarning("User not found: {Email}", model.Email);
                model.Mensagem = "Usuário ou senha inválidos.";
                return View(model);
            }

            _logger.LogInformation("User found: {Email}, Lockout: {Lockout}", model.Email, user.LockoutEnabled);

            var result = await _signInManager.PasswordSignInAsync(user, model.Password, false, false);
            if (result.Succeeded)
            {
                _logger.LogInformation("Login successful: {Email}", model.Email);
                if (!string.IsNullOrEmpty(model.ReturnUrl))
                    return Redirect(model.ReturnUrl);
                return RedirectToAction("Index", "Dashboard");
            }

            _logger.LogWarning("Login failed for {Email}: {Result}", model.Email, result);
            model.Mensagem = "Usuário ou senha inválidos.";
            return View(model);
        }

        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user != null)
            {
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                var resetLink = Url.Action("ResetPassword", "Login", new { email = model.Email, token }, Request.Scheme);
            }

            model.Sucesso = true;
            return View("CheckEmail", model);
        }

        [HttpGet]
        public IActionResult CheckEmail()
        {
            return View();
        }

        [HttpGet]
        public IActionResult ResetPassword(string email, string token)
        {
            return View(new ResetPasswordModel { Email = email, Token = token });
        }

        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                model.Mensagem = "Usuário não encontrado.";
                return View(model);
            }

            var result = await _userManager.ResetPasswordAsync(user, model.Token, model.Password);
            if (result.Succeeded)
                return RedirectToAction("ResetPasswordConfirmation");

            foreach (var error in result.Errors)
                ModelState.AddModelError("", error.Description);

            return View(model);
        }

        [HttpGet]
        public IActionResult ResetPasswordConfirmation()
        {
            return View();
        }

        public async Task<IActionResult> LogOff()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Authentication");
        }

        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
