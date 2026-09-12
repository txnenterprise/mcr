namespace MCR.API.Services.Interfaces
{
    public interface IUsuarioEstruturaNegocioService
    {
        Task<(List<Guid>? CorretoraIds, List<Guid>? CanalIds, List<Guid>? PontoAtendimentoIds)> ObterUsuarioPermissoes();
    }
}
