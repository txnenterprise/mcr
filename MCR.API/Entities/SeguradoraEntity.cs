using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MCR.API.Entities
{
    public class SeguradoraEntity
    {
        [Key]
        public Guid Id { get; set; }
        [Required]
        public string RazaoSocial { get; set; }
        [Required]
        public string NomeFantasia { get; set; }
        [Required]
        public string Logo { get; set; }
        [Required]
        public bool Ativo { get; set; }
        [NotMapped]
        public bool Sucesso { get; set; }
        [NotMapped]
        public string Mensagem { get; set; }
    }
    public enum SeguradorasEnum
    {
        Mapfre,
        SwissRe,
        Allianz,
        Tokio,
        SOMPO,
        Pottencial,
        Sombrero,
        FairFax
    }
}