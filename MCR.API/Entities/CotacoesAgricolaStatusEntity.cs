using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MCR.API.Entities;

public class CotacoesAgricolaStatusEntity
{
    [Key]
    public Guid Id { get; set; }

    public Guid CotacaoAgricolaId { get; set; }
    public string Status { get; set; }
    public string? StatusAnterior { get; set; }
    public DateTime DataStatus { get; set; }
    public Guid UsuarioId { get; set; }
    public string? Observacao { get; set; }

    // Standard Properties
    public bool Ativo { get; set; }
    public bool Excluido { get; set; }

    [NotMapped]
    public bool Sucesso { get; set; }
    [NotMapped]
    public string Mensagem { get; set; }
    [NotMapped]
    public int TotalItems { get; set; }
    [NotMapped]
    public int TotalPages { get; set; }

    public virtual CotacoesAgricolaEntity? CotacoesAgricola { get; set; }
}