using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MCR.API.Entities
{
    public class MicrosoftADEntity
    {
        [Key]
        public Guid Id { get; set; }
        [Required]
        public string RedirectAfterLogin { get; set; }
        [Required]
        public string ClientId { get; set; }
        [Required]
        public string ClientSecret { get; set; }
        [Required]
        public string URLToken { get; set; }
        [Required]
        public string URLMe { get; set; }
        public string TenantId { get; set; }
        public string Instance { get; set; }
        public string Domain { get; set; }
        public string SignedOutCallbackPath { get; set; }
        public string ScopeForAccessToken { get; set; }
        public string Authority { get; set; }
        [NotMapped]
        public string Mensagem { get; set; }
        [NotMapped]
        public bool Sucesso { get; set; }
    }
}