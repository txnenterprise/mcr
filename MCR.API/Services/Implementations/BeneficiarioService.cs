using Microsoft.EntityFrameworkCore;
using MCR.API.Entities;
using MCR.API.Repository;
using MCR.API.Services.Interfaces;

namespace MCR.API.Services.Implementations
{
    public class BeneficiarioService : IBeneficiarioService
    {
        private readonly DbContextMCR _context;

        public BeneficiarioService(DbContextMCR context)
        {
            _context = context;
        }

        public async Task<IEnumerable<BeneficiarioEntity>> ObterTodosPaginadoAsync(string nome = null, string cnpj = null, bool? ativo = null, int page = 1, int pageSize = 20)
        {
            var query = _context.Beneficiarios
                .Where(c => !c.Excluido)
                .AsQueryable();

            if (!string.IsNullOrEmpty(nome))
                query = query.Where(c => c.Nome.Contains(nome));
            if (!string.IsNullOrEmpty(cnpj))
                query = query.Where(c => c.CNPJ.Replace(".", "").Replace("-", "").Contains(cnpj.Replace(".", "").Replace("-", "")));
            if (ativo.HasValue)
                query = query.Where(c => c.Ativo == ativo.Value);

            var totalItems = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            var results = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            foreach (var beneficiario in results)
            {
                beneficiario.TotalItems = totalItems;
                beneficiario.TotalPages = totalPages;
                beneficiario.Sucesso = true;
                beneficiario.Mensagem = "Dados carregados com sucesso!";
            }

            return results;
        }

        public async Task<BeneficiarioEntity> ObterPorIdAsync(Guid id)
        {
            var beneficiario = await _context.Beneficiarios
                .FirstOrDefaultAsync(c => c.Id == id && !c.Excluido);

            if (beneficiario == null)
            {
                return new BeneficiarioEntity
                {
                    Sucesso = false,
                    Mensagem = "Beneficiario não encontrado ou excluído."
                };
            }

            beneficiario.Sucesso = true;
            return beneficiario;
        }

        public async Task<BeneficiarioEntity> CadastrarAsync(BeneficiarioEntity entity)
        {
            entity.Ativo = true;
            entity.Excluido = false;

            var cnpjExiste = await _context.Beneficiarios.Where(c => c.CNPJ.Replace(".", "").Replace("-", "").Equals(entity.CNPJ.Replace(".", "").Replace("-", ""))).CountAsync();

            if (cnpjExiste > 0)
            {
                entity.Sucesso = false;
                entity.Mensagem = $"O CNPJ {entity.CNPJ} já foi cadastrado.";
                return entity;
            }

            _context.Beneficiarios.Add(entity);
            var retorno = await _context.SaveChangesAsync();
            entity.Sucesso = retorno > 0;
            entity.Mensagem = retorno > 0 ? "Cadastrado com sucesso." : "Não foi possível cadastrar. Feche a tela e tente novamente.";

            return entity;
        }

        public async Task<BeneficiarioEntity> AtualizarAsync(BeneficiarioEntity entity)
        {
            var cnpjExiste = await _context.Beneficiarios.Where(c => c.CNPJ.Replace(".", "").Replace("-", "").Equals(entity.CNPJ.Replace(".", "").Replace("-", ""))).FirstOrDefaultAsync();

            if (cnpjExiste != null && !string.IsNullOrEmpty(cnpjExiste.CNPJ) && cnpjExiste.Id != entity.Id)
            {
                entity.Sucesso = false;
                entity.Mensagem = $"O CNPJ {entity.CNPJ} já foi cadastrado.";
                return entity;
            }

            _context.Beneficiarios.Update(entity);
            var retorno = await _context.SaveChangesAsync();
            entity.Sucesso = retorno > 0;
            entity.Mensagem = retorno > 0 ? "Atualizado com sucesso." : "Não foi possível atualizar. Feche a tela e tente novamente.";

            return entity;
        }
    }
}
