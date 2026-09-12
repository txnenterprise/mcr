using Microsoft.EntityFrameworkCore;
using MCR.API.Entities;
using MCR.API.Repository;
using MCR.API.Services.Interfaces;

namespace MCR.API.Services.Implementations
{
    public class PropriedadeService : IPropriedadeService
    {
        private readonly DbContextMCR _context;

        public PropriedadeService(DbContextMCR context)
        {
            _context = context;
        }

        private ValidationResult Validar(PropriedadeEntity entity)
        {
            var errors = new List<string>();

            if (string.IsNullOrEmpty(entity.Nome))
                errors.Add("Informe o nome.");
            if (string.IsNullOrEmpty(entity.Endereco))
                errors.Add("Informe o endereço.");
            if (string.IsNullOrEmpty(entity.Bairro))
                errors.Add("Informe o bairro.");
            if (string.IsNullOrEmpty(entity.Cidade))
                errors.Add("Informe a cidade.");
            if (string.IsNullOrEmpty(entity.Estado))
                errors.Add("Informe o estado.");
            if (string.IsNullOrEmpty(entity.CEP))
                errors.Add("Informe o CEP.");
            // SomaAreaTotalTalhao é calculado automaticamente após cadastro de talhões
            // Não validar aqui — aceitar 0 para propriedades novas

            return errors.Any() ? new ValidationResult(false, errors) : new ValidationResult(true, null);
        }

        public async Task<PropriedadeEntity> ObterPorIdAsync(Guid id)
        {
            var retorno = await _context.Propriedades.FindAsync(id);
            if (retorno == null || retorno.Excluido)
            {
                return new PropriedadeEntity
                {
                    Sucesso = false,
                    Mensagem = "Propriedade não encontrada ou excluída."
                };
            }
            retorno.Sucesso = true;
            return retorno;
        }

        public async Task<IEnumerable<PropriedadeEntity>> ObterTodosPaginadoAsync(
            string nome = null, string estado = null,
            string cidade = null, bool? ativo = null,
            int page = 1, int pageSize = 20)
        {
            var query = _context.Propriedades
                .AsQueryable();

            if (!string.IsNullOrEmpty(nome))
                query = query.Where(c => c.Nome.Contains(nome));
            if (!string.IsNullOrEmpty(estado))
                query = query.Where(c => c.Estado.Contains(estado));
            if (!string.IsNullOrEmpty(cidade))
                query = query.Where(c => c.Cidade.Contains(cidade));
            if (ativo.HasValue)
                query = query.Where(c => c.Ativo == ativo.Value);

            query = query.Where(c => c.Excluido == false);

            var totalItems = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            var results = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            foreach (var propriedade in results)
            {
                propriedade.TotalItems = totalItems;
                propriedade.TotalPages = totalPages;
                propriedade.Sucesso = true;
                propriedade.Mensagem = "Dados carregados com sucesso!";
            }

            return results;
        }

        public async Task<PropriedadeEntity> CadastrarAsync(PropriedadeEntity entity)
        {
            entity.Ativo = true;
            entity.Excluido = false;
            entity.ImagemGeralTodosTalhoes ??= "";

            var validationResult = Validar(entity);
            if (!validationResult.IsValid)
            {
                entity.Sucesso = false;
                entity.Mensagem = string.Join("; ", validationResult.Errors);
                return entity;
            }

            _context.Propriedades.Add(entity);
            var retorno = await _context.SaveChangesAsync();
            entity.Sucesso = retorno > 0;
            entity.Mensagem = retorno > 0 ? "Cadastrado com sucesso." : "Não foi possível cadastrar. Feche a tela e tente novamente.";
            return entity;
        }

        public async Task<PropriedadeEntity> AtualizarAsync(PropriedadeEntity entity)
        {
            var validationResult = Validar(entity);
            if (!validationResult.IsValid)
            {
                entity.Sucesso = false;
                entity.Mensagem = string.Join("; ", validationResult.Errors);
                return entity;
            }

            _context.Propriedades.Update(entity);
            var retorno = await _context.SaveChangesAsync();
            entity.Sucesso = retorno > 0;
            entity.Mensagem = retorno > 0 ? "Atualizado com sucesso." : "Não foi possível atualizar. Feche a tela e tente novamente.";
            return entity;
        }

        public async Task<IList<PropriedadeEntity>> PesquisarPropriedadesDisponiveisIndex(string nome, string estado, string cidade, string ativo)
        {
            var propriedadesVinculadasIds = await _context.VinculosPropriedadesClientes
                .Select(v => v.PropriedadeId)
                .Distinct()
                .ToListAsync();

            var query = _context.Propriedades.AsQueryable();

            if (!string.IsNullOrEmpty(nome))
                query = query.Where(c => c.Nome.Contains(nome));
            if (!string.IsNullOrEmpty(estado))
                query = query.Where(c => c.Estado.Contains(estado));
            if (!string.IsNullOrEmpty(cidade))
                query = query.Where(c => c.Cidade.Contains(cidade));
            if (ativo == "Sim")
                query = query.Where(c => c.Ativo);
            else if (ativo == "Não")
                query = query.Where(c => !c.Ativo);

            query = query.Where(c => !c.Excluido);

            if (propriedadesVinculadasIds.Any())
                query = query.Where(c => !propriedadesVinculadasIds.Contains(c.Id));

            var results = await query.ToListAsync();

            foreach (var propriedade in results)
            {
                propriedade.Sucesso = true;
                propriedade.Mensagem = "Dados carregados com sucesso!";
                propriedade.SomaAreaTotalTalhao = await _context.Talhoes
                    .Where(t => t.PropriedadeId == propriedade.Id && t.Ativo && !t.Excluido)
                    .SumAsync(t => t.Area);
            }

            return results;
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
