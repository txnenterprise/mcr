using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCR.API.Entities
{
    public class CotacaoFormularioRiscoEntity
    {
        [Key]
        public Guid Id { get; set; }
        [Required]
        public Guid CotacaoId { get; set; }
        [Required]
        public Guid BemId { get; set; }
        public bool EquipamentoAlugadoDuranteVigencia { get; set; }
        public bool EquipamentoCedidoTerceirosDuranteVigencia { get; set; }
        public bool EquipamentoAtividadesRurais { get; set; }
        public bool EquipamentoAtividadeFlorestal { get; set; }
        public bool EquipamentoOperaProximoAgua { get; set; }
        public bool SeguradoColaboradorOperador { get; set; }
    }
}