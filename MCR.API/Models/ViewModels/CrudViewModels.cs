using MCR.API.Entities;
using MCR.API.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace MCR.API.Models.ViewModels
{
    public class BeneficiarioModel
    {
        public BeneficiarioEntity Beneficiario { get; set; } = new();
        public string PesquisaNome { get; set; }
        public string PesquisaCNPJ { get; set; }
        public string PesquisaAtivoInativo { get; set; }
        public List<BeneficiarioEntity> ListaBeneficiarios { get; set; } = new();
        public int PaginaAtual { get; set; }
        public int TotalPaginas { get; set; }
        public string ObjectImagem { get; set; }
        public bool Sucesso { get; set; }
        public string Mensagem { get; set; }
    }

    public class UsuarioModel
    {
        public string Id { get; set; }
        public string Nome { get; set; }
        public string Email { get; set; }
        public string PerfilAcesso { get; set; }
        public string Document { get; set; }
        public string PhoneNumber { get; set; }
        public bool Bloqueado { get; set; }
        public bool Sucesso { get; set; }
        public string Mensagem { get; set; }
        public List<UsuarioListItem> ListaUsuarios { get; set; } = new();
        public string PesquisaNome { get; set; }
        public int PaginaAtual { get; set; }
        public int TotalPaginas { get; set; }
        public bool UtilizaOAuth { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
    }

    public class UsuarioListItem
    {
        public string Id { get; set; }
        public string Nome { get; set; }
        public string Email { get; set; }
        public string PerfilAcesso { get; set; }
        public string Document { get; set; }
        public string PhoneNumber { get; set; }
        public bool Bloqueado { get; set; }
        public string Corretora { get; set; }
        public string PontoAtendimento { get; set; }
    }

    public class ClienteModel
    {
        public ClienteEntity Cliente { get; set; } = new();
        public string PesquisaNome { get; set; }
        public string PesquisaCPF { get; set; }
        public string PesquisaAtivoInativo { get; set; }
        public List<ClienteEntity> ListaClientes { get; set; } = new();
        public int PaginaAtual { get; set; }
        public int TotalPaginas { get; set; }
        public string CotacaoId { get; set; }
        public string ObjectImagemCPF { get; set; }
        public string ObjectImagemRG { get; set; }
        public string JsonVinculosFamiliar { get; set; }
        public string selectedItems { get; set; }
        public PropriedadeModel PropriedadeModel { get; set; } = new();
        public bool Sucesso { get; set; }
        public string Mensagem { get; set; }
    }

    public class UsuarioPontoAtendimentoModel
    {
        public string PesquisaNomeUsuario { get; set; }
        public string PesquisaNomeLiderado { get; set; }
        public string PesquisaNomePontoAtendimento { get; set; }
        public int PaginaAtual { get; set; }
        public int TotalPaginas { get; set; }
        public List<UsuarioPontoAtendimentoItem> ListaUsuarioPontoAtendimentoDTO { get; set; } = new();
        public List<UsuarioCadastroItem> ListaUsuariosCadastrados { get; set; } = new();
        public List<CanalListItem> ListaPontosAtendimento { get; set; } = new();
        public List<CanalListItem> ListaCanais { get; set; } = new();
        public string jsonUsuarios { get; set; }
        public UsuarioPontoAtendimentoItem UsuarioPontoAtendimentoDTO { get; set; } = new();
        public bool Sucesso { get; set; }
        public string Mensagem { get; set; }
    }

    public class UsuarioPontoAtendimentoItem
    {
        public string Id { get; set; }
        public string UsuarioNome { get; set; }
        public string LideradoNome { get; set; }
        public string LideradoEmail { get; set; }
        public string CanalNome { get; set; }
        public string PontoAtendimentoNome { get; set; }
        public bool Excluido { get; set; }
        public bool Sucesso { get; set; }
        public string Mensagem { get; set; }
    }

    public class UsuarioCanalModel
    {
        public string PesquisaNomeUsuario { get; set; }
        public string PesquisaNomeLiderado { get; set; }
        public string PesquisaNomeCanal { get; set; }
        public int PaginaAtual { get; set; }
        public int TotalPaginas { get; set; }
        public List<UsuarioCanalListItem> ListaUsuarioCanal { get; set; } = new();
        public List<UsuarioCadastroItem> ListaUsuariosCadastrados { get; set; } = new();
        public List<CanalListItem> ListaCanais { get; set; } = new();
        public string jsonUsuarios { get; set; }
        public UsuarioCanalWrapper UsuarioCanal { get; set; } = new();
        public bool Sucesso { get; set; }
        public string Mensagem { get; set; }
    }

    public class UsuarioCanalWrapper
    {
        public string Id { get; set; }
        public string UsuarioNome { get; set; }
        public string LideradoNome { get; set; }
        public UsuarioCadastroItem UsuarioLiderado { get; set; } = new();
        public CanalListItem Canal { get; set; } = new();
        public string CanalNome { get; set; }
        public bool Excluido { get; set; }
        public bool Sucesso { get; set; }
        public string Mensagem { get; set; }
    }

    public class UsuarioCadastroItem
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
    }

    public class CanalListItem
    {
        public string Id { get; set; }
        public string RazaoSocial { get; set; }
    }

    public class UsuarioCanalListItem
    {
        public string Id { get; set; }
        public UsuarioCadastroItem Usuario { get; set; } = new();
        public UsuarioCadastroItem UsuarioLiderado { get; set; } = new();
        public CanalListItem Canal { get; set; } = new();
        public bool Excluido { get; set; }
        public bool Sucesso { get; set; }
    }

    public class CanalModel
    {
        public CanalEntity Canal { get; set; } = new();
        public string PesquisaNome { get; set; }
        public string PesquisaRazaoSocial { get; set; }
        public string PesquisaAtivoInativo { get; set; }
        public string PesquisaRazaoSocialPA { get; set; }
        public string PesquisaAtivoInativoPA { get; set; }
        public List<CanalEntity> ListaCanais { get; set; } = new();
        public int PaginaAtual { get; set; }
        public int TotalPaginas { get; set; }
        public string ObjectImagem { get; set; }
        public bool Sucesso { get; set; }
        public string Mensagem { get; set; }
    }

    public class SubvencaoFederalModel
    {
        public SubvencaoFederalEntity SubvencaoFederal { get; set; } = new();
        public List<SubvencaoFederalEntity> ListaSubvencoesFederais { get; set; } = new();
        public string PesquisaNome { get; set; }
        public string PesquisaAtivoInativo { get; set; }
        public int PaginaAtual { get; set; }
        public int TotalPaginas { get; set; }
        public bool Sucesso { get; set; }
        public string Mensagem { get; set; }
    }

    public class SubvencaoEstadualModel
    {
        public SubvencaoEstadualEntity SubvencaoEstadual { get; set; } = new();
        public List<SubvencaoEstadualEntity> ListaSubvencoesEstaduais { get; set; } = new();
        public string PesquisaNome { get; set; }
        public string PesquisaAtivoInativo { get; set; }
        public int PaginaAtual { get; set; }
        public int TotalPaginas { get; set; }
    }

    public class SeguradoraModel
    {
        public SeguradoraEntity Seguradora { get; set; } = new();
        public string PesquisaNome { get; set; }
        public string PesquisaAtivoInativo { get; set; }
        public int PaginaAtual { get; set; }
        public int TotalPaginas { get; set; }
        public bool Sucesso { get; set; }
        public string Mensagem { get; set; }
        public string ObjectImagem { get; set; }
    }

    public class SafraModel
    {
        public SafraEntity Safra { get; set; } = new();
        public string PesquisaDescricao { get; set; }
        public string PesquisaAtivoInativo { get; set; }
        public int PaginaAtual { get; set; }
        public int TotalPaginas { get; set; }
        public bool Sucesso { get; set; }
        public string Mensagem { get; set; }
    }

    public class TalhaoModel
    {
        public TalhaoEntity Talhao { get; set; } = new();
        public string PesquisaAtivoInativo { get; set; }
        public List<TalhaoEntity> ListaTalhoes { get; set; } = new();
        public int PaginaAtual { get; set; }
        public int TotalPaginas { get; set; }
        public string ImageJson { get; set; }
        public string KmlJson { get; set; }
        public string ImagensJson { get; set; }
        public string KmlsJson { get; set; }
        public List<string> ListaKmlFiles { get; set; } = new();
        public int ItensPorPagina { get; set; }
        public string PesquisaPropriedadeId { get; set; }
        public bool Sucesso { get; set; }
        public string Mensagem { get; set; }
    }

    public class CorretoraModel
    {
        public CorretoraEntity Corretora { get; set; } = new();
        public string PesquisaNome { get; set; }
        public string PesquisaAtivoInativo { get; set; }
        public List<CorretoraEntity> ListaCorretoras { get; set; } = new();
        public int PaginaAtual { get; set; }
        public int TotalPaginas { get; set; }
        public bool Sucesso { get; set; }
        public string Mensagem { get; set; }
    }

    public class PontoAtendimentoModel
    {
        public PontoAtendimentoEntity PontoAtendimento { get; set; } = new();
        public string PesquisaNome { get; set; }
        public string PesquisaRazaoSocial { get; set; }
        public string PesquisaCanalRazaoSocial { get; set; }
        public string PesquisaAtivoInativo { get; set; }
        public List<PontoAtendimentoEntity> ListaPontosAtendimento { get; set; } = new();
        public int PaginaAtual { get; set; }
        public int TotalPaginas { get; set; }
        public bool Sucesso { get; set; }
        public string Mensagem { get; set; }
        public string ObjectImagem { get; set; }
    }
}