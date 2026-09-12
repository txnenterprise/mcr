using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;



namespace MCR.API.Entities;

public class PropostasClientePropriedadesTalhoesEntity
{
    [Key]
    public Guid Id { get; set; }
    [ForeignKey(nameof(Propriedades))]
    public Guid PropostasClientePropriedadeId { get; set; }
    public Guid TalhaoId { get; set; }
    public string? Nome { get; set; }
    public decimal? Area { get; set; }
    public string? Latitude { get; set; }
    public string? Longitude { get; set; }
    public string? RoteiroAcesso { get; set; }
    public bool PossuiAnaliseFisicaSolo { get; set; }
    public decimal? PercentualAreia { get; set; }
    public decimal? PercentualSilte { get; set; }
    public decimal? PercentualArgila { get; set; }
    public string? TipoSolo { get; set; }
    public string? ClassificacaoSolo { get; set; }
    public string? ImagemTalhao { get; set; }
    public string? KmlTalhao { get; set; }

    public DateTime? DataPlantio { get; set; }
    public Guid? GrupoVariedadeId { get; set; }
    public Guid? VariedadeId { get; set; }
    public virtual PropostasClientePropriedadesEntity? Propriedades { get; set; }

    public virtual CulturaMaturacaoEntity? GrupoVariedade { get; set; }
    public virtual CulturaMaturacaoVariedadeEntity? Variedade { get; set; }

    public virtual TalhaoEntity? Talhao { get; set; }
}