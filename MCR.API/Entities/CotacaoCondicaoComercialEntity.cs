using System.ComponentModel.DataAnnotations;

namespace MCR.API.Entities
{
    public class CotacaoCondicaoComercialEntity
    {
        [Key]
        public Guid Id { get; set; }
        [Required]
        public Guid CotacaoId { get; set; }
        public decimal ComissaoSeguradoraMapfre { get; set; }
        public decimal DescontoAgravoSeguradoraMapfre { get; set; }
        public decimal MultiplicadorFranquiaSeguradoraMapfre { get; set; }
        public decimal ComissaoSeguradoraSwissRe { get; set; }
        public decimal DescontoAgravoSeguradoraSwissRe { get; set; }
        public decimal MultiplicadorFranquiaSeguradoraSwissRe { get; set; }
        public decimal ComissaoSeguradoraAllianz { get; set; }
        public decimal DescontoAgravoSeguradoraAllianz { get; set; }
        public decimal MultiplicadorFranquiaSeguradoraAllianz { get; set; }
        public decimal ComissaoSeguradoraTokio { get; set; }
        public decimal DescontoAgravoSeguradoraTokio { get; set; }
        public decimal MultiplicadorFranquiaSeguradoraTokio { get; set; }
        public decimal ComissaoSeguradoraSOMPO { get; set; }
        public decimal DescontoAgravoSeguradoraSOMPO { get; set; }
        public decimal MultiplicadorFranquiaSeguradoraSOMPO { get; set; }
        public decimal ComissaoSeguradoraPottencial { get; set; }
        public decimal DescontoAgravoSeguradoraPottencial { get; set; }
        public decimal MultiplicadorFranquiaSeguradoraPottencial { get; set; }
        public decimal ComissaoSeguradoraSombrero { get; set; }
        public decimal DescontoAgravoSeguradoraSombrero { get; set; }
        public decimal MultiplicadorFranquiaSeguradoraSombrero { get; set; }
        public decimal ComissaoSeguradoraFF { get; set; }
        public decimal DescontoAgravoSeguradoraFF { get; set; }
        public decimal MultiplicadorFranquiaSeguradoraFF { get; set; }

    }
}