using MCR.API.Entities;

namespace MCR.API.Services.Interfaces
{
    public interface ICulturaService
    {
        Task<CulturaEntity> ObterPorIdAsync(Guid id);
        Task<IEnumerable<CulturaEntity>> ObterTodosPaginadoAsync(string cultura = null, string grupoMaturacao = null, string variedade = null, bool? ativo = null, int page = 1, int pageSize = 20);
        Task<CulturaEntity> CadastrarAsync(CulturaEntity entity);
        Task<CulturaEntity> AtualizarAsync(CulturaEntity entity);
        Task<bool> InativarAsync(Guid id);
        Task<bool> AtivarAsync(Guid id);
    }

    public interface ISafraService
    {
        Task<SafraEntity> ObterPorIdAsync(Guid id);
        Task<IEnumerable<SafraEntity>> ObterTodosPaginadoAsync(string descricao = null, int? anoReferencia = null, bool? ativo = null, int page = 1, int pageSize = 20);
        Task<SafraEntity> CadastrarAsync(SafraEntity entity);
        Task<SafraEntity> AtualizarAsync(SafraEntity entity);
        Task<bool> InativarAsync(Guid id);
        Task<bool> AtivarAsync(Guid id);
    }

    public interface ISeguradoraService
    {
        Task<SeguradoraEntity> ObterPorIdAsync(Guid id);
        Task<IEnumerable<SeguradoraEntity>> ObterTodosPaginadoAsync(string nome = null, bool? ativo = null, int page = 1, int pageSize = 20);
        Task<SeguradoraEntity> CadastrarAsync(SeguradoraEntity entity);
        Task<SeguradoraEntity> AtualizarAsync(SeguradoraEntity entity);
        Task<bool> InativarAsync(Guid id);
        Task<bool> AtivarAsync(Guid id);
    }

    public interface ISubvencaoFederalService
    {
        Task<SubvencaoFederalEntity> ObterPorIdAsync(Guid id);
        Task<IEnumerable<SubvencaoFederalEntity>> ObterTodosPaginadoAsync(Guid? culturaId = null, int? anoCivil = null, bool? ativo = null, int page = 1, int pageSize = 20);
        Task<SubvencaoFederalEntity> CadastrarAsync(SubvencaoFederalEntity entity);
        Task<SubvencaoFederalEntity> AtualizarAsync(SubvencaoFederalEntity entity);
        Task<bool> InativarAsync(Guid id);
        Task<bool> AtivarAsync(Guid id);
    }

    public interface ISubvencaoEstadualService
    {
        Task<SubvencaoEstadualEntity> ObterPorIdAsync(Guid id);
        Task<IEnumerable<SubvencaoEstadualEntity>> ObterTodosPaginadoAsync(Guid? culturaId = null, string estado = null, int? anoCivil = null, bool? ativo = null, int page = 1, int pageSize = 20);
        Task<SubvencaoEstadualEntity> CadastrarAsync(SubvencaoEstadualEntity entity);
        Task<SubvencaoEstadualEntity> AtualizarAsync(SubvencaoEstadualEntity entity);
        Task<bool> InativarAsync(Guid id);
        Task<bool> AtivarAsync(Guid id);
    }

    public interface ICorretoraService
    {
        Task<CorretoraEntity> ObterPorIdAsync(Guid id);
        Task<IEnumerable<CorretoraEntity>> ObterTodosAsync();
        Task<CorretoraEntity> CadastrarAsync(CorretoraEntity entity);
        Task<CorretoraEntity> AtualizarAsync(CorretoraEntity entity);
    }

    public interface ICanalService
    {
        Task<CanalEntity> ObterPorIdAsync(Guid id);
        Task<IEnumerable<CanalEntity>> ObterTodosPaginadoAsync(string nome = null, Guid? corretoraId = null, int page = 1, int pageSize = 20);
        Task<IEnumerable<CanalEntity>> ObterPorCorretoraAsync(Guid corretoraId);
        Task<CanalEntity> CadastrarAsync(CanalEntity entity);
        Task<CanalEntity> AtualizarAsync(CanalEntity entity);
    }

    public interface IPontoAtendimentoService
    {
        Task<PontoAtendimentoEntity> ObterPorIdAsync(Guid id);
        Task<IEnumerable<PontoAtendimentoEntity>> ObterTodosPaginadoAsync(string nome = null, Guid? canalId = null, int page = 1, int pageSize = 20);
        Task<IEnumerable<PontoAtendimentoEntity>> ObterPorCanalAsync(Guid canalId);
        Task<PontoAtendimentoEntity> CadastrarAsync(PontoAtendimentoEntity entity);
        Task<PontoAtendimentoEntity> AtualizarAsync(PontoAtendimentoEntity entity);
    }

    public interface IBeneficiarioService
    {
        Task<BeneficiarioEntity> ObterPorIdAsync(Guid id);
        Task<IEnumerable<BeneficiarioEntity>> ObterTodosPaginadoAsync(string nome = null, string cnpj = null, bool? ativo = null, int page = 1, int pageSize = 20);
        Task<BeneficiarioEntity> CadastrarAsync(BeneficiarioEntity entity);
        Task<BeneficiarioEntity> AtualizarAsync(BeneficiarioEntity entity);
    }

    public interface IUsuarioService
    {
        Task<UsuarioEntity> ObterPorIdAsync(Guid id);
        Task<IEnumerable<UsuarioEntity>> ObterTodosPaginadoAsync(string nome = null, string email = null, int page = 1, int pageSize = 20);
        Task<UsuarioEntity> CadastrarAsync(UsuarioEntity entity, string password, string role);
        Task<UsuarioEntity> AtualizarAsync(UsuarioEntity entity);
        Task<bool> BloquearAsync(string userId);
        Task<bool> DesbloquearAsync(string userId);
        Task<bool> ChangePasswordAsync(string userId, string currentPassword, string newPassword);
        Task<(bool Sucesso, string Mensagem)> AlterarUsuarioAsync(MCR.API.Usuario.Domain.DTO.UsuarioAlterarDTO model, Guid usuarioCriadorId);
    }
}
