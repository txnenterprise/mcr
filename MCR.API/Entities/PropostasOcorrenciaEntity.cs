using System.ComponentModel.DataAnnotations;


namespace MCR.API.Entities;

public class PropostasOcorrenciaEntity
{
    [Key]
    public Guid Id { get; set; }

    public Guid PropostaId { get; set; }
    public Guid UsuarioId { get; set; }
    public DateTime DataCriacao { get; set; }
    public string Descricao { get; set; } = string.Empty;
    public string? CaminhoAnexo { get; set; }
    public string? NomeArquivo { get; set; }

    public virtual PropostasEntity? Proposta { get; set; }
    public virtual UsuarioEntity? Usuario { get; set; }
} 