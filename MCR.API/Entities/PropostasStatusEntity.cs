using System.ComponentModel.DataAnnotations;


namespace MCR.API.Entities;

public class PropostasStatusEntity
{
    [Key]
    public Guid Id { get; set; }

    public Guid PropostaId { get; set; }
    public Guid? UsuarioId { get; set; }
    public string? Status { get; set; }
    public DateTime DataStatus { get; set; }
    public string? StatusAnterior { get; set; }

    public string? UsuarioPerfil { get; set; }
    public virtual PropostasEntity? Proposta { get; set; }
    public virtual UsuarioEntity? Usuario { get; set; }
}
