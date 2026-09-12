using System.ComponentModel.DataAnnotations;

namespace MCR.API.Entities
{
    /// <summary>
    /// Relação N:N entre Produto e Subvenção Estadual.
    /// Um produto pode ter várias subvenções estaduais (uma por estado).
    /// Na cotação, usa-se a subvenção cujo Estado = estado do cliente/cotação.
    /// </summary>
    public class ProdutoSubvencaoEstadualEntity
    {
        [Key]
        public Guid Id { get; set; }
        public Guid ProdutoId { get; set; }
        public ProdutosEntity Produto { get; set; } = null!;
        public Guid SubvencaoEstadualId { get; set; }
        public SubvencaoEstadualEntity SubvencaoEstadual { get; set; } = null!;
    }
}
