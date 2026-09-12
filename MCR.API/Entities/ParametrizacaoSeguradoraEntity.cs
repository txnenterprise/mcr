using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MCR.API.Entities
{
    public class ParametrizacaoSeguradoraEntity
    {
        [Key]
        public Guid Id { get; set; }
        [Required]
        public bool ContratarCoberturaResponsabilidadeCivilParaMaquinaAgricola { get; set; }
        [Required]
        public decimal ValorCoberturaResponsabilidadeCivilParaMaquinaAgricola { get; set; }
        [Required]
        public bool ContratarCoberturaResponsabilidadeCivilParaEmpregador { get; set; }
        [Required]
        public decimal ValorCoberturaResponsabilidadeCivilParaEmpregador { get; set; }
        [Required]
        public bool ContratarCoberturaPerdaPagamentoAluguel { get; set; }
        [Required]
        public decimal ValorCoberturaPerdaPagamentoAluguel { get; set; }
        [Required]
        public bool ContratarCoberturaFurtoSimples { get; set; }
        [Required]
        public decimal ValorCoberturaFurtoSimples { get; set; }
        [Required]
        public bool ContratarCoberturaDanosEletricos { get; set; }
        [Required]
        public decimal ValorCoberturaDanosEletricos { get; set; }
        [Required]
        public bool ContratarCoberturaQuebraVidros { get; set; }
        [Required]
        public decimal ValorCoberturaQuebraVidros { get; set; }        
        [Required]
        public int NumeroParcelamento { get; set; }
        [Required]
        public decimal ComissaoSeguradoraMapfre { get; set; }
        [Required]
        public decimal DescontoAgravoSeguradoraMapfre { get; set; }
        [Required]
        public decimal MultiplicadorFranquiaSeguradoraMapfre { get; set; }
        [Required]
        public decimal ComissaoSeguradoraSwissRe { get; set; }
        [Required]
        public decimal DescontoAgravoSeguradoraSwissRe { get; set; }
        [Required]
        public decimal MultiplicadorFranquiaSeguradoraSwissRe { get; set; }
        [Required]
        public decimal ComissaoSeguradoraAllianz { get; set; }
        [Required]
        public decimal DescontoAgravoSeguradoraAllianz { get; set; }
        [Required]
        public decimal MultiplicadorFranquiaSeguradoraAllianz { get; set; }
        [Required]
        public decimal ComissaoSeguradoraTokio { get; set; }
        [Required]
        public decimal DescontoAgravoSeguradoraTokio { get; set; }
        [Required]
        public decimal MultiplicadorFranquiaSeguradoraTokio { get; set; }
        [Required]
        public decimal ComissaoSeguradoraSOMPO { get; set; }
        [Required]
        public decimal DescontoAgravoSeguradoraSOMPO { get; set; }
        [Required]
        public decimal MultiplicadorFranquiaSeguradoraSOMPO { get; set; }
        [Required]
        public decimal ComissaoSeguradoraPottencial { get; set; }
        [Required]
        public decimal DescontoAgravoSeguradoraPottencial { get; set; }
        [Required]
        public decimal MultiplicadorFranquiaSeguradoraPottencial { get; set; }
        [Required]
        public decimal ComissaoSeguradoraSombrero { get; set; }
        [Required]
        public decimal DescontoAgravoSeguradoraSombrero { get; set; }
        [Required]
        public decimal MultiplicadorFranquiaSeguradoraSombrero { get; set; }
        [Required]
        public decimal ComissaoSeguradoraFF { get; set; }
        [Required]
        public decimal DescontoAgravoSeguradoraFF { get; set; }
        [Required]
        public decimal MultiplicadorFranquiaSeguradoraFF { get; set; }
        [NotMapped]
        public bool Sucesso { get; set; }
        [NotMapped]
        public string Mensagem { get; set; }
    }
}