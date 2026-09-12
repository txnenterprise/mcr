namespace MCR.API.Entities
{
    public class UsuarioPontoAtendimentoDTO
    {
        public Guid Id { get; set; }
        public Guid PontoAtendimentoId { get; set; }
        public string PontoAtendimentoNome { get; set; }
        public Guid CanalId { get; set; }
        public string CanalNome { get; set; }
        public Guid UsuarioId { get; set; }
        public string UsuarioNome { get; set; }
        public string UsuarioEmail { get; set; }
        public Guid LideradoId { get; set; }
        public string LideradoNome { get; set; }
        public string LideradoEmail { get; set; }
        public bool Ativo { get; set; }
        public bool Excluido { get; set; }
        public string Mensagem { get; set; }
        public bool Sucesso { get; set; }
    }
}
