using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MCR.API.DTOs;
using MCR.API.Entities;
using MCR.API.Models;
using MCR.API.Services;

namespace MCR.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<UsuarioEntity> _userManager;
        private readonly SignInManager<UsuarioEntity> _signInManager;
        private readonly RoleManager<IdentityRole<Guid>> _roleManager;
        private readonly JwtSettings _jwtSettings;

        public AuthController(
            UserManager<UsuarioEntity> userManager,
            SignInManager<UsuarioEntity> signInManager,
            RoleManager<IdentityRole<Guid>> roleManager,
            IOptions<JwtSettings> jwtSettings)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _jwtSettings = jwtSettings.Value;
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null)
                return Unauthorized(ApiResponse<object>.Fail("Email ou senha inválidos."));

            var result = await _signInManager.CheckPasswordSignInAsync(user, request.Password, true);
            if (result.IsLockedOut)
                return Unauthorized(ApiResponse<object>.Fail("Usuário bloqueado por muitas tentativas. Tente novamente mais tarde."));
            if (!result.Succeeded)
                return Unauthorized(ApiResponse<object>.Fail("Email ou senha inválidos."));

            var token = await GerarTokenAsync(user);
            return Ok(ApiResponse<LoginResponse>.Ok(token));
        }

        [HttpPost("logout")]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return Ok(ApiResponse<object>.Ok(null, "Logout realizado com sucesso."));
        }

        [HttpPost("forgot-password")]
        [AllowAnonymous]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequest request)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null)
                return Ok(ApiResponse<object>.Ok(null, "Se o email existir, enviaremos o link de recuperação."));

            // TODO: Enviar o token por email quando o serviço de email estiver implementado
            return Ok(ApiResponse<object>.Ok(null, "Se o email existir, enviaremos o link de recuperação."));
        }

        [HttpPost("reset-password")]
        [AllowAnonymous]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request)
        {
            if (request.Password != request.ConfirmPassword)
                return BadRequest(ApiResponse<object>.Fail("As senhas não conferem."));

            var user = await _userManager.FindByIdAsync(request.UserId);
            if (user == null)
                return BadRequest(ApiResponse<object>.Fail("Usuário não encontrado."));

            var result = await _userManager.ResetPasswordAsync(user, request.Code, request.Password);
            if (!result.Succeeded)
                return BadRequest(ApiResponse<object>.Fail("Falha ao redefinir senha.",
                    result.Errors.Select(e => e.Description).ToList()));

            return Ok(ApiResponse<object>.Ok(null, "Senha redefinida com sucesso."));
        }

        [HttpPost("change-password")]
        [Authorize]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
        {
            if (request.NewPassword != request.ConfirmNewPassword)
                return BadRequest(ApiResponse<object>.Fail("As novas senhas não conferem."));

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return Unauthorized(ApiResponse<object>.Fail("Usuário não encontrado."));

            var result = await _userManager.ChangePasswordAsync(user, request.CurrentPassword, request.NewPassword);
            if (!result.Succeeded)
                return BadRequest(ApiResponse<object>.Fail("Falha ao alterar senha.",
                    result.Errors.Select(e => e.Description).ToList()));

            return Ok(ApiResponse<object>.Ok(null, "Senha alterada com sucesso."));
        }

        [HttpGet("me")]
        [Authorize]
        public async Task<IActionResult> Me()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return Unauthorized(ApiResponse<object>.Fail("Usuário não encontrado."));

            var roles = await _userManager.GetRolesAsync(user);
            return Ok(ApiResponse<object>.Ok(new
            {
                userId = user.Id,
                user.Name,
                user.Email,
                roles
            }));
        }

        private async Task<LoginResponse> GerarTokenAsync(UsuarioEntity user)
        {
            var roles = await _userManager.GetRolesAsync(user);
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Name, user.Name ?? user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

            var secretKey = Encoding.UTF8.GetBytes(_jwtSettings.Secret);
            var key = new SymmetricSecurityKey(secretKey);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddHours(_jwtSettings.ExpirationInHours),
                Issuer = _jwtSettings.Issuer,
                Audience = _jwtSettings.Audience,
                SigningCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature)
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return new LoginResponse
            {
                Token = tokenHandler.WriteToken(token),
                ExpiresAt = tokenDescriptor.Expires.Value,
                UserId = user.Id.ToString(),
                Name = user.Name ?? user.Email,
                Email = user.Email,
                Roles = roles.ToList()
            };
        }
    }
}
