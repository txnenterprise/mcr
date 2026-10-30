using System.ComponentModel.DataAnnotations;
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
        [Display(Name = "Corretora")]
        public string? DefaultCorretoraId { get; set; }
        [Display(Name = "Canal")]
        public string? DefaultCanalId { get; set; }
        [Display(Name = "Ponto Atendimento")]
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
        [Display(Name = "Corretora")]
        public string? DefaultCorretoraId { get; set; }
        [Display(Name = "Canal")]
        public string? DefaultCanalId { get; set; }
        [Display(Name = "Ponto Atendimento")]
        public string? DefaultPontoAtendimentoId { get; set; }
        public List<UsuarioEstruturaNegocioDTO> EstruturaNegocio { get; set; } = new();
    }
}
