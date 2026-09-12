using System.Globalization;
using System.Text;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using MCR.API.Entities;
using MCR.API.Middleware;
using MCR.API.Repository;
using MCR.API.Services;
using MCR.API.Services.Implementations;
using MCR.API.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Kestrel - aumentar limite para aceitar PDFs grandes via base64
builder.WebHost.ConfigureKestrel(options =>
{
    options.Limits.MaxRequestBodySize = 52428800; // 50 MB
});

// Culture
var cultureInfo = new CultureInfo("pt-BR");
CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;

// Database
AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

builder.Services.AddDbContext<DbContextMCR>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("TXNEnterpriseInsuranceQuoteContext"));
    options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
}, ServiceLifetime.Scoped);

// Data Protection - persistir chaves entre restarts
builder.Services.AddDataProtection()
    // REVERTER-VPS: restaurar p/ ".PersistKeysToFileSystem(new DirectoryInfo(\"/root/.aspnet/DataProtection-Keys\"))"
    .PersistKeysToFileSystem(new DirectoryInfo(
        builder.Environment.IsDevelopment()
            ? Path.Combine(builder.Environment.ContentRootPath, "App_Data", "DataProtection-Keys")
            : "/root/.aspnet/DataProtection-Keys"))
    .SetApplicationName("MCR.API");

// Identity
builder.Services.AddIdentity<UsuarioEntity, IdentityRole<Guid>>()
    .AddEntityFrameworkStores<DbContextMCR>()
    .AddDefaultTokenProviders();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Login/Authentication";
    options.AccessDeniedPath = "/Login/Authentication";
});

// JWT
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var secretKey = Encoding.UTF8.GetBytes(jwtSettings["Secret"]!);

builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = "IdentityOrJwt";
})
.AddPolicyScheme("IdentityOrJwt", "Identity or JWT", options =>
{
    options.ForwardDefaultSelector = context =>
    {
        if (context.Request.Headers.ContainsKey("Authorization"))
            return JwtBearerDefaults.AuthenticationScheme;
        return IdentityConstants.ApplicationScheme;
    };
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = builder.Environment.IsProduction();
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(secretKey),
        ValidateIssuer = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidateAudience = true,
        ValidAudience = jwtSettings["Audience"],
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero
    };
});

builder.Services.AddAuthorization();
builder.Services.AddHttpContextAccessor();

// Controllers + Views + JSON (runtime compilation)
builder.Services.AddControllersWithViews()
    .AddRazorRuntimeCompilation()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
        options.JsonSerializerOptions.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
    });

// Swagger with JWT support
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "MCR.API", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header usando Bearer. Exemplo: \"Bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
            },
            Array.Empty<string>()
        }
    });
});

// CORS
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        var origins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>();
        if (origins is { Length: > 0 })
            policy.WithOrigins(origins).AllowAnyHeader().AllowAnyMethod();
        else
            policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
    });
});

// DI - Services
builder.Services.AddScoped<IDashboardService, DashboardService>();
builder.Services.AddScoped<ICotacaoService, CotacaoService>();
builder.Services.AddScoped<ICotacoesAgricolaService, CotacoesAgricolaService>();
builder.Services.AddScoped<IPropostasService, PropostasService>();
builder.Services.AddScoped<IPropostaSeguradoService, PropostaSeguradoService>();
builder.Services.AddScoped<IPropostaRiscoService, PropostaRiscoService>();
builder.Services.AddScoped<IPropostaTalhaoService, PropostaTalhaoService>();
builder.Services.AddScoped<IPropostasBeneficiariosService, PropostasBeneficiariosService>();
builder.Services.AddScoped<IPropostasDocumentosService, PropostasDocumentosService>();
builder.Services.AddScoped<IPropostasObservacoesService, PropostasObservacoesService>();
builder.Services.AddScoped<IPropostasOcorrenciaService, PropostasOcorrenciaService>();
builder.Services.AddScoped<IPropostasStatusService, PropostasStatusService>();
builder.Services.AddScoped<IPropostasQuestionarioService, PropostasQuestionarioService>();
builder.Services.AddScoped<IPropostasFormaPagamentosService, PropostasFormaPagamentosService>();
builder.Services.AddScoped<IClienteService, ClienteService>();
builder.Services.AddScoped<IPropriedadeService, PropriedadeService>();
builder.Services.AddScoped<ITalhaoService, TalhaoService>();
builder.Services.AddScoped<IProdutosService, ProdutosService>();
builder.Services.AddScoped<IProdutosTaxasService, ProdutosTaxasService>();
builder.Services.AddScoped<IProdutosCanalService, ProdutosCanalService>();
builder.Services.AddScoped<ISafraService, SafraService>();
builder.Services.AddScoped<ICulturaService, CulturaService>();
builder.Services.AddScoped<ISeguradoraService, SeguradoraService>();
builder.Services.AddScoped<ISubvencaoFederalService, SubvencaoFederalService>();
builder.Services.AddScoped<ISubvencaoEstadualService, SubvencaoEstadualService>();
builder.Services.AddScoped<ICorretoraService, CorretoraService>();
builder.Services.AddScoped<ICanalService, CanalService>();
builder.Services.AddScoped<IPontoAtendimentoService, PontoAtendimentoService>();
builder.Services.AddScoped<IBeneficiarioService, BeneficiarioService>();
builder.Services.AddScoped<IBancoService, BancoService>();
builder.Services.AddScoped<IUsuarioService, UsuarioService>();
builder.Services.AddScoped<IUsuarioEstruturaNegocioService, UsuarioEstruturaNegocioService>();
builder.Services.AddScoped<IEstruturaNegocioService, EstruturaNegocioService>();
builder.Services.AddScoped<IMonitoramentoService, MonitoramentoService>();
builder.Services.AddHttpClient<ISatelliteProvider, CbersStacProvider>();
builder.Services.AddHttpClient<IRasterReader, ImageSharpRasterReader>();
builder.Services.AddScoped<INdviCalculator, NdviCalculator>();
builder.Services.AddScoped<IColorMapGenerator, ColorMapGenerator>();
builder.Services.AddScoped<INdviProcessingService, NdviProcessingService>();

builder.Services.Configure<JwtSettings>(jwtSettings);

var app = builder.Build();

// Middleware pipeline
app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
});
app.UseExceptionMiddleware();
app.UseStaticFiles();
app.UseCors();
app.UseSwagger();
app.UseSwaggerUI();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Login}/{action=Index}/{id?}");

// Health check
app.MapGet("/health", () => Results.Ok(new { Status = "Healthy", Timestamp = DateTime.UtcNow }));

using (var scope = app.Services.CreateScope())
{
    var skipMigrations = Environment.GetEnvironmentVariable("SKIP_MIGRATIONS") == "true";
    var db = scope.ServiceProvider.GetRequiredService<DbContextMCR>();
    if (skipMigrations)
    {
        Console.WriteLine("SKIP_MIGRATIONS=true - Pulando migrations (banco ja restaurado via dump).");
    }
    else
    {
        Console.WriteLine("Applying database migrations...");
        db.Database.Migrate();
        Console.WriteLine("Migrations applied successfully.");
    }

    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole<Guid>>>();
    var roles = new[] { "Administrador", "Corretor", "Gestor do Canal", "Consultor", "Assistente" };
    foreach (var role in roles)
    {
        if (!await roleManager.RoleExistsAsync(role))
        {
            await roleManager.CreateAsync(new IdentityRole<Guid>(role));
            Console.WriteLine($"Role '{role}' created.");
        }
    }
}

app.Run();
