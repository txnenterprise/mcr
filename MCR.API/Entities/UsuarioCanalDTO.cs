namespace MCR.API.Entities
{
    public class UsuarioCanalDTO
    {
        public Guid Id { get; set; }
        public Guid CanalId { get; set; }
        public Guid UsuarioId { get; set; }
        public string UsuarioNome { get; set; }
        public string UsuarioEmail { get; set; }
        public Guid LideradoId { get; set; }
        public string LideradoNome { get; set; }
        public string LideradoEmail { get; set; }
        public bool Ativo { get; set; }
        public bool Excluido { get; set; }
        public bool Sucesso { get; set; }
        public string RazaoSocial { get; set; }
    }
}
