using System.ComponentModel.DataAnnotations;


namespace MCR.API.Entities;

public class PropostasObservacoesEntity
{
    [Key]
    public Guid Id { get; set; }

    public Guid PropostaId { get; set; }
    public Guid UsuarioId { get; set; }
    public DateTime DataCriacao { get; set; }
    public string Observacao { get; set; } = string.Empty;

    public virtual PropostasEntity? Proposta { get; set; }
    public virtual UsuarioEntity? Usuario { get; set; }
}

