using Microsoft.EntityFrameworkCore;
using MCR.API.Repository;
using MCR.API.Services.Interfaces;

namespace MCR.API.Services.Implementations
{
    public class ValidacaoLimiteService : IValidacaoLimiteService
    {
        private readonly DbContextMCR _context;

        public ValidacaoLimiteService(DbContextMCR context)
        {
            _context = context;
        }

        public async Task<(bool ok, string? mensagem)> ValidarLimiteUsuariosAsync(Guid corretoraId, Guid? usuarioIdExcluir = null)
        {
            var corretora = await _context.CorretoraEntity
                .Include(c => c.Plano)
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == corretoraId);

            var nomeCorretora = corretora?.NomeFantasia ?? corretora?.RazaoSocial ?? "da corretora";

            // Regra: sem plano configurado NÃO é permitido criar/alterar usuários vinculados
            var plano = corretora?.Plano;
            if (plano == null)
                return (false, $"A corretora \"{nomeCorretora}\" não tem plano configurado. Configure o plano da corretora antes de cadastrar ou alterar usuários.");

            if (!plano.MaximoUsuarios.HasValue)
                return (true, null);

            var max = plano.MaximoUsuarios.Value;

            var query = _context.UsuarioEstruturaNegocio
                .Where(en => en.CorretoraId == corretoraId && en.Ativo && !en.Excluido);

            if (usuarioIdExcluir.HasValue)
                query = query.Where(en => en.UsuarioId != usuarioIdExcluir.Value);

            var count = await query.Select(en => en.UsuarioId).Distinct().CountAsync();

            if (count >= max)
                return (false, $"A corretora atingiu o limite de {max} usuário(s) vinculado(s) do plano \"{plano.Nome}\".");

            return (true, null);
        }

        public async Task<(bool ok, string? mensagem)> ValidarLimitePAsPorUsuarioAsync(Guid usuarioId, Guid? canalId)
        {
            if (!canalId.HasValue)
                return (true, null);

            var canal = await _context.Canais
                .Include(c => c.Corretora)
                    .ThenInclude(cr => cr!.Plano)
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == canalId.Value);

            var plano = canal?.Corretora?.Plano;
            if (plano == null || !plano.MaximoPAsPorUsuario.HasValue)
                return (true, null);

            var max = plano.MaximoPAsPorUsuario.Value;

            var count = await _context.PontosAtendimento
                .CountAsync(pa => pa.UsuarioCriadorId == usuarioId && !pa.Excluido);

            if (count >= max)
                return (false, $"Você atingiu o limite de {max} ponto(s) de atendimento do plano \"{plano.Nome}\".");

            return (true, null);
        }
    }
}
