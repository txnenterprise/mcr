using System.ComponentModel.DataAnnotations;

namespace MCR.API.Entities
{
    public class ParametrizacaoMultiCalculoParceiroEntity
    {
        [Key]
        public Guid Id { get; set; }
        [Required]
        public bool HabilitarMapfre { get; set; }
        [Required]
        public bool HabilitarPottencial { get; set; }
        [Required]
        public bool HabilitarSompo { get; set; }
        [Required]
        public bool HabilitarSwissRe { get; set; }
        [Required]
        public bool HabilitarSombrero { get; set; }
        [Required]
        public bool HabilitarFairFax { get; set; }
        [Required]
        public bool HabilitarAllianz { get; set; }
        [Required]
        public bool HabilitarTokio { get; set; }
        [Required]
        public string NomeParceiro { get; set; }
        [Required]
        public string LogoParceiro { get; set; }
        [Required]
        public string CorPredominante { get; set; }
    }
}