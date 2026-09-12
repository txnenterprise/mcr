using MCR.API.Entities;
using MCR.API.Propostas.Domain.DTO;
using MCR.API.Repository;
using MCR.API.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace MCR.API.Services.Implementations
{
    public class PropostaTalhaoService : IPropostaTalhaoService
    {
        private readonly DbContextMCR _context;

        public PropostaTalhaoService(DbContextMCR context)
        {
            _context = context;
        }

        public async Task<(bool Sucesso, string Mensagem)> VincularTalhoesProposta(PropostasTalhoesCadastrarDTO model)
        {
            try
            {
                foreach (var p in model.Propriedades)
                {
                    foreach (var item in p.Talhoes.Where(x => x.Selecionado))
                    {
                        if (!item.DataPlantio.HasValue)
                            return (false, "Para cada talhão selecionado, informe a data de plantio.");
                        if (!item.GrupoVariedadeId.HasValue || item.GrupoVariedadeId.Value == Guid.Empty)
                            return (false, "Para cada talhão selecionado, selecione o grupo da variedade.");
                        if (!item.VariedadeId.HasValue || item.VariedadeId.Value == Guid.Empty)
                            return (false, "Para cada talhão selecionado, selecione a variedade.");

                        var grupoExiste = await _context.CulturaMaturacao.AnyAsync(g => g.Id == item.GrupoVariedadeId.Value);
                        if (!grupoExiste)
                            return (false, $"Grupo de variedade não encontrado (ID: {item.GrupoVariedadeId}).");

                        var variedadeExiste = await _context.CulturaMaturacaoVariedades.AnyAsync(v => v.Id == item.VariedadeId.Value);
                        if (!variedadeExiste)
                            return (false, $"Variedade não encontrada (ID: {item.VariedadeId}).");
                    }
                }

                var propriedadeIds = model.Propriedades.Select(x => x.PropriedadeId).ToList();
                var talhoes = await _context.Talhoes.Where(x => propriedadeIds.Contains(x.PropriedadeId)).ToListAsync();

                foreach (var p in model.Propriedades)
                {
                    foreach (var item in p.Talhoes.Where(x => x.Selecionado).ToList())
                    {
                        var talhao = talhoes.FirstOrDefault(x => x.Id == item.TalhaoId);
                        if (talhao != null)
                        {
                            var newItem = new PropostasClientePropriedadesTalhoesEntity
                            {
                                Id = Guid.NewGuid(),
                                TalhaoId = talhao.Id,
                                ImagemTalhao = talhao.ImagemTalhao,
                                KmlTalhao = talhao.KmlTalhao,
                                Latitude = talhao.Latitude,
                                Longitude = talhao.Longitude,
                                Nome = talhao.Nome,
                                PercentualAreia = talhao.PercentualAreia,
                                PercentualArgila = talhao.PercentualArgila,
                                PercentualSilte = talhao.PercentualSilte,
                                PossuiAnaliseFisicaSolo = talhao.PossuiAnaliseFisicaSolo,
                                RoteiroAcesso = talhao.RoteiroAcesso,
                                TipoSolo = talhao.TipoSolo,
                                ClassificacaoSolo = talhao.ClassificacaoSolo,
                                Area = talhao.Area,
                                DataPlantio = item.DataPlantio,
                                GrupoVariedadeId = item.GrupoVariedadeId,
                                VariedadeId = item.VariedadeId,
                                PropostasClientePropriedadeId = p.PropostaPropriedadeId,
                            };
                            _context.PropostasClientePropriedadesTalhoes.Add(newItem);
                        }
                    }
                }

                await _context.SaveChangesAsync();
                return (true, "Talhão(ões) vinculado(s) com sucesso à proposta");
            }
            catch (Exception ex)
            {
                return (false, $"Erro ao vincular talhão: {ex.Message}");
            }
        }
    }
}
