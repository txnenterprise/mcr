using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace MCR.API.Propostas.Domain.DTO;

public class PropostasStatusSinistroDeferidoPagoDTO
{
    [Required(ErrorMessage = "Ação é obrigatória")]
    [DisplayName("Ação")]
    [StringLength(50, ErrorMessage = "Ação deve ter no máximo 50 caracteres")]
    public string Acao { get; set; }

    [DisplayName("Data do Pagamento")]
    [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
    public DateTime? DataPagamento { get; set; }

    [DisplayName("Observações")]
    [StringLength(1000, ErrorMessage = "Observações deve ter no máximo 1000 caracteres")]
    public string? Observacoes { get; set; }
}
