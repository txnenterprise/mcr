using MCR.API.Propostas.Domain.DTO;

namespace MCR.API.Services.Interfaces
{
    public interface IPropostasTimelineService
    {
        Task<List<PropostasTimelineDTO>> ObterTimelineProposta(Guid propostaId);
    }
}
