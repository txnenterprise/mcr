using MCR.API.Entities;
using MCR.API.Models;

namespace MCR.API.Services.Interfaces
{
    public interface IClienteService
    {
        Task<ClienteEntity> ObterPorIdAsync(Guid id);
        Task<IEnumerable<ClienteEntity>> ObterTodosPaginadoAsync(string nome = null, string cpf = null, string cidade = null, string estado = null, bool? ativo = null, int page = 1, int pageSize = 20);
        Task<ClienteEntity> CadastrarAsync(ClienteEntity cliente);
        Task<ClienteEntity> AtualizarAsync(ClienteEntity cliente);
        Task<ClienteEntity> ObterPorCpfAsync(string cpf);
        Task<IEnumerable<ClienteEntity>> PesquisarPorNomeAsync(string nome);
        Task<IEnumerable<PropriedadeEntity>> ObterPropriedadesDoClienteAsync(Guid clienteId);
        Task<IEnumerable<TalhaoEntity>> ObterTalhoesDaPropriedadeAsync(Guid propriedadeId);
        Task<List<FaixaRendaDTO>> ObterFaixasRendaAsync();
    }
}
