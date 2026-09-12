using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MCR.API.Entities;
using MCR.API.Models;
using ViewModels = MCR.API.Models.ViewModels;
using MCR.API.Repository;
using MCR.API.Services.Interfaces;

namespace MCR.API.Controllers
{
    [Authorize]
    public class ClienteController : Controller
    {
        private readonly IClienteService _clienteService;
        private readonly IPropriedadeService _propriedadeService;
        private readonly ITalhaoService _talhaoService;
        private readonly IPropostasService _propostasService;
        private readonly IBancoService _bancoService;
        private readonly DbContextMCR _context;

        public ClienteController(IClienteService clienteService, IPropriedadeService propriedadeService, ITalhaoService talhaoService, IPropostasService propostasService, IBancoService bancoService, DbContextMCR context)
        {
            _clienteService = clienteService;
            _propriedadeService = propriedadeService;
            _talhaoService = talhaoService;
            _propostasService = propostasService;
            _bancoService = bancoService;
            _context = context;
        }

        public async Task<IActionResult> Index(string pesquisaNome, string pesquisaCPF, string pesquisaAtivoInativo, int page = 1, int pageSize = 10)
        {
            bool? ativo = pesquisaAtivoInativo switch
            {
                "Sim" => true,
                "Não" => false,
                _ => null
            };

            var retorno = (await _clienteService.ObterTodosPaginadoAsync(pesquisaNome, pesquisaCPF, null, null, ativo, page, pageSize)).ToList();

            var model = new ViewModels.ClienteModel
            {
                PesquisaNome = pesquisaNome,
                PesquisaCPF = pesquisaCPF,
                PesquisaAtivoInativo = pesquisaAtivoInativo,
                PaginaAtual = page,
                TotalPaginas = retorno.FirstOrDefault()?.TotalPages ?? 0,
                ListaClientes = retorno
            };

            return View(model);
        }

        public async Task<IActionResult> Cadastrar()
        {
            var faixasRenda = await _clienteService.ObterFaixasRendaAsync();
            ViewBag.FaixasRenda = faixasRenda.Select(a => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
            {
                Text = a.Descricao,
                Value = a.Descricao
            }).ToList();

            var bancos = _bancoService.ObterTodos();
            ViewBag.Bancos = bancos.OrderBy(b => b.Nome).Select(a => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
            {
                Text = a.Nome,
                Value = a.Nome
            }).ToList();

            var model = new ViewModels.ClienteModel();
            model.Cliente.Sucesso = true;
            return View(model);
        }

        [HttpPost]
        [RequestSizeLimit(52428800)]
        public async Task<IActionResult> Cadastrar(ViewModels.ClienteModel model)
        {
            try
            {
                if (model?.Cliente == null)
                    return Json(new { success = false, message = "Dados inválidos." });

                model.Cliente.Ativo = true;
                model.Cliente.Excluido = false;
                model.Cliente.ImagemCPF = model.ObjectImagemCPF ?? "";
                model.Cliente.ImagemRG = model.ObjectImagemRG ?? "";

                // Processar vínculos familiares
                if (!string.IsNullOrEmpty(model.JsonVinculosFamiliar))
                {
                    var vinculos = System.Text.Json.JsonSerializer.Deserialize<List<VinculoFamiliarEntity>>(model.JsonVinculosFamiliar);
                    model.Cliente.VinculosFamiliares = vinculos ?? new List<VinculoFamiliarEntity>();
                }

                // Processar propriedades selecionadas (vínculos)
                if (!string.IsNullOrEmpty(model.selectedItems))
                {
                    var propriedadesSelecionadas = System.Text.Json.JsonSerializer.Deserialize<List<JsonPropriedade>>(model.selectedItems);
                    foreach (var prop in propriedadesSelecionadas)
                    {
                        var consultaPropriedade = await _propriedadeService.ObterPorIdAsync(new Guid(prop.id));
                        if (consultaPropriedade != null)
                            model.Cliente.Propriedades.Add(consultaPropriedade);
                    }
                }

                var cadastro = await _clienteService.CadastrarAsync(model.Cliente);
                if (cadastro.Sucesso)
                    return Json(new { success = true, message = "Cliente cadastrado com sucesso!", clienteId = cadastro.Id.ToString() });

                return Json(new { success = false, message = cadastro.Mensagem ?? "Erro ao cadastrar cliente." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Erro ao cadastrar cliente: " + ex.Message });
            }
        }

        public async Task<IActionResult> Gerenciar(Guid id)
        {
            var cliente = await _clienteService.ObterPorIdAsync(id);
            if (cliente == null)
                return RedirectToAction(nameof(Index));

            var faixasRenda = await _clienteService.ObterFaixasRendaAsync();
            ViewBag.FaixasRenda = faixasRenda.Select(a => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
            {
                Text = a.Descricao,
                Value = a.Descricao
            }).ToList();

            var bancos = _bancoService.ObterTodos();
            ViewBag.Bancos = bancos.OrderBy(b => b.Nome).Select(a => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
            {
                Text = a.Nome,
                Value = a.Nome
            }).ToList();

            var vinculosFamiliarJson = "";
            if (cliente.VinculosFamiliares != null && cliente.VinculosFamiliares.Any())
            {
                vinculosFamiliarJson = System.Text.Json.JsonSerializer.Serialize(cliente.VinculosFamiliares);
            }

            var selectedItemsJson = "";
            if (cliente.Propriedades != null && cliente.Propriedades.Any())
            {
                selectedItemsJson = System.Text.Json.JsonSerializer.Serialize(cliente.Propriedades.Select(p => new { id = p.Id.ToString(), nome = p.Nome, endereco = p.Endereco, cidade = p.Cidade, estado = p.Estado, areaTotal = p.SomaAreaTotalTalhao }));
            }

            var model = new ViewModels.ClienteModel
            {
                Cliente = cliente,
                JsonVinculosFamiliar = vinculosFamiliarJson,
                PropriedadeModel = new PropriedadeModel
                {
                    ListaPropriedades = cliente.Propriedades,
                    SelectedItemsJson = selectedItemsJson
                }
            };

            return View(model);
        }

        [HttpPost]
        [RequestSizeLimit(52428800)]
        public async Task<IActionResult> Gerenciar(MCR.API.Models.ClienteModel model)
        {
            try
            {
                if (model.Cliente == null)
                    return Json(new { success = false, message = "Dados do cliente não informados." });

                if (model.Cliente.Propriedades == null)
                    model.Cliente.Propriedades = new List<PropriedadeEntity>();

                if (!string.IsNullOrEmpty(model.JsonVinculosFamiliar))
                {
                    var vinculos = System.Text.Json.JsonSerializer.Deserialize<List<MCR.API.Entities.VinculoFamiliarEntity>>(model.JsonVinculosFamiliar);
                    model.Cliente.VinculosFamiliares = vinculos ?? new List<MCR.API.Entities.VinculoFamiliarEntity>();
                }

                if (!string.IsNullOrEmpty(model.selectedItems))
                {
                    var propriedadesSelecionadas = System.Text.Json.JsonSerializer.Deserialize<List<JsonPropriedade>>(model.selectedItems);
                    foreach (var prop in propriedadesSelecionadas)
                    {
                        var consultaPropriedade = await _propriedadeService.ObterPorIdAsync(new Guid(prop.id));
                        if (consultaPropriedade != null)
                            model.Cliente.Propriedades.Add(consultaPropriedade);
                    }
                }

                if (!string.IsNullOrEmpty(model.ObjectImagemCPF))
                    model.Cliente.ImagemCPF = model.ObjectImagemCPF;
                else
                    model.Cliente.ImagemCPF = await ObterImagemExistenteAsync(model.Cliente.Id, "CPF");

                if (!string.IsNullOrEmpty(model.ObjectImagemRG))
                    model.Cliente.ImagemRG = model.ObjectImagemRG;
                else
                    model.Cliente.ImagemRG = await ObterImagemExistenteAsync(model.Cliente.Id, "RG");

                var resultado = await _clienteService.AtualizarAsync(model.Cliente);

                if (resultado?.Sucesso == true)
                    return Json(new { success = true, message = resultado.Mensagem });

                return Json(new { success = false, message = resultado?.Mensagem ?? "Erro ao atualizar cliente." });
            }
            catch (Microsoft.EntityFrameworkCore.DbUpdateException)
            {
                return Json(new { success = false, message = "Erro ao salvar: verifique se todos os campos obrigatórios foram preenchidos corretamente." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Erro ao salvar: " + ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> CadastrarPropriedade([FromForm] PropriedadeEntity propriedade)
        {
            try
            {
                var request = HttpContext.Request;
                if (request.Form.Files.Count > 0)
                {
                    var file = request.Form.Files[0];
                    if (file.Length > 0)
                    {
                        using var memoryStream = new MemoryStream();
                        await file.CopyToAsync(memoryStream);
                        var bytes = memoryStream.ToArray();
                        var base64 = Convert.ToBase64String(bytes);
                        var contentType = file.ContentType?.ToLowerInvariant() ?? "";
                        var fileName = file.FileName?.ToLowerInvariant() ?? "";
                        if (contentType.Contains("pdf") || fileName.EndsWith(".pdf"))
                            propriedade.ImagemGeralTodosTalhoes = "data:application/pdf;base64," + base64;
                        else
                            propriedade.ImagemGeralTodosTalhoes = base64;
                    }
                }
                else if (!string.IsNullOrEmpty(request.Form["ImagemGeralTodosTalhoes"].ToString()))
                {
                    var img = request.Form["ImagemGeralTodosTalhoes"].ToString();
                    if (img.StartsWith("data:image/"))
                        propriedade.ImagemGeralTodosTalhoes = img.Substring(img.IndexOf(",") + 1);
                    else if (img.StartsWith("data:application/pdf;base64,"))
                        propriedade.ImagemGeralTodosTalhoes = img;
                    else
                        propriedade.ImagemGeralTodosTalhoes = img;
                }

                propriedade.Ativo = true;
                propriedade.Excluido = false;
                propriedade.ImagemGeralTodosTalhoes ??= "";
                propriedade.MatriculaLote ??= "";
                propriedade.CadastroAmbientalRural ??= "";

                var cadastro = await _propriedadeService.CadastrarAsync(propriedade);

                if (cadastro.Sucesso)
                {
                    return Json(new
                    {
                        success = true,
                        message = "Propriedade cadastrada com sucesso!",
                        propriedadeId = cadastro.Id.ToString()
                    });
                }

                return Json(new
                {
                    success = false,
                    message = cadastro.Mensagem ?? "Erro ao cadastrar propriedade."
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = "Erro ao cadastrar propriedade: " + ex.Message
                });
            }
        }

        [HttpPost]
        public async Task<IActionResult> AtualizarPropriedade([FromForm] PropriedadeEntity propriedade)
        {
            try
            {
                var request = HttpContext.Request;
                if (request.Form.Files.Count > 0)
                {
                    var file = request.Form.Files[0];
                    if (file.Length > 0)
                    {
                        using var memoryStream = new MemoryStream();
                        await file.CopyToAsync(memoryStream);
                        var bytes = memoryStream.ToArray();
                        var base64 = Convert.ToBase64String(bytes);
                        var contentType = file.ContentType?.ToLowerInvariant() ?? "";
                        var fileName = file.FileName?.ToLowerInvariant() ?? "";
                        if (contentType.Contains("pdf") || fileName.EndsWith(".pdf"))
                            propriedade.ImagemGeralTodosTalhoes = "data:application/pdf;base64," + base64;
                        else
                            propriedade.ImagemGeralTodosTalhoes = base64;
                    }
                }
                else
                {
                    var imagemBase64 = request.Form["ImagemGeralTodosTalhoes"].ToString();
                    if (!string.IsNullOrEmpty(imagemBase64))
                    {
                        if (imagemBase64.StartsWith("data:image/"))
                        {
                            propriedade.ImagemGeralTodosTalhoes = imagemBase64.Substring(imagemBase64.IndexOf(",") + 1);
                        }
                        else
                        {
                            propriedade.ImagemGeralTodosTalhoes = imagemBase64;
                        }
                    }
                }

                var atualizacao = await _propriedadeService.AtualizarAsync(propriedade);

                if (atualizacao != null)
                    return Json(new { success = true, message = "Propriedade atualizada com sucesso!" });

                return Json(new { success = false, message = "Erro ao atualizar propriedade." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Erro interno: " + ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> PesquisarPropriedades(string pesquisaNome, string pesquisaEstado, string pesquisaCidade, string pesquisaAtivoInativo)
        {
            var retorno = await _propriedadeService.PesquisarPropriedadesDisponiveisIndex(pesquisaNome, pesquisaEstado, pesquisaCidade, pesquisaAtivoInativo);
            return Json(new { listaPropriedades = retorno });
        }

        [HttpGet]
        public async Task<IActionResult> ObterPropriedadeParaGerenciar(Guid id)
        {
            try
            {
                var propriedade = await _propriedadeService.ObterPorIdAsync(id);

                if (propriedade != null && propriedade.Sucesso)
                {
                    return Json(new
                    {
                        success = true,
                        propriedade = new
                        {
                            id = propriedade.Id.ToString(),
                            nome = propriedade.Nome,
                            somaAreaTotalTalhao = propriedade.SomaAreaTotalTalhao,
                            matriculaLote = propriedade.MatriculaLote,
                            cadastroAmbientalRural = propriedade.CadastroAmbientalRural,
                            cep = propriedade.CEP,
                            endereco = propriedade.Endereco,
                            bairro = propriedade.Bairro,
                            numero = propriedade.Numero,
                            cidade = propriedade.Cidade,
                            estado = propriedade.Estado,
                            imagemGeralTodosTalhoes = propriedade.ImagemGeralTodosTalhoes,
                            ativo = propriedade.Ativo,
                            excluido = propriedade.Excluido
                        }
                    });
                }

                return Json(new { success = false, message = "Propriedade não encontrada." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Erro ao carregar dados da propriedade: " + ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> ObterTalhoesPropriedade(Guid id)
        {
            try
            {
                var talhoes = await _talhaoService.ObterPorPropriedadeAsync(id) ?? Enumerable.Empty<TalhaoEntity>();
                var lista = talhoes.Select(t => new
                {
                    t.Id,
                    t.Nome,
                    t.Area,
                    t.TipoSolo,
                    t.ClassificacaoSolo,
                    t.PossuiAnaliseFisicaSolo,
                    t.PercentualAreia,
                    t.PercentualSilte,
                    t.PercentualArgila,
                    t.Latitude,
                    t.Longitude,
                    t.RoteiroAcesso,
                    t.Ativo,
                    t.ImagemTalhao
                }).ToList();
                return Json(new { success = true, talhoes = lista });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> CadastrarTalhao([FromForm] TalhaoEntity talhao)
        {
            try
            {
                talhao.Ativo = true;
                talhao.Excluido = false;
                var cadastro = await _talhaoService.CadastrarAsync(talhao);
                if (cadastro.Sucesso)
                {
                    return Json(new { success = true, message = "Talhão cadastrado com sucesso!", talhaoId = cadastro.Id.ToString() });
                }
                return Json(new { success = false, message = cadastro.Mensagem ?? "Erro ao cadastrar talhão." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Erro ao cadastrar talhão: " + ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> ObterClienteByCpf(string cpf)
        {
            return Json(await _clienteService.ObterPorCpfAsync(cpf));
        }

        [HttpGet]
        public async Task<IActionResult> ObterClienteByNome(string nome)
        {
            var clientes = await _clienteService.PesquisarPorNomeAsync(nome);
            return Json(clientes);
        }

        [HttpGet]
        public async Task<IActionResult> PesquisarClientes(string pesquisaNome, string pesquisaCPF, int page = 1, int pageSize = 10)
        {
            try
            {
                var retorno = await _clienteService.ObterTodosPaginadoAsync(pesquisaNome, pesquisaCPF, null, null, null, page, pageSize);
                return Json(new { listaClientes = retorno });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Erro ao buscar clientes: " + ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> VincularClienteACotacao(Guid clienteId, Guid cotacaoId)
        {
            try
            {
                var cotacaoProposta = await _context.CotacoesAgricolaProposta
                    .Include(x => x.CotacoesAgricola)
                    .Include(x => x.Produto)
                    .Include(x => x.Seguradora)
                    .FirstOrDefaultAsync(x => x.Id == cotacaoId && !x.Excluido);
                if (cotacaoProposta == null)
                    return Json(new { success = false, message = "Proposta de cotação não encontrada (ID: " + cotacaoId + ")." });

                var cotacaoAgricola = cotacaoProposta.CotacoesAgricola;
                if (cotacaoAgricola == null)
                    return Json(new { success = false, message = "Cotação agrícola não encontrada. Verifique se a cotação possui dados válidos." });

                if (cotacaoProposta.SeguradoraId == Guid.Empty || cotacaoProposta.Seguradora == null)
                    return Json(new { success = false, message = "Esta proposta de cotação não possui seguradora vinculada. Verifique a cotação antes de gerar a proposta." });

                if (cotacaoProposta.ProdutoId == Guid.Empty || cotacaoProposta.Produto == null)
                    return Json(new { success = false, message = "Esta proposta de cotação não possui produto vinculado. Verifique a cotação antes de gerar a proposta." });

                if (cotacaoProposta.LMIProducaoTotal <= 0 && cotacaoProposta.LMIReplantioTotal <= 0)
                    return Json(new { success = false, message = "Os valores de LMI da cotação estão zerados. Verifique os dados da proposta na cotação." });

                cotacaoAgricola.ClienteId = clienteId;
                await _context.SaveChangesAsync();

                var proposta = new PropostasEntity
                {
                    Id = Guid.NewGuid(),
                    CotacaoAgricolaId = cotacaoAgricola.Id,
                    ClienteId = clienteId,
                    CulturaId = cotacaoAgricola.CulturaId,
                    SafraId = cotacaoAgricola.SafraId,
                    Estado = cotacaoAgricola.Estado ?? "",
                    Municipio = cotacaoAgricola.Municipio ?? "",
                    AreaTotal = cotacaoAgricola.AreaTotal,
                    IsModalidadeProdutividade = cotacaoAgricola.IsModalidadeProdutividade,
                    PrecoSaca = cotacaoAgricola.PrecoSaca,
                    ValorCusteio = cotacaoAgricola.ValorCusteio,
                    PlantioConsorciado = cotacaoAgricola.PlantioConsorciado,
                    LavouraIrrigada = cotacaoAgricola.LavouraIrrigada,
                    PlantioDireto = cotacaoAgricola.PlantioDireto,
                    PosCana = cotacaoAgricola.PosCana,
                    CustoProducao = cotacaoAgricola.CustoProducao,
                    SubvencaoFederal = cotacaoAgricola.SubvencaoFederal,
                    SubvencaoEstadual = cotacaoAgricola.SubvencaoEstadual,
                    CorretoraId = cotacaoAgricola.CorretoraId,
                    CanalId = cotacaoAgricola.CanalId,
                    PontoAtendimentoId = cotacaoAgricola.PontoAtendimentoId,
                    CodigoCotacao = cotacaoAgricola.CodigoCotacao,
                    DataCotacao = DateTime.UtcNow,
                    Status = "Proposta em negociação",
                    Ativo = true,
                    Excluido = false,
                    UsuarioId = cotacaoAgricola.UsuarioId
                };

                proposta.PropostasFormaPagamentos.Add(new PropostasFormaPagamentosEntity
                {
                    PropostaId = proposta.Id,
                    Id = Guid.NewGuid(),
                    ProdutoId = cotacaoProposta.ProdutoId,
                    FormaDePagamento = cotacaoProposta.Produto?.FormaDePagamento,
                    Parcelamento = cotacaoProposta.Produto?.Parcelamento,
                });

                var produtoProposta = new PropostasProdutosEntity
                {
                    Id = Guid.NewGuid(),
                    PropostaId = proposta.Id,
                    CotacoesAgricolaPropostaId = cotacaoProposta.Id,
                    Opcao = cotacaoProposta.Opcao,
                    SeguradoraId = cotacaoProposta.SeguradoraId,
                    ProdutoId = cotacaoProposta.ProdutoId,
                    RegulacaoSinistro = cotacaoProposta.RegulacaoSinistro ?? "",
                    ProdutividadeEsperada = cotacaoProposta.ProdutividadeEsperada,
                    NivelCobertura = cotacaoProposta.NivelCobertura,
                    ProdutividadeSegurada = cotacaoProposta.ProdutividadeSegurada,
                    LMIProducaoHectare = cotacaoProposta.LMIProducaoHectare,
                    LMIReplantioHectare = cotacaoProposta.LMIReplantioHectare,
                    LMIProducaoTotal = cotacaoProposta.LMIProducaoTotal,
                    LMIReplantioTotal = cotacaoProposta.LMIReplantioTotal,
                    PremioTotal = cotacaoProposta.PremioTotal,
                    SubvencaoFederal = cotacaoProposta.SubvencaoFederal,
                    SubvencaoEstadual = cotacaoProposta.SubvencaoEstadual,
                    ParcelaSegurado = cotacaoProposta.ParcelaSegurado,
                    CustoHectare = cotacaoProposta.CustoHectare,
                    CustoScHectare = cotacaoProposta.CustoScHectare,
                    CustoScTotal = cotacaoProposta.CustoScTotal
                };

                var statusProposta = new PropostasStatusEntity
                {
                    Id = Guid.NewGuid(),
                    PropostaId = proposta.Id,
                    Status = "Proposta em negociação",
                    DataStatus = DateTime.UtcNow,
                };

                _context.Propostas.Add(proposta);
                _context.PropostasProdutos.Add(produtoProposta);
                _context.PropostasStatus.Add(statusProposta);
                await _context.SaveChangesAsync();

                var url = $"/Propostas/Cadastrar/{proposta.Id}";
                return Json(new { success = true, message = "Proposta gerada com sucesso!", url });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Erro: " + ex.GetType().Name + " - " + ex.Message + " | Stack: " + (ex.StackTrace ?? "").Substring(0, Math.Min(300, (ex.StackTrace ?? "").Length)) });
            }
        }

        private async Task<string?> ObterImagemExistenteAsync(Guid clienteId, string tipo)
        {
            try
            {
                var cliente = await _context.Clientes.AsNoTracking()
                    .FirstOrDefaultAsync(c => c.Id == clienteId);
                return tipo == "CPF" ? cliente?.ImagemCPF : cliente?.ImagemRG;
            }
            catch
            {
                return null;
            }
        }

        [HttpGet("Cliente/DownloadDocumento/{id}/{tipo}")]
        public async Task<IActionResult> DownloadDocumento(Guid id, string tipo)
        {
            try
            {
                var cliente = await _clienteService.ObterPorIdAsync(id);
                if (cliente == null || !cliente.Sucesso)
                    return NotFound();

                var dataUrl = tipo == "CPF" ? cliente.ImagemCPF : cliente.ImagemRG;
                if (string.IsNullOrEmpty(dataUrl))
                    return NotFound();

                var parts = dataUrl.Split(new[] { ',' }, 2);
                if (parts.Length != 2)
                    return BadRequest();

                var meta = parts[0];
                var base64 = parts[1];
                var bytes = Convert.FromBase64String(base64);

                var mimeType = "application/octet-stream";
                var extension = ".bin";
                if (meta.Contains("application/pdf")) { mimeType = "application/pdf"; extension = ".pdf"; }
                else if (meta.Contains("image/png")) { mimeType = "image/png"; extension = ".png"; }
                else if (meta.Contains("image/jpeg") || meta.Contains("image/jpg")) { mimeType = "image/jpeg"; extension = ".jpg"; }
                else if (meta.Contains("image/gif")) { mimeType = "image/gif"; extension = ".gif"; }
                else if (meta.Contains("image/webp")) { mimeType = "image/webp"; extension = ".webp"; }

                var fileName = $"documento_{tipo}{extension}";
                return File(bytes, mimeType, fileName);
            }
            catch
            {
                return BadRequest();
            }
        }
    }
}
