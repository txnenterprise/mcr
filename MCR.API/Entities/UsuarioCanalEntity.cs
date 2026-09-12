using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace MCR.API.Entities
{
    public class UsuarioCanalEntity
    {
        [Key]
        public Guid Id { get; set; }
        public Guid CanalId { get; set; }
        public Guid UsuarioId { get; set; }
        public Guid LideradoId { get; set; }

        public bool Ativo { get; set; }
        public bool Excluido { get; set; }
        [NotMapped]
        public bool Sucesso { get; set; }
        [NotMapped]
        public string Mensagem { get; set; }
        [NotMapped]
        public int TotalItems { get; set; }
        [NotMapped]
        public int TotalPages { get; set; }

        public virtual UsuarioEntity? Usuario { get; set; }

        public virtual UsuarioEntity? UsuarioLiderado { get; set; }
        public virtual CanalEntity? Canal { get; set; }
    }
}