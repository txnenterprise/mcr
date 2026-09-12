using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MCR.API.Entities
{
    public class ParametrizacaoCustomizacaoRelatorioEntity
    {
        [Key]
        public Guid Id { get; set; }
        [Required]
        public string Titulo1 { get; set; }
        [Required]
        public int TamanhoTitulo1 { get; set; }
        [Required]
        public string CorTitulo1 { get; set; }
        [Required]
        public string Texto1 { get; set; }
        [Required]
        public int TamanhoTexto1 { get; set; }
        [Required]
        public string CorTexto1 { get; set; }
        public string Titulo2 { get; set; }
        [Required]
        public int TamanhoTitulo2 { get; set; }
        public string CorTitulo2 { get; set; }
        public string Texto2 { get; set; }
        [Required]
        public int TamanhoTexto2 { get; set; }
        public string CorTexto2 { get; set; }
        public string Imagem1 { get; set; }
        [NotMapped]
        public string Mensagem { get; set; }
        [NotMapped]
        public bool Sucesso { get; set; }
    }
}