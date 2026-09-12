using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace MCR.API.Propostas.Domain.DTO;

public class PropostasStatusApoliceEmitidaDTO
{
    [DisplayName("Apólice Emitida")]
    public string ApoliceEmitida { get; set; }

    [DisplayName("Apólice Emitida")]
    public string ApoliceEmitidaNome { get; set; }

    [Required(ErrorMessage = "Número da apólice é obrigatório")]
    [DisplayName("Número da Apólice")]
    [StringLength(50, ErrorMessage = "Número da apólice deve ter no máximo 50 caracteres")]
    public string NumeroApolice { get; set; }

    [Required(ErrorMessage = "Início de vigência é obrigatório")]
    [DisplayName("Início de Vigência")]
    [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
    public DateTime InicioVigencia { get; set; }

    [Required(ErrorMessage = "Fim de vigência é obrigatório")]
    [DisplayName("Fim de Vigência")]
    [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
    public DateTime FinalVigencia { get; set; }

    [DisplayName("Boleto")]
    public string? Boleto { get; set; }

    [DisplayName("Boleto")]
    public string? BoletoNome { get; set; }

    [Required(ErrorMessage = "Data de vencimento é obrigatória")]
    [DisplayName("Data de Vencimento")]
    [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
    public DateTime? DataVencimento { get; set; }
}
