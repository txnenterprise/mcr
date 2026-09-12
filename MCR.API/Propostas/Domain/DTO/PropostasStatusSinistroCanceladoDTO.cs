using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace MCR.API.Propostas.Domain.DTO;

public class PropostasStatusSinistroCanceladoDTO
{
    [Required(ErrorMessage = "Ação é obrigatória")]
    [DisplayName("Ação")]
    [StringLength(50, ErrorMessage = "Ação deve ter no máximo 50 caracteres")]
    public string Acao { get; set; }

    [Required(ErrorMessage = "Motivo do cancelamento é obrigatório")]
    [DisplayName("Motivo do Cancelamento")]
    [StringLength(1000, ErrorMessage = "Motivo do cancelamento deve ter no máximo 1000 caracteres")]
    public string MotivoCancelamento { get; set; }
}
