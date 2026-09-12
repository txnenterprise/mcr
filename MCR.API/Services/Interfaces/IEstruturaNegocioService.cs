using MCR.API.Entities;

namespace MCR.API.Services.Interfaces
{
    public interface IEstruturaNegocioService
    {
        Task<IList<CanalEntity>> PesquisarCanaisPontosAtendimento(string razaoSocial, string ativo, string razaoSocialPA,
                                                                    string ativoPA, int page, int pageSize);
    }
}
