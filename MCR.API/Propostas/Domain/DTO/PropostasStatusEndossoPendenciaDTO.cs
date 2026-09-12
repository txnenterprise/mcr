using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace MCR.API.Propostas.Domain.DTO;

public class PropostasStatusEndossoPendenciaDTO
{
    [Required(ErrorMessage = "Ação é obrigatória")]
    [DisplayName("Ação")]
    [StringLength(50, ErrorMessage = "Ação deve ter no máximo 50 caracteres")]
    public string Acao { get; set; }

    [DisplayName("Upload Arquivo")]
    public string UploadArquivo { get; set; }

    [DisplayName("Upload Arquivo")]
    public string UploadArquivoNome { get; set; }

    [Required(ErrorMessage = "Descrição da pendência é obrigatória")]
    [DisplayName("Descrição da Pendência")]
    [StringLength(1000, ErrorMessage = "Descrição da pendência deve ter no máximo 1000 caracteres")]
    public string DescricaoPendencia { get; set; }

    [Required(ErrorMessage = "Tipo de documento é obrigatório")]
    [DisplayName("Tipo de Documento")]
    [StringLength(100, ErrorMessage = "Tipo de documento deve ter no máximo 100 caracteres")]
    public string TipoDocumento { get; set; }
}
