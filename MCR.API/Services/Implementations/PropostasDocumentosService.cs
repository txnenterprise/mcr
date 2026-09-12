using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using MCR.API.Entities;
using MCR.API.Repository;
using MCR.API.Services.Interfaces;
using MCR.API.Shared.Extensions;

namespace MCR.API.Services.Implementations;

public class PropostasDocumentosService : IPropostasDocumentosService
{
    private readonly DbContextMCR _context;
    private readonly IHttpContextAccessor _contextAccessor;

    public PropostasDocumentosService(DbContextMCR context, IHttpContextAccessor contextAccessor)
    {
        _context = context;
        _contextAccessor = contextAccessor;
    }

    public async Task<IEnumerable<PropostasDocumentosEntity>> ListarPorPropostaAsync(Guid propostaId)
    {
        return await _context.PropostasDocumentos
            .Include(d => d.Usuario)
            .Where(d => d.PropostaId == propostaId)
            .OrderByDescending(d => d.DataUpload)
            .ToListAsync();
    }

    public async Task<PropostasDocumentosEntity> UploadDocumentoAsync(Guid propostaId, string nome, string contentType, byte[] conteudo, Guid usuarioId, string? statusProposta = null)
    {
        var documento = new PropostasDocumentosEntity
        {
            Id = Guid.NewGuid(),
            PropostaId = propostaId,
            UserId = usuarioId,
            DataUpload = DateTime.UtcNow,
            NomeArquivo = nome,
            UrlDocumento = Convert.ToBase64String(conteudo),
            TipoDocumento = contentType,
            StatusProposta = statusProposta
        };

        _context.PropostasDocumentos.Add(documento);
        await _context.SaveChangesAsync();

        return documento;
    }

    public async Task<bool> ExcluirDocumentoAsync(Guid id)
    {
        var documento = await _context.PropostasDocumentos
            .FirstOrDefaultAsync(d => d.Id == id);

        if (documento == null)
            return false;

        _context.PropostasDocumentos.Remove(documento);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<(byte[] Conteudo, string Nome, string ContentType)> DownloadDocumentoAsync(Guid id)
    {
        var documento = await _context.PropostasDocumentos
            .FirstOrDefaultAsync(d => d.Id == id);

        if (documento == null || string.IsNullOrEmpty(documento.UrlDocumento))
            return (Array.Empty<byte>(), string.Empty, string.Empty);

        var conteudo = Convert.FromBase64String(documento.UrlDocumento);
        return (conteudo, documento.NomeArquivo ?? "documento", documento.TipoDocumento ?? "application/octet-stream");
    }
}
