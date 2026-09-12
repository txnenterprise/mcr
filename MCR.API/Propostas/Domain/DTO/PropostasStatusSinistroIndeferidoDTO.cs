using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace MCR.API.Propostas.Domain.DTO;

public class PropostasStatusSinistroIndeferidoDTO
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

    [DisplayName("Data do Indeferimento")]
    [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
    public DateTime? DataIndeferimento { get; set; }

    [DisplayName("Observações")]
    [StringLength(1000, ErrorMessage = "Observações deve ter no máximo 1000 caracteres")]
    public string? Observacoes { get; set; }
}
