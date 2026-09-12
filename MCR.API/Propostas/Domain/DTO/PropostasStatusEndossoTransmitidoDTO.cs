using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace MCR.API.Propostas.Domain.DTO;

public class PropostasStatusEndossoTransmitidoDTO
{
    [Required(ErrorMessage = "Ação é obrigatória")]
    [DisplayName("Ação")]
    [StringLength(50, ErrorMessage = "Ação deve ter no máximo 50 caracteres")]
    public string Acao { get; set; }

    [DisplayName("Número do Endosso")]
    [StringLength(50, ErrorMessage = "Número do endosso deve ter no máximo 50 caracteres")]
    public string? NumeroEndosso { get; set; }

    [DisplayName("Upload Arquivo")]
    public string UploadArquivo { get; set; }

    [DisplayName("Upload Arquivo")]
    public string UploadArquivoNome { get; set; }

    [DisplayName("Tipo de Documento")]
    [StringLength(100, ErrorMessage = "Tipo de documento deve ter no máximo 100 caracteres")]
    public string? TipoDocumento { get; set; }

    [DisplayName("Observações")]
    [StringLength(1000, ErrorMessage = "Observações deve ter no máximo 1000 caracteres")]
    public string? Observacoes { get; set; }
}
