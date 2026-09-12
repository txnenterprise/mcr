using MCR.API.Entities;

namespace MCR.API.Usuario.Domain.DTO
{
    public class UsuarioCadastroDTO
    {
        public string PerfilAcesso { get; set; }
        public string Nome { get; set; }
        public string Email { get; set; }
        public string Document { get; set; }
        public string PhoneNumber { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
        public string? DefaultCorretoraId { get; set; }
        public string? DefaultCanalId { get; set; }
        public string? DefaultPontoAtendimentoId { get; set; }
    }

    public class UsuarioAlterarDTO
    {
        public string Id { get; set; }
        public string PerfilAcesso { get; set; }
        public string Nome { get; set; }
        public string Email { get; set; }
        public string Document { get; set; }
        public string PhoneNumber { get; set; }
        public bool UtilizaOAuth { get; set; }
        public string? DefaultCorretoraId { get; set; }
        public string? DefaultCanalId { get; set; }
        public string? DefaultPontoAtendimentoId { get; set; }
        public List<UsuarioEstruturaNegocioDTO> EstruturaNegocio { get; set; } = new();
    }
}
