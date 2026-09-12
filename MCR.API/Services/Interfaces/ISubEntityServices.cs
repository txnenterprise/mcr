using MCR.API.Entities;
using MCR.API.Propostas.Domain.DTO;

namespace MCR.API.Services.Interfaces
{
    public interface IPropostasBeneficiariosService
    {
        Task<IEnumerable<PropostasBeneficiariosEntity>> ObterPorPropostaAsync(Guid propostaId);
        Task<PropostasBeneficiariosDTO> ObterBeneficiariosPorPropostaAsync(Guid propostaId);
        Task<(bool Sucesso, string Mensagem)> SalvarBeneficiariosAsync(PropostasBeneficiariosDTO request);
        Task<bool> RemoverBeneficiarioAsync(Guid id);
    }

    public interface IPropostasDocumentosService
    {
        Task<IEnumerable<PropostasDocumentosEntity>> ListarPorPropostaAsync(Guid propostaId);
        Task<PropostasDocumentosEntity> UploadDocumentoAsync(Guid propostaId, string nome, string contentType, byte[] conteudo, Guid usuarioId, string? statusProposta = null);
        Task<bool> ExcluirDocumentoAsync(Guid id);
        Task<(byte[] Conteudo, string Nome, string ContentType)> DownloadDocumentoAsync(Guid id);
    }

    public interface IPropostasFormaPagamentosService
    {
        Task<IEnumerable<PropostasFormaPagamentosEntity>> ObterPorPropostaIdAsync(Guid propostaId);
    }

    public interface IPropostasObservacoesService
    {
        Task<IEnumerable<PropostasObservacoesEntity>> ObterPorPropostaAsync(Guid propostaId);
        Task<bool> SalvarObservacaoAsync(Guid propostaId, string observacao, Guid usuarioId);
    }

    public interface IPropostasOcorrenciaService
    {
        Task<IEnumerable<PropostasOcorrenciaEntity>> ObterPorPropostaAsync(Guid propostaId);
        Task<bool> SalvarOcorrenciaAsync(Guid propostaId, string descricao, Guid usuarioId, byte[] anexo = null, string nomeAnexo = null);
        Task<(byte[] Conteudo, string Nome)> DownloadAnexoAsync(Guid ocorrenciaId);
    }

    public interface IPropostasQuestionarioService
    {
        Task<object> ObterQuestionarioAsync(Guid propostaId);
        Task<bool> SalvarQuestionarioAsync(Guid propostaId, object dados);
        Task<List<PropostasQuestionarioCulturasDTO>> ListarCulturasAsync();
    }

    public interface IPropostaRiscoService
    {
        Task<bool> VincularRiscosAsync(Guid propostaId, List<Guid> riscoIds);
        Task<bool> RemoverRiscoAsync(Guid id);
    }

    public interface IPropostaSeguradoService
    {
        Task<bool> VincularSeguradoAsync(Guid propostaId, Guid clienteId);
        Task<bool> RemoverSeguradoAsync(Guid id);
        Task<List<PropostasSeguradosDTO>> ObterPropostaSeguradosPorId(Guid propostaId);
    }

    public interface IPropostasStatusService
    {
        Task<IEnumerable<PropostasStatusEntity>> ObterStatusPorPropostaAsync(Guid propostaId);
        Task<bool> SalvarStatusAsync(Guid propostaId, string status, string observacao, Guid usuarioId, byte[] anexo = null);
        Task<(bool Sucesso, string Mensagem)> SalvarStatusProposta(PropostasStatusDTO model, Guid usuarioId);
        Task<(byte[] Conteudo, string Nome)> DownloadAnexoStatusAsync(Guid statusId);
        List<string> ListarStatusProposta();
        List<string> ListarStatusPropostaFiltrado(Guid propostaId);
        Task<IEnumerable<PropostasStatusDTO>> ObterPropostaStatusLista(Guid propostaId);
    }

    public interface IProdutosTaxasService
    {
        Task<ProdutosTaxasEntity> ObterPorIdAsync(Guid id);
        Task<IEnumerable<ProdutosTaxasEntity>> ObterPorProdutoIdAsync(Guid produtoId);
        Task<bool> AdicionarAsync(ProdutosTaxasEntity taxa);
        Task<bool> AtualizarAsync(ProdutosTaxasEntity taxa);
        Task<bool> ExcluirAsync(Guid id);
        Task<bool> ImportarCsvAsync(Guid produtoId, Stream arquivo);
        Task<bool> ExcluirTodasPorProdutoAsync(Guid produtoId);
    }

    public interface IProdutosCanalService
    {
        Task<bool> AdicionarAsync(Guid produtoId, Guid pontoAtendimentoId);
        Task<bool> ExcluirAsync(Guid id);
    }

    public interface IPropriedadeService
    {
        Task<PropriedadeEntity> ObterPorIdAsync(Guid id);
        Task<IEnumerable<PropriedadeEntity>> ObterTodosPaginadoAsync(string nome = null, string estado = null, string cidade = null, bool? ativo = null, int page = 1, int pageSize = 20);
        Task<PropriedadeEntity> CadastrarAsync(PropriedadeEntity entity);
        Task<PropriedadeEntity> AtualizarAsync(PropriedadeEntity entity);
        Task<IList<PropriedadeEntity>> PesquisarPropriedadesDisponiveisIndex(string nome, string estado, string cidade, string ativo);
    }

    public interface ITalhaoService
    {
        Task<TalhaoEntity> ObterPorIdAsync(Guid id);
        Task<IEnumerable<TalhaoEntity>> ObterTodosPaginadoAsync(string descricao = null, Guid? propriedadeId = null, int page = 1, int pageSize = 20);
        Task<IEnumerable<TalhaoEntity>> ObterPorPropriedadeAsync(Guid propriedadeId);
        Task<TalhaoEntity> CadastrarAsync(TalhaoEntity entity);
        Task<TalhaoEntity> AtualizarAsync(TalhaoEntity entity);
    }
}
