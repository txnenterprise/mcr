namespace MCR.API.Models.ViewModels
{
    public class EstruturaRiscoModel
    {
        public string PesquisaNome { get; set; }
        public string PesquisaCPF { get; set; }
        public string PesquisaAtivoInativo { get; set; }
        public List<EstruturaRiscoItem> ListaEstruturaRisco { get; set; } = new();
        public int PaginaAtual { get; set; }
        public int TotalPaginas { get; set; }
    }

    public class EstruturaRiscoItem
    {
        public EstruturaRiscoCliente Cliente { get; set; }
        public List<EstruturaRiscoPropriedade> Propriedades { get; set; } = new();
        public List<EstruturaRiscoTalhao> Talhoes { get; set; } = new();
    }

    public class EstruturaRiscoCliente
    {
        public string Nome { get; set; }
        public string CPF { get; set; }
        public string Telefone { get; set; }
        public string Cidade { get; set; }
        public string Estado { get; set; }
    }

    public class EstruturaRiscoPropriedade
    {
        public string Id { get; set; }
        public string Nome { get; set; }
        public string Cidade { get; set; }
        public string Estado { get; set; }
    }

    public class EstruturaRiscoTalhao
    {
        public string PropriedadeId { get; set; }
        public string Nome { get; set; }
        public decimal Area { get; set; }
        public string TipoSolo { get; set; }
        public string ClassificacaoSolo { get; set; }
    }
}
