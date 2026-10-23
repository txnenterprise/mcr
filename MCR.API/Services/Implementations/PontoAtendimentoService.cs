using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using MCR.API.Entities;
using MCR.API.Repository;
using MCR.API.Services.Interfaces;

namespace MCR.API.Services.Implementations
{
    public class PontoAtendimentoService : IPontoAtendimentoService
    {
        private readonly DbContextMCR _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IValidacaoLimiteService _validacaoLimiteService;

        public PontoAtendimentoService(
            DbContextMCR context,
            IHttpContextAccessor httpContextAccessor,
            IValidacaoLimiteService validacaoLimiteService)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            _validacaoLimiteService = validacaoLimiteService;
        }

        public async Task<PontoAtendimentoEntity> ObterPorIdAsync(Guid id)
        {
            var retorno = await _context.PontosAtendimento.FindAsync(id);
            if (retorno == null || retorno.Excluido)
            {
                return new PontoAtendimentoEntity
                {
                    Sucesso = false,
                    Mensagem = "Ponto de atendimento não encontrado ou excluído."
                };
            }
            retorno.Sucesso = true;
            return retorno;
        }

        public async Task<IEnumerable<PontoAtendimentoEntity>> ObterTodosPaginadoAsync(string nome = null, Guid? canalId = null, int page = 1, int pageSize = 20)
        {
            var query = _context.PontosAtendimento.Include(p => p.Canal).AsQueryable();

            if (!string.IsNullOrEmpty(nome))
                query = query.Where(c => c.RazaoSocial.Contains(nome));

            if (canalId.HasValue)
                query = query.Where(c => c.CanalId == canalId.Value);

            query = query.Where(c => c.Excluido == false);

            var totalItens = await query.AsNoTracking().CountAsync();
            var totalPaginas = (int)Math.Ceiling(totalItens / (double)pageSize);

            var pontosAtendimentoPaginados = await query
                .AsNoTracking()
                .OrderByDescending(c => c.RazaoSocial)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            foreach (var item in pontosAtendimentoPaginados)
            {
                item.Sucesso = true;
                item.TotalItems = totalItens;
                item.TotalPages = totalPaginas;
            }

            return pontosAtendimentoPaginados;
        }

        public async Task<IEnumerable<PontoAtendimentoEntity>> ObterPorCanalAsync(Guid canalId)
        {
            var pontos = await _context.PontosAtendimento
                .Where(pa => pa.CanalId == canalId && pa.Excluido == false)
                .ToListAsync();

            pontos.ForEach(p => p.Sucesso = true);

            return pontos;
        }

        /// <summary>
        /// Resolve o usuário logado com cascata de claims. Fail-closed: null = criação bloqueada.
        /// </summary>
        private async Task<Guid?> ObterUsuarioLogadoIdAsync()
        {
            var user = _httpContextAccessor.HttpContext?.User;
            if (user?.Identity?.IsAuthenticated != true)
                return null;

            foreach (var tipo in new[] { System.Security.Claims.ClaimTypes.NameIdentifier, "nameid", "sub" })
            {
                var valor = user.Claims.FirstOrDefault(c => c.Type == tipo)?.Value;
                if (Guid.TryParse(valor, out var id) && id != Guid.Empty)
                    return id;
            }

            var nome = user.Identity!.Name;
            if (!string.IsNullOrEmpty(nome))
            {
                var normalizado = nome.ToUpperInvariant();
                var entidade = await _context.Usuarios.AsNoTracking()
                    .FirstOrDefaultAsync(u => u.NormalizedEmail == normalizado);
                if (entidade != null)
                    return entidade.Id;
            }

            return null;
        }

        public async Task<PontoAtendimentoEntity> CadastrarAsync(PontoAtendimentoEntity entity)
        {
            var usuarioCriadorId = await ObterUsuarioLogadoIdAsync();
            if (!usuarioCriadorId.HasValue)
            {
                entity.Sucesso = false;
                entity.Mensagem = "Não foi possível identificar o usuário logado — criação de PA bloqueada. Faça login novamente.";
                return entity;
            }

            entity.UsuarioCriadorId = usuarioCriadorId;

            var (limiteOk, limiteMsg) = await _validacaoLimiteService
                .ValidarLimitePAsPorUsuarioAsync(usuarioCriadorId.Value, entity.CanalId);
            if (!limiteOk)
            {
                entity.Sucesso = false;
                entity.Mensagem = limiteMsg ?? "Limite de pontos de atendimento do plano atingido.";
                return entity;
            }

            entity.Ativo = true;
            entity.Excluido = false;

            _context.PontosAtendimento.Add(entity);
            var retorno = await _context.SaveChangesAsync();
            entity.Sucesso = retorno > 0;
            entity.Mensagem = retorno > 0 ? "Cadastrado com sucesso." : "Não foi possível cadastrar. Feche a tela e tente novamente.";
            return entity;
        }

        public async Task<PontoAtendimentoEntity> AtualizarAsync(PontoAtendimentoEntity entity)
        {
            // Preserva o criador: não vem do form e o Update marcaria como null
            var existente = await _context.PontosAtendimento.FindAsync(entity.Id);
            if (existente != null)
                entity.UsuarioCriadorId = existente.UsuarioCriadorId;

            _context.PontosAtendimento.Update(entity);
            var retorno = await _context.SaveChangesAsync();
            entity.Sucesso = retorno > 0;
            entity.Mensagem = retorno > 0 ? "Atualizado com sucesso." : "Não foi possível atualizar. Feche a tela e tente novamente.";
            return entity;
        }
    }
}
