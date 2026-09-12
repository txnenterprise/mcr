using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MCR.API.Entities
{
    public class LoginSeguradoraEntity
    {
        [Key]
        public Guid Id { get; set; }
        [Required]
        public string Seguradora { get; set; }
        [Required]
        public string CorretorLogado { get; set; }
        public string Link { get; set; }
        public string Susep { get; set; }
        public string CodigoInterno { get; set; }
        public string Usuario { get; set; }
        public string Senha { get; set; }
        public string Token { get; set; }
        public string ChaveAPIKey { get; set; }
        public string ChaveAPIValue { get; set; }
        [NotMapped]
        public string CorretoraVinculada { get; set; }
        public Guid CorretoraId { get; set; }
        [NotMapped]
        public bool Sucesso { get; set; }
        [NotMapped]
        public string Mensagem { get; set; }
    }
}