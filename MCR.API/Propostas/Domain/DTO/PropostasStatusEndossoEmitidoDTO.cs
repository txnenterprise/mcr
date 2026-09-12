using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace MCR.API.Propostas.Domain.DTO;

public class PropostasStatusEndossoEmitidoDTO
{
    [Required(ErrorMessage = "Ação é obrigatória")]
    [DisplayName("Ação")]
    [StringLength(50, ErrorMessage = "Ação deve ter no máximo 50 caracteres")]
    public string Acao { get; set; }

    [Required(ErrorMessage = "Número do endosso é obrigatório")]
    [DisplayName("Número do Endosso")]
    [StringLength(50, ErrorMessage = "Número do endosso deve ter no máximo 50 caracteres")]
    public string NumeroEndosso { get; set; }

    [DisplayName("Upload Arquivo")]
    public string UploadArquivo { get; set; }

    [DisplayName("Upload Arquivo")]
    public string UploadArquivoNome { get; set; }

    [DisplayName("Tipo de Documento")]
    [StringLength(100, ErrorMessage = "Tipo de documento deve ter no máximo 100 caracteres")]
    public string? TipoDocumento { get; set; }

    [DisplayName("LMI Total")]
    [StringLength(50, ErrorMessage = "LMI Total deve ter no máximo 50 caracteres")]
    public string? LMITotal { get; set; }

    [DisplayName("Prêmio Total")]
    [StringLength(50, ErrorMessage = "Prêmio Total deve ter no máximo 50 caracteres")]
    public string? PremioTotal { get; set; }

    [DisplayName("Sub Federal")]
    [StringLength(50, ErrorMessage = "Sub Federal deve ter no máximo 50 caracteres")]
    public string? SubFederal { get; set; }

    [DisplayName("Sub Estadual")]
    [StringLength(50, ErrorMessage = "Sub Estadual deve ter no máximo 50 caracteres")]
    public string? SubEstadual { get; set; }

    [DisplayName("Parcela do Segurado")]
    [DisplayFormat(DataFormatString = "{0:N2}", ApplyFormatInEditMode = true)]
    public decimal? ParcSegurado { get; set; }

    [DisplayName("Área Total")]
    public decimal? AreaTotal { get; set; }

    [DisplayName("Início de Vigência")]
    [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
    public DateTime? InicioVigencia { get; set; }

    [DisplayName("Fim de Vigência")]
    [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
    public DateTime? FimVigencia { get; set; }

    [DisplayName("Observações")]
    [StringLength(1000, ErrorMessage = "Observações deve ter no máximo 1000 caracteres")]
    public string? Observacoes { get; set; }
}
