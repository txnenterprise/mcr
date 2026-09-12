using System.ComponentModel.DataAnnotations;


namespace MCR.API.Entities
{
    public class ProdutosCanalPontoAtendimentoEntity
    {
        [Key]
        public Guid Id { get; set; }
        public Guid ProdutoId { get; set; }
        public ProdutosEntity Produto { get; set; }
        public Guid CanalId { get; set; }
        public CanalEntity Canal { get; set; }
        public Guid PontoAtendimentoId { get; set; }
        public PontoAtendimentoEntity PontoAtendimento { get; set; }
    }
}