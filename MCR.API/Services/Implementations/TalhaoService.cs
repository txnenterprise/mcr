using Microsoft.EntityFrameworkCore;
using MCR.API.Entities;
using MCR.API.Repository;
using MCR.API.Services.Interfaces;

namespace MCR.API.Services.Implementations
{
    public class TalhaoService : ITalhaoService
    {
        private readonly DbContextMCR _context;

        public TalhaoService(DbContextMCR context)
        {
            _context = context;
        }

        private ValidationResult Validar(TalhaoEntity entity)
        {
            var errors = new List<string>();

            if (string.IsNullOrEmpty(entity.Nome))
                errors.Add("Informe o nome.");
            if (entity.PropriedadeId == Guid.Empty)
                errors.Add("Informe uma propriedade.");
            if (entity.Area <= 0)
                errors.Add("Informe a área.");
            if (string.IsNullOrEmpty(entity.Latitude))
                errors.Add("Informe a latitude.");
            if (string.IsNullOrEmpty(entity.Longitude))
                errors.Add("Informe a longitude.");
            if (string.IsNullOrEmpty(entity.RoteiroAcesso))
                errors.Add("Informe o roteiro de acesso.");
            if (string.IsNullOrEmpty(entity.TipoSolo))
                errors.Add("Informe o tipo de solo.");
            if (string.IsNullOrEmpty(entity.ClassificacaoSolo))
                errors.Add("Informe a classificação do solo.");

            if (entity.PossuiAnaliseFisicaSolo)
            {
                if (entity.PercentualAreia == 0 || entity.PercentualArgila == 0 || entity.PercentualSilte == 0)
                    errors.Add("Como possui análise física do solo, informe o percentual de areia, silte e argila.");
            }
            else
            {
                entity.PercentualAreia = 0;
                entity.PercentualArgila = 0;
                entity.PercentualSilte = 0;
            }

            return errors.Any() ? new ValidationResult(false, errors) : new ValidationResult(true, null);
        }

        public async Task<TalhaoEntity> ObterPorIdAsync(Guid id)
        {
            var query = _context.Talhoes.AsQueryable();
            query = query.Where(c => c.Id == id && c.Excluido == false);

            var retorno = await query.FirstOrDefaultAsync();

            if (retorno == null || retorno.Excluido)
            {
                return new TalhaoEntity
                {
                    Sucesso = false,
                    Mensagem = "Talhão não encontrado ou excluído."
                };
            }

            retorno.Sucesso = true;
            return retorno;
        }

        public async Task<IEnumerable<TalhaoEntity>> ObterTodosPaginadoAsync(
            string descricao = null, Guid? propriedadeId = null,
            int page = 1, int pageSize = 20)
        {
            var query = _context.Talhoes
                .AsQueryable();

            if (!string.IsNullOrEmpty(descricao))
                query = query.Where(c => c.Nome.Contains(descricao));
            if (propriedadeId.HasValue && propriedadeId != Guid.Empty)
                query = query.Where(c => c.PropriedadeId == propriedadeId.Value);

            query = query.Where(c => c.Excluido == false);

            var totalItens = await query.AsNoTracking().CountAsync();
            var totalPaginas = (int)Math.Ceiling(totalItens / (double)pageSize);

            var talhoesPaginados = await query
                .AsNoTracking()
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            foreach (var item in talhoesPaginados)
            {
                item.Sucesso = true;
                item.TotalItems = totalItens;
                item.TotalPages = totalPaginas;
            }

            return talhoesPaginados;
        }

        public async Task<IEnumerable<TalhaoEntity>> ObterPorPropriedadeAsync(Guid propriedadeId)
        {
            var results = await _context.Talhoes
                .Where(c => c.PropriedadeId == propriedadeId && c.Excluido == false)
                .ToListAsync();

            foreach (var talhao in results)
            {
                talhao.Sucesso = true;
                talhao.Mensagem = "Dados carregados com sucesso!";
            }

            return results;
        }

        public async Task<TalhaoEntity> CadastrarAsync(TalhaoEntity entity)
        {
            entity.Ativo = true;
            entity.Excluido = false;

            var validationResult = Validar(entity);
            if (!validationResult.IsValid)
            {
                entity.Sucesso = false;
                entity.Mensagem = string.Join("; ", validationResult.Errors);
                return entity;
            }

            _context.Talhoes.Add(entity);
            var retorno = await _context.SaveChangesAsync();
            entity.Sucesso = retorno > 0;
            entity.Mensagem = retorno > 0 ? "Cadastrado com sucesso." : "Não foi possível cadastrar. Feche a tela e tente novamente.";
            return entity;
        }

        public async Task<TalhaoEntity> AtualizarAsync(TalhaoEntity entity)
        {
            var validationResult = Validar(entity);
            if (!validationResult.IsValid)
            {
                entity.Sucesso = false;
                entity.Mensagem = string.Join("; ", validationResult.Errors);
                return entity;
            }

            _context.Talhoes.Update(entity);
            var retorno = await _context.SaveChangesAsync();
            entity.Sucesso = retorno > 0;
            entity.Mensagem = retorno > 0 ? "Atualizado com sucesso." : "Não foi possível atualizar. Feche a tela e tente novamente.";
            return entity;
        }

        private class ValidationResult
        {
            public bool IsValid { get; }
            public List<string> Errors { get; }

            public ValidationResult(bool isValid, List<string> errors)
            {
                IsValid = isValid;
                Errors = errors ?? new List<string>();
            }
        }
    }
}
