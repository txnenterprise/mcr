using System.ComponentModel.DataAnnotations;


namespace MCR.API.Entities;

public class PropostasDocumentosEntity
{
    [Key]
    public Guid Id { get; set; }
    public Guid? UserId { get; set; }
    public Guid PropostaId { get; set; }
    public string TipoDocumento { get; set; }
    public string? StatusProposta { get; set; }
    public DateTime DataUpload { get; set; }
    public string UrlDocumento { get; set; }
    public string NomeArquivo { get; set; }

    public virtual PropostasEntity? Proposta { get; set; }

    public virtual UsuarioEntity? Usuario { get; set; }
}
