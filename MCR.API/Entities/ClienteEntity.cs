using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;



namespace MCR.API.Entities
{
    public class ClienteEntity
    {
        [Key]
        public Guid Id { get; set; }
        public string Nome { get; set; }
        public string CPF { get; set; }
        public string? ImagemCPF { get; set; }
        public string DataNascimento { get; set; }
        public string RG { get; set; }
        public string? ImagemRG { get; set; }
        public string DataExpedicaoRG { get; set; }
        public string OrgaoExpeditorRG { get; set; }
        public string EstadoCivil { get; set; }
        public string Sexo { get; set; }
        public string Telefone { get; set; }
        public string Celular { get; set; }
        public string Email { get; set; }
        public string Profissao { get; set; }
        public string FaixaRenda { get; set; }
        public string Banco { get; set; }
        public string Agencia { get; set; }
        public string Conta { get; set; }
        public string ChavePIX { get; set; }
        public string Endereco { get; set; }
        public string Bairro { get; set; }
        public string Numero { get; set; }
        public string? Complemento { get; set; }
        public string Cidade { get; set; }
        public string Estado { get; set; }
        public string CEP { get; set; }
        public bool Ativo { get; set; } = true;
        public bool Excluido { get; set; } = false;

        [NotMapped]
        public IList<VinculoFamiliarEntity>? VinculosFamiliares { get; set; } = new List<VinculoFamiliarEntity>();
        [NotMapped]
        public IList<VinculoPropriedadeClienteEntity> VinculosPropriedads { get; set; } = new List<VinculoPropriedadeClienteEntity>();
        [NotMapped]
        public IList<PropriedadeEntity> Propriedades { get; set; } = new List<PropriedadeEntity>();
        [NotMapped]
        public int QuantidadePropriedadesAssociadas { get; set; }
        [NotMapped]
        public int QuantidadeVinculosFamiliaresAssociados { get; set; }
        [NotMapped]
        public bool Sucesso { get; set; }
        [NotMapped]
        public string Mensagem { get; set; }
        [NotMapped]
        public int TotalItems { get; set; }
        [NotMapped]
        public int TotalPages { get; set; }

        public virtual ICollection<CotacoesAgricolaEntity> CotacoesAgricola { get; set; } = new List<CotacoesAgricolaEntity>();

        public virtual ICollection<PropostasEntity> Propostas { get; set; } = new List<PropostasEntity>();

        public virtual ICollection<PropostasSeguradosEntity> PropostasSegurados { get; set; } = new List<PropostasSeguradosEntity>();

        public virtual ICollection<PropostasClientePropriedadesEntity> PropostasClientePropriedades { get; set; } = new List<PropostasClientePropriedadesEntity>();

        public virtual ICollection<PropostasBeneficiariosEntity> PropostasBeneficiarios { get; set; } = new List<PropostasBeneficiariosEntity>();

        public virtual ICollection<PropostasQuestionarioFamiliarEntity>? PropostasQuestionarioFamiliar { get; set; } = new List<PropostasQuestionarioFamiliarEntity>();
    }
}
