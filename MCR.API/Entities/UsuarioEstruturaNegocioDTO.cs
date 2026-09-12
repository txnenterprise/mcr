using System.Text.Json.Serialization;

namespace MCR.API.Entities
{
    public class UsuarioEstruturaNegocioDTO
    {
        [JsonPropertyName("corretoraId")]
        public Guid? CorretoraId { get; set; }

        [JsonPropertyName("corretoraText")]
        public string? CorretoraRazao { get; set; }

        [JsonPropertyName("canalId")]
        public Guid? CanalId { get; set; }

        [JsonPropertyName("canalText")]
        public string? CanalRazao { get; set; }

        [JsonPropertyName("pontoAtendimentoId")]
        public Guid? PontoAtendimentoId { get; set; }

        [JsonPropertyName("pontoAtendimentoText")]
        public string? PontoAtendimentoRazao { get; set; }
    }
}
