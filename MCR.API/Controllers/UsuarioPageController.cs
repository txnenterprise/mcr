using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MCR.API.Entities;
using MCR.API.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace MCR.API.Controllers
{
    [Authorize(Roles = "Administrador, Corretor, Gestor do Canal, Consultor")]
    [Route("Usuario")]
    public class UsuarioPageController : Controller
    {
        private readonly IUsuarioService _usuarioService;
        private readonly UserManager<UsuarioEntity> _userManager;
        private readonly ILogger<UsuarioPageController> _logger;

        public UsuarioPageController(IUsuarioService usuarioService, UserManager<UsuarioEntity> userManager, ILogger<UsuarioPageController> logger)
        {
            _usuarioService = usuarioService;
            _userManager = userManager;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> Index(string pesquisaNome, string pesquisaEmail, int page = 1, int pageSize = 10)
        {
            var perfilLogado = User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value;
            var perfisPermitidos = ObterPerfisPermitidos(perfilLogado);

            var retorno = (await _usuarioService.ObterTodosPaginadoAsync(pesquisaNome, pesquisaEmail, page, pageSize)).ToList();
            retorno = retorno.Where(u => perfisPermitidos.Contains(u.Role)).ToList();

            var lista = retorno.Select(u =>
            {
                var en = u.UsuarioEstruturaNegocio?.FirstOrDefault();
                return new Models.ViewModels.UsuarioListItem
                {
                    Id = u.Id.ToString(),
                    Nome = u.Name ?? u.UserName,
                    Email = u.Email,
                    PhoneNumber = u.PhoneNumber,
                    Document = u.Document,
                    Bloqueado = u.LockoutEnabled && u.LockoutEnd.HasValue && u.LockoutEnd > System.DateTimeOffset.UtcNow,
                    PerfilAcesso = u.Role,
                    Corretora = en?.Corretora?.NomeFantasia ?? string.Empty,
                    PontoAtendimento = en?.PontoAtendimento?.NomeFantasia ?? string.Empty
                };
            }).ToList();

            ViewBag.PesquisaNome = pesquisaNome;
            ViewBag.PesquisaEmail = pesquisaEmail;
            ViewBag.PaginaAtual = page;

            var model = new Models.ViewModels.UsuarioModel
            {
                ListaUsuarios = lista,
                PesquisaNome = pesquisaNome,
                PaginaAtual = page
            };

            return View("~/Views/Usuario/Index.cshtml", model);
        }

        [HttpGet("Cadastrar")]
        public IActionResult Cadastrar() => View("~/Views/Usuario/Cadastrar.cshtml");

        [HttpPost("Registrar")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Registrar(MCR.API.Entities.UsuarioCadastroDTO model)
        {
            try
            {
                if (!ModelState.IsValid)
                    return Json(new { success = false, message = "Verifique os campos obrigatórios." });

                var entity = new MCR.API.Entities.UsuarioEntity
                {
                    UserName = model.Email,
                    Email = model.Email,
                    Name = model.Nome,
                    Document = model.Document,
                    PhoneNumber = model.PhoneNumber,
                    Role = model.PerfilAcesso,
                    LoginProvider = "Local"
                };

                var cadastro = await _usuarioService.CadastrarAsync(entity, model.Password, model.PerfilAcesso);

                if (cadastro.Success)
                    return Json(new { success = true, message = "Usuário cadastrado com sucesso!" });

                return Json(new { success = false, message = cadastro.Message ?? "Erro ao cadastrar usuário." });
            }
            catch (Exception ex)
            {
                var fullMessage = ex.Message;
                var inner = ex.InnerException;
                while (inner != null)
                {
                    fullMessage += " | Inner: " + inner.Message;
                    inner = inner.InnerException;
                }
                _logger.LogError(ex, "Erro ao cadastrar usuário");
                return Json(new { success = false, message = "Erro ao cadastrar usuário: " + fullMessage });
            }
        }

        [HttpGet("Editar/{id}")]
        public async Task<IActionResult> Editar(Guid id)
        {
            var usuario = await _usuarioService.ObterPorIdAsync(id);
            ViewBag.UserId = id;
            return View("~/Views/Usuario/Editar.cshtml", usuario);
        }

        [HttpPost("Alterar")]
        public async Task<IActionResult> Alterar(MCR.API.Usuario.Domain.DTO.UsuarioAlterarDTO model)
        {
            try
            {
                if (string.IsNullOrEmpty(model.PerfilAcesso))
                    return Json(new { success = false, message = "Perfil de acesso é obrigatório." });

                var usuarioId = Guid.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value);
                var result = await _usuarioService.AlterarUsuarioAsync(model, usuarioId);
                return Json(new { success = result.Sucesso, message = result.Mensagem });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Erro ao alterar: " + ex.Message });
            }
        }

        [HttpPost("Bloquear/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Bloquear(Guid id)
        {
            _logger.LogInformation("Controller Bloquear chamado para userId {UserId}", id);
            var sucesso = await _usuarioService.BloquearAsync(id.ToString());
            if (sucesso)
            {
                _logger.LogInformation("Bloqueio bem-sucedido para userId {UserId}", id);
                TempData["Success"] = "Usuário bloqueado com sucesso.";
            }
            else
            {
                _logger.LogError("Falha ao bloquear usuário {UserId}", id);
                TempData["Error"] = "Erro ao bloquear usuário.";
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpGet("Bloquear/{id}")]
        public async Task<IActionResult> BloquearGet(Guid id)
        {
            _logger.LogInformation("Controller BloquearGet chamado para userId {UserId}", id);
            var sucesso = await _usuarioService.BloquearAsync(id.ToString());
            if (!sucesso)
                _logger.LogError("Falha ao bloquear usuário via GET {UserId}", id);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost("Desbloquear/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Desbloquear(Guid id)
        {
            _logger.LogInformation("Controller Desbloquear chamado para userId {UserId}", id);
            var sucesso = await _usuarioService.DesbloquearAsync(id.ToString());
            if (sucesso)
            {
                _logger.LogInformation("Desbloqueio bem-sucedido para userId {UserId}", id);
                TempData["Success"] = "Usuário desbloqueado com sucesso.";
            }
            else
            {
                _logger.LogError("Falha ao desbloquear usuário {UserId}", id);
                TempData["Error"] = "Erro ao desbloquear usuário.";
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpGet("Desbloquear/{id}")]
        public async Task<IActionResult> DesbloquearGet(Guid id)
        {
            _logger.LogInformation("Controller DesbloquearGet chamado para userId {UserId}", id);
            var sucesso = await _usuarioService.DesbloquearAsync(id.ToString());
            if (!sucesso)
                _logger.LogError("Falha ao desbloquear usuário via GET {UserId}", id);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost("ChangePassword")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordModel model)
        {
            var userId = model.UserId.HasValue && model.UserId.Value != Guid.Empty
                ? model.UserId.Value.ToString()
                : User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
                return Json(new { success = false, message = "Usuário não encontrado." });

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return Json(new { success = false, message = "Usuário não encontrado." });

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var result = await _userManager.ResetPasswordAsync(user, token, model.newPass);

            if (!result.Succeeded)
                return Json(new { success = false, message = result.Errors.FirstOrDefault()?.Description ?? "Erro ao alterar a senha." });

            return Json(new { success = true, message = "Senha alterada com sucesso!" });
        }

        public class ChangePasswordModel
        {
            public string newPass { get; set; }
            public Guid? UserId { get; set; }
        }

        private static List<string> ObterPerfisPermitidos(string? perfilLogado)
        {
            return perfilLogado switch
            {
                "Administrador" => new List<string> { "Administrador", "Corretor", "Gestor do Canal", "Consultor", "Assistente" },
                "Corretor" => new List<string> { "Corretor", "Gestor do Canal", "Consultor", "Assistente" },
                "Gestor do Canal" => new List<string> { "Gestor do Canal", "Consultor", "Assistente" },
                "Consultor" => new List<string> { "Consultor", "Assistente" },
                _ => new List<string>()
            };
        }
    }
}
