namespace MCR.API.Models
{
    public class CotacaoInformacaoSeguradoModel
    {
        public string Nome { get; set; }
        public string TipoPessoa { get; set; }
        public string CPFCNPJ { get; set; }
        public string CEP { get; set; }
        public string Endereco { get; set; }
        public string Bairro { get; set; }
        public string Numero { get; set; }
        public string Estado { get; set; }
        public string Cidade { get; set; }
        public bool ReplicarBeneficiario { get; set; }
    }
}