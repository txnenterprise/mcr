using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MCR.API.Entities;
using MCR.API.Repository;
using MCR.API.Services.Interfaces;
using MCR.API.Shared.Extensions;

namespace MCR.API.Services.Implementations
{
    public class UsuarioService : IUsuarioService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UserManager<UsuarioEntity> _userManager;
        private readonly RoleManager<IdentityRole<Guid>> _roleManager;
        private readonly DbContextMCR _context;
        private readonly IUsuarioEstruturaNegocioService _usuarioEstruturaNegocioService;
        private readonly ILogger<UsuarioService> _logger;

        public UsuarioService(
            IHttpContextAccessor httpContextAccessor,
            UserManager<UsuarioEntity> userManager,
            RoleManager<IdentityRole<Guid>> roleManager,
            DbContextMCR context,
            IUsuarioEstruturaNegocioService usuarioEstruturaNegocioService,
            ILogger<UsuarioService> logger)
        {
            _httpContextAccessor = httpContextAccessor;
            _userManager = userManager;
            _roleManager = roleManager;
            _context = context;
            _usuarioEstruturaNegocioService = usuarioEstruturaNegocioService;
            _logger = logger;
        }

        public async Task<UsuarioEntity> ObterPorIdAsync(Guid id)
        {
            if (id == Guid.Empty)
            {
                return new UsuarioEntity { Success = false, Message = "Id do usuário não informado." };
            }

            var user = await _userManager.FindByIdAsync(id.ToString());
            if (user == null || user.Id == Guid.Empty)
                return new UsuarioEntity { Success = false, Message = $"Nenhum usuário localizado para o ID {id}." };

            user.Role = await GetUserRole(user);

            return ValidateUser(user, "Obter usuário por ID.");
        }

        public async Task<IEnumerable<UsuarioEntity>> ObterTodosPaginadoAsync(string nome = null, string email = null, int page = 1, int pageSize = 20)
        {
            var permissoes = await _usuarioEstruturaNegocioService.ObterUsuarioPermissoes();

            var userQuery = _context.Usuarios.Include(x => x.UsuarioEstruturaNegocio).AsQueryable();

            if (permissoes.CorretoraIds != null && permissoes.CorretoraIds.Any())
                userQuery = userQuery.Where(x => x.UsuarioEstruturaNegocio.Any(p => p.CorretoraId.HasValue && permissoes.CorretoraIds.Contains(p.CorretoraId.Value))).AsQueryable();

            if (permissoes.CanalIds != null && permissoes.CanalIds.Any())
                userQuery = userQuery.Where(x => x.UsuarioEstruturaNegocio.Any(p => p.CanalId.HasValue && permissoes.CanalIds.Contains(p.CanalId.Value))).AsQueryable();

            if (permissoes.PontoAtendimentoIds != null && permissoes.PontoAtendimentoIds.Any())
                userQuery = userQuery.Where(x => x.UsuarioEstruturaNegocio.Any(p => p.PontoAtendimentoId.HasValue && permissoes.PontoAtendimentoIds.Contains(p.PontoAtendimentoId.Value))).AsQueryable();

            var idsPorEstrutura = await userQuery.Select(x => x.Id).ToListAsync();
            var roleAdminId = await _context.Roles.Where(r => r.Name == "Administrador").Select(r => r.Id).FirstOrDefaultAsync();
            var idsAdmin = roleAdminId != Guid.Empty
                ? await _context.UserRoles.Where(ur => ur.RoleId == roleAdminId).Select(ur => ur.UserId).ToListAsync()
                : new List<Guid>();
            var todosIds = idsPorEstrutura.Union(idsAdmin).Distinct().ToList();

            if (_httpContextAccessor.IsConsultor())
            {
                var roleCorretorId = await _context.Roles.Where(r => r.Name == "Corretor").Select(r => r.Id).FirstOrDefaultAsync();
                var roleAssistenteId = await _context.Roles.Where(r => r.Name == "Assistente").Select(r => r.Id).FirstOrDefaultAsync();
                var idsCorretor = roleCorretorId != Guid.Empty ? await _context.UserRoles.Where(ur => ur.RoleId == roleCorretorId).Select(ur => ur.UserId).ToListAsync() : new List<Guid>();
                var idsAssistente = roleAssistenteId != Guid.Empty ? await _context.UserRoles.Where(ur => ur.RoleId == roleAssistenteId).Select(ur => ur.UserId).ToListAsync() : new List<Guid>();
                todosIds = todosIds.Union(idsCorretor).Union(idsAssistente).Distinct().ToList();
            }

            userQuery = _context.Usuarios
                .Include(x => x.UsuarioEstruturaNegocio).ThenInclude(en => en.Corretora)
                .Include(x => x.UsuarioEstruturaNegocio).ThenInclude(en => en.Canal)
                .Include(x => x.UsuarioEstruturaNegocio).ThenInclude(en => en.PontoAtendimento)
                .Include(x => x.UsuarioCanal).ThenInclude(uc => uc.Canal)
                .Include(x => x.UsuarioPontoAtendimento).ThenInclude(up => up.PontoAtendimento)
                .Where(x => todosIds.Contains(x.Id));

            if (!string.IsNullOrWhiteSpace(nome))
                userQuery = userQuery.Where(x => x.Name.Contains(nome));

            if (!string.IsNullOrWhiteSpace(email))
                userQuery = userQuery.Where(x => x.Email.Contains(email));

            var usersList = await userQuery
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            if (usersList == null || usersList.Count == 0)
                return new List<UsuarioEntity> { new UsuarioEntity { Success = false, Message = "Nenhum usuário cadastrado." } };

            foreach (var user in usersList)
            {
                user.Role = await GetUserRole(user);
                user.Success = true;
                user.Message = "Sucesso";
            }
            return usersList;
        }

        public async Task<UsuarioEntity> CadastrarAsync(UsuarioEntity entity, string password, string role)
        {
            entity.Password = password;
            entity.ConfirmPassword = password;
            entity.Role = role;

            return await Create(entity, true);
        }

        public async Task<UsuarioEntity> AtualizarAsync(UsuarioEntity entity)
        {
            var validation = ValidateUser(entity, "Atualizar", null);
            if (!validation.Success)
                return validation;

            var getUser = await FindByEmailAsync(entity.Email);
            if (getUser == null || getUser.Id == Guid.Empty)
                return new UsuarioEntity { Success = false, Message = $"Nenhum usuário localizado para o e-mail {entity.Email}." };

            entity.Document = string.IsNullOrEmpty(entity.Document) ? "Não informado" : entity.Document;
            entity.PhoneNumber = string.IsNullOrEmpty(entity.PhoneNumber) ? "Não informado" : entity.PhoneNumber;

            getUser.Name = entity.Name;
            getUser.UserName = entity.Email;
            getUser.Email = entity.Email;

            await _userManager.RemoveFromRoleAsync(getUser, getUser.Role);
            getUser.Role = entity.Role;
            await _userManager.AddToRoleAsync(getUser, getUser.Role);

            var result = await _userManager.UpdateAsync(getUser);
            return ValidateUser(getUser, "Atualizar", result);
        }

        public async Task<bool> BloquearAsync(string userId)
        {
            _logger.LogInformation("Tentando bloquear usuário {UserId}", userId);

            if (string.IsNullOrEmpty(userId))
            {
                _logger.LogWarning("BloquearAsync chamado com userId vazio");
                return false;
            }

            var existingUser = await _userManager.FindByIdAsync(userId);
            if (existingUser == null)
            {
                _logger.LogWarning("Usuário {UserId} não encontrado para bloqueio", userId);
                return false;
            }

            _logger.LogInformation("Usuário {UserId} encontrado. Email: {Email}. Aplicando LockoutEnabled=true", userId, existingUser.Email);

            var enableResult = await _userManager.SetLockoutEnabledAsync(existingUser, true);
            if (!enableResult.Succeeded)
            {
                _logger.LogError("Falha ao setar LockoutEnabled=true para usuário {UserId}. Erros: {Errors}",
                    userId, string.Join(", ", enableResult.Errors.Select(e => e.Description)));
                return false;
            }

            _logger.LogInformation("Aplicando SetLockoutEndDateAsync com DateTimeOffset.MaxValue para usuário {UserId}", userId);
            var lockoutResult = await _userManager.SetLockoutEndDateAsync(existingUser, DateTimeOffset.MaxValue);
            if (!lockoutResult.Succeeded)
            {
                _logger.LogError("Falha ao setar LockoutEndDate para usuário {UserId}. Erros: {Errors}",
                    userId, string.Join(", ", lockoutResult.Errors.Select(e => e.Description)));
                return false;
            }

            _logger.LogInformation("Usuário {UserId} bloqueado com sucesso", userId);
            return true;
        }

        public async Task<bool> DesbloquearAsync(string userId)
        {
            _logger.LogInformation("Tentando desbloquear usuário {UserId}", userId);

            if (string.IsNullOrEmpty(userId))
            {
                _logger.LogWarning("DesbloquearAsync chamado com userId vazio");
                return false;
            }

            var existingUser = await _userManager.FindByIdAsync(userId);
            if (existingUser == null)
            {
                _logger.LogWarning("Usuário {UserId} não encontrado para desbloqueio", userId);
                return false;
            }

            _logger.LogInformation("Limpando LockoutEnd para usuário {UserId}", userId);
            await _userManager.SetLockoutEndDateAsync(existingUser, null);

            _logger.LogInformation("Aplicando LockoutEnabled=false para usuário {UserId}", userId);
            var result = await _userManager.SetLockoutEnabledAsync(existingUser, false);
            if (!result.Succeeded)
            {
                _logger.LogError("Falha ao desbloquear usuário {UserId}. Erros: {Errors}",
                    userId, string.Join(", ", result.Errors.Select(e => e.Description)));
                return false;
            }

            _logger.LogInformation("Usuário {UserId} desbloqueado com sucesso", userId);
            return true;
        }

        public async Task<bool> ChangePasswordAsync(string userId, string currentPassword, string newPassword)
        {
            if (string.IsNullOrEmpty(userId))
                return false;

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return false;

            var changePasswordResult = await _userManager.ChangePasswordAsync(user, currentPassword, newPassword);
            return changePasswordResult.Succeeded;
        }

        private async Task<UsuarioEntity> Create(UsuarioEntity user, bool checkPassword = true)
        {
            var validation = ValidateUser(user, "Cadastrar", null);
            try
            {
                if (!validation.Success)
                    return validation;

                if (checkPassword)
                {
                    if (user.Password != user.ConfirmPassword)
                    {
                        user.Success = false;
                        user.Message = "Os campos de senha precisam ser iguais.";
                        return user;
                    }
                }

                user.Document = string.IsNullOrEmpty(user.Document) ? "Não informado" : user.Document;
                user.PhoneNumber = string.IsNullOrEmpty(user.PhoneNumber) ? "Não informado" : user.PhoneNumber;
                user.EmailConfirmed = true;
                user.PhoneNumberConfirmed = true;
                user.Id = Guid.NewGuid();
                user.TwoFactorEnabled = true;
                user.LockoutEnabled = true;
                user.AccessFailedCount = 10;
                var result = checkPassword
                             ? await _userManager.CreateAsync(user, user.Password)
                             : await _userManager.CreateAsync(user);

                var validationResult = ValidateUser(user, "Cadastrar", result);
                if (!validationResult.Success)
                    return validation;

                var roleResult = await _userManager.AddToRoleAsync(user, user.Role);
                var validationRoleResult = ValidateUser(user, "Cadastrar", roleResult);

                if (!validationRoleResult.Success)
                {
                    await _userManager.DeleteAsync(user);
                    return validationRoleResult;
                }

                string userId = user.Id.ToString();
                await _context.Database.ExecuteSqlRawAsync(@$"UPDATE public.""AspNetUsers"" SET ""LockoutEnabled"" = false, ""AccessFailedCount"" = 0, ""TwoFactorEnabled"" = false WHERE ""Id"" = '{userId}'");

                user.Success = true;
                user.Message = "Usuário cadastrado com sucesso!";
            }
            catch (Exception ex)
            {
                throw;
            }
            return user;
        }

        private UsuarioEntity ValidateUser(UsuarioEntity user, string action, IdentityResult identityResult = null)
        {
            if (user == null)
            {
                return new UsuarioEntity { Success = false, Message = "Usuário não informado." };
            }

            if (identityResult != null)
            {
                if (!identityResult.Succeeded)
                {
                    if (identityResult.Errors != null && identityResult.Errors.Any())
                    {
                        user.Success = false;
                        user.Message = $"Usuário - Erro na ação {action}. Detalhes: {string.Join(", ", identityResult.Errors.Select(c => c.Description))}";
                        return user;
                    }
                    else
                    {
                        user.Success = false;
                        user.Message = $"Usuário - Erro na ação {action} (erro desconhecido).";
                        return user;
                    }
                }
            }

            user.Success = true;
            user.Message = $"Usuário - Sucesso na ação {action}!";
            return user;
        }

        private async Task<string> GetUserRole(UsuarioEntity user)
        {
            var roles = await _userManager.GetRolesAsync(user);
            if (roles.Count > 0)
                return roles[0];

            return string.Empty;
        }

        private async Task<UsuarioEntity> FindByEmailAsync(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                return new UsuarioEntity { Success = false, Message = "E-mail do usuário não informado." };
            }

            var user = await _userManager.FindByEmailAsync(email);
            if (user == null || user.Id == Guid.Empty)
                return new UsuarioEntity { Success = false, Message = $"Nenhum usuário localizado para o e-mail {email}." };

            user.Role = await GetUserRole(user);

            return ValidateUser(user, "Obter Usuário por E-Mail");
        }

        public async Task<(bool Sucesso, string Mensagem)> AlterarUsuarioAsync(MCR.API.Usuario.Domain.DTO.UsuarioAlterarDTO model, Guid usuarioCriadorId)
        {
            try
            {
                if (Guid.TryParse(model.Id, out var userId))
                {
                    if (_context.Usuarios.Any(x => x.Email == model.Email && x.Id != userId))
                        return (false, $"O e-mail {model.Email} já está cadastrado para outro usuário.");

                    var existing = await _userManager.FindByIdAsync(userId.ToString());
                    if (existing == null)
                        return (false, "Usuário não encontrado.");

                    existing.Name = model.Nome;
                    existing.Email = model.Email;
                    existing.UserName = model.Email;
                    existing.PhoneNumber = model.PhoneNumber ?? string.Empty;
                    existing.Document = model.Document ?? string.Empty;
                    existing.Role = model.PerfilAcesso;

                    var result = await _userManager.UpdateAsync(existing);
                    if (result.Succeeded)
                    {
                        // Só altera EstruturaNegocio se veio preenchido no formulário
                        if (model.EstruturaNegocio != null)
                        {
                            // Remove estruturas antigas
                            var antigas = _context.UsuarioEstruturaNegocio
                                .Where(x => x.UsuarioId == userId).ToList();
                            _context.UsuarioEstruturaNegocio.RemoveRange(antigas);

                            // Cria novas estruturas a partir do formulário
                            if (model.EstruturaNegocio.Any())
                            {
                                var novas = model.EstruturaNegocio.Select(item => new MCR.API.Entities.UsuarioEstruturaNegocioEntity
                                {
                                    Id = Guid.NewGuid(),
                                    UsuarioCriadorId = usuarioCriadorId,
                                    UsuarioId = userId,
                                    CorretoraId = item.CorretoraId,
                                    CanalId = item.CanalId,
                                    PontoAtendimentoId = item.PontoAtendimentoId,
                                    Ativo = true,
                                    Excluido = false
                                }).ToList();
                                _context.UsuarioEstruturaNegocio.AddRange(novas);
                            }
                        }

                        await _context.SaveChangesAsync();
                        return (true, "Usuário alterado com sucesso!");
                    }

                    var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                    return (false, $"Erro ao alterar: {errors}");
                }
                return (false, "ID do usuário inválido.");
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }
    }
}
