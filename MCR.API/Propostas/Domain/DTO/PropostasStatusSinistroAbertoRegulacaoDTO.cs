using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace MCR.API.Propostas.Domain.DTO;

public class PropostasStatusSinistroAbertoRegulacaoDTO
{
    [Required(ErrorMessage = "Ação é obrigatória")]
    [DisplayName("Ação")]
    [StringLength(50, ErrorMessage = "Ação deve ter no máximo 50 caracteres")]
    public string Acao { get; set; }

    [DisplayName("Upload Arquivo")]
    public string UploadArquivo { get; set; }

    [DisplayName("Upload Arquivo")]
    public string UploadArquivoNome { get; set; }

    [DisplayName("Tipo de Documento")]
    [StringLength(100, ErrorMessage = "Tipo de documento deve ter no máximo 100 caracteres")]
    public string? TipoDocumento { get; set; }

    [DisplayName("Protocolo de Aviso de Sinistro")]
    [StringLength(100, ErrorMessage = "Protocolo de aviso de sinistro deve ter no máximo 100 caracteres")]
    public string? ProtocoloAvisoSinistro { get; set; }

    [DisplayName("Data de Aviso do Sinistro")]
    [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
    public DateTime? DataAvisoSinistro { get; set; }

    [DisplayName("Empresa/Perito")]
    [StringLength(200, ErrorMessage = "Empresa/Perito deve ter no máximo 200 caracteres")]
    public string? EmpresaPerito { get; set; }

    [DisplayName("Telefone")]
    [StringLength(20, ErrorMessage = "Telefone deve ter no máximo 20 caracteres")]
    public string? Telefone { get; set; }

    [DisplayName("Observações")]
    [StringLength(1000, ErrorMessage = "Observações deve ter no máximo 1000 caracteres")]
    public string? Observacoes { get; set; }
}
