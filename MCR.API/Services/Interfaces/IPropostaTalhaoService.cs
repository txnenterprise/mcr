using MCR.API.Propostas.Domain.DTO;

namespace MCR.API.Services.Interfaces
{
    public interface IPropostaTalhaoService
    {
        Task<(bool Sucesso, string Mensagem)> VincularTalhoesProposta(PropostasTalhoesCadastrarDTO model);
    }
}
