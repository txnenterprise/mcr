using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace MCR.API.Propostas.Domain.DTO;

public class PropostasStatusSinistroComunicarDTO
{
    [Required(ErrorMessage = "Ação é obrigatória")]
    [DisplayName("Ação")]
    [StringLength(50, ErrorMessage = "Ação deve ter no máximo 50 caracteres")]
    public string Acao { get; set; }

    [Required(ErrorMessage = "Cobertura é obrigatória")]
    [DisplayName("Cobertura")]
    [StringLength(50, ErrorMessage = "Cobertura deve ter no máximo 50 caracteres")]
    public string Cobertura { get; set; }

    [Required(ErrorMessage = "Evento é obrigatório")]
    [DisplayName("Evento")]
    [StringLength(100, ErrorMessage = "Evento deve ter no máximo 100 caracteres")]
    public string Evento { get; set; }

    [DisplayName("Severidade do Dano")]
    [StringLength(100, ErrorMessage = "Severidade do dano deve ter no máximo 100 caracteres")]
    public string? SeveridadeDano { get; set; }

    [DisplayName("Dano % Estimado")]
    [Range(0, 100, ErrorMessage = "Dano estimado deve estar entre 0 e 100")]
    public decimal? DanoEstimado { get; set; }

    [DisplayName("Área Total Afetada")]
    [Range(0, double.MaxValue, ErrorMessage = "Área total afetada deve ser maior ou igual a zero")]
    public decimal? AreaTotalAfetada { get; set; }

    [DisplayName("Data Início")]
    [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
    public DateTime? DataInicio { get; set; }

    [DisplayName("Data Final")]
    [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
    public DateTime? DataFinal { get; set; }

    [DisplayName("Tipo do Responsável pela Vistoria")]
    [StringLength(50, ErrorMessage = "Tipo do responsável pela vistoria deve ter no máximo 50 caracteres")]
    public string? TipoRespVistoria { get; set; }

    [DisplayName("Nome do Responsável pela Vistoria")]
    [StringLength(200, ErrorMessage = "Nome do responsável pela vistoria deve ter no máximo 200 caracteres")]
    public string? NomeRespVistoria { get; set; }

    [DisplayName("CPF do Responsável pela Vistoria")]
    [StringLength(14, ErrorMessage = "CPF deve ter no máximo 14 caracteres")]
    public string? CPFRespVistoria { get; set; }

    [DisplayName("Observações")]
    [StringLength(1000, ErrorMessage = "Observações deve ter no máximo 1000 caracteres")]
    public string? Observacao { get; set; }

    public string? ResponsavelSelecionado { get; set; }
}
