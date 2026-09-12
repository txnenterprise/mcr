using Microsoft.AspNetCore.Mvc;
using MCR.API.Produto.Domain.DTO;
using MCR.API.Services.Interfaces;

namespace MCR.API.Controllers
{
    public class ProdutosCanalController : Controller
    {
        private readonly IProdutosCanalService _service;

        public ProdutosCanalController(IProdutosCanalService service)
        {
            _service = service;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Adicionar(ProdutosCanalRegisterDTO model)
        {
            try
            {
                if (!model.CanalId.HasValue || model.CanalId.Value == Guid.Empty)
                    return Json(new { success = false, message = "Selecione um Canal." });

                var algumSucesso = false;
                var msgs = new List<string>();

                if (model.PontoAtendimentoSelected != null && model.PontoAtendimentoSelected.Any())
                {
                    foreach (var paId in model.PontoAtendimentoSelected)
                    {
                        var result = await _service.AdicionarAsync(model.ProdutoId, paId);
                        if (result) algumSucesso = true;
                        else msgs.Add("Erro ao vincular PA.");
                    }
                }
                else
                {
                    var result = await _service.AdicionarAsync(model.ProdutoId, model.CanalId.Value);
                    if (result) algumSucesso = true;
                    else msgs.Add("Erro ao vincular.");
                }

                var mensagem = algumSucesso ? "Vinculado com sucesso!" : string.Join(" ", msgs);
                return Json(new { success = algumSucesso, message = mensagem });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Erro ao adicionar: " + ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> ProdutosCanalList(Guid produtoId)
        {
            return ViewComponent("ProdutosCanalList", new { produtoId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Excluir(Guid pontoAtendimentoId)
        {
            try
            {
                var result = await _service.ExcluirAsync(pontoAtendimentoId);
                return Json(new { success = result, message = result ? "Vínculo excluído com sucesso!" : "Erro ao excluir." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Erro ao excluir: " + ex.Message });
            }
        }
    }
}
