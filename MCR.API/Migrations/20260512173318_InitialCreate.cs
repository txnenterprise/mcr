using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace MCR.API.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:PostgresExtension:uuid-ossp", ",,");

            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "uuid_generate_v4()"),
                    Name = table.Column<string>(type: "text", nullable: false),
                    LoginProvider = table.Column<string>(type: "text", nullable: false),
                    Document = table.Column<string>(type: "text", nullable: false),
                    EmailConfirmed = table.Column<bool>(type: "boolean", nullable: false),
                    PhoneNumberConfirmed = table.Column<bool>(type: "boolean", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    LockoutEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "integer", nullable: false),
                    UserName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    PasswordHash = table.Column<string>(type: "text", nullable: true),
                    SecurityStamp = table.Column<string>(type: "text", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "text", nullable: true),
                    PhoneNumber = table.Column<string>(type: "text", nullable: true),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Bancos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CodigoInterno = table.Column<string>(type: "text", nullable: false),
                    Nome = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Bancos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Beneficiarios",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Nome = table.Column<string>(type: "text", nullable: false),
                    CNPJ = table.Column<string>(type: "text", nullable: false),
                    ImagemCNPJ = table.Column<string>(type: "text", nullable: false),
                    Banco = table.Column<string>(type: "text", nullable: false),
                    Agencia = table.Column<string>(type: "text", nullable: false),
                    Conta = table.Column<string>(type: "text", nullable: false),
                    ChavePIX = table.Column<string>(type: "text", nullable: false),
                    Ativo = table.Column<bool>(type: "boolean", nullable: false),
                    Excluido = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Beneficiarios", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Clientes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Nome = table.Column<string>(type: "text", nullable: false),
                    CPF = table.Column<string>(type: "text", nullable: false),
                    ImagemCPF = table.Column<string>(type: "text", nullable: true),
                    DataNascimento = table.Column<string>(type: "text", nullable: false),
                    RG = table.Column<string>(type: "text", nullable: false),
                    ImagemRG = table.Column<string>(type: "text", nullable: true),
                    DataExpedicaoRG = table.Column<string>(type: "text", nullable: false),
                    OrgaoExpeditorRG = table.Column<string>(type: "text", nullable: false),
                    EstadoCivil = table.Column<string>(type: "text", nullable: false),
                    Sexo = table.Column<string>(type: "text", nullable: false),
                    Telefone = table.Column<string>(type: "text", nullable: false),
                    Celular = table.Column<string>(type: "text", nullable: false),
                    Email = table.Column<string>(type: "text", nullable: false),
                    Profissao = table.Column<string>(type: "text", nullable: false),
                    FaixaRenda = table.Column<string>(type: "text", nullable: false),
                    Banco = table.Column<string>(type: "text", nullable: false),
                    Agencia = table.Column<string>(type: "text", nullable: false),
                    Conta = table.Column<string>(type: "text", nullable: false),
                    ChavePIX = table.Column<string>(type: "text", nullable: false),
                    Endereco = table.Column<string>(type: "text", nullable: false),
                    Bairro = table.Column<string>(type: "text", nullable: false),
                    Numero = table.Column<string>(type: "text", nullable: false),
                    Complemento = table.Column<string>(type: "text", nullable: false),
                    Cidade = table.Column<string>(type: "text", nullable: false),
                    Estado = table.Column<string>(type: "text", nullable: false),
                    CEP = table.Column<string>(type: "text", nullable: false),
                    Ativo = table.Column<bool>(type: "boolean", nullable: false),
                    Excluido = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Clientes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Corretoras",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    RazaoSocial = table.Column<string>(type: "text", nullable: false),
                    NomeFantasia = table.Column<string>(type: "text", nullable: false),
                    CNPJ = table.Column<string>(type: "text", nullable: false),
                    Endereco = table.Column<string>(type: "text", nullable: true),
                    Bairro = table.Column<string>(type: "text", nullable: true),
                    Numero = table.Column<string>(type: "text", nullable: true),
                    Cidade = table.Column<string>(type: "text", nullable: true),
                    Estado = table.Column<string>(type: "text", nullable: true),
                    CEP = table.Column<string>(type: "text", nullable: false),
                    Ativa = table.Column<bool>(type: "boolean", nullable: false),
                    EmAtraso = table.Column<bool>(type: "boolean", nullable: false),
                    PlanoContratado = table.Column<string>(type: "text", nullable: true),
                    ImagemLogo = table.Column<string>(type: "text", nullable: true),
                    DiaVencimento = table.Column<int>(type: "integer", nullable: false),
                    EmailSeguro = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    EmailCopiaSeguro = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    EmailSinistro = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    EmailCopiaSinistro = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Corretoras", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CotacaoCoberturas",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CotacaoId = table.Column<Guid>(type: "uuid", nullable: false),
                    BemId = table.Column<Guid>(type: "uuid", nullable: false),
                    AplicarCoberturaTotal = table.Column<bool>(type: "boolean", nullable: false),
                    ContratarResponsabilidadeCivilMaquinariaAgricola = table.Column<bool>(type: "boolean", nullable: false),
                    ValorResponsabilidadeCivilMaquinariaAgricola = table.Column<decimal>(type: "numeric", nullable: false),
                    ContratarResponsabilidadeCivilEmpregador = table.Column<bool>(type: "boolean", nullable: false),
                    ValorResponsabilidadeCivilEmpregador = table.Column<decimal>(type: "numeric", nullable: false),
                    ContratarCoberturaRelativaPerdaPagamentoAluguel = table.Column<bool>(type: "boolean", nullable: false),
                    ValorCoberturaRelativaPerdaPagamentoAluguel = table.Column<decimal>(type: "numeric", nullable: false),
                    ContratarFurtoSimples = table.Column<bool>(type: "boolean", nullable: false),
                    ValorFurtoSimples = table.Column<decimal>(type: "numeric", nullable: false),
                    ContratarDanosEletricos = table.Column<bool>(type: "boolean", nullable: false),
                    ValorDanosEletricos = table.Column<decimal>(type: "numeric", nullable: false),
                    ContratarQuebraVidros = table.Column<bool>(type: "boolean", nullable: false),
                    ValorQuebraVidros = table.Column<decimal>(type: "numeric", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CotacaoCoberturas", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CotacaoCondicoesComerciais",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CotacaoId = table.Column<Guid>(type: "uuid", nullable: false),
                    ComissaoSeguradoraMapfre = table.Column<decimal>(type: "numeric", nullable: false),
                    DescontoAgravoSeguradoraMapfre = table.Column<decimal>(type: "numeric", nullable: false),
                    MultiplicadorFranquiaSeguradoraMapfre = table.Column<decimal>(type: "numeric", nullable: false),
                    ComissaoSeguradoraSwissRe = table.Column<decimal>(type: "numeric", nullable: false),
                    DescontoAgravoSeguradoraSwissRe = table.Column<decimal>(type: "numeric", nullable: false),
                    MultiplicadorFranquiaSeguradoraSwissRe = table.Column<decimal>(type: "numeric", nullable: false),
                    ComissaoSeguradoraAllianz = table.Column<decimal>(type: "numeric", nullable: false),
                    DescontoAgravoSeguradoraAllianz = table.Column<decimal>(type: "numeric", nullable: false),
                    MultiplicadorFranquiaSeguradoraAllianz = table.Column<decimal>(type: "numeric", nullable: false),
                    ComissaoSeguradoraTokio = table.Column<decimal>(type: "numeric", nullable: false),
                    DescontoAgravoSeguradoraTokio = table.Column<decimal>(type: "numeric", nullable: false),
                    MultiplicadorFranquiaSeguradoraTokio = table.Column<decimal>(type: "numeric", nullable: false),
                    ComissaoSeguradoraSOMPO = table.Column<decimal>(type: "numeric", nullable: false),
                    DescontoAgravoSeguradoraSOMPO = table.Column<decimal>(type: "numeric", nullable: false),
                    MultiplicadorFranquiaSeguradoraSOMPO = table.Column<decimal>(type: "numeric", nullable: false),
                    ComissaoSeguradoraPottencial = table.Column<decimal>(type: "numeric", nullable: false),
                    DescontoAgravoSeguradoraPottencial = table.Column<decimal>(type: "numeric", nullable: false),
                    MultiplicadorFranquiaSeguradoraPottencial = table.Column<decimal>(type: "numeric", nullable: false),
                    ComissaoSeguradoraSombrero = table.Column<decimal>(type: "numeric", nullable: false),
                    DescontoAgravoSeguradoraSombrero = table.Column<decimal>(type: "numeric", nullable: false),
                    MultiplicadorFranquiaSeguradoraSombrero = table.Column<decimal>(type: "numeric", nullable: false),
                    ComissaoSeguradoraFF = table.Column<decimal>(type: "numeric", nullable: false),
                    DescontoAgravoSeguradoraFF = table.Column<decimal>(type: "numeric", nullable: false),
                    MultiplicadorFranquiaSeguradoraFF = table.Column<decimal>(type: "numeric", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CotacaoCondicoesComerciais", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CotacaoFormulariosRiscos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CotacaoId = table.Column<Guid>(type: "uuid", nullable: false),
                    BemId = table.Column<Guid>(type: "uuid", nullable: false),
                    EquipamentoAlugadoDuranteVigencia = table.Column<bool>(type: "boolean", nullable: false),
                    EquipamentoCedidoTerceirosDuranteVigencia = table.Column<bool>(type: "boolean", nullable: false),
                    EquipamentoAtividadesRurais = table.Column<bool>(type: "boolean", nullable: false),
                    EquipamentoAtividadeFlorestal = table.Column<bool>(type: "boolean", nullable: false),
                    EquipamentoOperaProximoAgua = table.Column<bool>(type: "boolean", nullable: false),
                    SeguradoColaboradorOperador = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CotacaoFormulariosRiscos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CotacaoInformacaoBeneficiarios",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CotacaoId = table.Column<Guid>(type: "uuid", nullable: false),
                    TipoPessoa = table.Column<string>(type: "text", nullable: false),
                    NomeBeneficiario = table.Column<string>(type: "text", nullable: false),
                    CPFCNPJBeneficiario = table.Column<string>(type: "text", nullable: false),
                    BancoBeneficiario = table.Column<string>(type: "text", nullable: true),
                    AgenciaBeneficiario = table.Column<string>(type: "text", nullable: true),
                    ContaBeneficiario = table.Column<string>(type: "text", nullable: true),
                    DigitoContaBeneficiario = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CotacaoInformacaoBeneficiarios", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CotacaoInformacaoBens",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CotacaoId = table.Column<Guid>(type: "uuid", nullable: false),
                    TipoEquipamento = table.Column<string>(type: "text", nullable: false),
                    AnoFabricacao = table.Column<int>(type: "integer", nullable: false),
                    ValorEquipamento = table.Column<decimal>(type: "numeric", nullable: false),
                    MarcaEquipamento = table.Column<string>(type: "text", nullable: false),
                    ModeloEquipamento = table.Column<string>(type: "text", nullable: false),
                    NumeroSerieEquipamento = table.Column<string>(type: "text", nullable: false),
                    NumeroChassiEquipamento = table.Column<string>(type: "text", nullable: false),
                    InformarNotaFiscal = table.Column<bool>(type: "boolean", nullable: false),
                    DataNotaFiscal = table.Column<DateTime>(type: "timestamp", nullable: false),
                    NumeroNotaFiscal = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CotacaoInformacaoBens", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CotacaoInformacaoSegurados",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CotacaoId = table.Column<Guid>(type: "uuid", nullable: false),
                    Nome = table.Column<string>(type: "text", nullable: false),
                    TipoPessoa = table.Column<string>(type: "text", nullable: false),
                    CPFCNPJ = table.Column<string>(type: "text", nullable: false),
                    CEP = table.Column<string>(type: "text", nullable: false),
                    Endereco = table.Column<string>(type: "text", nullable: false),
                    Bairro = table.Column<string>(type: "text", nullable: false),
                    Numero = table.Column<string>(type: "text", nullable: false),
                    Estado = table.Column<string>(type: "text", nullable: false),
                    Cidade = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CotacaoInformacaoSegurados", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CotacaoInformacaoSeguros",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CotacaoId = table.Column<Guid>(type: "uuid", nullable: false),
                    TempoVigenciaSeguro = table.Column<string>(type: "text", nullable: false),
                    TipoSeguro = table.Column<string>(type: "text", nullable: false),
                    ApoliceRenovacao = table.Column<string>(type: "text", nullable: true),
                    SeguradoraAnterior = table.Column<string>(type: "text", nullable: true),
                    BemFinanciado = table.Column<bool>(type: "boolean", nullable: false),
                    BancoBeneficiarioInformacaoSeguro = table.Column<string>(type: "text", nullable: true),
                    FormaPagamentoSeguro = table.Column<string>(type: "text", nullable: true),
                    QuantidadeParcelas = table.Column<int>(type: "integer", nullable: false),
                    PrazoSeguro = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CotacaoInformacaoSeguros", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CotacaoRetornoSeguradoras",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CotacaoId = table.Column<Guid>(type: "uuid", nullable: false),
                    NumeroCotacao = table.Column<string>(type: "text", nullable: true),
                    Seguradora = table.Column<string>(type: "text", nullable: false),
                    DataHoraRetorno = table.Column<DateTime>(type: "timestamp", nullable: false),
                    FormaPagamento = table.Column<string>(type: "text", nullable: true),
                    DiaPagamento = table.Column<string>(type: "text", nullable: true),
                    NumeroParcelas = table.Column<string>(type: "text", nullable: true),
                    Efetivada = table.Column<bool>(type: "boolean", nullable: false),
                    DataHoraEfetivacao = table.Column<DateTime>(type: "timestamp", nullable: false),
                    CorretorEfetivou = table.Column<string>(type: "text", nullable: true),
                    Premio = table.Column<string>(type: "text", nullable: true),
                    JsonRetornoPremioCobertura = table.Column<string>(type: "text", nullable: true),
                    Parcelamento = table.Column<string>(type: "text", nullable: true),
                    MensagemComplementar = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CotacaoRetornoSeguradoras", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Cotacoes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Corretor = table.Column<string>(type: "text", nullable: false),
                    DataHoraCotacao = table.Column<DateTime>(type: "timestamp", nullable: false),
                    Cancelado = table.Column<bool>(type: "boolean", nullable: false),
                    Efetivada = table.Column<bool>(type: "boolean", nullable: false),
                    DataHoraEfetivacao = table.Column<DateTime>(type: "timestamp", nullable: false),
                    Seguradora = table.Column<string>(type: "text", nullable: true),
                    JsonCotacao = table.Column<string>(type: "text", nullable: true),
                    CodigoCotacao = table.Column<string>(type: "text", nullable: true),
                    LinkAcessoCotacao = table.Column<string>(type: "text", nullable: true),
                    Premio = table.Column<decimal>(type: "numeric", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cotacoes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CotacoesCondicaoComercialSeguradoras",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CotacaoId = table.Column<Guid>(type: "uuid", nullable: false),
                    Seguradora = table.Column<string>(type: "text", nullable: false),
                    Comissao = table.Column<decimal>(type: "numeric", nullable: false),
                    DescontoAgravo = table.Column<decimal>(type: "numeric", nullable: false),
                    MultiplicadorFranquia = table.Column<decimal>(type: "numeric", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CotacoesCondicaoComercialSeguradoras", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CotacoesPublicacoesRetornoJob",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CotacaoId = table.Column<Guid>(type: "uuid", nullable: false),
                    JsonPublicacao = table.Column<string>(type: "text", nullable: false),
                    Publicado = table.Column<bool>(type: "boolean", nullable: false),
                    Fila = table.Column<string>(type: "text", nullable: false),
                    Seguradora = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CotacoesPublicacoesRetornoJob", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Culturas",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Nome = table.Column<string>(type: "text", nullable: false),
                    Ativo = table.Column<bool>(type: "boolean", nullable: false),
                    Excluido = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Culturas", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CulturasPrePlantio",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Ativo = table.Column<bool>(type: "boolean", nullable: false),
                    Excluido = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CulturasPrePlantio", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EmailServerSetting",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ServerAddress = table.Column<string>(type: "text", nullable: false),
                    ServerPort = table.Column<int>(type: "integer", nullable: false),
                    ServerUseSsl = table.Column<bool>(type: "boolean", nullable: false),
                    Username = table.Column<string>(type: "text", nullable: false),
                    Password = table.Column<string>(type: "text", nullable: false),
                    EnviromentToAction = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmailServerSetting", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Equipamentos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CodigoInterno = table.Column<string>(type: "text", nullable: false),
                    Nome = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Equipamentos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "IdentityUserLogin<string>",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "text", nullable: false),
                    ProviderKey = table.Column<string>(type: "text", nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "text", nullable: true),
                    UserId = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IdentityUserLogin<string>", x => new { x.LoginProvider, x.ProviderKey });
                });

            migrationBuilder.CreateTable(
                name: "LoginSeguradoras",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Seguradora = table.Column<string>(type: "text", nullable: false),
                    CorretorLogado = table.Column<string>(type: "text", nullable: false),
                    Link = table.Column<string>(type: "text", nullable: true),
                    Susep = table.Column<string>(type: "text", nullable: true),
                    CodigoInterno = table.Column<string>(type: "text", nullable: true),
                    Usuario = table.Column<string>(type: "text", nullable: true),
                    Senha = table.Column<string>(type: "text", nullable: true),
                    Token = table.Column<string>(type: "text", nullable: true),
                    ChaveAPIKey = table.Column<string>(type: "text", nullable: true),
                    ChaveAPIValue = table.Column<string>(type: "text", nullable: true),
                    CorretoraId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LoginSeguradoras", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Marcas",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CodigoInterno = table.Column<string>(type: "text", nullable: false),
                    Nome = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Marcas", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MicrosoftAD",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    RedirectAfterLogin = table.Column<string>(type: "text", nullable: false),
                    ClientId = table.Column<string>(type: "text", nullable: false),
                    ClientSecret = table.Column<string>(type: "text", nullable: false),
                    URLToken = table.Column<string>(type: "text", nullable: false),
                    URLMe = table.Column<string>(type: "text", nullable: false),
                    TenantId = table.Column<string>(type: "text", nullable: false),
                    Instance = table.Column<string>(type: "text", nullable: false),
                    Domain = table.Column<string>(type: "text", nullable: false),
                    SignedOutCallbackPath = table.Column<string>(type: "text", nullable: false),
                    ScopeForAccessToken = table.Column<string>(type: "text", nullable: false),
                    Authority = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MicrosoftAD", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Modelos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CodigoInterno = table.Column<string>(type: "text", nullable: false),
                    Nome = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Modelos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ParametrizacaoBeneficiario",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Banco = table.Column<string>(type: "text", nullable: false),
                    Agencia = table.Column<string>(type: "text", nullable: false),
                    Conta = table.Column<string>(type: "text", nullable: false),
                    DigitoConta = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ParametrizacaoBeneficiario", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ParametrizacaoCustomizacaoRelatorio",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Titulo1 = table.Column<string>(type: "text", nullable: false),
                    TamanhoTitulo1 = table.Column<int>(type: "integer", nullable: false),
                    CorTitulo1 = table.Column<string>(type: "text", nullable: false),
                    Texto1 = table.Column<string>(type: "text", nullable: false),
                    TamanhoTexto1 = table.Column<int>(type: "integer", nullable: false),
                    CorTexto1 = table.Column<string>(type: "text", nullable: false),
                    Titulo2 = table.Column<string>(type: "text", nullable: true),
                    TamanhoTitulo2 = table.Column<int>(type: "integer", nullable: false),
                    CorTitulo2 = table.Column<string>(type: "text", nullable: true),
                    Texto2 = table.Column<string>(type: "text", nullable: true),
                    TamanhoTexto2 = table.Column<int>(type: "integer", nullable: false),
                    CorTexto2 = table.Column<string>(type: "text", nullable: true),
                    Imagem1 = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ParametrizacaoCustomizacaoRelatorio", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ParametrizacaoMultiCalculoParceiro",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    HabilitarMapfre = table.Column<bool>(type: "boolean", nullable: false),
                    HabilitarPottencial = table.Column<bool>(type: "boolean", nullable: false),
                    HabilitarSompo = table.Column<bool>(type: "boolean", nullable: false),
                    HabilitarSwissRe = table.Column<bool>(type: "boolean", nullable: false),
                    HabilitarSombrero = table.Column<bool>(type: "boolean", nullable: false),
                    HabilitarFairFax = table.Column<bool>(type: "boolean", nullable: false),
                    HabilitarAllianz = table.Column<bool>(type: "boolean", nullable: false),
                    HabilitarTokio = table.Column<bool>(type: "boolean", nullable: false),
                    NomeParceiro = table.Column<string>(type: "text", nullable: false),
                    LogoParceiro = table.Column<string>(type: "text", nullable: false),
                    CorPredominante = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ParametrizacaoMultiCalculoParceiro", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ParametrizacaoRisco",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    EquipamentoOperaProximoAgua = table.Column<bool>(type: "boolean", nullable: false),
                    SeguradoColaboradorOperador = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ParametrizacaoRisco", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ParametrizacaoSeguradora",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ContratarCoberturaResponsabilidadeCivilParaMaquinaAgricola = table.Column<bool>(type: "boolean", nullable: false),
                    ValorCoberturaResponsabilidadeCivilParaMaquinaAgricola = table.Column<decimal>(type: "numeric", nullable: false),
                    ContratarCoberturaResponsabilidadeCivilParaEmpregador = table.Column<bool>(type: "boolean", nullable: false),
                    ValorCoberturaResponsabilidadeCivilParaEmpregador = table.Column<decimal>(type: "numeric", nullable: false),
                    ContratarCoberturaPerdaPagamentoAluguel = table.Column<bool>(type: "boolean", nullable: false),
                    ValorCoberturaPerdaPagamentoAluguel = table.Column<decimal>(type: "numeric", nullable: false),
                    ContratarCoberturaFurtoSimples = table.Column<bool>(type: "boolean", nullable: false),
                    ValorCoberturaFurtoSimples = table.Column<decimal>(type: "numeric", nullable: false),
                    ContratarCoberturaDanosEletricos = table.Column<bool>(type: "boolean", nullable: false),
                    ValorCoberturaDanosEletricos = table.Column<decimal>(type: "numeric", nullable: false),
                    ContratarCoberturaQuebraVidros = table.Column<bool>(type: "boolean", nullable: false),
                    ValorCoberturaQuebraVidros = table.Column<decimal>(type: "numeric", nullable: false),
                    NumeroParcelamento = table.Column<int>(type: "integer", nullable: false),
                    ComissaoSeguradoraMapfre = table.Column<decimal>(type: "numeric", nullable: false),
                    DescontoAgravoSeguradoraMapfre = table.Column<decimal>(type: "numeric", nullable: false),
                    MultiplicadorFranquiaSeguradoraMapfre = table.Column<decimal>(type: "numeric", nullable: false),
                    ComissaoSeguradoraSwissRe = table.Column<decimal>(type: "numeric", nullable: false),
                    DescontoAgravoSeguradoraSwissRe = table.Column<decimal>(type: "numeric", nullable: false),
                    MultiplicadorFranquiaSeguradoraSwissRe = table.Column<decimal>(type: "numeric", nullable: false),
                    ComissaoSeguradoraAllianz = table.Column<decimal>(type: "numeric", nullable: false),
                    DescontoAgravoSeguradoraAllianz = table.Column<decimal>(type: "numeric", nullable: false),
                    MultiplicadorFranquiaSeguradoraAllianz = table.Column<decimal>(type: "numeric", nullable: false),
                    ComissaoSeguradoraTokio = table.Column<decimal>(type: "numeric", nullable: false),
                    DescontoAgravoSeguradoraTokio = table.Column<decimal>(type: "numeric", nullable: false),
                    MultiplicadorFranquiaSeguradoraTokio = table.Column<decimal>(type: "numeric", nullable: false),
                    ComissaoSeguradoraSOMPO = table.Column<decimal>(type: "numeric", nullable: false),
                    DescontoAgravoSeguradoraSOMPO = table.Column<decimal>(type: "numeric", nullable: false),
                    MultiplicadorFranquiaSeguradoraSOMPO = table.Column<decimal>(type: "numeric", nullable: false),
                    ComissaoSeguradoraPottencial = table.Column<decimal>(type: "numeric", nullable: false),
                    DescontoAgravoSeguradoraPottencial = table.Column<decimal>(type: "numeric", nullable: false),
                    MultiplicadorFranquiaSeguradoraPottencial = table.Column<decimal>(type: "numeric", nullable: false),
                    ComissaoSeguradoraSombrero = table.Column<decimal>(type: "numeric", nullable: false),
                    DescontoAgravoSeguradoraSombrero = table.Column<decimal>(type: "numeric", nullable: false),
                    MultiplicadorFranquiaSeguradoraSombrero = table.Column<decimal>(type: "numeric", nullable: false),
                    ComissaoSeguradoraFF = table.Column<decimal>(type: "numeric", nullable: false),
                    DescontoAgravoSeguradoraFF = table.Column<decimal>(type: "numeric", nullable: false),
                    MultiplicadorFranquiaSeguradoraFF = table.Column<decimal>(type: "numeric", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ParametrizacaoSeguradora", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Propriedades",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Nome = table.Column<string>(type: "text", nullable: false),
                    Endereco = table.Column<string>(type: "text", nullable: false),
                    Bairro = table.Column<string>(type: "text", nullable: false),
                    Numero = table.Column<string>(type: "text", nullable: false),
                    Cidade = table.Column<string>(type: "text", nullable: false),
                    Estado = table.Column<string>(type: "text", nullable: false),
                    CEP = table.Column<string>(type: "text", nullable: false),
                    SomaAreaTotalTalhao = table.Column<decimal>(type: "numeric", nullable: false),
                    ImagemGeralTodosTalhoes = table.Column<string>(type: "text", nullable: false),
                    MatriculaLote = table.Column<string>(type: "text", nullable: false),
                    CadastroAmbientalRural = table.Column<string>(type: "text", nullable: false),
                    Ativo = table.Column<bool>(type: "boolean", nullable: false),
                    Excluido = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Propriedades", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Safras",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Descricao = table.Column<string>(type: "text", nullable: false),
                    AnoReferencia = table.Column<string>(type: "text", nullable: false),
                    Ativo = table.Column<bool>(type: "boolean", nullable: false),
                    Excluido = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Safras", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SeguradoraAnterior",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CodigoInterno = table.Column<string>(type: "text", nullable: false),
                    Nome = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SeguradoraAnterior", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Seguradoras",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    RazaoSocial = table.Column<string>(type: "text", nullable: false),
                    NomeFantasia = table.Column<string>(type: "text", nullable: false),
                    Logo = table.Column<string>(type: "text", nullable: false),
                    Ativo = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Seguradoras", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SubvencoesEstaduais",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Cultura = table.Column<string>(type: "text", nullable: false),
                    Estado = table.Column<string>(type: "text", nullable: false),
                    Porentagem = table.Column<decimal>(type: "numeric", nullable: false),
                    LimiteReal = table.Column<decimal>(type: "numeric", nullable: false),
                    CPF = table.Column<string>(type: "text", nullable: false),
                    AnoCivil = table.Column<string>(type: "text", nullable: false),
                    Ativo = table.Column<bool>(type: "boolean", nullable: false),
                    Excluido = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubvencoesEstaduais", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SubvencoesFederais",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Cultura = table.Column<string>(type: "text", nullable: false),
                    Porentagem = table.Column<decimal>(type: "numeric", nullable: false),
                    LimiteReal = table.Column<decimal>(type: "numeric", nullable: false),
                    CPF = table.Column<string>(type: "text", nullable: false),
                    AnoCivil = table.Column<string>(type: "text", nullable: false),
                    Ativo = table.Column<bool>(type: "boolean", nullable: false),
                    Excluido = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubvencoesFederais", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Talhoes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    PropriedadeId = table.Column<Guid>(type: "uuid", nullable: false),
                    Nome = table.Column<string>(type: "text", nullable: false),
                    Area = table.Column<decimal>(type: "numeric", nullable: false),
                    Latitude = table.Column<string>(type: "text", nullable: false),
                    Longitude = table.Column<string>(type: "text", nullable: false),
                    RoteiroAcesso = table.Column<string>(type: "text", nullable: false),
                    PossuiAnaliseFisicaSolo = table.Column<bool>(type: "boolean", nullable: false),
                    PercentualAreia = table.Column<decimal>(type: "numeric", nullable: false),
                    PercentualSilte = table.Column<decimal>(type: "numeric", nullable: false),
                    PercentualArgila = table.Column<decimal>(type: "numeric", nullable: false),
                    TipoSolo = table.Column<string>(type: "text", nullable: false),
                    ClassificacaoSolo = table.Column<string>(type: "text", nullable: false),
                    ImagemTalhao = table.Column<string>(type: "text", nullable: false),
                    KmlTalhao = table.Column<string>(type: "text", nullable: false),
                    Ativo = table.Column<bool>(type: "boolean", nullable: false),
                    Excluido = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Talhoes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RoleId = table.Column<Guid>(type: "uuid", nullable: false),
                    ClaimType = table.Column<string>(type: "text", nullable: true),
                    ClaimValue = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    ClaimType = table.Column<string>(type: "text", nullable: true),
                    ClaimValue = table.Column<string>(type: "text", nullable: true),
                    UsuarioEntityId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_UsuarioEntityId",
                        column: x => x.UsuarioEntityId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "text", nullable: false),
                    ProviderKey = table.Column<string>(type: "text", nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "text", nullable: true),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    RoleId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    LoginProvider = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Value = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "VinculosFamiliares",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Cpf = table.Column<string>(type: "text", nullable: false),
                    Nome = table.Column<string>(type: "text", nullable: false),
                    Telefone = table.Column<string>(type: "text", nullable: false),
                    Celular = table.Column<string>(type: "text", nullable: false),
                    Email = table.Column<string>(type: "text", nullable: false),
                    RelacaoParental = table.Column<string>(type: "text", nullable: false),
                    Ativo = table.Column<bool>(type: "boolean", nullable: false),
                    Excluido = table.Column<bool>(type: "boolean", nullable: false),
                    ClienteId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VinculosFamiliares", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VinculosFamiliares_Clientes_ClienteId",
                        column: x => x.ClienteId,
                        principalTable: "Clientes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Canais",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    RazaoSocial = table.Column<string>(type: "text", nullable: false),
                    NomeFantasia = table.Column<string>(type: "text", nullable: false),
                    CNPJ = table.Column<string>(type: "text", nullable: false),
                    Endereco = table.Column<string>(type: "text", nullable: false),
                    Bairro = table.Column<string>(type: "text", nullable: false),
                    Numero = table.Column<string>(type: "text", nullable: false),
                    Cidade = table.Column<string>(type: "text", nullable: false),
                    Estado = table.Column<string>(type: "text", nullable: false),
                    CEP = table.Column<string>(type: "text", nullable: false),
                    Email = table.Column<string>(type: "text", nullable: false),
                    Telefone = table.Column<string>(type: "text", nullable: false),
                    Celular = table.Column<string>(type: "text", nullable: false),
                    Ativo = table.Column<bool>(type: "boolean", nullable: false),
                    Excluido = table.Column<bool>(type: "boolean", nullable: false),
                    ImagemLogo = table.Column<string>(type: "text", nullable: false),
                    CorretoraId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Canais", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Canais_Corretoras_CorretoraId",
                        column: x => x.CorretoraId,
                        principalTable: "Corretoras",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "CulturaMaturacao",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    GrupoMaturacao = table.Column<string>(type: "text", nullable: false),
                    Ativo = table.Column<bool>(type: "boolean", nullable: false),
                    Excluido = table.Column<bool>(type: "boolean", nullable: false),
                    CulturaId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CulturaMaturacao", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CulturaMaturacao_Culturas_CulturaId",
                        column: x => x.CulturaId,
                        principalTable: "Culturas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "VinculosPropriedadesClientes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ClienteId = table.Column<Guid>(type: "uuid", nullable: false),
                    PropriedadeId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VinculosPropriedadesClientes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VinculosPropriedadesClientes_Propriedades_PropriedadeId",
                        column: x => x.PropriedadeId,
                        principalTable: "Propriedades",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Produtos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    SafraId = table.Column<Guid>(type: "uuid", nullable: false),
                    CulturaId = table.Column<Guid>(type: "uuid", nullable: false),
                    SeguradoraId = table.Column<Guid>(type: "uuid", nullable: false),
                    NomeProduto = table.Column<string>(type: "text", nullable: false),
                    PorcentagemComissao = table.Column<decimal>(type: "numeric", nullable: false),
                    PorcentagemRepasse = table.Column<decimal>(type: "numeric", nullable: false),
                    ProcessoSusep = table.Column<string>(type: "text", nullable: false),
                    Modalidade = table.Column<string>(type: "text", nullable: false),
                    ValorSacaMinimo = table.Column<decimal>(type: "numeric", nullable: false),
                    ValorSacaMaximo = table.Column<decimal>(type: "numeric", nullable: false),
                    ValorSacaSugerido = table.Column<decimal>(type: "numeric", nullable: false),
                    ValorCusteioMinimo = table.Column<decimal>(type: "numeric", nullable: false),
                    ValorCusteioMaximo = table.Column<decimal>(type: "numeric", nullable: false),
                    ValorCusteioSugerido = table.Column<decimal>(type: "numeric", nullable: false),
                    AreaMinimaItem = table.Column<decimal>(type: "numeric", nullable: false),
                    AreaMinimaTotal = table.Column<decimal>(type: "numeric", nullable: false),
                    PremioMinimo = table.Column<decimal>(type: "numeric", nullable: false),
                    FormaDePagamento = table.Column<string>(type: "text", nullable: false),
                    Parcelamento = table.Column<string>(type: "text", nullable: false),
                    TipoSolo = table.Column<string[]>(type: "text[]", nullable: false),
                    AjusteProdutividade = table.Column<decimal>(type: "numeric", nullable: true),
                    AjusteTaxa = table.Column<decimal>(type: "numeric", nullable: true),
                    ClassificacaoSolosAceitos = table.Column<string[]>(type: "text[]", nullable: false),
                    Replantio = table.Column<string>(type: "text", nullable: false),
                    PorcentagemCoberturaProducao = table.Column<decimal>(type: "numeric", nullable: true),
                    ValorCoberturaAdicional = table.Column<decimal>(type: "numeric", nullable: true),
                    TaxaCoberturaAdicional = table.Column<decimal>(type: "numeric", nullable: true),
                    RegulacaoSinistro = table.Column<string>(type: "text", nullable: false),
                    UtilizaSubvencaoFederal = table.Column<bool>(type: "boolean", nullable: false),
                    SubvencaoFederalId = table.Column<Guid>(type: "uuid", nullable: true),
                    UtilizaSubvencaoEstadual = table.Column<bool>(type: "boolean", nullable: false),
                    SubvencaoEstadualId = table.Column<Guid>(type: "uuid", nullable: true),
                    TermoDeCiencia = table.Column<string>(type: "text", nullable: true),
                    CapacidadeDisponivel = table.Column<decimal>(type: "numeric", nullable: true),
                    KiloPorSaca = table.Column<decimal>(type: "numeric", nullable: true),
                    AceitaPlantioConsorciado = table.Column<bool>(type: "boolean", nullable: false),
                    PlantioConsorciadoAjusteProdutividade = table.Column<decimal>(type: "numeric", nullable: false),
                    PlantioConsorciadoAjusteTaxa = table.Column<decimal>(type: "numeric", nullable: false),
                    AceitaPlantioConvencional = table.Column<bool>(type: "boolean", nullable: false),
                    PlantioConvencionalAjusteProdutividade = table.Column<decimal>(type: "numeric", nullable: false),
                    PlantioConvencionalAjusteTaxa = table.Column<decimal>(type: "numeric", nullable: false),
                    LavouraIrrigada = table.Column<bool>(type: "boolean", nullable: false),
                    LavouraIrrigadaAjusteProdutividade = table.Column<decimal>(type: "numeric", nullable: false),
                    LavouraIrrigadaAjusteTaxa = table.Column<decimal>(type: "numeric", nullable: false),
                    AceitaPlantioPosCanaDeAcucar = table.Column<bool>(type: "boolean", nullable: false),
                    PlantioPosCanaAjusteProdutividade = table.Column<decimal>(type: "numeric", nullable: false),
                    PlantioPosCanaAjusteTaxa = table.Column<decimal>(type: "numeric", nullable: false),
                    Ativo = table.Column<bool>(type: "boolean", nullable: false),
                    Excluido = table.Column<bool>(type: "boolean", nullable: false),
                    DataCriacao = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Produtos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Produtos_Culturas_CulturaId",
                        column: x => x.CulturaId,
                        principalTable: "Culturas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Produtos_Safras_SafraId",
                        column: x => x.SafraId,
                        principalTable: "Safras",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Produtos_Seguradoras_SeguradoraId",
                        column: x => x.SeguradoraId,
                        principalTable: "Seguradoras",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Produtos_SubvencoesEstaduais_SubvencaoEstadualId",
                        column: x => x.SubvencaoEstadualId,
                        principalTable: "SubvencoesEstaduais",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Produtos_SubvencoesFederais_SubvencaoFederalId",
                        column: x => x.SubvencaoFederalId,
                        principalTable: "SubvencoesFederais",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "PontosAtendimento",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CanalId = table.Column<Guid>(type: "uuid", nullable: false),
                    RazaoSocial = table.Column<string>(type: "text", nullable: false),
                    NomeFantasia = table.Column<string>(type: "text", nullable: false),
                    CNPJ = table.Column<string>(type: "text", nullable: false),
                    Endereco = table.Column<string>(type: "text", nullable: false),
                    Bairro = table.Column<string>(type: "text", nullable: false),
                    Numero = table.Column<string>(type: "text", nullable: false),
                    Cidade = table.Column<string>(type: "text", nullable: false),
                    Estado = table.Column<string>(type: "text", nullable: false),
                    CEP = table.Column<string>(type: "text", nullable: false),
                    Email = table.Column<string>(type: "text", nullable: false),
                    Telefone = table.Column<string>(type: "text", nullable: false),
                    Celular = table.Column<string>(type: "text", nullable: false),
                    Ativo = table.Column<bool>(type: "boolean", nullable: false),
                    Excluido = table.Column<bool>(type: "boolean", nullable: false),
                    EmailSeguro = table.Column<string>(type: "text", nullable: true),
                    EmailCopiaSeguro = table.Column<string>(type: "text", nullable: true),
                    EmailSinistro = table.Column<string>(type: "text", nullable: true),
                    EmailCopiaSinistro = table.Column<string>(type: "text", nullable: true),
                    ImagemLogo = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PontosAtendimento", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PontosAtendimento_Canais_CanalId",
                        column: x => x.CanalId,
                        principalTable: "Canais",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UsuariosCanal",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CanalId = table.Column<Guid>(type: "uuid", nullable: false),
                    UsuarioId = table.Column<Guid>(type: "uuid", nullable: false),
                    LideradoId = table.Column<Guid>(type: "uuid", nullable: false),
                    Ativo = table.Column<bool>(type: "boolean", nullable: false),
                    Excluido = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UsuariosCanal", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UsuariosCanal_AspNetUsers_LideradoId",
                        column: x => x.LideradoId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UsuariosCanal_AspNetUsers_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UsuariosCanal_Canais_CanalId",
                        column: x => x.CanalId,
                        principalTable: "Canais",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CulturaMaturacaoVariedades",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Nome = table.Column<string>(type: "text", nullable: false),
                    Ativo = table.Column<bool>(type: "boolean", nullable: false),
                    Excluido = table.Column<bool>(type: "boolean", nullable: false),
                    CulturaMaturacaoId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CulturaMaturacaoVariedades", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CulturaMaturacaoVariedades_CulturaMaturacao_CulturaMaturaca~",
                        column: x => x.CulturaMaturacaoId,
                        principalTable: "CulturaMaturacao",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProdutosSubvencoesEstaduais",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ProdutoId = table.Column<Guid>(type: "uuid", nullable: false),
                    SubvencaoEstadualId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProdutosSubvencoesEstaduais", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProdutosSubvencoesEstaduais_Produtos_ProdutoId",
                        column: x => x.ProdutoId,
                        principalTable: "Produtos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProdutosSubvencoesEstaduais_SubvencoesEstaduais_SubvencaoEs~",
                        column: x => x.SubvencaoEstadualId,
                        principalTable: "SubvencoesEstaduais",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProdutosTaxas",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ProdutoId = table.Column<Guid>(type: "uuid", nullable: false),
                    UF = table.Column<string>(type: "text", nullable: false),
                    Municipio = table.Column<string>(type: "text", nullable: false),
                    ProdutividadeEsperada = table.Column<decimal>(type: "numeric", nullable: false),
                    TaxaNc65 = table.Column<decimal>(type: "numeric", nullable: false),
                    TaxaNc70 = table.Column<decimal>(type: "numeric", nullable: true),
                    TaxaNc75 = table.Column<decimal>(type: "numeric", nullable: true),
                    Cpf = table.Column<string>(type: "text", nullable: true),
                    Ativo = table.Column<bool>(type: "boolean", nullable: false),
                    Excluido = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProdutosTaxas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProdutosTaxas_Produtos_ProdutoId",
                        column: x => x.ProdutoId,
                        principalTable: "Produtos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CotacoesAgricola",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ClienteId = table.Column<Guid>(type: "uuid", nullable: true),
                    ClienteCPF = table.Column<string>(type: "text", nullable: true),
                    ClienteNome = table.Column<string>(type: "text", nullable: true),
                    CulturaId = table.Column<Guid>(type: "uuid", nullable: false),
                    SafraId = table.Column<Guid>(type: "uuid", nullable: false),
                    Estado = table.Column<string>(type: "text", nullable: false),
                    Municipio = table.Column<string>(type: "text", nullable: false),
                    AreaTotal = table.Column<decimal>(type: "numeric", nullable: false),
                    IsModalidadeProdutividade = table.Column<bool>(type: "boolean", nullable: false),
                    PrecoSaca = table.Column<decimal>(type: "numeric", nullable: true),
                    ValorCusteio = table.Column<decimal>(type: "numeric", nullable: true),
                    PlantioConsorciado = table.Column<bool>(type: "boolean", nullable: false),
                    LavouraIrrigada = table.Column<bool>(type: "boolean", nullable: false),
                    PlantioDireto = table.Column<bool>(type: "boolean", nullable: false),
                    PosCana = table.Column<bool>(type: "boolean", nullable: false),
                    CustoProducao = table.Column<decimal>(type: "numeric", nullable: false),
                    SubvencaoFederal = table.Column<bool>(type: "boolean", nullable: false),
                    SubvencaoEstadual = table.Column<bool>(type: "boolean", nullable: false),
                    CorretoraId = table.Column<Guid>(type: "uuid", nullable: true),
                    CanalId = table.Column<Guid>(type: "uuid", nullable: true),
                    PontoAtendimentoId = table.Column<Guid>(type: "uuid", nullable: true),
                    CodigoCotacao = table.Column<string>(type: "text", nullable: true),
                    DataCotacao = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    Status = table.Column<string>(type: "text", nullable: true),
                    Ativo = table.Column<bool>(type: "boolean", nullable: false),
                    Excluido = table.Column<bool>(type: "boolean", nullable: false),
                    UsuarioId = table.Column<Guid>(type: "uuid", nullable: false),
                    DataInsucesso = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CotacoesAgricola", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CotacoesAgricola_AspNetUsers_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CotacoesAgricola_Canais_CanalId",
                        column: x => x.CanalId,
                        principalTable: "Canais",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_CotacoesAgricola_Clientes_ClienteId",
                        column: x => x.ClienteId,
                        principalTable: "Clientes",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_CotacoesAgricola_Corretoras_CorretoraId",
                        column: x => x.CorretoraId,
                        principalTable: "Corretoras",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_CotacoesAgricola_Culturas_CulturaId",
                        column: x => x.CulturaId,
                        principalTable: "Culturas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CotacoesAgricola_PontosAtendimento_PontoAtendimentoId",
                        column: x => x.PontoAtendimentoId,
                        principalTable: "PontosAtendimento",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_CotacoesAgricola_Safras_SafraId",
                        column: x => x.SafraId,
                        principalTable: "Safras",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProdutosCanalPontosAtendimento",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ProdutoId = table.Column<Guid>(type: "uuid", nullable: false),
                    CanalId = table.Column<Guid>(type: "uuid", nullable: false),
                    PontoAtendimentoId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProdutosCanalPontosAtendimento", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProdutosCanalPontosAtendimento_Canais_CanalId",
                        column: x => x.CanalId,
                        principalTable: "Canais",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProdutosCanalPontosAtendimento_PontosAtendimento_PontoAtend~",
                        column: x => x.PontoAtendimentoId,
                        principalTable: "PontosAtendimento",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProdutosCanalPontosAtendimento_Produtos_ProdutoId",
                        column: x => x.ProdutoId,
                        principalTable: "Produtos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UsuarioEstruturaNegocio",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CorretoraId = table.Column<Guid>(type: "uuid", nullable: true),
                    CanalId = table.Column<Guid>(type: "uuid", nullable: true),
                    PontoAtendimentoId = table.Column<Guid>(type: "uuid", nullable: true),
                    UsuarioId = table.Column<Guid>(type: "uuid", nullable: false),
                    UsuarioCriadorId = table.Column<Guid>(type: "uuid", nullable: false),
                    Ativo = table.Column<bool>(type: "boolean", nullable: false),
                    Excluido = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UsuarioEstruturaNegocio", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UsuarioEstruturaNegocio_AspNetUsers_UsuarioCriadorId",
                        column: x => x.UsuarioCriadorId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UsuarioEstruturaNegocio_AspNetUsers_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UsuarioEstruturaNegocio_Canais_CanalId",
                        column: x => x.CanalId,
                        principalTable: "Canais",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UsuarioEstruturaNegocio_Corretoras_CorretoraId",
                        column: x => x.CorretoraId,
                        principalTable: "Corretoras",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_UsuarioEstruturaNegocio_PontosAtendimento_PontoAtendimentoId",
                        column: x => x.PontoAtendimentoId,
                        principalTable: "PontosAtendimento",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UsuariosPontoAtendimento",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    PontoAtendimentoId = table.Column<Guid>(type: "uuid", nullable: false),
                    CanalId = table.Column<Guid>(type: "uuid", nullable: false),
                    UsuarioId = table.Column<Guid>(type: "uuid", nullable: false),
                    LideradoId = table.Column<Guid>(type: "uuid", nullable: false),
                    Ativo = table.Column<bool>(type: "boolean", nullable: false),
                    Excluido = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UsuariosPontoAtendimento", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UsuariosPontoAtendimento_AspNetUsers_LideradoId",
                        column: x => x.LideradoId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UsuariosPontoAtendimento_AspNetUsers_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UsuariosPontoAtendimento_Canais_CanalId",
                        column: x => x.CanalId,
                        principalTable: "Canais",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UsuariosPontoAtendimento_PontosAtendimento_PontoAtendimento~",
                        column: x => x.PontoAtendimentoId,
                        principalTable: "PontosAtendimento",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CotacoesAgricolaClassificacaoSolo",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CotacaoAgricolaId = table.Column<Guid>(type: "uuid", nullable: false),
                    ClassificacaoSolo = table.Column<string>(type: "text", nullable: false),
                    CotacoesAgricolaId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CotacoesAgricolaClassificacaoSolo", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CotacoesAgricolaClassificacaoSolo_CotacoesAgricola_Cotacoes~",
                        column: x => x.CotacoesAgricolaId,
                        principalTable: "CotacoesAgricola",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CotacoesAgricolaProposta",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CotacaoAgricolaId = table.Column<Guid>(type: "uuid", nullable: false),
                    Opcao = table.Column<int>(type: "integer", nullable: false),
                    TipoOferta = table.Column<string>(type: "text", nullable: false),
                    SeguradoraId = table.Column<Guid>(type: "uuid", nullable: false),
                    ProdutoId = table.Column<Guid>(type: "uuid", nullable: false),
                    RegulacaoSinistro = table.Column<string>(type: "text", nullable: false),
                    ProdutividadeEsperada = table.Column<decimal>(type: "numeric", nullable: false),
                    NivelCobertura = table.Column<decimal>(type: "numeric", nullable: false),
                    ProdutividadeSegurada = table.Column<decimal>(type: "numeric", nullable: false),
                    LMIProducaoHectare = table.Column<decimal>(type: "numeric", nullable: false),
                    LMIReplantioHectare = table.Column<decimal>(type: "numeric", nullable: false),
                    LMIProducaoTotal = table.Column<decimal>(type: "numeric", nullable: false),
                    LMIReplantioTotal = table.Column<decimal>(type: "numeric", nullable: false),
                    PremioTotal = table.Column<decimal>(type: "numeric", nullable: false),
                    SubvencaoFederal = table.Column<decimal>(type: "numeric", nullable: false),
                    SubvencaoEstadual = table.Column<decimal>(type: "numeric", nullable: false),
                    ParcelaSegurado = table.Column<decimal>(type: "numeric", nullable: false),
                    CustoHectare = table.Column<decimal>(type: "numeric", nullable: false),
                    CustoScHectare = table.Column<decimal>(type: "numeric", nullable: false),
                    CustoScTotal = table.Column<decimal>(type: "numeric", nullable: false),
                    Ativo = table.Column<bool>(type: "boolean", nullable: false),
                    Excluido = table.Column<bool>(type: "boolean", nullable: false),
                    CotacoesAgricolaId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CotacoesAgricolaProposta", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CotacoesAgricolaProposta_CotacoesAgricola_CotacoesAgricolaId",
                        column: x => x.CotacoesAgricolaId,
                        principalTable: "CotacoesAgricola",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_CotacoesAgricolaProposta_Produtos_ProdutoId",
                        column: x => x.ProdutoId,
                        principalTable: "Produtos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CotacoesAgricolaProposta_Seguradoras_SeguradoraId",
                        column: x => x.SeguradoraId,
                        principalTable: "Seguradoras",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CotacoesAgricolaStatus",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CotacaoAgricolaId = table.Column<Guid>(type: "uuid", nullable: false),
                    Status = table.Column<string>(type: "text", nullable: false),
                    DataStatus = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    Observacao = table.Column<string>(type: "text", nullable: false),
                    Ativo = table.Column<bool>(type: "boolean", nullable: false),
                    Excluido = table.Column<bool>(type: "boolean", nullable: false),
                    CotacoesAgricolaId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CotacoesAgricolaStatus", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CotacoesAgricolaStatus_CotacoesAgricola_CotacoesAgricolaId",
                        column: x => x.CotacoesAgricolaId,
                        principalTable: "CotacoesAgricola",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "CotacoesAgricolaTipoSolo",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CotacaoAgricolaId = table.Column<Guid>(type: "uuid", nullable: false),
                    TipoSolo = table.Column<int>(type: "integer", nullable: false),
                    CotacoesAgricolaId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CotacoesAgricolaTipoSolo", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CotacoesAgricolaTipoSolo_CotacoesAgricola_CotacoesAgricolaId",
                        column: x => x.CotacoesAgricolaId,
                        principalTable: "CotacoesAgricola",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Propostas",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CotacaoAgricolaId = table.Column<Guid>(type: "uuid", nullable: false),
                    ClienteId = table.Column<Guid>(type: "uuid", nullable: false),
                    CulturaId = table.Column<Guid>(type: "uuid", nullable: false),
                    SafraId = table.Column<Guid>(type: "uuid", nullable: false),
                    Estado = table.Column<string>(type: "text", nullable: false),
                    Municipio = table.Column<string>(type: "text", nullable: false),
                    AreaTotal = table.Column<decimal>(type: "numeric", nullable: false),
                    IsModalidadeProdutividade = table.Column<bool>(type: "boolean", nullable: false),
                    PrecoSaca = table.Column<decimal>(type: "numeric", nullable: true),
                    ValorCusteio = table.Column<decimal>(type: "numeric", nullable: true),
                    PlantioConsorciado = table.Column<bool>(type: "boolean", nullable: false),
                    LavouraIrrigada = table.Column<bool>(type: "boolean", nullable: false),
                    PlantioDireto = table.Column<bool>(type: "boolean", nullable: false),
                    PosCana = table.Column<bool>(type: "boolean", nullable: false),
                    CustoProducao = table.Column<decimal>(type: "numeric", nullable: false),
                    SubvencaoFederal = table.Column<bool>(type: "boolean", nullable: false),
                    SubvencaoEstadual = table.Column<bool>(type: "boolean", nullable: false),
                    CorretoraId = table.Column<Guid>(type: "uuid", nullable: true),
                    CanalId = table.Column<Guid>(type: "uuid", nullable: true),
                    PontoAtendimentoId = table.Column<Guid>(type: "uuid", nullable: true),
                    CodigoCotacao = table.Column<string>(type: "text", nullable: true),
                    DataCotacao = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    Status = table.Column<string>(type: "text", nullable: true),
                    Ativo = table.Column<bool>(type: "boolean", nullable: false),
                    Excluido = table.Column<bool>(type: "boolean", nullable: false),
                    UsuarioId = table.Column<Guid>(type: "uuid", nullable: false),
                    DataInsucesso = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    CodigoProposta = table.Column<int>(type: "integer", nullable: false),
                    CotacoesAgricolaId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Propostas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Propostas_AspNetUsers_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Propostas_Canais_CanalId",
                        column: x => x.CanalId,
                        principalTable: "Canais",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Propostas_Clientes_ClienteId",
                        column: x => x.ClienteId,
                        principalTable: "Clientes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Propostas_Corretoras_CorretoraId",
                        column: x => x.CorretoraId,
                        principalTable: "Corretoras",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Propostas_CotacoesAgricola_CotacoesAgricolaId",
                        column: x => x.CotacoesAgricolaId,
                        principalTable: "CotacoesAgricola",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Propostas_Culturas_CulturaId",
                        column: x => x.CulturaId,
                        principalTable: "Culturas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Propostas_PontosAtendimento_PontoAtendimentoId",
                        column: x => x.PontoAtendimentoId,
                        principalTable: "PontosAtendimento",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Propostas_Safras_SafraId",
                        column: x => x.SafraId,
                        principalTable: "Safras",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PropostasBeneficiarios",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    PropostaId = table.Column<Guid>(type: "uuid", nullable: false),
                    ClienteId = table.Column<Guid>(type: "uuid", nullable: true),
                    BeneficiarioId = table.Column<Guid>(type: "uuid", nullable: true),
                    Nome = table.Column<string>(type: "text", nullable: true),
                    Documento = table.Column<string>(type: "text", nullable: true),
                    ImagemDocumento = table.Column<string>(type: "text", nullable: true),
                    Banco = table.Column<string>(type: "text", nullable: true),
                    Agencia = table.Column<string>(type: "text", nullable: true),
                    Conta = table.Column<string>(type: "text", nullable: true),
                    ChavePIX = table.Column<string>(type: "text", nullable: true),
                    Percentual = table.Column<decimal>(type: "numeric", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PropostasBeneficiarios", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PropostasBeneficiarios_Beneficiarios_BeneficiarioId",
                        column: x => x.BeneficiarioId,
                        principalTable: "Beneficiarios",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_PropostasBeneficiarios_Clientes_ClienteId",
                        column: x => x.ClienteId,
                        principalTable: "Clientes",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_PropostasBeneficiarios_Propostas_PropostaId",
                        column: x => x.PropostaId,
                        principalTable: "Propostas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PropostasClassificacaoSolo",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    PropostaId = table.Column<Guid>(type: "uuid", nullable: false),
                    ClassificacaoSolo = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PropostasClassificacaoSolo", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PropostasClassificacaoSolo_Propostas_PropostaId",
                        column: x => x.PropostaId,
                        principalTable: "Propostas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PropostasClientePropriedades",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    PropostaId = table.Column<Guid>(type: "uuid", nullable: false),
                    ClienteId = table.Column<Guid>(type: "uuid", nullable: false),
                    PropriedadeId = table.Column<Guid>(type: "uuid", nullable: false),
                    Nome = table.Column<string>(type: "text", nullable: true),
                    Endereco = table.Column<string>(type: "text", nullable: true),
                    Bairro = table.Column<string>(type: "text", nullable: true),
                    Numero = table.Column<string>(type: "text", nullable: true),
                    Cidade = table.Column<string>(type: "text", nullable: true),
                    Estado = table.Column<string>(type: "text", nullable: true),
                    CEP = table.Column<string>(type: "text", nullable: true),
                    SomaAreaTotalTalhao = table.Column<decimal>(type: "numeric", nullable: false),
                    ImagemGeralTodosTalhoes = table.Column<string>(type: "text", nullable: true),
                    MatriculaLote = table.Column<string>(type: "text", nullable: true),
                    CadastroAmbientalRural = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PropostasClientePropriedades", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PropostasClientePropriedades_Clientes_ClienteId",
                        column: x => x.ClienteId,
                        principalTable: "Clientes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PropostasClientePropriedades_Propostas_PropostaId",
                        column: x => x.PropostaId,
                        principalTable: "Propostas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PropostasClientePropriedades_Propriedades_PropriedadeId",
                        column: x => x.PropriedadeId,
                        principalTable: "Propriedades",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PropostasDocumentos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: true),
                    PropostaId = table.Column<Guid>(type: "uuid", nullable: false),
                    TipoDocumento = table.Column<string>(type: "text", nullable: false),
                    DataUpload = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    UrlDocumento = table.Column<string>(type: "text", nullable: false),
                    NomeArquivo = table.Column<string>(type: "text", nullable: false),
                    UsuarioId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PropostasDocumentos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PropostasDocumentos_AspNetUsers_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_PropostasDocumentos_Propostas_PropostaId",
                        column: x => x.PropostaId,
                        principalTable: "Propostas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PropostasFormaPagamentos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    PropostaId = table.Column<Guid>(type: "uuid", nullable: false),
                    ProdutoId = table.Column<Guid>(type: "uuid", nullable: false),
                    FormaDePagamento = table.Column<string>(type: "text", nullable: true),
                    Parcelamento = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PropostasFormaPagamentos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PropostasFormaPagamentos_Produtos_ProdutoId",
                        column: x => x.ProdutoId,
                        principalTable: "Produtos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PropostasFormaPagamentos_Propostas_PropostaId",
                        column: x => x.PropostaId,
                        principalTable: "Propostas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PropostasObservacoes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    PropostaId = table.Column<Guid>(type: "uuid", nullable: false),
                    UsuarioId = table.Column<Guid>(type: "uuid", nullable: false),
                    DataCriacao = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    Observacao = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PropostasObservacoes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PropostasObservacoes_AspNetUsers_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PropostasObservacoes_Propostas_PropostaId",
                        column: x => x.PropostaId,
                        principalTable: "Propostas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PropostasOcorrencias",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    PropostaId = table.Column<Guid>(type: "uuid", nullable: false),
                    UsuarioId = table.Column<Guid>(type: "uuid", nullable: false),
                    DataCriacao = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    Descricao = table.Column<string>(type: "text", nullable: false),
                    CaminhoAnexo = table.Column<string>(type: "text", nullable: true),
                    NomeArquivo = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PropostasOcorrencias", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PropostasOcorrencias_AspNetUsers_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PropostasOcorrencias_Propostas_PropostaId",
                        column: x => x.PropostaId,
                        principalTable: "Propostas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PropostasProdutos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    PropostaId = table.Column<Guid>(type: "uuid", nullable: false),
                    CotacoesAgricolaPropostaId = table.Column<Guid>(type: "uuid", nullable: false),
                    Opcao = table.Column<int>(type: "integer", nullable: false),
                    SeguradoraId = table.Column<Guid>(type: "uuid", nullable: false),
                    ProdutoId = table.Column<Guid>(type: "uuid", nullable: false),
                    RegulacaoSinistro = table.Column<string>(type: "text", nullable: false),
                    ProdutividadeEsperada = table.Column<decimal>(type: "numeric", nullable: false),
                    NivelCobertura = table.Column<decimal>(type: "numeric", nullable: false),
                    ProdutividadeSegurada = table.Column<decimal>(type: "numeric", nullable: false),
                    LMIProducaoHectare = table.Column<decimal>(type: "numeric", nullable: false),
                    LMIReplantioHectare = table.Column<decimal>(type: "numeric", nullable: false),
                    LMIProducaoTotal = table.Column<decimal>(type: "numeric", nullable: false),
                    LMIReplantioTotal = table.Column<decimal>(type: "numeric", nullable: false),
                    PremioTotal = table.Column<decimal>(type: "numeric", nullable: false),
                    SubvencaoFederal = table.Column<decimal>(type: "numeric", nullable: false),
                    SubvencaoEstadual = table.Column<decimal>(type: "numeric", nullable: false),
                    ParcelaSegurado = table.Column<decimal>(type: "numeric", nullable: false),
                    CustoHectare = table.Column<decimal>(type: "numeric", nullable: false),
                    CustoScHectare = table.Column<decimal>(type: "numeric", nullable: false),
                    CustoScTotal = table.Column<decimal>(type: "numeric", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PropostasProdutos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PropostasProdutos_CotacoesAgricolaProposta_CotacoesAgricola~",
                        column: x => x.CotacoesAgricolaPropostaId,
                        principalTable: "CotacoesAgricolaProposta",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PropostasProdutos_Produtos_ProdutoId",
                        column: x => x.ProdutoId,
                        principalTable: "Produtos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PropostasProdutos_Propostas_PropostaId",
                        column: x => x.PropostaId,
                        principalTable: "Propostas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PropostasProdutos_Seguradoras_SeguradoraId",
                        column: x => x.SeguradoraId,
                        principalTable: "Seguradoras",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PropostasQuestionario",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    PropostaId = table.Column<Guid>(type: "uuid", nullable: false),
                    SistemaPlantio = table.Column<string>(type: "text", nullable: true),
                    LavouraPlantada = table.Column<bool>(type: "boolean", nullable: false),
                    PossuiDanosPreExistentes = table.Column<bool>(type: "boolean", nullable: true),
                    ConheceZARC = table.Column<bool>(type: "boolean", nullable: false),
                    PossuiOutroSeguro = table.Column<bool>(type: "boolean", nullable: false),
                    LavouraImplantadaAposOutraArea = table.Column<bool>(type: "boolean", nullable: false),
                    NotasFiscaisProprioSegurado = table.Column<bool>(type: "boolean", nullable: false),
                    CulturaAnteriorId = table.Column<Guid>(type: "uuid", nullable: true),
                    LavouraIrrigada = table.Column<bool>(type: "boolean", nullable: false),
                    PossuiCreditoBancario = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PropostasQuestionario", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PropostasQuestionario_Culturas_CulturaAnteriorId",
                        column: x => x.CulturaAnteriorId,
                        principalTable: "Culturas",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_PropostasQuestionario_Propostas_PropostaId",
                        column: x => x.PropostaId,
                        principalTable: "Propostas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PropostasSegurados",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    PropostaId = table.Column<Guid>(type: "uuid", nullable: false),
                    ClienteId = table.Column<Guid>(type: "uuid", nullable: false),
                    VinculoFamiliarId = table.Column<Guid>(type: "uuid", nullable: true),
                    Nome = table.Column<string>(type: "text", nullable: false),
                    CPF = table.Column<string>(type: "text", nullable: false),
                    Telefone = table.Column<string>(type: "text", nullable: true),
                    Email = table.Column<string>(type: "text", nullable: true),
                    RelacaoParental = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PropostasSegurados", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PropostasSegurados_Clientes_ClienteId",
                        column: x => x.ClienteId,
                        principalTable: "Clientes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PropostasSegurados_Propostas_PropostaId",
                        column: x => x.PropostaId,
                        principalTable: "Propostas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PropostasSegurados_VinculosFamiliares_VinculoFamiliarId",
                        column: x => x.VinculoFamiliarId,
                        principalTable: "VinculosFamiliares",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "PropostasStatus",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    PropostaId = table.Column<Guid>(type: "uuid", nullable: false),
                    UsuarioId = table.Column<Guid>(type: "uuid", nullable: true),
                    Status = table.Column<string>(type: "text", nullable: true),
                    DataStatus = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    StatusAnterior = table.Column<string>(type: "text", nullable: true),
                    UsuarioPerfil = table.Column<string>(type: "text", nullable: true),
                    Discriminator = table.Column<string>(type: "text", nullable: false),
                    PropostaSeguradora = table.Column<string>(type: "text", nullable: true),
                    PropostaSeguradoraNome = table.Column<string>(type: "text", nullable: true),
                    NumeroProposta = table.Column<string>(type: "text", nullable: true),
                    LmiTotal = table.Column<string>(type: "text", nullable: true),
                    PropostaStatusAceitaEntity_PremioTotal = table.Column<string>(type: "text", nullable: true),
                    PropostaStatusAceitaEntity_SubFederal = table.Column<string>(type: "text", nullable: true),
                    PropostaStatusAceitaEntity_SubEstadual = table.Column<string>(type: "text", nullable: true),
                    ParcelaSegurado = table.Column<decimal>(type: "numeric", nullable: true),
                    PropostaStatusAceitaEntity_Boleto = table.Column<string>(type: "text", nullable: true),
                    PropostaStatusAceitaEntity_BoletoNome = table.Column<string>(type: "text", nullable: true),
                    PropostaStatusAceitaEntity_DataVencimento = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    ApoliceEmitida = table.Column<string>(type: "text", nullable: true),
                    ApoliceEmitidaNome = table.Column<string>(type: "text", nullable: true),
                    NumeroApolice = table.Column<string>(type: "text", nullable: true),
                    PropostaStatusApoliceEmitidaEntity_InicioVigencia = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    FinalVigencia = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    Boleto = table.Column<string>(type: "text", nullable: true),
                    BoletoNome = table.Column<string>(type: "text", nullable: true),
                    DataVencimento = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    PropostaStatusEndossoEmitidoEntity_Acao = table.Column<string>(type: "text", nullable: true),
                    PropostaStatusEndossoEmitidoEntity_NumeroEndosso = table.Column<string>(type: "text", nullable: true),
                    PropostaStatusEndossoEmitidoEntity_UploadArquivo = table.Column<string>(type: "text", nullable: true),
                    PropostaStatusEndossoEmitidoEntity_UploadArquivoNome = table.Column<string>(type: "text", nullable: true),
                    PropostaStatusEndossoEmitidoEntity_TipoDocumento = table.Column<string>(type: "text", nullable: true),
                    LMITotal = table.Column<string>(type: "text", nullable: true),
                    PremioTotal = table.Column<string>(type: "text", nullable: true),
                    SubFederal = table.Column<string>(type: "text", nullable: true),
                    SubEstadual = table.Column<string>(type: "text", nullable: true),
                    ParcSegurado = table.Column<decimal>(type: "numeric", nullable: true),
                    AreaTotal = table.Column<decimal>(type: "numeric", nullable: true),
                    InicioVigencia = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    FimVigencia = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    PropostaStatusEndossoEmitidoEntity_Observacoes = table.Column<string>(type: "text", nullable: true),
                    PropostaStatusEndossoPendenciaEntity_Acao = table.Column<string>(type: "text", nullable: true),
                    PropostaStatusEndossoPendenciaEntity_UploadArquivo = table.Column<string>(type: "text", nullable: true),
                    PropostaStatusEndossoPendenciaEntity_UploadArquivoNome = table.Column<string>(type: "text", nullable: true),
                    DescricaoPendencia = table.Column<string>(type: "text", nullable: true),
                    PropostaStatusEndossoPendenciaEntity_TipoDocumento = table.Column<string>(type: "text", nullable: true),
                    PropostaStatusEndossoSolicitacaoEntity_Acao = table.Column<string>(type: "text", nullable: true),
                    PropostaStatusEndossoSolicitacaoEntity_UploadArquivo = table.Column<string>(type: "text", nullable: true),
                    PropostaStatusEndossoSolicitacaoEntity_UploadArquivoNome = table.Column<string>(type: "text", nullable: true),
                    DescricaoAlteracao = table.Column<string>(type: "text", nullable: true),
                    PropostaStatusEndossoSolicitacaoEntity_RetornoPendencia = table.Column<string>(type: "text", nullable: true),
                    PropostaStatusEndossoTransmitidoEntity_Acao = table.Column<string>(type: "text", nullable: true),
                    NumeroEndosso = table.Column<string>(type: "text", nullable: true),
                    PropostaStatusEndossoTransmitidoEntity_UploadArquivo = table.Column<string>(type: "text", nullable: true),
                    PropostaStatusEndossoTransmitidoEntity_UploadArquivoNome = table.Column<string>(type: "text", nullable: true),
                    PropostaStatusEndossoTransmitidoEntity_TipoDocumento = table.Column<string>(type: "text", nullable: true),
                    PropostaStatusEndossoTransmitidoEntity_Observacoes = table.Column<string>(type: "text", nullable: true),
                    Justificativa = table.Column<string>(type: "text", nullable: true),
                    PropostaStatusPendenciaEntity_UploadArquivo = table.Column<string>(type: "text", nullable: true),
                    PropostaStatusPendenciaEntity_UploadArquivoNome = table.Column<string>(type: "text", nullable: true),
                    PropostaStatusPendenciaEntity_RetornoPendencia = table.Column<string>(type: "text", nullable: true),
                    PropostaStatusPendenciaEntity_TipoDocumento = table.Column<string>(type: "text", nullable: true),
                    PropostaStatusSinistroAbertoRegulacaoEntity_Acao = table.Column<string>(type: "text", nullable: true),
                    PropostaStatusSinistroAbertoRegulacaoEntity_UploadArquivo = table.Column<string>(type: "text", nullable: true),
                    PropostaStatusSinistroAbertoRegulacaoEntity_UploadArquivoNome = table.Column<string>(type: "text", nullable: true),
                    PropostaStatusSinistroAbertoRegulacaoEntity_TipoDocumento = table.Column<string>(type: "text", nullable: true),
                    ProtocoloAvisoSinistro = table.Column<string>(type: "text", nullable: true),
                    DataAvisoSinistro = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    EmpresaPerito = table.Column<string>(type: "text", nullable: true),
                    Telefone = table.Column<string>(type: "text", nullable: true),
                    PropostaStatusSinistroAbertoRegulacaoEntity_Observacoes = table.Column<string>(type: "text", nullable: true),
                    PropostaStatusSinistroAguardPagamentoEntity_Acao = table.Column<string>(type: "text", nullable: true),
                    PropostaStatusSinistroAguardPagamentoEntity_UploadArquivo = table.Column<string>(type: "text", nullable: true),
                    PropostaStatusSinistroAguardPagamentoEntity_UploadArquivoNome = table.Column<string>(type: "text", nullable: true),
                    PropostaStatusSinistroAguardPagamentoEntity_TipoDocumento = table.Column<string>(type: "text", nullable: true),
                    DataDeferimento = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    ValorIndenizacao = table.Column<decimal>(type: "numeric", nullable: true),
                    PropostaStatusSinistroAguardPagamentoEntity_Observacoes = table.Column<string>(type: "text", nullable: true),
                    PropostaStatusSinistroCanceladoEntity_Acao = table.Column<string>(type: "text", nullable: true),
                    MotivoCancelamento = table.Column<string>(type: "text", nullable: true),
                    PropostaStatusSinistroComunicarEntity_Acao = table.Column<string>(type: "text", nullable: true),
                    Cobertura = table.Column<string>(type: "text", nullable: true),
                    Evento = table.Column<string>(type: "text", nullable: true),
                    SeveridadeDano = table.Column<string>(type: "text", nullable: true),
                    DanoEstimado = table.Column<decimal>(type: "numeric", nullable: true),
                    AreaTotalAfetada = table.Column<decimal>(type: "numeric", nullable: true),
                    DataInicio = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    DataFinal = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    TipoRespVistoria = table.Column<string>(type: "text", nullable: true),
                    NomeRespVistoria = table.Column<string>(type: "text", nullable: true),
                    CPFRespVistoria = table.Column<string>(type: "text", nullable: true),
                    Observacao = table.Column<string>(type: "text", nullable: true),
                    PropostaStatusSinistroDeferidoPagoEntity_Acao = table.Column<string>(type: "text", nullable: true),
                    DataPagamento = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    PropostaStatusSinistroDeferidoPagoEntity_Observacoes = table.Column<string>(type: "text", nullable: true),
                    PropostaStatusSinistroIndeferidoEntity_Acao = table.Column<string>(type: "text", nullable: true),
                    PropostaStatusSinistroIndeferidoEntity_UploadArquivo = table.Column<string>(type: "text", nullable: true),
                    PropostaStatusSinistroIndeferidoEntity_UploadArquivoNome = table.Column<string>(type: "text", nullable: true),
                    PropostaStatusSinistroIndeferidoEntity_TipoDocumento = table.Column<string>(type: "text", nullable: true),
                    DataIndeferimento = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    Observacoes = table.Column<string>(type: "text", nullable: true),
                    Acao = table.Column<string>(type: "text", nullable: true),
                    UploadArquivo = table.Column<string>(type: "text", nullable: true),
                    UploadArquivoNome = table.Column<string>(type: "text", nullable: true),
                    RetornoPendencia = table.Column<string>(type: "text", nullable: true),
                    TipoDocumento = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PropostasStatus", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PropostasStatus_AspNetUsers_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_PropostasStatus_Propostas_PropostaId",
                        column: x => x.PropostaId,
                        principalTable: "Propostas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PropostasTipoSolo",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    PropostaId = table.Column<Guid>(type: "uuid", nullable: false),
                    TipoSolo = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PropostasTipoSolo", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PropostasTipoSolo_Propostas_PropostaId",
                        column: x => x.PropostaId,
                        principalTable: "Propostas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PropostasClientePropriedadesTalhoes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    PropostasClientePropriedadeId = table.Column<Guid>(type: "uuid", nullable: false),
                    TalhaoId = table.Column<Guid>(type: "uuid", nullable: false),
                    Nome = table.Column<string>(type: "text", nullable: true),
                    Area = table.Column<decimal>(type: "numeric", nullable: true),
                    Latitude = table.Column<string>(type: "text", nullable: true),
                    Longitude = table.Column<string>(type: "text", nullable: true),
                    RoteiroAcesso = table.Column<string>(type: "text", nullable: true),
                    PossuiAnaliseFisicaSolo = table.Column<bool>(type: "boolean", nullable: false),
                    PercentualAreia = table.Column<decimal>(type: "numeric", nullable: true),
                    PercentualSilte = table.Column<decimal>(type: "numeric", nullable: true),
                    PercentualArgila = table.Column<decimal>(type: "numeric", nullable: true),
                    TipoSolo = table.Column<string>(type: "text", nullable: true),
                    ClassificacaoSolo = table.Column<string>(type: "text", nullable: true),
                    ImagemTalhao = table.Column<string>(type: "text", nullable: true),
                    KmlTalhao = table.Column<string>(type: "text", nullable: true),
                    DataPlantio = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    GrupoVariedadeId = table.Column<Guid>(type: "uuid", nullable: true),
                    VariedadeId = table.Column<Guid>(type: "uuid", nullable: true),
                    PropriedadesId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PropostasClientePropriedadesTalhoes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PropostasClientePropriedadesTalhoes_CulturaMaturacaoVarieda~",
                        column: x => x.VariedadeId,
                        principalTable: "CulturaMaturacaoVariedades",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_PropostasClientePropriedadesTalhoes_CulturaMaturacao_GrupoV~",
                        column: x => x.GrupoVariedadeId,
                        principalTable: "CulturaMaturacao",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_PropostasClientePropriedadesTalhoes_PropostasClienteProprie~",
                        column: x => x.PropriedadesId,
                        principalTable: "PropostasClientePropriedades",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_PropostasClientePropriedadesTalhoes_Talhoes_TalhaoId",
                        column: x => x.TalhaoId,
                        principalTable: "Talhoes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PropostasQuestionarioBancos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    QuestionarioId = table.Column<Guid>(type: "uuid", nullable: false),
                    BeneficiarioId = table.Column<Guid>(type: "uuid", nullable: false),
                    NumeroCreditoCedula = table.Column<string>(type: "text", nullable: true),
                    DataVencimento = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PropostasQuestionarioBancos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PropostasQuestionarioBancos_PropostasBeneficiarios_Benefici~",
                        column: x => x.BeneficiarioId,
                        principalTable: "PropostasBeneficiarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PropostasQuestionarioBancos_PropostasQuestionario_Questiona~",
                        column: x => x.QuestionarioId,
                        principalTable: "PropostasQuestionario",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PropostasQuestionarioFamiliar",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    QuestionarioId = table.Column<Guid>(type: "uuid", nullable: false),
                    ClienteId = table.Column<Guid>(type: "uuid", nullable: false),
                    VinculoFamiliarId = table.Column<Guid>(type: "uuid", nullable: true),
                    Nome = table.Column<string>(type: "text", nullable: true),
                    CPF = table.Column<string>(type: "text", nullable: true),
                    Telefone = table.Column<string>(type: "text", nullable: true),
                    Email = table.Column<string>(type: "text", nullable: true),
                    Relacao = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PropostasQuestionarioFamiliar", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PropostasQuestionarioFamiliar_Clientes_ClienteId",
                        column: x => x.ClienteId,
                        principalTable: "Clientes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PropostasQuestionarioFamiliar_PropostasQuestionario_Questio~",
                        column: x => x.QuestionarioId,
                        principalTable: "PropostasQuestionario",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PropostasQuestionarioFamiliar_VinculosFamiliares_VinculoFam~",
                        column: x => x.VinculoFamiliarId,
                        principalTable: "VinculosFamiliares",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "PropostasVistoria",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    PropostaId = table.Column<Guid>(type: "uuid", nullable: false),
                    PropostasSeguradoId = table.Column<Guid>(type: "uuid", nullable: true),
                    Nome = table.Column<string>(type: "text", nullable: true),
                    CPF = table.Column<string>(type: "text", nullable: true),
                    Telefone = table.Column<string>(type: "text", nullable: true),
                    PropostasSeguradosId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PropostasVistoria", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PropostasVistoria_PropostasSegurados_PropostasSeguradosId",
                        column: x => x.PropostasSeguradosId,
                        principalTable: "PropostasSegurados",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_PropostasVistoria_Propostas_PropostaId",
                        column: x => x.PropostaId,
                        principalTable: "Propostas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UsuarioEntityId",
                table: "AspNetUserClaims",
                column: "UsuarioEntityId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Canais_CorretoraId",
                table: "Canais",
                column: "CorretoraId");

            migrationBuilder.CreateIndex(
                name: "IX_CotacaoCoberturas_BemId",
                table: "CotacaoCoberturas",
                column: "BemId");

            migrationBuilder.CreateIndex(
                name: "IX_CotacaoCoberturas_CotacaoId",
                table: "CotacaoCoberturas",
                column: "CotacaoId");

            migrationBuilder.CreateIndex(
                name: "IX_CotacaoCondicoesComerciais_CotacaoId",
                table: "CotacaoCondicoesComerciais",
                column: "CotacaoId");

            migrationBuilder.CreateIndex(
                name: "IX_CotacaoFormulariosRiscos_BemId",
                table: "CotacaoFormulariosRiscos",
                column: "BemId");

            migrationBuilder.CreateIndex(
                name: "IX_CotacaoFormulariosRiscos_CotacaoId",
                table: "CotacaoFormulariosRiscos",
                column: "CotacaoId");

            migrationBuilder.CreateIndex(
                name: "IX_CotacaoInformacaoBeneficiarios_CotacaoId",
                table: "CotacaoInformacaoBeneficiarios",
                column: "CotacaoId");

            migrationBuilder.CreateIndex(
                name: "IX_CotacaoInformacaoBens_CotacaoId",
                table: "CotacaoInformacaoBens",
                column: "CotacaoId");

            migrationBuilder.CreateIndex(
                name: "IX_CotacaoInformacaoSegurados_CotacaoId",
                table: "CotacaoInformacaoSegurados",
                column: "CotacaoId");

            migrationBuilder.CreateIndex(
                name: "IX_CotacaoInformacaoSeguros_CotacaoId",
                table: "CotacaoInformacaoSeguros",
                column: "CotacaoId");

            migrationBuilder.CreateIndex(
                name: "IX_CotacaoRetornoSeguradoras_CotacaoId",
                table: "CotacaoRetornoSeguradoras",
                column: "CotacaoId");

            migrationBuilder.CreateIndex(
                name: "IX_CotacoesAgricola_CanalId",
                table: "CotacoesAgricola",
                column: "CanalId");

            migrationBuilder.CreateIndex(
                name: "IX_CotacoesAgricola_ClienteId",
                table: "CotacoesAgricola",
                column: "ClienteId");

            migrationBuilder.CreateIndex(
                name: "IX_CotacoesAgricola_CorretoraId",
                table: "CotacoesAgricola",
                column: "CorretoraId");

            migrationBuilder.CreateIndex(
                name: "IX_CotacoesAgricola_CulturaId",
                table: "CotacoesAgricola",
                column: "CulturaId");

            migrationBuilder.CreateIndex(
                name: "IX_CotacoesAgricola_PontoAtendimentoId",
                table: "CotacoesAgricola",
                column: "PontoAtendimentoId");

            migrationBuilder.CreateIndex(
                name: "IX_CotacoesAgricola_SafraId",
                table: "CotacoesAgricola",
                column: "SafraId");

            migrationBuilder.CreateIndex(
                name: "IX_CotacoesAgricola_UsuarioId",
                table: "CotacoesAgricola",
                column: "UsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_CotacoesAgricolaClassificacaoSolo_CotacoesAgricolaId",
                table: "CotacoesAgricolaClassificacaoSolo",
                column: "CotacoesAgricolaId");

            migrationBuilder.CreateIndex(
                name: "IX_CotacoesAgricolaProposta_CotacoesAgricolaId",
                table: "CotacoesAgricolaProposta",
                column: "CotacoesAgricolaId");

            migrationBuilder.CreateIndex(
                name: "IX_CotacoesAgricolaProposta_ProdutoId",
                table: "CotacoesAgricolaProposta",
                column: "ProdutoId");

            migrationBuilder.CreateIndex(
                name: "IX_CotacoesAgricolaProposta_SeguradoraId",
                table: "CotacoesAgricolaProposta",
                column: "SeguradoraId");

            migrationBuilder.CreateIndex(
                name: "IX_CotacoesAgricolaStatus_CotacoesAgricolaId",
                table: "CotacoesAgricolaStatus",
                column: "CotacoesAgricolaId");

            migrationBuilder.CreateIndex(
                name: "IX_CotacoesAgricolaTipoSolo_CotacoesAgricolaId",
                table: "CotacoesAgricolaTipoSolo",
                column: "CotacoesAgricolaId");

            migrationBuilder.CreateIndex(
                name: "IX_CotacoesCondicaoComercialSeguradoras_CotacaoId",
                table: "CotacoesCondicaoComercialSeguradoras",
                column: "CotacaoId");

            migrationBuilder.CreateIndex(
                name: "IX_CotacoesPublicacoesRetornoJob_CotacaoId",
                table: "CotacoesPublicacoesRetornoJob",
                column: "CotacaoId");

            migrationBuilder.CreateIndex(
                name: "IX_CulturaMaturacao_CulturaId",
                table: "CulturaMaturacao",
                column: "CulturaId");

            migrationBuilder.CreateIndex(
                name: "IX_CulturaMaturacaoVariedades_CulturaMaturacaoId",
                table: "CulturaMaturacaoVariedades",
                column: "CulturaMaturacaoId");

            migrationBuilder.CreateIndex(
                name: "IX_PontosAtendimento_CanalId",
                table: "PontosAtendimento",
                column: "CanalId");

            migrationBuilder.CreateIndex(
                name: "IX_Produtos_CulturaId",
                table: "Produtos",
                column: "CulturaId");

            migrationBuilder.CreateIndex(
                name: "IX_Produtos_SafraId",
                table: "Produtos",
                column: "SafraId");

            migrationBuilder.CreateIndex(
                name: "IX_Produtos_SeguradoraId",
                table: "Produtos",
                column: "SeguradoraId");

            migrationBuilder.CreateIndex(
                name: "IX_Produtos_SubvencaoEstadualId",
                table: "Produtos",
                column: "SubvencaoEstadualId");

            migrationBuilder.CreateIndex(
                name: "IX_Produtos_SubvencaoFederalId",
                table: "Produtos",
                column: "SubvencaoFederalId");

            migrationBuilder.CreateIndex(
                name: "IX_ProdutosCanalPontosAtendimento_CanalId",
                table: "ProdutosCanalPontosAtendimento",
                column: "CanalId");

            migrationBuilder.CreateIndex(
                name: "IX_ProdutosCanalPontosAtendimento_PontoAtendimentoId",
                table: "ProdutosCanalPontosAtendimento",
                column: "PontoAtendimentoId");

            migrationBuilder.CreateIndex(
                name: "IX_ProdutosCanalPontosAtendimento_ProdutoId",
                table: "ProdutosCanalPontosAtendimento",
                column: "ProdutoId");

            migrationBuilder.CreateIndex(
                name: "IX_ProdutosSubvencoesEstaduais_ProdutoId",
                table: "ProdutosSubvencoesEstaduais",
                column: "ProdutoId");

            migrationBuilder.CreateIndex(
                name: "IX_ProdutosSubvencoesEstaduais_SubvencaoEstadualId",
                table: "ProdutosSubvencoesEstaduais",
                column: "SubvencaoEstadualId");

            migrationBuilder.CreateIndex(
                name: "IX_ProdutosTaxas_ProdutoId",
                table: "ProdutosTaxas",
                column: "ProdutoId");

            migrationBuilder.CreateIndex(
                name: "IX_Propostas_CanalId",
                table: "Propostas",
                column: "CanalId");

            migrationBuilder.CreateIndex(
                name: "IX_Propostas_ClienteId",
                table: "Propostas",
                column: "ClienteId");

            migrationBuilder.CreateIndex(
                name: "IX_Propostas_CorretoraId",
                table: "Propostas",
                column: "CorretoraId");

            migrationBuilder.CreateIndex(
                name: "IX_Propostas_CotacoesAgricolaId",
                table: "Propostas",
                column: "CotacoesAgricolaId");

            migrationBuilder.CreateIndex(
                name: "IX_Propostas_CulturaId",
                table: "Propostas",
                column: "CulturaId");

            migrationBuilder.CreateIndex(
                name: "IX_Propostas_PontoAtendimentoId",
                table: "Propostas",
                column: "PontoAtendimentoId");

            migrationBuilder.CreateIndex(
                name: "IX_Propostas_SafraId",
                table: "Propostas",
                column: "SafraId");

            migrationBuilder.CreateIndex(
                name: "IX_Propostas_UsuarioId",
                table: "Propostas",
                column: "UsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_PropostasBeneficiarios_BeneficiarioId",
                table: "PropostasBeneficiarios",
                column: "BeneficiarioId");

            migrationBuilder.CreateIndex(
                name: "IX_PropostasBeneficiarios_ClienteId",
                table: "PropostasBeneficiarios",
                column: "ClienteId");

            migrationBuilder.CreateIndex(
                name: "IX_PropostasBeneficiarios_PropostaId",
                table: "PropostasBeneficiarios",
                column: "PropostaId");

            migrationBuilder.CreateIndex(
                name: "IX_PropostasClassificacaoSolo_PropostaId",
                table: "PropostasClassificacaoSolo",
                column: "PropostaId");

            migrationBuilder.CreateIndex(
                name: "IX_PropostasClientePropriedades_ClienteId",
                table: "PropostasClientePropriedades",
                column: "ClienteId");

            migrationBuilder.CreateIndex(
                name: "IX_PropostasClientePropriedades_PropostaId",
                table: "PropostasClientePropriedades",
                column: "PropostaId");

            migrationBuilder.CreateIndex(
                name: "IX_PropostasClientePropriedades_PropriedadeId",
                table: "PropostasClientePropriedades",
                column: "PropriedadeId");

            migrationBuilder.CreateIndex(
                name: "IX_PropostasClientePropriedadesTalhoes_GrupoVariedadeId",
                table: "PropostasClientePropriedadesTalhoes",
                column: "GrupoVariedadeId");

            migrationBuilder.CreateIndex(
                name: "IX_PropostasClientePropriedadesTalhoes_PropriedadesId",
                table: "PropostasClientePropriedadesTalhoes",
                column: "PropriedadesId");

            migrationBuilder.CreateIndex(
                name: "IX_PropostasClientePropriedadesTalhoes_TalhaoId",
                table: "PropostasClientePropriedadesTalhoes",
                column: "TalhaoId");

            migrationBuilder.CreateIndex(
                name: "IX_PropostasClientePropriedadesTalhoes_VariedadeId",
                table: "PropostasClientePropriedadesTalhoes",
                column: "VariedadeId");

            migrationBuilder.CreateIndex(
                name: "IX_PropostasDocumentos_PropostaId",
                table: "PropostasDocumentos",
                column: "PropostaId");

            migrationBuilder.CreateIndex(
                name: "IX_PropostasDocumentos_UsuarioId",
                table: "PropostasDocumentos",
                column: "UsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_PropostasFormaPagamentos_ProdutoId",
                table: "PropostasFormaPagamentos",
                column: "ProdutoId");

            migrationBuilder.CreateIndex(
                name: "IX_PropostasFormaPagamentos_PropostaId",
                table: "PropostasFormaPagamentos",
                column: "PropostaId");

            migrationBuilder.CreateIndex(
                name: "IX_PropostasObservacoes_PropostaId",
                table: "PropostasObservacoes",
                column: "PropostaId");

            migrationBuilder.CreateIndex(
                name: "IX_PropostasObservacoes_UsuarioId",
                table: "PropostasObservacoes",
                column: "UsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_PropostasOcorrencias_PropostaId",
                table: "PropostasOcorrencias",
                column: "PropostaId");

            migrationBuilder.CreateIndex(
                name: "IX_PropostasOcorrencias_UsuarioId",
                table: "PropostasOcorrencias",
                column: "UsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_PropostasProdutos_CotacoesAgricolaPropostaId",
                table: "PropostasProdutos",
                column: "CotacoesAgricolaPropostaId");

            migrationBuilder.CreateIndex(
                name: "IX_PropostasProdutos_ProdutoId",
                table: "PropostasProdutos",
                column: "ProdutoId");

            migrationBuilder.CreateIndex(
                name: "IX_PropostasProdutos_PropostaId",
                table: "PropostasProdutos",
                column: "PropostaId");

            migrationBuilder.CreateIndex(
                name: "IX_PropostasProdutos_SeguradoraId",
                table: "PropostasProdutos",
                column: "SeguradoraId");

            migrationBuilder.CreateIndex(
                name: "IX_PropostasQuestionario_CulturaAnteriorId",
                table: "PropostasQuestionario",
                column: "CulturaAnteriorId");

            migrationBuilder.CreateIndex(
                name: "IX_PropostasQuestionario_PropostaId",
                table: "PropostasQuestionario",
                column: "PropostaId");

            migrationBuilder.CreateIndex(
                name: "IX_PropostasQuestionarioBancos_BeneficiarioId",
                table: "PropostasQuestionarioBancos",
                column: "BeneficiarioId");

            migrationBuilder.CreateIndex(
                name: "IX_PropostasQuestionarioBancos_QuestionarioId",
                table: "PropostasQuestionarioBancos",
                column: "QuestionarioId");

            migrationBuilder.CreateIndex(
                name: "IX_PropostasQuestionarioFamiliar_ClienteId",
                table: "PropostasQuestionarioFamiliar",
                column: "ClienteId");

            migrationBuilder.CreateIndex(
                name: "IX_PropostasQuestionarioFamiliar_QuestionarioId",
                table: "PropostasQuestionarioFamiliar",
                column: "QuestionarioId");

            migrationBuilder.CreateIndex(
                name: "IX_PropostasQuestionarioFamiliar_VinculoFamiliarId",
                table: "PropostasQuestionarioFamiliar",
                column: "VinculoFamiliarId");

            migrationBuilder.CreateIndex(
                name: "IX_PropostasSegurados_ClienteId",
                table: "PropostasSegurados",
                column: "ClienteId");

            migrationBuilder.CreateIndex(
                name: "IX_PropostasSegurados_PropostaId",
                table: "PropostasSegurados",
                column: "PropostaId");

            migrationBuilder.CreateIndex(
                name: "IX_PropostasSegurados_VinculoFamiliarId",
                table: "PropostasSegurados",
                column: "VinculoFamiliarId");

            migrationBuilder.CreateIndex(
                name: "IX_PropostasStatus_PropostaId",
                table: "PropostasStatus",
                column: "PropostaId");

            migrationBuilder.CreateIndex(
                name: "IX_PropostasStatus_UsuarioId",
                table: "PropostasStatus",
                column: "UsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_PropostasTipoSolo_PropostaId",
                table: "PropostasTipoSolo",
                column: "PropostaId");

            migrationBuilder.CreateIndex(
                name: "IX_PropostasVistoria_PropostaId",
                table: "PropostasVistoria",
                column: "PropostaId");

            migrationBuilder.CreateIndex(
                name: "IX_PropostasVistoria_PropostasSeguradosId",
                table: "PropostasVistoria",
                column: "PropostasSeguradosId");

            migrationBuilder.CreateIndex(
                name: "IX_UsuarioEstruturaNegocio_CanalId",
                table: "UsuarioEstruturaNegocio",
                column: "CanalId");

            migrationBuilder.CreateIndex(
                name: "IX_UsuarioEstruturaNegocio_CorretoraId",
                table: "UsuarioEstruturaNegocio",
                column: "CorretoraId");

            migrationBuilder.CreateIndex(
                name: "IX_UsuarioEstruturaNegocio_PontoAtendimentoId",
                table: "UsuarioEstruturaNegocio",
                column: "PontoAtendimentoId");

            migrationBuilder.CreateIndex(
                name: "IX_UsuarioEstruturaNegocio_UsuarioCriadorId",
                table: "UsuarioEstruturaNegocio",
                column: "UsuarioCriadorId");

            migrationBuilder.CreateIndex(
                name: "IX_UsuarioEstruturaNegocio_UsuarioId",
                table: "UsuarioEstruturaNegocio",
                column: "UsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_UsuariosCanal_CanalId",
                table: "UsuariosCanal",
                column: "CanalId");

            migrationBuilder.CreateIndex(
                name: "IX_UsuariosCanal_LideradoId",
                table: "UsuariosCanal",
                column: "LideradoId");

            migrationBuilder.CreateIndex(
                name: "IX_UsuariosCanal_UsuarioId",
                table: "UsuariosCanal",
                column: "UsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_UsuariosPontoAtendimento_CanalId",
                table: "UsuariosPontoAtendimento",
                column: "CanalId");

            migrationBuilder.CreateIndex(
                name: "IX_UsuariosPontoAtendimento_LideradoId",
                table: "UsuariosPontoAtendimento",
                column: "LideradoId");

            migrationBuilder.CreateIndex(
                name: "IX_UsuariosPontoAtendimento_PontoAtendimentoId",
                table: "UsuariosPontoAtendimento",
                column: "PontoAtendimentoId");

            migrationBuilder.CreateIndex(
                name: "IX_UsuariosPontoAtendimento_UsuarioId",
                table: "UsuariosPontoAtendimento",
                column: "UsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_VinculosFamiliares_ClienteId",
                table: "VinculosFamiliares",
                column: "ClienteId");

            migrationBuilder.CreateIndex(
                name: "IX_VinculosPropriedadesClientes_PropriedadeId",
                table: "VinculosPropriedadesClientes",
                column: "PropriedadeId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "Bancos");

            migrationBuilder.DropTable(
                name: "CotacaoCoberturas");

            migrationBuilder.DropTable(
                name: "CotacaoCondicoesComerciais");

            migrationBuilder.DropTable(
                name: "CotacaoFormulariosRiscos");

            migrationBuilder.DropTable(
                name: "CotacaoInformacaoBeneficiarios");

            migrationBuilder.DropTable(
                name: "CotacaoInformacaoBens");

            migrationBuilder.DropTable(
                name: "CotacaoInformacaoSegurados");

            migrationBuilder.DropTable(
                name: "CotacaoInformacaoSeguros");

            migrationBuilder.DropTable(
                name: "CotacaoRetornoSeguradoras");

            migrationBuilder.DropTable(
                name: "Cotacoes");

            migrationBuilder.DropTable(
                name: "CotacoesAgricolaClassificacaoSolo");

            migrationBuilder.DropTable(
                name: "CotacoesAgricolaStatus");

            migrationBuilder.DropTable(
                name: "CotacoesAgricolaTipoSolo");

            migrationBuilder.DropTable(
                name: "CotacoesCondicaoComercialSeguradoras");

            migrationBuilder.DropTable(
                name: "CotacoesPublicacoesRetornoJob");

            migrationBuilder.DropTable(
                name: "CulturasPrePlantio");

            migrationBuilder.DropTable(
                name: "EmailServerSetting");

            migrationBuilder.DropTable(
                name: "Equipamentos");

            migrationBuilder.DropTable(
                name: "IdentityUserLogin<string>");

            migrationBuilder.DropTable(
                name: "LoginSeguradoras");

            migrationBuilder.DropTable(
                name: "Marcas");

            migrationBuilder.DropTable(
                name: "MicrosoftAD");

            migrationBuilder.DropTable(
                name: "Modelos");

            migrationBuilder.DropTable(
                name: "ParametrizacaoBeneficiario");

            migrationBuilder.DropTable(
                name: "ParametrizacaoCustomizacaoRelatorio");

            migrationBuilder.DropTable(
                name: "ParametrizacaoMultiCalculoParceiro");

            migrationBuilder.DropTable(
                name: "ParametrizacaoRisco");

            migrationBuilder.DropTable(
                name: "ParametrizacaoSeguradora");

            migrationBuilder.DropTable(
                name: "ProdutosCanalPontosAtendimento");

            migrationBuilder.DropTable(
                name: "ProdutosSubvencoesEstaduais");

            migrationBuilder.DropTable(
                name: "ProdutosTaxas");

            migrationBuilder.DropTable(
                name: "PropostasClassificacaoSolo");

            migrationBuilder.DropTable(
                name: "PropostasClientePropriedadesTalhoes");

            migrationBuilder.DropTable(
                name: "PropostasDocumentos");

            migrationBuilder.DropTable(
                name: "PropostasFormaPagamentos");

            migrationBuilder.DropTable(
                name: "PropostasObservacoes");

            migrationBuilder.DropTable(
                name: "PropostasOcorrencias");

            migrationBuilder.DropTable(
                name: "PropostasProdutos");

            migrationBuilder.DropTable(
                name: "PropostasQuestionarioBancos");

            migrationBuilder.DropTable(
                name: "PropostasQuestionarioFamiliar");

            migrationBuilder.DropTable(
                name: "PropostasStatus");

            migrationBuilder.DropTable(
                name: "PropostasTipoSolo");

            migrationBuilder.DropTable(
                name: "PropostasVistoria");

            migrationBuilder.DropTable(
                name: "SeguradoraAnterior");

            migrationBuilder.DropTable(
                name: "UsuarioEstruturaNegocio");

            migrationBuilder.DropTable(
                name: "UsuariosCanal");

            migrationBuilder.DropTable(
                name: "UsuariosPontoAtendimento");

            migrationBuilder.DropTable(
                name: "VinculosPropriedadesClientes");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "CulturaMaturacaoVariedades");

            migrationBuilder.DropTable(
                name: "PropostasClientePropriedades");

            migrationBuilder.DropTable(
                name: "Talhoes");

            migrationBuilder.DropTable(
                name: "CotacoesAgricolaProposta");

            migrationBuilder.DropTable(
                name: "PropostasBeneficiarios");

            migrationBuilder.DropTable(
                name: "PropostasQuestionario");

            migrationBuilder.DropTable(
                name: "PropostasSegurados");

            migrationBuilder.DropTable(
                name: "CulturaMaturacao");

            migrationBuilder.DropTable(
                name: "Propriedades");

            migrationBuilder.DropTable(
                name: "Produtos");

            migrationBuilder.DropTable(
                name: "Beneficiarios");

            migrationBuilder.DropTable(
                name: "Propostas");

            migrationBuilder.DropTable(
                name: "VinculosFamiliares");

            migrationBuilder.DropTable(
                name: "Seguradoras");

            migrationBuilder.DropTable(
                name: "SubvencoesEstaduais");

            migrationBuilder.DropTable(
                name: "SubvencoesFederais");

            migrationBuilder.DropTable(
                name: "CotacoesAgricola");

            migrationBuilder.DropTable(
                name: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "Clientes");

            migrationBuilder.DropTable(
                name: "Culturas");

            migrationBuilder.DropTable(
                name: "PontosAtendimento");

            migrationBuilder.DropTable(
                name: "Safras");

            migrationBuilder.DropTable(
                name: "Canais");

            migrationBuilder.DropTable(
                name: "Corretoras");
        }
    }
}
