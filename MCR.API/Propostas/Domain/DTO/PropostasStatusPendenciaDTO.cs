using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace MCR.API.Propostas.Domain.DTO;

public class PropostasStatusPendenciaDTO
{
    [DisplayName("Arquivo Upload")]
    public string UploadArquivo { get; set; }

    [DisplayName("Arquivo Upload")]
    public string UploadArquivoNome { get; set; }

    [Required(ErrorMessage = "Retorno da pendência é obrigatório")]
    [DisplayName("Retorno da Pendência")]
    [StringLength(1000, ErrorMessage = "Retorno da pendência deve ter no máximo 1000 caracteres")]
    public string RetornoPendencia { get; set; }

    [Required(ErrorMessage = "Tipo de documento é obrigatório")]
    [DisplayName("Tipo de Documento")]
    [StringLength(100, ErrorMessage = "Tipo de documento deve ter no máximo 100 caracteres")]
    public string TipoDocumento { get; set; }
}
