using System.ComponentModel.DataAnnotations;

namespace MCR.API.Entities
{
    public class UsuarioAlterarDTO
    {
        public Guid Id { get; set; }

        [Required(ErrorMessage = "O campo Nome é obrigatório")]
        [Display(Name = "Nome")]
        public string Nome { get; set; }

        [Required(ErrorMessage = "O campo Email é obrigatório")]
        [Display(Name = "E-Mail")]
        public string Email { get; set; }

        [Required(ErrorMessage = "O campo Perfil de Acesso é obrigatório")]
        [Display(Name = "Perfil de Acesso")]
        public string? PerfilAcesso { get; set; }

        [Display(Name = "Documento")]
        public string? Document { get; set; }

        [Display(Name = "Telefone")]
        public string? PhoneNumber { get; set; }

        [Display(Name = "Utiliza OAuth")]
        public bool UtilizaOAuth { get; set; }
        public IList<UsuarioEstruturaNegocioDTO> EstruturaNegocio { get; set; } = new List<UsuarioEstruturaNegocioDTO>();

        [Display(Name = "Corretora")]
        public Guid? DefaultCorretoraId { get; set; }
        [Display(Name = "Canal")]
        public Guid? DefaultCanalId { get; set; }
        [Display(Name = "Ponto Atendimento")]
        public Guid? DefaultPontoAtendimentoId { get; set; }
    }
}
