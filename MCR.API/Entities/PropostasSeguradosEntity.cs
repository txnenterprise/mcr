using System.ComponentModel.DataAnnotations;


namespace MCR.API.Entities;

public class PropostasSeguradosEntity
{
    [Key]
    public Guid Id { get; set; }

    public Guid PropostaId { get; set; }
    public Guid ClienteId { get; set; }
    public Guid? VinculoFamiliarId { get; set; }

    public string Nome { get; set; } = string.Empty;
    public string CPF { get; set; } = string.Empty;
    public string? Telefone { get; set; }

    public string? Email { get; set; }
    public string? RelacaoParental { get; set; }

    public virtual PropostasEntity? Proposta { get; set; }

    public virtual ClienteEntity? Cliente { get; set; }
    public virtual VinculoFamiliarEntity? VinculoFamiliar { get; set; }

    public virtual ICollection<PropostasVistoriaEntity> PropostasVistoria { get; set; } = new List<PropostasVistoriaEntity>();
}
