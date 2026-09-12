using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MCR.API.Entities
{
    public class SatelliteSceneEntity
    {
        [Key]
        public Guid Id { get; set; }
        public Guid PropertyId { get; set; }
        public Guid TalhaoId { get; set; }
        public string Collection { get; set; }
        public string SceneId { get; set; }
        public DateTime AcquisitionDate { get; set; }
        public decimal? CloudCover { get; set; }
        public string AssetUrl { get; set; }
        public string Bbox { get; set; }
        public string AssetsJson { get; set; }
        public DateTime CreatedAt { get; set; }

        [NotMapped]
        public bool Sucesso { get; set; }
        [NotMapped]
        public string Mensagem { get; set; }
    }
}
