using MCR.API.Propostas.Domain.DTO;

namespace MCR.API.Services.Interfaces
{
    public interface IPropostasVistoriaService
    {
        Task<PropostasVistoriaDTO> ObterPessoasPropostasVistoria(Guid propostaId, Guid clienteId);
    }
}
