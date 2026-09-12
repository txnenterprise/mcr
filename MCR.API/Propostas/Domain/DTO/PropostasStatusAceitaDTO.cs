using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace MCR.API.Propostas.Domain.DTO;

public class PropostasStatusAceitaDTO
{
    [DisplayName("Proposta Seguradora")]
    public string PropostaSeguradora { get; set; }

    [DisplayName("Proposta Seguradora")]
    public string PropostaSeguradoraNome { get; set; }

    [Required(ErrorMessage = "Número da proposta é obrigatório")]
    [DisplayName("Número da Proposta")]
    [StringLength(50, ErrorMessage = "Número da proposta deve ter no máximo 50 caracteres")]
    public string NumeroProposta { get; set; }

    [DisplayName("LMI Total")]
    [StringLength(50, ErrorMessage = "LMI Total deve ter no máximo 50 caracteres")]
    public string LmiTotal { get; set; }

    [DisplayName("Prêmio Total")]
    [StringLength(50, ErrorMessage = "Prêmio Total deve ter no máximo 50 caracteres")]
    public string PremioTotal { get; set; }

    [DisplayName("Sub Federal")]
    [StringLength(50, ErrorMessage = "Sub Federal deve ter no máximo 50 caracteres")]
    public string SubFederal { get; set; }

    [DisplayName("Sub Estadual")]
    [StringLength(50, ErrorMessage = "Sub Estadual deve ter no máximo 50 caracteres")]
    public string SubEstadual { get; set; }

    [Required(ErrorMessage = "Parcela do segurado é obrigatória")]
    [DisplayName("Parcela do Segurado")]
    [DisplayFormat(DataFormatString = "{0:N2}", ApplyFormatInEditMode = true)]
    public decimal ParcelaSegurado { get; set; }

    [DisplayName("Boleto")]
    public string? Boleto { get; set; }

    [DisplayName("Boleto")]
    public string? BoletoNome { get; set; }

    [Required(ErrorMessage = "Data de vencimento é obrigatória")]
    [DisplayName("Data de Vencimento")]
    [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
    public DateTime? DataVencimento { get; set; }
}
