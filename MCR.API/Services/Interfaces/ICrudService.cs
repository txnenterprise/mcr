namespace MCR.API.Services.Interfaces
{
    public interface ICRUDService<T> where T : class
    {
        Task<T> ObterPorIdAsync(Guid id);
        Task<IEnumerable<T>> ObterTodosAsync();
        Task<T> CadastrarAsync(T entity);
        Task<T> AtualizarAsync(T entity);
        Task<bool> InativarAsync(Guid id);
        Task<bool> AtivarAsync(Guid id);
    }
}
