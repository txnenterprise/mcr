using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace MCR.API.Shared.Extensions;

public static class HttpUserExtensions
{
    public static Guid? GetUserId(this IHttpContextAccessor httpContextAccessor)
    {
        var userIdClaim = httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier);
        return userIdClaim != null ? Guid.Parse(userIdClaim.Value) : null;
    }

    public static string GetRole(this IHttpContextAccessor httpContextAccessor)
    {
        var roleClaim = httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Role);
        return roleClaim?.Value ?? string.Empty;
    }

    public static bool IsCorretor(this IHttpContextAccessor httpContextAccessor)
    {
        var roleClaim = httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Role);
        return roleClaim != null && roleClaim.Value == "Corretor";
    }

    public static bool IsAdministrador(this IHttpContextAccessor httpContextAccessor)
    {
        var roleClaim = httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Role);
        return roleClaim != null && roleClaim.Value == "Administrador";
    }

    public static bool IsConsultor(this IHttpContextAccessor httpContextAccessor)
    {
        var roleClaim = httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Role);
        return roleClaim != null && roleClaim.Value == "Consultor";
    }

    public static bool IsAssistente(this IHttpContextAccessor httpContextAccessor)
    {
        var roleClaim = httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Role);
        return roleClaim != null && roleClaim.Value == "Assistente";
    }
}
