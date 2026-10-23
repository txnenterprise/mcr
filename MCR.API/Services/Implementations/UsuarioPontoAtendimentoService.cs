using Microsoft.EntityFrameworkCore;
using MCR.API.Entities;
using MCR.API.Repository;

namespace MCR.API.Services.Implementations
{
    public interface IUsuarioPontoAtendimentoService
    {
        Task<(List<UsuarioPontoAtendimentoEntity> Lista, int TotalItems, int TotalPages)> ObterUsuarioPontoAtendimento(
            string pesquisaNomeUsuario, string pesquisaNomeLiderado, string pesquisaNomePontoAtendimento,
            string pesquisaNomeCanal, int page, int pageSize);
        Task<UsuarioPontoAtendimentoEntity> ObterPorIdAsync(Guid id);
        Task<(bool Sucesso, string? Mensagem, List<UsuarioPontoAtendimentoEntity> Lista)> AdicionarUsuarioPontoAtendimentoAsync(List<UsuarioPontoAtendimentoEntity> vinculos);
        Task<UsuarioPontoAtendimentoEntity> RemoverUsuarioPontoAtendimentoAsync(Guid id);
    }

    public class UsuarioPontoAtendimentoService : IUsuarioPontoAtendimentoService
    {
        private readonly DbContextMCR _context;

        public UsuarioPontoAtendimentoService(DbContextMCR context)
        {
            _context = context;
        }

        public async Task<(List<UsuarioPontoAtendimentoEntity> Lista, int TotalItems, int TotalPages)> ObterUsuarioPontoAtendimento(
            string pesquisaNomeUsuario, string pesquisaNomeLiderado, string pesquisaNomePontoAtendimento,
            string pesquisaNomeCanal, int page, int pageSize)
        {
            var query = _context.UsuariosPontoAtendimento
                .Include(u => u.Usuario)
                .Include(u => u.Liderado)
                .Include(u => u.PontoAtendimento)
                .Include(u => u.Canal)
                .Where(u => !u.Excluido)
                .AsQueryable();

            if (!string.IsNullOrEmpty(pesquisaNomeUsuario))
                query = query.Where(u => u.Usuario.Name.Contains(pesquisaNomeUsuario));
            if (!string.IsNullOrEmpty(pesquisaNomeLiderado))
                query = query.Where(u => u.Liderado.Name.Contains(pesquisaNomeLiderado));
            if (!string.IsNullOrEmpty(pesquisaNomePontoAtendimento))
                query = query.Where(u => u.PontoAtendimento.NomeFantasia.Contains(pesquisaNomePontoAtendimento));
            if (!string.IsNullOrEmpty(pesquisaNomeCanal))
                query = query.Where(u => u.Canal.NomeFantasia.Contains(pesquisaNomeCanal));

            var totalItems = await query.CountAsync();
            var totalPages = (int)Math.Ceiling((double)totalItems / pageSize);
            var lista = await query.OrderBy(u => u.Usuario.Name).Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

            return (lista, totalItems, totalPages);
        }

        public async Task<UsuarioPontoAtendimentoEntity> ObterPorIdAsync(Guid id)
        {
            return await _context.UsuariosPontoAtendimento
                .Include(u => u.Usuario)
                .Include(u => u.Liderado)
                .Include(u => u.PontoAtendimento)
                .Include(u => u.Canal)
                .FirstOrDefaultAsync(u => u.Id == id && !u.Excluido);
        }

        /// <summary>
        /// Adiciona vínculos usuário↔PA validando o MaximoPAsPorUsuario do plano da corretora do PA.
        /// Usuário-alvo = Liderado; conta vínculos distintos existentes (como Liderado OU Usuario) + novos do lote.
        /// Sem plano ou limite null = sem limite (regra vigente).
        /// </summary>
        public async Task<(bool Sucesso, string? Mensagem, List<UsuarioPontoAtendimentoEntity> Lista)> AdicionarUsuarioPontoAtendimentoAsync(List<UsuarioPontoAtendimentoEntity> vinculos)
        {
            var lista = new List<UsuarioPontoAtendimentoEntity>();
            if (vinculos == null || vinculos.Count == 0)
                return (true, null, lista);

            // Carrega a cadeia PA → canal → corretora → plano do lote
            var paIds = vinculos.Select(v => v.PontoAtendimentoId).Distinct().ToList();
            var paCanal = await _context.PontosAtendimento.AsNoTracking()
                .Where(p => paIds.Contains(p.Id))
                .ToDictionaryAsync(p => p.Id, p => p.CanalId);

            var canalIds = paCanal.Values.Distinct().ToList();
            var canalCorretora = await _context.Canais.AsNoTracking()
                .Where(c => canalIds.Contains(c.Id))
                .ToDictionaryAsync(c => c.Id, c => c.CorretoraId);

            var corretoraIds = canalCorretora.Values.Where(id => id.HasValue).Select(id => id!.Value).Distinct().ToList();
            var planos = await _context.CorretoraEntity.AsNoTracking()
                .Include(c => c.Plano)
                .Where(c => corretoraIds.Contains(c.Id))
                .ToDictionaryAsync(c => c.Id, c => c.Plano);

            // Validação por (usuário-alvo, corretora)
            var grupos = vinculos
                .Where(v => paCanal.ContainsKey(v.PontoAtendimentoId)
                            && canalCorretora.ContainsKey(paCanal[v.PontoAtendimentoId])
                            && canalCorretora[paCanal[v.PontoAtendimentoId]].HasValue)
                .GroupBy(v => new
                {
                    Alvo = v.LideradoId,
                    CorretoraId = canalCorretora[paCanal[v.PontoAtendimentoId]]!.Value
                });

            foreach (var grupo in grupos)
            {
                var plano = planos.TryGetValue(grupo.Key.CorretoraId, out var pl) ? pl : null;
                if (plano == null || !plano.MaximoPAsPorUsuario.HasValue)
                    continue;

                var max = plano.MaximoPAsPorUsuario.Value;
                var alvo = grupo.Key.Alvo;
                var novos = grupo.Select(v => v.PontoAtendimentoId).Distinct().ToList();

                var existentes = await _context.UsuariosPontoAtendimento.AsNoTracking()
                    .Where(u => !u.Excluido && (u.LideradoId == alvo || u.UsuarioId == alvo))
                    .Select(u => u.PontoAtendimentoId)
                    .Distinct()
                    .ToListAsync();

                var total = existentes.Union(novos).Count();
                if (total > max)
                {
                    var nomeCorretora = await _context.CorretoraEntity.AsNoTracking()
                        .Where(c => c.Id == grupo.Key.CorretoraId)
                        .Select(c => c.NomeFantasia)
                        .FirstOrDefaultAsync() ?? "da corretora";
                    return (false,
                        $"A corretora \"{nomeCorretora}\" (plano \"{plano.Nome}\") permite no máximo {max} ponto(s) de atendimento por usuário. Vinculados: {total}.",
                        new List<UsuarioPontoAtendimentoEntity>());
                }
            }

            foreach (var v in vinculos)
            {
                v.Id = Guid.NewGuid();
                v.Ativo = true;
                _context.UsuariosPontoAtendimento.Add(v);
            }
            await _context.SaveChangesAsync();
            return (true, null, vinculos);
        }

        public async Task<UsuarioPontoAtendimentoEntity> RemoverUsuarioPontoAtendimentoAsync(Guid id)
        {
            var entity = await _context.UsuariosPontoAtendimento.FindAsync(id);
            if (entity != null)
            {
                entity.Excluido = true;
                entity.Sucesso = true;
                await _context.SaveChangesAsync();
            }
            return entity;
        }
    }
}
