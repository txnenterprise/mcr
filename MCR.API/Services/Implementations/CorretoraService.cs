using Microsoft.EntityFrameworkCore;
using MCR.API.Entities;
using MCR.API.Repository;
using MCR.API.Services.Interfaces;

namespace MCR.API.Services.Implementations
{
    public class CorretoraService : ICorretoraService
    {
        private readonly DbContextMCR _context;

        public CorretoraService(DbContextMCR context)
        {
            _context = context;
        }

        public async Task<CorretoraEntity> ObterPorIdAsync(Guid id)
        {
            if (id == Guid.Empty)
                return new CorretoraEntity { Sucesso = false, Mensagem = "Id não foi informado." };

            var retorno = await _context.CorretoraEntity.Where(c => c.Id == id).FirstOrDefaultAsync();

            if (retorno == null)
                return new CorretoraEntity { Sucesso = false, Mensagem = "Nenhum cadastro localizado para esse Id." };

            retorno.Sucesso = true;

            return retorno;
        }

        public async Task<IEnumerable<CorretoraEntity>> ObterTodosAsync()
        {
            var retorno = await _context.CorretoraEntity.Include(c => c.Plano).ToListAsync();

            if (retorno == null)
                return new List<CorretoraEntity> { new CorretoraEntity { Sucesso = false, Mensagem = "Nenhum cadastro localizado." } };

            retorno.ForEach(c => c.Sucesso = true);

            return retorno;
        }

        public async Task<CorretoraEntity> CadastrarAsync(CorretoraEntity entity)
        {
            entity.EmAtraso = false;
            entity.DiaVencimento = 10;
            _context.CorretoraEntity.Add(entity);
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

        public async Task<CorretoraEntity> AtualizarAsync(CorretoraEntity entity)
        {
            var getEntity = await _context.CorretoraEntity.FindAsync(entity.Id);
            if (getEntity == null)
            {
                entity.Mensagem = "Não foi possível obter as informações para atualizar. getEntity is null.";
                entity.Sucesso = false;
                return entity;
            }

            // Plano é configurável uma única vez: depois de salvo, fica somente para visualização
            if (getEntity.PlanoId.HasValue)
            {
                if (entity.PlanoId.HasValue && entity.PlanoId != getEntity.PlanoId)
                {
                    entity.Mensagem = "O plano desta corretora já foi configurado e não pode ser alterado.";
                    entity.Sucesso = false;
                    return entity;
                }
                entity.PlanoId = getEntity.PlanoId; // select desabilitado não envia o valor — preserva
            }

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
    }
}
