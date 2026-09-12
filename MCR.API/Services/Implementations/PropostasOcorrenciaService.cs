using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using MCR.API.Entities;
using MCR.API.Repository;
using MCR.API.Services.Interfaces;
using MCR.API.Shared.Extensions;

namespace MCR.API.Services.Implementations;

public class PropostasOcorrenciaService : IPropostasOcorrenciaService
{
    private readonly DbContextMCR _context;
    private readonly IHttpContextAccessor _contextAccessor;

    public PropostasOcorrenciaService(DbContextMCR context, IHttpContextAccessor contextAccessor)
    {
        _context = context;
        _contextAccessor = contextAccessor;
    }

    public async Task<IEnumerable<PropostasOcorrenciaEntity>> ObterPorPropostaAsync(Guid propostaId)
    {
        return await _context.PropostasOcorrencias
            .Include(x => x.Usuario)
            .Where(x => x.PropostaId == propostaId)
            .OrderByDescending(x => x.DataCriacao)
            .ToListAsync();
    }

    public async Task<bool> SalvarOcorrenciaAsync(Guid propostaId, string descricao, Guid usuarioId, byte[]? anexo = null, string? nomeAnexo = null)
    {
        try
        {
            var ocorrencia = new PropostasOcorrenciaEntity
            {
                Id = Guid.NewGuid(),
                PropostaId = propostaId,
                UsuarioId = usuarioId,
                DataCriacao = DateTime.UtcNow,
                Descricao = descricao,
                CaminhoAnexo = anexo != null ? Convert.ToBase64String(anexo) : null,
                NomeArquivo = nomeAnexo
            };

            await _context.PropostasOcorrencias.AddAsync(ocorrencia);
            await _context.SaveChangesAsync();
            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<(byte[] Conteudo, string Nome)> DownloadAnexoAsync(Guid ocorrenciaId)
    {
        var ocorrencia = await _context.PropostasOcorrencias
            .FirstOrDefaultAsync(x => x.Id == ocorrenciaId);

        if (ocorrencia == null || string.IsNullOrEmpty(ocorrencia.CaminhoAnexo))
            return (Array.Empty<byte>(), string.Empty);

        var conteudo = Convert.FromBase64String(ocorrencia.CaminhoAnexo);
        return (conteudo, ocorrencia.NomeArquivo ?? "anexo");
    }
}
