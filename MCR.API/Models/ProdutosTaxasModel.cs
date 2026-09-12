using Microsoft.AspNetCore.Mvc.Rendering;
using MCR.API.Entities;

namespace MCR.API.Models
{
    public class ProdutosViewModel
    {
        public ProdutosEntity Produto { get; set; } = new();

        public string? PesquisaSafra { get; set; }
        public string? PesquisaCultura { get; set; }
        public string? PesquisaSeguradora { get; set; }
        public string? PesquisaAtivoInativo { get; set; } = "Ativo";
        public int? PaginaAtual { get; set; }
        public int? TotalPaginas { get; set; }

        public List<ProdutosListaItem> ListaProdutos { get; set; } = new();

        public List<SelectListItem> SafraOptions { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> CulturaOptions { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> SeguradoraOptions { get; set; } = new List<SelectListItem>();
        public ICollection<SelectListItem> CanalOptions { get; set; } = new List<SelectListItem>();
        public ICollection<SelectListItem> CanalEntities { get; set; } = new List<SelectListItem>();
    }

    public class ProdutosListaItem
    {
        public Guid Id { get; set; }
        public string SafraAno { get; set; } = "";
        public string CulturaNome { get; set; } = "";
        public string SeguradoraNome { get; set; } = "";
        public string NomeProduto { get; set; } = "";
        public string Modalidade { get; set; } = "";
        public bool Ativo { get; set; }
    }
}
