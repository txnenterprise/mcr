using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MCR.API.Entities
{
    public abstract class BaseEntity
    {
        [Key]
        public Guid Id { get; set; }
        [NotMapped]
        public bool Success { get; set; }
        [NotMapped]
        public string Message { get; set; }
    }
}