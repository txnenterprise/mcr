using System.Text.Json;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MCR.API.Entities;
using MCR.API.Repository;
using MCR.API.Services.Interfaces;

namespace MCR.API.Services.Implementations
{
    public class CotacaoService : ICotacaoService
    {
        private readonly DbContextMCR _context;

        public CotacaoService(DbContextMCR context)
        {
            _context = context;
        }

        private CotacaoEntity Validar(CotacaoEntity entity)
        {
            if (entity == null)
            {
                return new CotacaoEntity
                {
                    Mensagem = "Cadastro nulo ou vazio.",
                    Sucesso = false
                };
            }

            if (entity.InformacaoSeguro == null)
            {
                entity.Mensagem = "Informações do seguro não informadas.";
                entity.Sucesso = false;
                return entity;
            }
            if (string.IsNullOrEmpty(entity.InformacaoSeguro.TempoVigenciaSeguro))
            {
                entity.Mensagem = "Tempo de Vigência é de preenchimento obrigatório.";
                entity.Sucesso = false;
                return entity;
            }
            if (string.IsNullOrEmpty(entity.InformacaoSeguro.TipoSeguro))
            {
                entity.Mensagem = "Tipo de Seguro é de preenchimento obrigatório.";
                entity.Sucesso = false;
                return entity;
            }

            if (entity.InformacaoSegurado == null)
            {
                entity.Mensagem = "Informações do segurado não informadas.";
                entity.Sucesso = false;
                return entity;
            }
            if (string.IsNullOrEmpty(entity.InformacaoSegurado.Nome))
            {
                entity.Mensagem = "Nome do segurado não informado.";
                entity.Sucesso = false;
                return entity;
            }
            if (string.IsNullOrEmpty(entity.InformacaoSegurado.TipoPessoa))
            {
                entity.Mensagem = "Tipo de Pessoa não informado.";
                entity.Sucesso = false;
                return entity;
            }
            if (string.IsNullOrEmpty(entity.InformacaoSegurado.CPFCNPJ))
            {
                entity.Mensagem = "CPF ou CNPJ não informado.";
                entity.Sucesso = false;
                return entity;
            }
            if (string.IsNullOrEmpty(entity.InformacaoSegurado.CEP))
            {
                entity.Mensagem = "CEP não informado.";
                entity.Sucesso = false;
                return entity;
            }
            if (string.IsNullOrEmpty(entity.InformacaoSegurado.Endereco))
            {
                entity.Mensagem = "Endereço não informado.";
                entity.Sucesso = false;
                return entity;
            }
            if (string.IsNullOrEmpty(entity.InformacaoSegurado.Numero))
            {
                entity.Mensagem = "Número não informado.";
                entity.Sucesso = false;
                return entity;
            }
            if (string.IsNullOrEmpty(entity.InformacaoSegurado.Bairro))
            {
                entity.Mensagem = "Bairro não informado.";
                entity.Sucesso = false;
                return entity;
            }
            if (string.IsNullOrEmpty(entity.InformacaoSegurado.Estado))
            {
                entity.Mensagem = "Estado não informado.";
                entity.Sucesso = false;
                return entity;
            }

            if (entity.ListaBensDTO == null)
            {
                entity.Mensagem = "Nenhum bem informado.";
                entity.Sucesso = false;
                return entity;
            }
            if (entity.ListaBensDTO.Count == 0)
            {
                entity.Mensagem = "Nenhum bem informado.";
                entity.Sucesso = false;
                return entity;
            }
            foreach (var bem in entity.ListaBensDTO)
            {
                if (string.IsNullOrEmpty(bem.Equipamento.TipoEquipamento))
                {
                    entity.Mensagem = "Tipo de Equipamento não informado.";
                    entity.Sucesso = false;
                    return entity;
                }
                if (bem.Equipamento.AnoFabricacao == 0)
                {
                    entity.Mensagem = $"Ano de Fabricação não informado (Equipamento: {bem.Equipamento.TipoEquipamento}).";
                    entity.Sucesso = false;
                    return entity;
                }
                if (bem.Equipamento.ValorEquipamento == 0)
                {
                    entity.Mensagem = $"Valor do Equipamento não informado (Equipamento: {bem.Equipamento.TipoEquipamento}).";
                    entity.Sucesso = false;
                    return entity;
                }
                if (string.IsNullOrEmpty(bem.Equipamento.MarcaEquipamento))
                {
                    entity.Mensagem = $"Marca do Equipamento não informado (Equipamento: {bem.Equipamento.TipoEquipamento}).";
                    entity.Sucesso = false;
                    return entity;
                }
                if (string.IsNullOrEmpty(bem.Equipamento.ModeloEquipamento))
                {
                    entity.Mensagem = $"Modelo do Equipamento não informado (Equipamento: {bem.Equipamento.TipoEquipamento}).";
                    entity.Sucesso = false;
                    return entity;
                }
                if (bem.Equipamento.InformarNotaFiscal)
                {
                    if (string.IsNullOrEmpty(bem.Equipamento.NumeroNotaFiscal))
                    {
                        entity.Mensagem = $"Número da Nota Fiscal não informado (Equipamento: {bem.Equipamento.TipoEquipamento}).";
                        entity.Sucesso = false;
                        return entity;
                    }
                    if (bem.Equipamento.DataNotaFiscal == DateTime.MinValue)
                    {
                        entity.Mensagem = $"Data da Nota Fiscal não informado (Equipamento: {bem.Equipamento.TipoEquipamento}).";
                        entity.Sucesso = false;
                        return entity;
                    }
                }
            }

            if (entity.CotacaoInformacoesBeneficiario == null)
            {
                entity.Mensagem = "Informações do Beneficiário não foram informadas.";
                entity.Sucesso = false;
                return entity;
            }
            if (string.IsNullOrEmpty(entity.CotacaoInformacoesBeneficiario.TipoPessoa))
            {
                entity.Mensagem = "Tipo de Pessoa (Beneficiário) não informado.";
                entity.Sucesso = false;
                return entity;
            }
            if (string.IsNullOrEmpty(entity.CotacaoInformacoesBeneficiario.NomeBeneficiario))
            {
                entity.Mensagem = "Nome (Beneficiário) não informado.";
                entity.Sucesso = false;
                return entity;
            }
            if (string.IsNullOrEmpty(entity.CotacaoInformacoesBeneficiario.CPFCNPJBeneficiario))
            {
                entity.Mensagem = "CPF/CNPJ (Beneficiário) não informado.";
                entity.Sucesso = false;
                return entity;
            }

            entity.Mensagem = "Validação efetuada com sucesso!";
            entity.Sucesso = true;
            return entity;
        }

        private IList<CotacaoCondicaoComercialSeguradoraEntity> EstruturarCondicaoComercialPorSeguradora(CotacaoEntity entity)
        {
            return new List<CotacaoCondicaoComercialSeguradoraEntity>
            {
                new() { CotacaoId = entity.Id, Seguradora = "Mapfre", Comissao = entity.CotacaoCondicaoComercial.ComissaoSeguradoraMapfre, DescontoAgravo = entity.CotacaoCondicaoComercial.DescontoAgravoSeguradoraMapfre, MultiplicadorFranquia = entity.CotacaoCondicaoComercial.MultiplicadorFranquiaSeguradoraMapfre },
                new() { CotacaoId = entity.Id, Seguradora = "SwissRe", Comissao = entity.CotacaoCondicaoComercial.ComissaoSeguradoraSwissRe, DescontoAgravo = entity.CotacaoCondicaoComercial.DescontoAgravoSeguradoraSwissRe, MultiplicadorFranquia = entity.CotacaoCondicaoComercial.MultiplicadorFranquiaSeguradoraSwissRe },
                new() { CotacaoId = entity.Id, Seguradora = "Allianz", Comissao = entity.CotacaoCondicaoComercial.ComissaoSeguradoraAllianz, DescontoAgravo = entity.CotacaoCondicaoComercial.DescontoAgravoSeguradoraAllianz, MultiplicadorFranquia = entity.CotacaoCondicaoComercial.MultiplicadorFranquiaSeguradoraAllianz },
                new() { CotacaoId = entity.Id, Seguradora = "Tokio", Comissao = entity.CotacaoCondicaoComercial.ComissaoSeguradoraTokio, DescontoAgravo = entity.CotacaoCondicaoComercial.DescontoAgravoSeguradoraTokio, MultiplicadorFranquia = entity.CotacaoCondicaoComercial.MultiplicadorFranquiaSeguradoraTokio },
                new() { CotacaoId = entity.Id, Seguradora = "SOMPO", Comissao = entity.CotacaoCondicaoComercial.ComissaoSeguradoraSOMPO, DescontoAgravo = entity.CotacaoCondicaoComercial.DescontoAgravoSeguradoraSOMPO, MultiplicadorFranquia = entity.CotacaoCondicaoComercial.MultiplicadorFranquiaSeguradoraSOMPO },
                new() { CotacaoId = entity.Id, Seguradora = "Pottencial", Comissao = entity.CotacaoCondicaoComercial.ComissaoSeguradoraPottencial, DescontoAgravo = entity.CotacaoCondicaoComercial.DescontoAgravoSeguradoraPottencial, MultiplicadorFranquia = entity.CotacaoCondicaoComercial.MultiplicadorFranquiaSeguradoraPottencial },
                new() { CotacaoId = entity.Id, Seguradora = "Sombrero", Comissao = entity.CotacaoCondicaoComercial.ComissaoSeguradoraSombrero, DescontoAgravo = entity.CotacaoCondicaoComercial.DescontoAgravoSeguradoraSombrero, MultiplicadorFranquia = entity.CotacaoCondicaoComercial.MultiplicadorFranquiaSeguradoraSombrero },
                new() { CotacaoId = entity.Id, Seguradora = "FairFax", Comissao = entity.CotacaoCondicaoComercial.ComissaoSeguradoraFF, DescontoAgravo = entity.CotacaoCondicaoComercial.DescontoAgravoSeguradoraFF, MultiplicadorFranquia = entity.CotacaoCondicaoComercial.MultiplicadorFranquiaSeguradoraFF }
            };
        }

        public async Task<CotacaoEntity> CadastrarAsync(CotacaoEntity entity)
        {
            var validacoes = Validar(entity);
            if (!validacoes.Sucesso)
                return validacoes;

            entity.Cancelado = false;
            entity.Efetivada = false;
            entity.DataHoraCotacao = DateTime.Now;
            entity.JsonCotacao = JsonSerializer.Serialize(entity);

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                _context.Add(entity);
                await _context.SaveChangesAsync();

                entity.CotacaoCondicaoComercial.CotacaoId = entity.Id;
                _context.Add(entity.CotacaoCondicaoComercial);

                entity.CotacaoInformacoesBeneficiario.CotacaoId = entity.Id;
                _context.Add(entity.CotacaoInformacoesBeneficiario);

                entity.InformacaoSegurado.CotacaoId = entity.Id;
                _context.Add(entity.InformacaoSegurado);

                entity.InformacaoSeguro.CotacaoId = entity.Id;
                _context.Add(entity.InformacaoSeguro);

                foreach (var equipamento in entity.ListaBensDTO)
                {
                    equipamento.Equipamento.CotacaoId = entity.Id;
                    _context.Add(equipamento.Equipamento);

                    equipamento.CotacaoCobertura.CotacaoId = entity.Id;
                    equipamento.CotacaoCobertura.BemId = equipamento.Id;
                    _context.Add(equipamento.CotacaoCobertura);

                    equipamento.CotacaoFormularioRisco.CotacaoId = entity.Id;
                    equipamento.CotacaoFormularioRisco.BemId = equipamento.Id;
                    _context.Add(equipamento.CotacaoFormularioRisco);
                }

                var listaCondicaoComercialSeguradora = EstruturarCondicaoComercialPorSeguradora(entity);
                foreach (var cotacaoSeguradora in listaCondicaoComercialSeguradora)
                {
                    cotacaoSeguradora.CotacaoId = entity.Id;
                    _context.Add(cotacaoSeguradora);
                }

                var result = await _context.SaveChangesAsync();
                if (result <= 0)
                {
                    await transaction.RollbackAsync();
                    entity.Sucesso = false;
                    entity.Mensagem = "Erro ao efetuar o cadastro.";
                    return entity;
                }

                await transaction.CommitAsync();
                entity.Mensagem = "Cadastro realizado com sucesso!";
                entity.Sucesso = true;
                return entity;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                entity.Sucesso = false;
                entity.Mensagem = "Erro ao efetuar o cadastro: " + ex.Message;
                return entity;
            }
        }

        public async Task<CotacaoEntity?> AtualizarAsync(CotacaoEntity entity)
        {
            return null;
        }

        public async Task<bool> EfetivarAsync(Guid id)
        {
            var cotacao = await _context.CotacaoEntity.FirstOrDefaultAsync(c => c.Id == id);
            if (cotacao == null) return false;

            cotacao.Efetivada = true;
            cotacao.DataHoraEfetivacao = DateTime.Now;
            _context.Entry(cotacao).State = EntityState.Modified;
            var retorno = await _context.SaveChangesAsync();
            return retorno > 0;
        }

        public async Task<bool> DesfazerEfetivacaoAsync(Guid id)
        {
            var cotacao = await _context.CotacaoEntity.FirstOrDefaultAsync(c => c.Id == id);
            if (cotacao == null) return false;

            cotacao.Efetivada = false;
            cotacao.DataHoraEfetivacao = DateTime.MinValue;
            _context.Entry(cotacao).State = EntityState.Modified;
            var retorno = await _context.SaveChangesAsync();
            return retorno > 0;
        }

        public async Task<CotacaoEntity> ObterPorIdAsync(Guid id)
        {
            if (id == Guid.Empty)
                return new CotacaoEntity { Sucesso = false, Mensagem = "Id não foi informado." };

            var retorno = await _context.CotacaoEntity.Where(c => c.Id == id)
                .Select(c => new
                {
                    Cotacao = c,
                    InformacaoSeguro = _context.CotacaoInformacaoSeguroEntity.Where(i => i.CotacaoId == id).FirstOrDefault(),
                    InformacaoSegurado = _context.CotacaoInformacaoSeguradoEntity.Where(ii => ii.CotacaoId == id).FirstOrDefault(),
                    ListaBens = _context.CotacaoInformacaoBemEntity.Where(b => b.CotacaoId == id).ToList(),
                    ListaCotacaoFormularioRisco = _context.CotacaoFormularioRiscoEntity.Where(r => r.CotacaoId == id).ToList(),
                    ListaCotacaoCobertura = _context.CotacaoCoberturaEntity.Where(co => co.CotacaoId == id).ToList(),
                    CotacaoCondicaoComercial = _context.CotacaoCondicaoComercialEntity.Where(com => com.CotacaoId == id).FirstOrDefault(),
                    CotacaoInformacoesBeneficiario = _context.CotacaoInformacoesBeneficiarioEntity.Where(com => com.CotacaoId == id).FirstOrDefault(),
                    ListaCotacaoRetornoSeguradora = _context.CotacaoRetornoSeguradoraEntity.Where(ret => ret.CotacaoId == c.Id).ToList()
                }).FirstOrDefaultAsync();

            if (retorno == null)
                return new CotacaoEntity { Sucesso = false, Mensagem = "Nenhuma cotação localizada para esse Id." };

            retorno.Cotacao.Sucesso = true;
            retorno.Cotacao.InformacaoSeguro = retorno.InformacaoSeguro;
            retorno.Cotacao.InformacaoSegurado = retorno.InformacaoSegurado;
            retorno.Cotacao.ListaBens = retorno.ListaBens;
            retorno.Cotacao.ListaCotacaoFormularioRisco = retorno.ListaCotacaoFormularioRisco;
            retorno.Cotacao.ListaCotacaoCobertura = retorno.ListaCotacaoCobertura;
            retorno.Cotacao.CotacaoCondicaoComercial = retorno.CotacaoCondicaoComercial;
            retorno.Cotacao.CotacaoInformacoesBeneficiario = retorno.CotacaoInformacoesBeneficiario;
            retorno.Cotacao.ListaCotacaoRetornoSeguradora = retorno.ListaCotacaoRetornoSeguradora;

            return retorno.Cotacao;
        }

        public async Task<CotacaoEntity> ObterPorNumeroCotacaoAsync(string numeroCotacao)
        {
            if (string.IsNullOrEmpty(numeroCotacao))
                return new CotacaoEntity { Sucesso = false, Mensagem = "Número da cotação não foi informado." };

            var cotacaoRetornoSeguradora = await _context.CotacaoRetornoSeguradoraEntity
                .FirstOrDefaultAsync(c => c.NumeroCotacao == numeroCotacao);

            if (cotacaoRetornoSeguradora == null)
                return new CotacaoEntity { Sucesso = false, Mensagem = $"Nenhuma cotação localizada para o número {numeroCotacao}." };

            var id = cotacaoRetornoSeguradora.CotacaoId;
            var cotacao = await _context.CotacaoEntity.FirstOrDefaultAsync(c => c.Id == id);
            if (cotacao == null)
                return new CotacaoEntity { Sucesso = false, Mensagem = "Nenhuma cotação localizada para esse Id." };

            cotacao.Sucesso = true;
            cotacao.ListaCotacaoRetornoSeguradora = new List<CotacaoRetornoSeguradoraEntity> { cotacaoRetornoSeguradora };
            return cotacao;
        }

        public async Task<IEnumerable<CotacaoEntity>> ObterTodosAsync(
            string corretor = null, string seguradora = null,
            DateTime? dataInicio = null, DateTime? dataFim = null,
            bool? efetivada = null, int page = 1, int pageSize = 20)
        {
            var query = _context.CotacaoEntity.AsQueryable();

            if (!string.IsNullOrEmpty(corretor))
                query = query.Where(c => c.Corretor.Contains(corretor));

            if (!string.IsNullOrEmpty(seguradora))
                query = query.Where(c => c.Seguradora == seguradora);

            if (dataInicio.HasValue)
                query = query.Where(c => c.DataHoraCotacao >= dataInicio.Value);

            if (dataFim.HasValue)
                query = query.Where(c => c.DataHoraCotacao <= dataFim.Value);

            if (efetivada.HasValue)
                query = query.Where(c => c.Efetivada == efetivada.Value);

            return await query
                .OrderByDescending(c => c.DataHoraCotacao)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<IEnumerable<CotacaoRetornoSeguradoraEntity>> ObterRetornosPorCotacaoIdAsync(Guid cotacaoId)
        {
            return await _context.CotacaoRetornoSeguradoraEntity
                .Where(c => c.CotacaoId == cotacaoId)
                .ToListAsync();
        }

        public async Task<byte[]?> GerarRelatorioPdfAsync(Guid id)
        {
            return null;
        }

        public async Task<int> ObterTotalItensAsync()
        {
            return await _context.CotacaoEntity.CountAsync();
        }
    }
}
