using System.ComponentModel.DataAnnotations;

namespace MCR.API.Entities
{
    public class EstruturaMultiCalculoEntity
    {
        [Key]
        public Guid Id { get; set; }
        [Required]
        public bool UtilizaFormaPagamentoSeguro { get; set; }
        public string FormaPagamentoSeguro { get; set; }
        [Required]
        public bool UtilizaPrazoSeguro { get; set; }//Débito em conta / Boleto / Cartão crédito [Parametrização: para em 4x o parcelamento]
        public string PrazoSeguro { get; set; }//Anual / Pró-rata / Plurianual
        [Required]
        public bool UtilizaEquipamentoZeroQuilometro { get; set; }
        public string EquipamentoZeroQuilometro { get; set; } //Sim/Não
        [Required]
        public bool UtilizarListaEquipamentoSeguradoraEspecifica { get; set; }
        public ListaEquipamentoSeguradoras ListaEquipamentoSeguradoras { get; set; }
        [Required]
        public bool UtilizarListaModeloSeguradoraEspecifica { get; set; }
        public ListaModeloSeguradoras ListaModeloSeguradoras { get; set; }
        [Required]
        public bool UtilizarListaMarcaSeguradoraEspecifica { get; set; }
        public ListaMarcaSeguradoras ListaMarcaSeguradoras { get; set; }
        [Required]
        public bool UtilizarCoberturasParaEmpregador { get; set; }
        [Required]
        public bool UtilizarCoberturasParaPerdaAluguel { get; set; }
    }

    public enum ListaEquipamentoSeguradoras
    {
        [Display(Name = "Mapfre")]
        Mapfre,
        [Display(Name = "Allianz")]
        Allianz,
        [Display(Name = "Pottencial")]
        Pottencial
    }
    public enum ListaModeloSeguradoras
    {
        [Display(Name = "Mapfre")]
        Mapfre,
        [Display(Name = "Allianz")]
        Allianz,
        [Display(Name = "Pottencial")]
        Pottencial
    }
    public enum ListaMarcaSeguradoras
    {
        [Display(Name = "Mapfre")]
        Mapfre,
        [Display(Name = "Allianz")]
        Allianz,
        [Display(Name = "Pottencial")]
        Pottencial
    }
}
