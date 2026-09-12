using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MCR.API.Entities;
using MCR.API.Propostas.Domain.DTO;
using MCR.API.Repository;
using MCR.API.Services.Interfaces;

namespace MCR.API.Controllers
{
    public class PropostasVistoriaController : Controller
    {
        private readonly DbContextMCR _context;
        private readonly ICulturaService _culturaService;

        public PropostasVistoriaController(DbContextMCR context, ICulturaService culturaService)
        {
            _context = context;
            _culturaService = culturaService;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> VincularPropostaVistoria(PropostasVistoriaCadastrarDTO model)
        {
            try
            {
                var nome = model.NomeNovo;
                var cpf = model.CPFNovo;
                var telefone = model.TelefoneNovo;
                var propostasSeguradoId = model.PessoasVistoria?.PropostasSeguradoId;

                if (propostasSeguradoId.HasValue)
                {
                    var segurado = await _context.PropostasSegurados.FindAsync(propostasSeguradoId.Value);
                    if (segurado != null)
                    {
                        nome = segurado.Nome;
                        cpf = segurado.CPF;
                        telefone = segurado.Telefone;
                    }
                }

                if (string.IsNullOrWhiteSpace(nome))
                    return Json(new { success = false, message = "Nome é obrigatório." });

                var vistoria = new PropostasVistoriaEntity
                {
                    Id = Guid.NewGuid(),
                    PropostaId = model.PropostaId,
                    Nome = nome,
                    CPF = cpf ?? string.Empty,
                    Telefone = telefone,
                    PropostasSeguradoId = propostasSeguradoId
                };

                _context.PropostasVistoria.Add(vistoria);
                await _context.SaveChangesAsync();

                return Json(new { success = true, message = "Pessoa vinculada à vistoria com sucesso." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Erro ao vincular pessoa: " + ex.Message });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoverPropostaVistoria(Guid vistoriaId)
        {
            try
            {
                var vistoria = await _context.PropostasVistoria.FindAsync(vistoriaId);
                if (vistoria == null)
                    return Json(new { success = false, message = "Registro não encontrado." });

                _context.PropostasVistoria.Remove(vistoria);
                await _context.SaveChangesAsync();

                return Json(new { success = true, message = "Pessoa removida da vistoria." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Erro ao remover: " + ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> ListarCulturaVariedadePorGrupoId(Guid grupoId)
        {
            var list = await _context.CulturaMaturacaoVariedades
                .Where(v => v.CulturaMaturacaoId == grupoId && v.Ativo && !v.Excluido)
                .OrderBy(v => v.Nome)
                .Select(v => new { id = v.Id, text = v.Nome })
                .ToListAsync();
            return Ok(list);
        }
    }
}
