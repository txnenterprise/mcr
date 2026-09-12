using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCR.API.Entities
{
    public class CotacaoCoberturaEntity
    {
        [Key]
        public Guid Id { get; set; }
        [Required]
        public Guid CotacaoId { get; set; }
        [Required]
        public Guid BemId { get; set; }
        public bool AplicarCoberturaTotal { get; set; }
        public bool ContratarResponsabilidadeCivilMaquinariaAgricola { get; set; }
        public decimal ValorResponsabilidadeCivilMaquinariaAgricola { get; set; }
        public bool ContratarResponsabilidadeCivilEmpregador { get; set; }
        public decimal ValorResponsabilidadeCivilEmpregador { get; set; }
        public bool ContratarCoberturaRelativaPerdaPagamentoAluguel { get; set; }
        public decimal ValorCoberturaRelativaPerdaPagamentoAluguel { get; set; }
        public bool ContratarFurtoSimples { get; set; }
        public decimal ValorFurtoSimples { get; set; }
        public bool ContratarDanosEletricos { get; set; }
        public decimal ValorDanosEletricos { get; set; }
        public bool ContratarQuebraVidros { get; set; }
        public decimal ValorQuebraVidros { get; set; }
    }
}