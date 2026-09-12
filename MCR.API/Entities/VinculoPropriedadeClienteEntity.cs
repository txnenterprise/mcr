using System.ComponentModel.DataAnnotations;

namespace MCR.API.Entities
{
    public class VinculoPropriedadeClienteEntity
    {
        [Key]
        public Guid Id { get; set; }
        public Guid ClienteId { get; set; }
        public Guid PropriedadeId { get; set; }

        public PropriedadeEntity? Propriedade { get; set; }
    }
}