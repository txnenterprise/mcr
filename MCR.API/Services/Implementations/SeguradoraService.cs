using Microsoft.EntityFrameworkCore;
using MCR.API.Entities;
using MCR.API.Repository;
using MCR.API.Services.Interfaces;

namespace MCR.API.Services.Implementations
{
    public class SeguradoraService : ISeguradoraService
    {
        private readonly DbContextMCR _context;

        public SeguradoraService(DbContextMCR context)
        {
            _context = context;
        }

        private SeguradoraEntity Validar(SeguradoraEntity entity)
        {
            if (entity == null)
            {
                return new SeguradoraEntity
                {
                    Mensagem = "Cadastro nulo ou vazio.",
                    Sucesso = false
                };
            }

            if (string.IsNullOrEmpty(entity.RazaoSocial))
            {
                entity.Mensagem = "Informe a Razão Social.";
                entity.Sucesso = false;
                return entity;
            }
            if (string.IsNullOrEmpty(entity.NomeFantasia))
            {
                entity.Mensagem = "Informe o Nome Fantasia.";
                entity.Sucesso = false;
                return entity;
            }
            if (string.IsNullOrEmpty(entity.Logo))
            {
                entity.Mensagem = "Informe o Logo.";
                entity.Sucesso = false;
                return entity;
            }

            entity.Mensagem = "Validação efetuada com sucesso!";
            entity.Sucesso = true;
            return entity;
        }

        public async Task<SeguradoraEntity> CadastrarAsync(SeguradoraEntity entity)
        {
            var validacoes = Validar(entity);
            if (!validacoes.Sucesso)
                return validacoes;

            entity.Ativo = true;

            _context.SeguradoraEntity.Add(entity);
            var retorno = await _context.SaveChangesAsync();

            if (retorno > 0)
            {
                entity.Mensagem = "Cadastro efetuado com sucesso!";
                entity.Sucesso = true;
                return entity;
            }
            else
            {
                entity.Mensagem = $"Não foi possível efetuar o cadastro. Retorno is {retorno}.";
                entity.Sucesso = false;
                return entity;
            }
        }

        public async Task<SeguradoraEntity> AtualizarAsync(SeguradoraEntity entity)
        {
            var validacoes = Validar(entity);
            if (!validacoes.Sucesso)
                return validacoes;

            var getEntity = await _context.SeguradoraEntity.FindAsync(entity.Id);
            if (getEntity == null)
            {
                entity.Mensagem = "Não foi possível obter as informações para atualizar. getEntity is null.";
                entity.Sucesso = false;
                return entity;
            }
            entity.Ativo = true;
            _context.Entry(entity).State = EntityState.Modified;
            var retorno = await _context.SaveChangesAsync();

            if (retorno > 0)
            {
                entity.Mensagem = "Atualização efetuada com sucesso!";
                entity.Sucesso = true;
                return entity;
            }
            else
            {
                entity.Mensagem = $"Não foi possível atualizar o cadastro. Retorno is {retorno}.";
                entity.Sucesso = false;
                return entity;
            }
        }

        public async Task<SeguradoraEntity> ObterPorIdAsync(Guid id)
        {
            if (id == Guid.Empty)
                return new SeguradoraEntity { Sucesso = false, Mensagem = "Id não foi informado." };

            var retorno = await _context.SeguradoraEntity.Where(c => c.Id == id).FirstOrDefaultAsync();

            if (retorno == null)
                return new SeguradoraEntity { Sucesso = false, Mensagem = "Nenhum cadastro localizado para esse Id." };

            retorno.Sucesso = true;

            return retorno;
        }

        public async Task<IEnumerable<SeguradoraEntity>> ObterTodosPaginadoAsync(string nome = null, bool? ativo = null, int page = 1, int pageSize = 20)
        {
            var query = _context.SeguradoraEntity.AsQueryable();

            if (!string.IsNullOrEmpty(nome))
                query = query.Where(c => c.RazaoSocial.Contains(nome) || c.NomeFantasia.Contains(nome));

            if (ativo.HasValue)
                query = query.Where(c => c.Ativo == ativo.Value);

            var totalItems = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            var resultados = await query
                .OrderBy(c => c.RazaoSocial)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return resultados;
        }

        public async Task<bool> InativarAsync(Guid id)
        {
            var entity = await _context.SeguradoraEntity.FindAsync(id);
            if (entity == null)
                return false;

            entity.Ativo = false;
            _context.Entry(entity).State = EntityState.Modified;
            var retorno = await _context.SaveChangesAsync();

            return retorno > 0;
        }

        public async Task<bool> AtivarAsync(Guid id)
        {
            var entity = await _context.SeguradoraEntity.FindAsync(id);
            if (entity == null)
                return false;

            entity.Ativo = true;
            _context.Entry(entity).State = EntityState.Modified;
            var retorno = await _context.SaveChangesAsync();

            return retorno > 0;
        }
    }
}
