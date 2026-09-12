using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MCR.API.Propostas.Domain.DTO;
using MCR.API.Repository;

namespace MCR.API.Components.Propostas
{
    public class PropostasSeguradosListarViewComponent : ViewComponent
    {
        private readonly DbContextMCR _context;

        public PropostasSeguradosListarViewComponent(DbContextMCR context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync(string clienteId)
        {
            var resultado = new List<PropostasSeguradosDTO>();

            if (!Guid.TryParse(clienteId, out var clienteGuid))
                return View(resultado);

            var cliente = await _context.Clientes.FirstOrDefaultAsync(x => x.Id == clienteGuid);
            if (cliente == null) return View(resultado);

            resultado.Add(new PropostasSeguradosDTO
            {
                ClienteId = cliente.Id,
                Nome = cliente.Nome,
                CPF = cliente.CPF,
                Telefone = cliente.Telefone,
                Email = cliente.Email,
                RelacaoParental = "Proprio cliente"
            });

            var familiares = await _context.VinculosFamiliares
                .Where(v => v.ClienteId == clienteGuid)
                .ToListAsync();

            foreach (var familiar in familiares)
            {
                resultado.Add(new PropostasSeguradosDTO
                {
                    ClienteId = cliente.Id,
                    VinculoFamiliarId = familiar.Id,
                    Nome = familiar.Nome,
                    CPF = familiar.Cpf,
                    Telefone = familiar.Telefone,
                    Email = familiar.Email,
                    RelacaoParental = familiar.RelacaoParental
                });
            }

            return View(resultado);
        }
    }
}
