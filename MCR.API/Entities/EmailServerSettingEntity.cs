using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MCR.API.Entities
{
    public class EmailServerSettingEntity
    {
        [Key]
        public Guid Id { get; set; }
        [Required]
        public string ServerAddress { get; set; }
        [Required]
        public int ServerPort { get; set; }
        [Required]
        public bool ServerUseSsl { get; set; }
        [Required]
        public string Username { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        public string EnviromentToAction { get; set; }
        [NotMapped]
        public string Mensagem { get; set; }
        [NotMapped]
        public bool Sucesso { get; set; }
    }
}