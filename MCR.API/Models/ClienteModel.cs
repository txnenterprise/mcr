using MCR.API.Entities;

namespace MCR.API.Models
{
    public class ClienteModel
    {
        public ClienteEntity Cliente { get; set; }
        public IList<ClienteEntity> ListaClientes { get; set; }
        public PropriedadeModel PropriedadeModel { get; set; }
        public string? JsonVinculosFamiliar { get; set; }
        public string selectedItems { get; set; }
        public int PaginaAtual { get; set; }
        public int ItensPorPagina { get; set; }
        public int TotalPaginas { get; set; }
        public string? ObjectImagemCPF { get; set; }
        public string? ObjectImagemRG { get; set; }
        public string? PesquisaNome { get; set; }
        public string? PesquisaAtivoInativo { get; set; }
        public string? PesquisaCPF { get; set; }
        public Guid PesquisaPontoAtendimento { get; set; }

        public Guid? CotacaoId { get; set; }
    }
    partial class JsonPropriedade
    {
        public string id { get; set; }
        public string nome { get; set; }
        public string endereco { get; set; }
        public string cidade { get; set; }
        public string estado { get; set; }
    }
}
