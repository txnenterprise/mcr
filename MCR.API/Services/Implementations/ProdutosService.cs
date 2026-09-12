using Microsoft.EntityFrameworkCore;
using MCR.API.Entities;
using MCR.API.Repository;
using MCR.API.Services.Interfaces;

namespace MCR.API.Services.Implementations
{
    public class ProdutosService : IProdutosService
    {
        private readonly DbContextMCR _context;

        public ProdutosService(DbContextMCR context)
        {
            _context = context;
        }

        public async Task<ProdutosEntity> ObterPorIdAsync(Guid id)
        {
            var entidade = await _context.Produtos
                .Include(p => p.Taxas)
                .Include(c => c.ProdutosCanalPontoAtendimento)
                .Include(p => p.Safra)
                .Include(p => p.Cultura)
                .Include(p => p.Seguradora)
                .Include(p => p.SubvencaoFederal)
                .Include(p => p.SubvencaoEstadual)
                .Include(p => p.ProdutosSubvencoesEstaduais)
                    .ThenInclude(ps => ps.SubvencaoEstadual)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (entidade == null || entidade.Excluido)
                return null;

            return entidade;
        }

        public async Task<IEnumerable<ProdutosEntity>> ObterTodosPaginadoAsync(string descricao = null, Guid? seguradoraId = null, Guid? safraId = null, Guid? culturaId = null, bool? ativo = null, int page = 1, int pageSize = 20)
        {
            var query = _context.Produtos
                .Include(p => p.Safra)
                .Include(p => p.Cultura)
                .Include(p => p.Seguradora)
                .AsQueryable();

            if (!string.IsNullOrEmpty(descricao))
                query = query.Where(p => p.NomeProduto.Contains(descricao));

            query = query.Where(p => !p.Excluido);

            if (seguradoraId.HasValue)
                query = query.Where(p => p.SeguradoraId == seguradoraId.Value);

            if (safraId.HasValue)
                query = query.Where(p => p.SafraId == safraId.Value);

            if (culturaId.HasValue)
                query = query.Where(p => p.CulturaId == culturaId.Value);

            if (ativo.HasValue)
                query = query.Where(p => p.Ativo == ativo.Value);

            return await query
                .AsNoTracking()
                .OrderByDescending(p => p.DataCriacao)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<ProdutosEntity> CadastrarAsync(ProdutosEntity produto)
        {
            produto.Ativo = true;
            produto.Excluido = false;
            produto.DataCriacao = DateTime.UtcNow;

            _context.Produtos.Add(produto);
            await _context.SaveChangesAsync();

            return await _context.Produtos
                .Include(p => p.ProdutosSubvencoesEstaduais)
                .FirstOrDefaultAsync(p => p.Id == produto.Id);
        }

        public async Task<ProdutosEntity> AtualizarAsync(ProdutosEntity produto)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var produtoExistente = await _context.Produtos
                    .Include(p => p.ProdutosSubvencoesEstaduais)
                    .FirstOrDefaultAsync(p => p.Id == produto.Id);

                if (produtoExistente == null)
                    return null;

                produtoExistente.SafraId = produto.SafraId;
                produtoExistente.CulturaId = produto.CulturaId;
                produtoExistente.SeguradoraId = produto.SeguradoraId;
                produtoExistente.NomeProduto = produto.NomeProduto;
                produtoExistente.PorcentagemComissao = produto.PorcentagemComissao;
                produtoExistente.PorcentagemRepasse = produto.PorcentagemRepasse;
                produtoExistente.ProcessoSusep = produto.ProcessoSusep;
                produtoExistente.Modalidade = produto.Modalidade;
                produtoExistente.ValorSacaMinimo = produto.ValorSacaMinimo;
                produtoExistente.ValorSacaMaximo = produto.ValorSacaMaximo;
                produtoExistente.ValorSacaSugerido = produto.ValorSacaSugerido;
                produtoExistente.ValorCusteioMinimo = produto.ValorCusteioMinimo;
                produtoExistente.ValorCusteioMaximo = produto.ValorCusteioMaximo;
                produtoExistente.ValorCusteioSugerido = produto.ValorCusteioSugerido;
                produtoExistente.AreaMinimaItem = produto.AreaMinimaItem;
                produtoExistente.AreaMinimaTotal = produto.AreaMinimaTotal;
                produtoExistente.PremioMinimo = produto.PremioMinimo;
                produtoExistente.FormaDePagamento = produto.FormaDePagamento;
                produtoExistente.Parcelamento = produto.Parcelamento;
                produtoExistente.TipoSolo = produto.TipoSolo;
                produtoExistente.AjusteProdutividade = produto.AjusteProdutividade;
                produtoExistente.AjusteTaxa = produto.AjusteTaxa;
                produtoExistente.ClassificacaoSolosAceitos = produto.ClassificacaoSolosAceitos;
                produtoExistente.Replantio = produto.Replantio;
                produtoExistente.PorcentagemCoberturaProducao = produto.PorcentagemCoberturaProducao;
                produtoExistente.ValorCoberturaAdicional = produto.ValorCoberturaAdicional;
                produtoExistente.TaxaCoberturaAdicional = produto.TaxaCoberturaAdicional;
                produtoExistente.RegulacaoSinistro = produto.RegulacaoSinistro;
                produtoExistente.UtilizaSubvencaoFederal = produto.UtilizaSubvencaoFederal;
                produtoExistente.SubvencaoFederalId = produto.SubvencaoFederalId;
                produtoExistente.UtilizaSubvencaoEstadual = produto.UtilizaSubvencaoEstadual;
                produtoExistente.SubvencaoEstadualId = produto.SubvencaoEstadualId;
                produtoExistente.TermoDeCiencia = produto.TermoDeCiencia;
                produtoExistente.CapacidadeDisponivel = produto.CapacidadeDisponivel;
                produtoExistente.KiloPorSaca = produto.KiloPorSaca;
                produtoExistente.AceitaPlantioConsorciado = produto.AceitaPlantioConsorciado;
                produtoExistente.PlantioConsorciadoAjusteProdutividade = produto.PlantioConsorciadoAjusteProdutividade;
                produtoExistente.PlantioConsorciadoAjusteTaxa = produto.PlantioConsorciadoAjusteTaxa;
                produtoExistente.AceitaPlantioConvencional = produto.AceitaPlantioConvencional;
                produtoExistente.PlantioConvencionalAjusteProdutividade = produto.PlantioConvencionalAjusteProdutividade;
                produtoExistente.PlantioConvencionalAjusteTaxa = produto.PlantioConvencionalAjusteTaxa;
                produtoExistente.LavouraIrrigada = produto.LavouraIrrigada;
                produtoExistente.LavouraIrrigadaAjusteProdutividade = produto.LavouraIrrigadaAjusteProdutividade;
                produtoExistente.LavouraIrrigadaAjusteTaxa = produto.LavouraIrrigadaAjusteTaxa;
                produtoExistente.AceitaPlantioPosCanaDeAcucar = produto.AceitaPlantioPosCanaDeAcucar;
                produtoExistente.PlantioPosCanaAjusteProdutividade = produto.PlantioPosCanaAjusteProdutividade;
                produtoExistente.PlantioPosCanaAjusteTaxa = produto.PlantioPosCanaAjusteTaxa;

                var novosIds = produto.ProdutosSubvencoesEstaduais?.Select(ps => ps.SubvencaoEstadualId).ToList() ?? new List<Guid>();
                var existentes = produtoExistente.ProdutosSubvencoesEstaduais?.ToList() ?? new List<ProdutoSubvencaoEstadualEntity>();
                foreach (var ps in existentes.Where(ps => !novosIds.Contains(ps.SubvencaoEstadualId)))
                    _context.ProdutosSubvencoesEstaduais.Remove(ps);
                foreach (var id in novosIds.Except(existentes.Select(ps => ps.SubvencaoEstadualId)))
                    _context.ProdutosSubvencoesEstaduais.Add(new ProdutoSubvencaoEstadualEntity { ProdutoId = produtoExistente.Id, SubvencaoEstadualId = id });

                _context.Produtos.Update(produtoExistente);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return produtoExistente;
            }
            catch (DbUpdateConcurrencyException)
            {
                await transaction.RollbackAsync();
                throw new InvalidOperationException("Erro de concorrência: Os dados foram modificados por outro usuário. Por favor, atualize a página e tente novamente.");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw new InvalidOperationException($"Erro ao salvar as alterações: {ex.Message}", ex);
            }
        }

        public async Task<bool> ExcluirAsync(Guid id)
        {
            var entity = await _context.Produtos.FindAsync(id);
            if (entity == null)
                return false;

            entity.Excluido = true;
            entity.Ativo = false;
            _context.Produtos.Update(entity);
            var retorno = await _context.SaveChangesAsync();
            return retorno > 0;
        }

        public async Task<ProdutosEntity> DuplicarAsync(Guid id)
        {
            var produtoOriginal = await _context.Produtos
                .Include(p => p.Taxas)
                .Include(p => p.ProdutosCanalPontoAtendimento)
                .Include(p => p.ProdutosSubvencoesEstaduais)
                .FirstOrDefaultAsync(p => p.Id == id && !p.Excluido);

            if (produtoOriginal == null)
                return null;

            var novoProduto = new ProdutosEntity
            {
                NomeProduto = produtoOriginal.NomeProduto + " (Cópia)",
                SafraId = produtoOriginal.SafraId,
                CulturaId = produtoOriginal.CulturaId,
                SeguradoraId = produtoOriginal.SeguradoraId,
                PorcentagemComissao = produtoOriginal.PorcentagemComissao,
                PorcentagemRepasse = produtoOriginal.PorcentagemRepasse,
                ProcessoSusep = produtoOriginal.ProcessoSusep,
                Modalidade = produtoOriginal.Modalidade,
                ValorSacaMinimo = produtoOriginal.ValorSacaMinimo,
                ValorSacaMaximo = produtoOriginal.ValorSacaMaximo,
                ValorSacaSugerido = produtoOriginal.ValorSacaSugerido,
                ValorCusteioMinimo = produtoOriginal.ValorCusteioMinimo,
                ValorCusteioMaximo = produtoOriginal.ValorCusteioMaximo,
                ValorCusteioSugerido = produtoOriginal.ValorCusteioSugerido,
                AreaMinimaItem = produtoOriginal.AreaMinimaItem,
                AreaMinimaTotal = produtoOriginal.AreaMinimaTotal,
                PremioMinimo = produtoOriginal.PremioMinimo,
                FormaDePagamento = produtoOriginal.FormaDePagamento,
                Parcelamento = produtoOriginal.Parcelamento,
                TipoSolo = produtoOriginal.TipoSolo,
                AjusteProdutividade = produtoOriginal.AjusteProdutividade,
                AjusteTaxa = produtoOriginal.AjusteTaxa,
                ClassificacaoSolosAceitos = produtoOriginal.ClassificacaoSolosAceitos,
                Replantio = produtoOriginal.Replantio,
                PorcentagemCoberturaProducao = produtoOriginal.PorcentagemCoberturaProducao,
                ValorCoberturaAdicional = produtoOriginal.ValorCoberturaAdicional,
                TaxaCoberturaAdicional = produtoOriginal.TaxaCoberturaAdicional,
                RegulacaoSinistro = produtoOriginal.RegulacaoSinistro,
                UtilizaSubvencaoFederal = produtoOriginal.UtilizaSubvencaoFederal,
                SubvencaoFederalId = produtoOriginal.SubvencaoFederalId,
                UtilizaSubvencaoEstadual = produtoOriginal.UtilizaSubvencaoEstadual,
                SubvencaoEstadualId = produtoOriginal.SubvencaoEstadualId,
                TermoDeCiencia = produtoOriginal.TermoDeCiencia,
                CapacidadeDisponivel = produtoOriginal.CapacidadeDisponivel,
                KiloPorSaca = produtoOriginal.KiloPorSaca,
                AceitaPlantioConsorciado = produtoOriginal.AceitaPlantioConsorciado,
                PlantioConsorciadoAjusteProdutividade = produtoOriginal.PlantioConsorciadoAjusteProdutividade,
                PlantioConsorciadoAjusteTaxa = produtoOriginal.PlantioConsorciadoAjusteTaxa,
                AceitaPlantioConvencional = produtoOriginal.AceitaPlantioConvencional,
                PlantioConvencionalAjusteProdutividade = produtoOriginal.PlantioConvencionalAjusteProdutividade,
                PlantioConvencionalAjusteTaxa = produtoOriginal.PlantioConvencionalAjusteTaxa,
                LavouraIrrigada = produtoOriginal.LavouraIrrigada,
                LavouraIrrigadaAjusteProdutividade = produtoOriginal.LavouraIrrigadaAjusteProdutividade,
                LavouraIrrigadaAjusteTaxa = produtoOriginal.LavouraIrrigadaAjusteTaxa,
                AceitaPlantioPosCanaDeAcucar = produtoOriginal.AceitaPlantioPosCanaDeAcucar,
                PlantioPosCanaAjusteProdutividade = produtoOriginal.PlantioPosCanaAjusteProdutividade,
                PlantioPosCanaAjusteTaxa = produtoOriginal.PlantioPosCanaAjusteTaxa,
                Ativo = true,
                Excluido = false,
                DataCriacao = DateTime.UtcNow
            };

            _context.Produtos.Add(novoProduto);
            await _context.SaveChangesAsync();

            foreach (var ps in produtoOriginal.ProdutosSubvencoesEstaduais ?? Enumerable.Empty<ProdutoSubvencaoEstadualEntity>())
            {
                _context.ProdutosSubvencoesEstaduais.Add(new ProdutoSubvencaoEstadualEntity
                {
                    ProdutoId = novoProduto.Id,
                    SubvencaoEstadualId = ps.SubvencaoEstadualId
                });
            }

            foreach (var taxa in produtoOriginal.Taxas.Where(t => !t.Excluido))
            {
                _context.ProdutosTaxas.Add(new ProdutosTaxasEntity
                {
                    ProdutoId = novoProduto.Id,
                    UF = taxa.UF,
                    Municipio = taxa.Municipio,
                    ProdutividadeEsperada = taxa.ProdutividadeEsperada,
                    TaxaNc65 = taxa.TaxaNc65,
                    TaxaNc70 = taxa.TaxaNc70,
                    TaxaNc75 = taxa.TaxaNc75,
                    Cpf = taxa.Cpf,
                    Ativo = true,
                    Excluido = false
                });
            }

            foreach (var canal in produtoOriginal.ProdutosCanalPontoAtendimento)
            {
                _context.ProdutosCanalPontosAtendimento.Add(new ProdutosCanalPontoAtendimentoEntity
                {
                    ProdutoId = novoProduto.Id,
                    CanalId = canal.CanalId,
                    PontoAtendimentoId = canal.PontoAtendimentoId,
                });
            }

            await _context.SaveChangesAsync();
            return novoProduto;
        }

        public async Task<bool> ToggleAtivoAsync(Guid id)
        {
            var entity = await _context.Produtos.FindAsync(id);
            if (entity == null)
                return false;

            entity.Ativo = !entity.Ativo;
            _context.Produtos.Update(entity);
            var retorno = await _context.SaveChangesAsync();
            return retorno > 0;
        }
    }
}
