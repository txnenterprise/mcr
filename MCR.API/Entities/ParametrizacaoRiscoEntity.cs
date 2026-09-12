using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MCR.API.Entities
{
    public class ParametrizacaoRiscoEntity
    {
        [Key]
        public Guid Id { get; set; }
        [Required]
        public bool EquipamentoOperaProximoAgua { get; set; }
        [Required]
        public bool SeguradoColaboradorOperador { get; set; }
        [NotMapped]
        public string Mensagem {  get; set; }
        [NotMapped]
        public bool Sucesso { get; set; }
    }
}