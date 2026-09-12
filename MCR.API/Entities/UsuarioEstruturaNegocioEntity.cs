using System.ComponentModel.DataAnnotations;



namespace MCR.API.Entities
{
    public class UsuarioEstruturaNegocioEntity
    {
        [Key]
        public Guid Id { get; set; }
        public Guid? CorretoraId { get; set; }
        public Guid? CanalId { get; set; }
        public Guid? PontoAtendimentoId { get; set; }
        public Guid UsuarioId { get; set; }
        public Guid UsuarioCriadorId { get; set; }

        public bool Ativo { get; set; }
        public bool Excluido { get; set; }

        public virtual UsuarioEntity? Usuario { get; set; }

        public virtual UsuarioEntity? UsuarioCriador { get; set; }

        public virtual PontoAtendimentoEntity? PontoAtendimento { get; set; }
        public virtual CanalEntity? Canal { get; set; }

        public virtual CorretoraEntity? Corretora { get; set; }
    }
}