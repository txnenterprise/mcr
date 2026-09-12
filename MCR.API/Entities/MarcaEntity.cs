using System.ComponentModel.DataAnnotations;

namespace MCR.API.Entities
{
    public class MarcaEntity
    {
        [Key]
        public Guid Id { get; set; }
        [Required]
        public string CodigoInterno { get; set; }
        [Required]
        public string Nome { get; set; }
    }
}