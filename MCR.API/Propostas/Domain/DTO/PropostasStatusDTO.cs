using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace MCR.API.Propostas.Domain.DTO;

public class PropostasStatusDTO
{
    public PropostasStatusDTO() { }

    public PropostasStatusDTO(Guid propostaId)
    {
        PropostaId = propostaId;
    }

    public Guid? Id { get; set; }
    public Guid PropostaId { get; set; }
    public Guid? UsuarioId { get; set; }

    [Required(ErrorMessage = "Status é obrigatório")]
    [DisplayName("Status")]
    [StringLength(50, ErrorMessage = "Status deve ter no máximo 50 caracteres")]
    public string Status { get; set; }

    public DateTime? DataStatus { get; set; }
    public string? NomeUsuario { get; set; }
    public string? UsuarioPerfil { get; set; }
    public string? StatusAnterior { get; set; }

    public PropostasStatusJustificativaDTO? JustificativaDTO { get; set; }
    public PropostasStatusPendenciaDTO? PendenciaDTO { get; set; }
    public PropostasStatusAceitaDTO? AceitaDTO { get; set; }
    public PropostasStatusApoliceEmitidaDTO? ApoliceEmitidaDTO { get; set; }
    public PropostasStatusEndossoSolicitacaoDTO? EndossoSolicitacaoDTO { get; set; }
    public PropostasStatusEndossoPendenciaDTO? EndossoPendenciaDTO { get; set; }
    public PropostasStatusEndossoTransmitidoDTO? EndossoTransmitidoDTO { get; set; }
    public PropostasStatusEndossoEmitidoDTO? EndossoEmitidoDTO { get; set; }
    public PropostasStatusSinistroComunicarDTO? SinistroComunicarDTO { get; set; }
    public PropostasStatusSinistroPendenciaDTO? SinistroPendenciaDTO { get; set; }
    public PropostasStatusSinistroCanceladoDTO? SinistroCanceladoDTO { get; set; }
    public PropostasStatusSinistroAbertoRegulacaoDTO? SinistroAbertoRegulacaoDTO { get; set; }
    public PropostasStatusSinistroAguardPagamentoDTO? SinistroAguardPagamentoDTO { get; set; }
    public PropostasStatusSinistroDeferidoPagoDTO? SinistroDeferidoPagoDTO { get; set; }
    public PropostasStatusSinistroIndeferidoDTO? SinistroIndeferidoDTO { get; set; }
}
