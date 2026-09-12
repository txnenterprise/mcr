using Microsoft.EntityFrameworkCore;
using MCR.API.Entities;
using MCR.API.Repository;
using MCR.API.Services.Interfaces;

namespace MCR.API.Services.Implementations
{
    public class ProdutosTaxasService : IProdutosTaxasService
    {
        private readonly DbContextMCR _context;

        public ProdutosTaxasService(DbContextMCR context)
        {
            _context = context;
        }

        public async Task<ProdutosTaxasEntity> ObterPorIdAsync(Guid id)
        {
            var entity = await _context.ProdutosTaxas
                .FirstOrDefaultAsync(t => t.Id == id && !t.Excluido);

            if (entity == null)
                return null;

            return entity;
        }

        public async Task<IEnumerable<ProdutosTaxasEntity>> ObterPorProdutoIdAsync(Guid produtoId)
        {
            return await _context.ProdutosTaxas
                .Where(t => t.ProdutoId == produtoId && !t.Excluido)
                .ToListAsync();
        }

        public async Task<bool> AdicionarAsync(ProdutosTaxasEntity taxa)
        {
            taxa.Ativo = true;
            taxa.Excluido = false;

            try
            {
                _context.ProdutosTaxas.Add(taxa);
                var retorno = await _context.SaveChangesAsync();
                return retorno > 0;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> AtualizarAsync(ProdutosTaxasEntity taxa)
        {
            try
            {
                var existing = await _context.ProdutosTaxas.FindAsync(taxa.Id);
                if (existing == null) return false;

                existing.UF = taxa.UF;
                existing.Municipio = taxa.Municipio;
                existing.ProdutividadeEsperada = taxa.ProdutividadeEsperada;
                existing.TaxaNc65 = taxa.TaxaNc65;
                existing.TaxaNc70 = taxa.TaxaNc70;
                existing.TaxaNc75 = taxa.TaxaNc75;
                existing.Cpf = taxa.Cpf;

                var retorno = await _context.SaveChangesAsync();
                return retorno > 0;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> ExcluirAsync(Guid id)
        {
            var entity = await _context.ProdutosTaxas.FindAsync(id);
            if (entity == null)
                return false;

            entity.Excluido = true;
            entity.Ativo = false;
            _context.ProdutosTaxas.Update(entity);
            var retorno = await _context.SaveChangesAsync();
            return retorno > 0;
        }

        public async Task<bool> ImportarCsvAsync(Guid produtoId, Stream arquivo)
        {
            try
            {
                using var reader = new StreamReader(arquivo);
                var header = await reader.ReadLineAsync();
                var taxas = new List<ProdutosTaxasEntity>();

                while (!reader.EndOfStream)
                {
                    var line = await reader.ReadLineAsync();
                    if (string.IsNullOrWhiteSpace(line))
                        continue;

                    var colunas = line.Split(';');
                    if (colunas.Length < 4)
                        continue;

                    var taxa = new ProdutosTaxasEntity
                    {
                        ProdutoId = produtoId,
                        UF = colunas[0].Trim(),
                        Municipio = colunas[1].Trim(),
                        ProdutividadeEsperada = decimal.TryParse(colunas[2].Trim(), out var prod) ? prod : 0,
                        TaxaNc65 = decimal.TryParse(colunas[3].Trim(), out var taxa65) ? taxa65 : 0,
                        TaxaNc70 = colunas.Length > 4 && decimal.TryParse(colunas[4].Trim(), out var taxa70) ? taxa70 : null,
                        TaxaNc75 = colunas.Length > 5 && decimal.TryParse(colunas[5].Trim(), out var taxa75) ? taxa75 : null,
                        Cpf = colunas.Length > 6 ? colunas[6].Trim() : null,
                        Ativo = true,
                        Excluido = false
                    };

                    taxas.Add(taxa);
                }

                _context.ProdutosTaxas.AddRange(taxas);
                var retorno = await _context.SaveChangesAsync();
                return retorno > 0;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> ExcluirTodasPorProdutoAsync(Guid produtoId)
        {
            var taxas = await _context.ProdutosTaxas
                .Where(t => t.ProdutoId == produtoId && !t.Excluido)
                .ToListAsync();

            if (!taxas.Any())
                return false;

            foreach (var taxa in taxas)
            {
                taxa.Excluido = true;
                taxa.Ativo = false;
            }

            _context.ProdutosTaxas.UpdateRange(taxas);
            var retorno = await _context.SaveChangesAsync();
            return retorno > 0;
        }
    }
}
