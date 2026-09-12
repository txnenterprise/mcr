using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace MCR.API.Propostas.Domain.DTO;

public class PropostasStatusEndossoSolicitacaoDTO
{
    [Required(ErrorMessage = "Ação é obrigatória")]
    [DisplayName("Ação")]
    [StringLength(50, ErrorMessage = "Ação deve ter no máximo 50 caracteres")]
    public string Acao { get; set; }

    [DisplayName("Upload Arquivo")]
    public string UploadArquivo { get; set; }

    [DisplayName("Upload Arquivo")]
    public string UploadArquivoNome { get; set; }

    [DisplayName("Descrição da Alteração")]
    [StringLength(1000, ErrorMessage = "Descrição da alteração deve ter no máximo 1000 caracteres")]
    public string? DescricaoAlteracao { get; set; }

    [DisplayName("Retorno da Pendência")]
    [StringLength(1000, ErrorMessage = "Retorno da pendência deve ter no máximo 1000 caracteres")]
    public string? RetornoPendencia { get; set; }
}
