using MCR.API.Entities;
using MCR.API.DTOs;

namespace MCR.API.Services.Interfaces
{
    public interface IPropostasService
    {
        Task<PropostasEntity> ObterPorIdAsync(Guid id);
        Task<IEnumerable<PropostasEntity>> ObterTodosPaginadoAsync(ObterPropostasPaginadoRequestDTO request);
        Task<PropostasEntity> CadastrarAsync(PropostasEntity proposta);
        Task<(bool Sucesso, string Mensagem, Guid PropostaId)> GerarPropostaAsync(Guid cotacaoAgricolaId, Guid? usuarioId = null);
        Task<bool> EnviarTransmissaoAsync(Guid propostaId);
        Task<PropostaValidacaoDTO> ObterStatusValidacaoAsync(Guid propostaId);
        Task<byte[]> BaixarKmlTalhaoAsync(Guid talhaoId);
        Task<string> ObterStatusProposta(Guid propostaId);
        Task<(bool Sucesso, string Mensagem, MCR.API.Propostas.Domain.DTO.PropostaDetalhesDTO? Proposta)> ObterPropostaDetalhes(Guid propostaId);
    }

    public class ObterPropostasPaginadoRequestDTO
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 20;
        public Guid? PontoAtendimentoId { get; set; }
        public Guid? CanalId { get; set; }
        public Guid? CorretoraId { get; set; }
        public string Status { get; set; }
        public DateTime? DataInicio { get; set; }
        public DateTime? DataFim { get; set; }
        public string Codigo { get; set; }
        public List<string> StatusFilter { get; set; }
    }

    public class PropostaValidacaoDTO
    {
        public bool Valida { get; set; }
        public List<string> Erros { get; set; }
        public string Status { get; set; }
        public bool SeguradosPreenchido { get; set; }
        public bool RiscosPreenchido { get; set; }
        public bool VistoriaPreenchido { get; set; }
        public bool BeneficiariosPreenchido { get; set; }
        public bool QuestionariosPreenchido { get; set; }
        public bool ObservacoesPreenchido { get; set; }
        public bool FormasPagamentoPreenchido { get; set; }
        public bool DocumentosPreenchido { get; set; }
    }
}
