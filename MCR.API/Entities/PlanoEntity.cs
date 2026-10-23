using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MCR.API.Entities
{
    /// <summary>
    /// Plano de limites (imutável após criação).
    /// MaximoUsuarios/MaximoPAsPorUsuario nulos = ILIMITADO.
    /// </summary>
    public class PlanoEntity
    {
        [Key]
        public Guid Id { get; set; }

        [Required(ErrorMessage = "O campo Nome é obrigatório")]
        [Display(Name = "Nome")]
        [StringLength(60, ErrorMessage = "O nome deve ter no máximo {1} caracteres.")]
        public string Nome { get; set; } = string.Empty;

        [Display(Name = "Máximo de Usuários")]
        [Range(1, int.MaxValue, ErrorMessage = "O máximo de usuários deve ser maior que zero.")]
        public int? MaximoUsuarios { get; set; }

        [Display(Name = "Máximo de PA Vinculado")]
        [Range(1, int.MaxValue, ErrorMessage = "O máximo de PA vinculado deve ser maior que zero.")]
        public int? MaximoPAsPorUsuario { get; set; }

        public bool Ativo { get; set; }
        public bool Excluido { get; set; }

        [NotMapped]
        public bool Sucesso { get; set; }
        [NotMapped]
        public string Mensagem { get; set; } = string.Empty;
        [NotMapped]
        public int TotalItems { get; set; }
        [NotMapped]
        public int TotalPages { get; set; }
    }
}
