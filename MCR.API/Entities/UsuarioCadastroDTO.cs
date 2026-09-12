using System.ComponentModel.DataAnnotations;

namespace MCR.API.Entities
{
    public class UsuarioCadastroDTO
    {
        public Guid? Id { get; set; }

        [Required(ErrorMessage = "O campo Nome é obrigatório")]
        [Display(Name = "Nome")]
        public string Nome { get; set; }

        [Required(ErrorMessage = "O campo Email é obrigatório")]
        [Display(Name = "E-Mail")]
        public string Email { get; set; }

        [Required(ErrorMessage = "O campo Senha é obrigatório")]
        [Display(Name = "Senha")]
        [StringLength(100, ErrorMessage = "A senha deve ter pelo menos {2} caracteres.", MinimumLength = 8)]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,}$",
            ErrorMessage = "A senha deve conter no mínimo 8 caracteres, pelo menos uma letra maiúscula, uma letra minúscula, um número e um caractere especial.")]
        public string Password { get; set; }

        [Required(ErrorMessage = "O campo Confirmar Senha é obrigatório")]
        [Display(Name = "Confirmar Senha")]
        [Compare("Password", ErrorMessage = "As senhas não conferem.")]
        public string ConfirmPassword { get; set; }

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
