using Microsoft.EntityFrameworkCore;
using MCR.API.Entities;
using MCR.API.Repository;

namespace MCR.API.Services.Implementations
{
    public class EstruturaRiscoService
    {
        private readonly DbContextMCR _context;

        public EstruturaRiscoService(DbContextMCR context)
        {
            _context = context;
        }

        public async Task<IList<EstruturaRiscoDTO>> PesquisarIndex(
            string nome,
            string cpf,
            string ativo,
            int page = 1,
            int pageSize = 10)
        {
            var query = _context.Clientes
                .Where(c => !c.Excluido)
                .AsQueryable();

            if (!string.IsNullOrEmpty(nome))
                query = query.Where(c => c.Nome.Contains(nome));
            if (!string.IsNullOrEmpty(cpf))
                query = query.Where(c => c.CPF.Replace(".", "").Replace("-", "").Contains(cpf.Replace(".", "").Replace("-", "")));
            if (ativo == "Sim")
                query = query.Where(c => c.Ativo);
            else if (ativo == "Não")
                query = query.Where(c => !c.Ativo);

            var totalItems = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            var results = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var listaEstruturaRisco = new List<EstruturaRiscoDTO>();

            foreach (var cliente in results)
            {
                var estruturaRisco = new EstruturaRiscoDTO();
                estruturaRisco.Cliente = cliente;
                estruturaRisco.Propriedades = new List<PropriedadeEntity>();

                var vinculosPropriedades = await _context.VinculosPropriedadesClientes
                    .Where(c => c.ClienteId == cliente.Id)
                    .ToListAsync();

                if (vinculosPropriedades != null && vinculosPropriedades.Count > 0)
                {
                    foreach (var vinculo in vinculosPropriedades)
                    {
                        var propriedade = await _context.Propriedades
                            .Where(c => c.Id == vinculo.PropriedadeId)
                            .FirstOrDefaultAsync();

                        if (propriedade != null)
                        {
                            estruturaRisco.Propriedades.Add(propriedade);
                            estruturaRisco.Talhoes = new List<TalhaoEntity>();

                            var talhoes = await _context.Talhoes
                                .Where(c => c.PropriedadeId == propriedade.Id)
                                .ToListAsync();

                            if (talhoes != null && talhoes.Count > 0)
                            {
                                foreach (var talhao in talhoes)
                                {
                                    estruturaRisco.Talhoes.Add(talhao);
                                }
                            }
                        }
                    }
                }

                estruturaRisco.Sucesso = true;
                estruturaRisco.TotalItems = totalItems;
                estruturaRisco.TotalPages = totalPages;
                listaEstruturaRisco.Add(estruturaRisco);
            }

            return listaEstruturaRisco;
        }
    }

    public class EstruturaRiscoDTO
    {
        public ClienteEntity Cliente { get; set; }
        public List<PropriedadeEntity> Propriedades { get; set; }
        public List<TalhaoEntity> Talhoes { get; set; }
        public bool Sucesso { get; set; }
        public int TotalItems { get; set; }
        public int TotalPages { get; set; }
    }
}
