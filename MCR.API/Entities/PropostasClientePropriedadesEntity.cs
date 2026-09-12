using System.ComponentModel.DataAnnotations;


namespace MCR.API.Entities;

public class PropostasClientePropriedadesEntity
{
    [Key]
    public Guid Id { get; set; }
    public Guid PropostaId { get; set; }
    public Guid ClienteId { get; set; }
    public Guid PropriedadeId { get; set; }
    public string? Nome { get; set; }
    public string? Endereco { get; set; }
    public string? Bairro { get; set; }
    public string? Numero { get; set; }
    public string? Cidade { get; set; }
    public string? Estado { get; set; }
    public string? CEP { get; set; }
    public decimal SomaAreaTotalTalhao { get; set; }
    public string? ImagemGeralTodosTalhoes { get; set; }
    public string? MatriculaLote { get; set; }
    public string? CadastroAmbientalRural { get; set; }
    public virtual PropostasEntity? Proposta { get; set; }
    public virtual ClienteEntity? Cliente { get; set; }
    public virtual PropriedadeEntity? Propriedade { get; set; }

    public virtual ICollection<PropostasClientePropriedadesTalhoesEntity> PropostasClientePropriedadesTalhoes { get; set; } = new List<PropostasClientePropriedadesTalhoesEntity>();
}
