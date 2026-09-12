using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCR.API.Entities
{
    public class ParametrizacaoCabEntity
    {
        [Key]
        public Guid Id { get; set; }
        [Required]
        public Guid EstruturaMultiCalculoId { get; set; }
        public EstruturaMultiCalculoEntity EstruturaMultiCalculo { get; set; }
        [Required]
        public Guid CorretoraId { get; set; }
        public CorretoraEntity Corretora { get; set; }
        [NotMapped]
        public bool Sucesso { get; set; }
        [NotMapped]
        public string Mensagem { get; set; }
    }
}
