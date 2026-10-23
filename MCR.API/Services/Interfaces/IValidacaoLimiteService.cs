using MCR.API.Entities;

namespace MCR.API.Services.Interfaces
{
    public interface IValidacaoLimiteService
    {
        /// <summary>
        /// Valida se a corretora ainda tem espaço no limite de usuários do plano.
        /// usuarioIdExcluir: usuário cujos vínculos atuais serão removidos (edição) e não devem contar.
        /// Sem plano ou limite null = sem limite (ok).
        /// </summary>
        Task<(bool ok, string? mensagem)> ValidarLimiteUsuariosAsync(Guid corretoraId, Guid? usuarioIdExcluir = null);

        /// <summary>
        /// Valida se o usuário ainda tem espaço no limite de PA do plano (plano vem do canal → corretora).
        /// Sem canal, sem plano ou limite null = sem limite (ok).
        /// </summary>
        Task<(bool ok, string? mensagem)> ValidarLimitePAsPorUsuarioAsync(Guid usuarioId, Guid? canalId);
    }
}
