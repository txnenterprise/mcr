using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using MCR.API.Repository;
using MCR.API.Services.Interfaces;
using MCR.API.Shared.Extensions;

namespace MCR.API.Services.Implementations
{
    public class UsuarioEstruturaNegocioService : IUsuarioEstruturaNegocioService
    {
        private readonly DbContextMCR _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UsuarioEstruturaNegocioService(DbContextMCR context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<(List<Guid>? CorretoraIds, List<Guid>? CanalIds, List<Guid>? PontoAtendimentoIds)> ObterUsuarioPermissoes()
        {
            if (_httpContextAccessor.IsAdministrador())
                return (null, null, null);

            var userId = _httpContextAccessor.GetUserId();
            if (!userId.HasValue)
                return (new List<Guid>(), new List<Guid>(), new List<Guid>());

            var userEn = await _context.UsuarioEstruturaNegocio
                .Where(x => x.UsuarioId == userId.Value)
                .ToListAsync();

            var corretoraIds = userEn.Where(x => x.CorretoraId.HasValue).Select(x => x.CorretoraId.Value).ToList();
            var canalIds = userEn.Where(x => x.CanalId.HasValue).Select(x => x.CanalId.Value).ToList();
            var pontoAtendimentoIds = userEn.Where(x => x.PontoAtendimentoId.HasValue).Select(x => x.PontoAtendimentoId.Value).ToList();

            return (corretoraIds, canalIds, pontoAtendimentoIds);
        }
    }
}
