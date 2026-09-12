using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCR.API.Entities
{
    public class ParametrizacaoItemEntity
    {
        [Key]
        public Guid Id { get; set; }
    }
}
