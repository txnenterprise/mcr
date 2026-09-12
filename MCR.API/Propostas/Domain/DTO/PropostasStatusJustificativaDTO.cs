using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace MCR.API.Propostas.Domain.DTO;

public class PropostasStatusJustificativaDTO
{
    [Required(ErrorMessage = "Justificativa é obrigatória")]
    [DisplayName("Justificativa")]
    [StringLength(1000, ErrorMessage = "Justificativa deve ter no máximo 1000 caracteres")]
    public string Justificativa { get; set; }
}
