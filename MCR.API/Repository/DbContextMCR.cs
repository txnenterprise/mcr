using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MCR.API.Entities;

namespace MCR.API.Repository
{
    public class DbContextMCR : IdentityDbContext<UsuarioEntity, IdentityRole<Guid>, Guid>
    {
        public DbContextMCR(DbContextOptions<DbContextMCR> options) : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
        }

        /*Multi-Cálculo*/
        public DbSet<CorretoraEntity> CorretoraEntity { get; set; }
        public DbSet<ParametrizacaoSeguradoraEntity> ParametrizacaoSeguradoraEntity { get; set; }
        public DbSet<ParametrizacaoBeneficiarioEntity> ParametrizacaoBeneficiarioEntity { get; set; }
        public DbSet<ParametrizacaoRiscoEntity> ParametrizacaoRiscoEntity { get; set; }
        public DbSet<ParametrizacaoCustomizacaoRelatorioEntity> ParametrizacaoCustomizacaoRelatorioEntity { get; set; }
        public DbSet<CotacaoInformacaoSeguroEntity> CotacaoInformacaoSeguroEntity { get; set; }
        public DbSet<CotacaoInformacaoSeguradoEntity> CotacaoInformacaoSeguradoEntity { get; set; }
        public DbSet<CotacaoInformacaoBemEntity> CotacaoInformacaoBemEntity { get; set; }
        public DbSet<CotacaoFormularioRiscoEntity> CotacaoFormularioRiscoEntity { get; set; }
        public DbSet<CotacaoCoberturaEntity> CotacaoCoberturaEntity { get; set; }
        public DbSet<CotacaoCondicaoComercialEntity> CotacaoCondicaoComercialEntity { get; set; }
        public DbSet<CotacaoInformacoesBeneficiarioEntity> CotacaoInformacoesBeneficiarioEntity { get; set; }
        public DbSet<SeguradoraAnteriorEntity> SeguradoraAnteriorEntity { get; set; }
        public DbSet<SeguradoraEntity> SeguradoraEntity { get; set; }
        public DbSet<CotacaoEntity> CotacaoEntity { get; set; }
        public DbSet<BancoEntity> BancoEntity { get; set; }
        public DbSet<EquipamentoEntity> EquipamentoEntity { get; set; }
        public DbSet<MarcaEntity> MarcaEntity { get; set; }
        public DbSet<ModeloEntity> ModeloEntity { get; set; }
        public DbSet<CotacaoCondicaoComercialSeguradoraEntity> CotacaoCondicaoComercialSeguradoraEntity { get; set; }
        public DbSet<CotacaoPublicacaoRetornoJobEntity> CotacaoPublicacaoRetornoJobEntity { get; set; }
        public DbSet<CotacaoRetornoSeguradoraEntity> CotacaoRetornoSeguradoraEntity { get; set; }

        public DbSet<LoginSeguradoraEntity> LoginSeguradoraEntity { get; set; }
        public DbSet<ParametrizacaoMultiCalculoParceiroEntity> ParametrizacaoMultiCalculoParceiroEntity { get; set; }
        /*Fim Multi-Cálculo*/

        //Beneficiario
        public DbSet<BeneficiarioEntity> Beneficiarios { get; set; }

        //EstruturaNegocio
        public DbSet<CanalEntity> Canais { get; set; }
        public DbSet<PontoAtendimentoEntity> PontosAtendimento { get; set; }

        //Cotacao
        public DbSet<CotacoesAgricolaEntity> CotacoesAgricola { get; set; }
        public DbSet<CotacoesAgricolaPropostaEntity> CotacoesAgricolaProposta { get; set; }
        public DbSet<CotacoesAgricolaStatusEntity> CotacoesAgricolaStatus { get; set; }
        public DbSet<CotacoesAgricolaTipoSoloEntity> CotacoesAgricolaTipoSolo { get; set; }
        public DbSet<CotacoesAgricolaClassificacaoSoloEntity> CotacoesAgricolaClassificacaoSolo { get; set; }

        // Produto - Entidades Refatoradas
        public DbSet<CulturaPrePlantioEntity> CulturasPrePlantio { get; set; }
        public DbSet<CulturaEntity> Culturas { get; set; }
        public DbSet<CulturaMaturacaoVariedadeEntity> CulturaMaturacaoVariedades { get; set; }
        public DbSet<CulturaMaturacaoEntity> CulturaMaturacao { get; set; }
        public DbSet<ProdutosEntity> Produtos { get; set; } // Novo (antiga ProdutosTaxasEntity)
        public DbSet<ProdutosTaxasEntity> ProdutosTaxas { get; set; } // Renomeado (antiga TaxasEntity)
        public DbSet<ProdutosCanalPontoAtendimentoEntity> ProdutosCanalPontosAtendimento { get; set; } // Novo (antiga CanalSelectionPontoAtendimento)
        public DbSet<SafraEntity> Safras { get; set; }
        public DbSet<SubvencaoEstadualEntity> SubvencoesEstaduais { get; set; }
        public DbSet<SubvencaoFederalEntity> SubvencoesFederais { get; set; }
        public DbSet<ProdutoSubvencaoEstadualEntity> ProdutosSubvencoesEstaduais { get; set; }

        // Propriedade
        public DbSet<ClienteEntity> Clientes { get; set; }
        public DbSet<PropriedadeEntity> Propriedades { get; set; }
        public DbSet<TalhaoEntity> Talhoes { get; set; }
        public DbSet<VinculoFamiliarEntity> VinculosFamiliares { get; set; }
        public DbSet<VinculoPropriedadeClienteEntity> VinculosPropriedadesClientes { get; set; }

        // Usuario
        public DbSet<UsuarioCanalEntity> UsuariosCanal { get; set; }
        public DbSet<UsuarioPontoAtendimentoEntity> UsuariosPontoAtendimento { get; set; }
        public DbSet<UsuarioEntity> Usuarios { get; set; }
        public DbSet<UsuarioEstruturaNegocioEntity> UsuarioEstruturaNegocio { get; set; }



        //Propostas
        public DbSet<PropostasEntity> Propostas { get; set; }
        public DbSet<PropostasProdutosEntity> PropostasProdutos { get; set; }
        public DbSet<PropostasStatusEntity> PropostasStatus { get; set; }
        public DbSet<PropostaStatusJustificativaEntity> PropostaStatusJustificativa { get; set; }
        public DbSet<PropostaStatusPendenciaEntity> PropostaStatusPendencia { get; set; }
        public DbSet<PropostaStatusAceitaEntity> PropostaStatusAceita { get; set; }
        public DbSet<PropostaStatusApoliceEmitidaEntity> PropostaStatusApoliceEmitida { get; set; }
        public DbSet<PropostaStatusEndossoSolicitacaoEntity> PropostaStatusEndossoSolicitacao { get; set; }
        public DbSet<PropostaStatusEndossoPendenciaEntity> PropostaStatusEndossoPendencia { get; set; }
        public DbSet<PropostaStatusEndossoTransmitidoEntity> PropostaStatusEndossoTransmitido { get; set; }
        public DbSet<PropostaStatusEndossoEmitidoEntity> PropostaStatusEndossoEmitido { get; set; }
        public DbSet<PropostaStatusSinistroComunicarEntity> PropostaStatusSinistroComunicar { get; set; }
        public DbSet<PropostaStatusSinistroPendenciaEntity> PropostaStatusSinistroPendencia { get; set; }
        public DbSet<PropostaStatusSinistroCanceladoEntity> PropostaStatusSinistroCancelado { get; set; }
        public DbSet<PropostaStatusSinistroAbertoRegulacaoEntity> PropostaStatusSinistroAbertoRegulacao { get; set; }
        public DbSet<PropostaStatusSinistroAguardPagamentoEntity> PropostaStatusSinistroAguardPagamento { get; set; }
        public DbSet<PropostaStatusSinistroDeferidoPagoEntity> PropostaStatusSinistroDeferidoPago { get; set; }
        public DbSet<PropostaStatusSinistroIndeferidoEntity> PropostaStatusSinistroIndeferido { get; set; }
        public DbSet<PropostasTipoSoloEntity> PropostasTipoSolo { get; set; }
        public DbSet<PropostasClassificacaoSoloEntity> PropostasClassificacaoSolo { get; set; }
        public DbSet<PropostasSeguradosEntity> PropostasSegurados { get; set; }
        public DbSet<PropostasClientePropriedadesEntity> PropostasClientePropriedades { get; set; }
        public DbSet<PropostasVistoriaEntity> PropostasVistoria { get; set; }
        public DbSet<PropostasClientePropriedadesTalhoesEntity> PropostasClientePropriedadesTalhoes { get; set; }
        public DbSet<PropostasBeneficiariosEntity> PropostasBeneficiarios { get; set; }

        public DbSet<PropostasQuestionarioEntity> PropostasQuestionario { get; set; }
        public DbSet<PropostasQuestionarioFamiliarEntity> PropostasQuestionarioFamiliar { get; set; }
        public DbSet<PropostasQuestionarioBancosEntity> PropostasQuestionarioBancos { get; set; }

        public DbSet<PropostasObservacoesEntity> PropostasObservacoes { get; set; }
        public DbSet<PropostasDocumentosEntity> PropostasDocumentos { get; set; }
        public DbSet<PropostasFormaPagamentosEntity> PropostasFormaPagamentos { get; set; }
        public DbSet<PropostasOcorrenciaEntity> PropostasOcorrencias { get; set; }



        //Monitoring
        public DbSet<MonitoringConfigurationEntity> MonitoringConfigurations { get; set; }
        public DbSet<MonitoringExecutionEntity> MonitoringExecutions { get; set; }
        public DbSet<MonitoringAlertEntity> MonitoringAlerts { get; set; }
        public DbSet<SatelliteSceneEntity> SatelliteScenes { get; set; }
        public DbSet<MonitoringAnalysisEntity> MonitoringAnalyses { get; set; }

        //Email
        public DbSet<EmailServerSettingEntity> EmailServerSetting { get; set; }

        //oAuth
        public DbSet<MicrosoftADEntity> MicrosoftAD { get; set; }

        public async Task<int> ExecuteSqlScriptAsync(string sqlScript)
        {
            return await Database.ExecuteSqlRawAsync(sqlScript);
        }
        public async Task<List<Dictionary<string, object>>> ExecuteSqlQueryAsync(string sqlQuery)
        {
            var connection = Database.GetDbConnection();

            try
            {
                await connection.OpenAsync();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = sqlQuery;
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        var result = new List<Dictionary<string, object>>();

                        while (await reader.ReadAsync())
                        {
                            var row = Enumerable.Range(0, reader.FieldCount)
                                                .ToDictionary(reader.GetName, reader.GetValue);

                            result.Add(row);
                        }

                        return result;
                    }
                }
            }
            finally
            {
                await connection.CloseAsync();
            }
        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.HasPostgresExtension("uuid-ossp");

            /*Multi-Cálculo*/

            builder.ApplyConfiguration(new CorretoraMap());
            builder.ApplyConfiguration(new SeguradoraMap());
            builder.ApplyConfiguration(new SeguradoraAnteriorMap());
            builder.ApplyConfiguration(new BancoMap());
            builder.ApplyConfiguration(new EquipamentoMap());
            builder.ApplyConfiguration(new MarcaMap());
            builder.ApplyConfiguration(new ModeloMap());

            builder.ApplyConfiguration(new ParametrizacaoSeguradoraMap());
            builder.ApplyConfiguration(new ParametrizacaoBeneficiarioMap());
            builder.ApplyConfiguration(new ParametrizacaoRiscoMap());
            builder.ApplyConfiguration(new ParametrizacaoCustomizacaoRelatorioMap());
            builder.ApplyConfiguration(new ParametrizacaoMultiCalculoParceiroMap());

            builder.ApplyConfiguration(new CotacaoMap());
            builder.ApplyConfiguration(new CotacaoInformacaoSeguroMap());
            builder.ApplyConfiguration(new CotacaoInformacaoSeguradoMap());
            builder.ApplyConfiguration(new CotacaoInformacaoBemMap());
            builder.ApplyConfiguration(new CotacaoInformacaoBeneficiarioMap());
            builder.ApplyConfiguration(new CotacaoCondicaoComercialMap());
            builder.ApplyConfiguration(new CotacaoFormularioRiscoMap());
            builder.ApplyConfiguration(new CotacaoCoberturaMap());
            builder.ApplyConfiguration(new CotacaoCondicaoComercialSeguradoraMap());
            builder.ApplyConfiguration(new LoginSeguradoraMap());

            builder.ApplyConfiguration(new CotacaoPublicacaoRetornoJobMap());
            builder.ApplyConfiguration(new CotacaoRetornoSeguradoraMap());

            // Monitoring
            builder.Entity<MonitoringConfigurationEntity>(entity =>
            {
                entity.ToTable("MonitoringConfigurations");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
                entity.Property(e => e.PropertyId).IsRequired();
                entity.Property(e => e.Enabled).IsRequired();
                entity.Property(e => e.UpdateFrequency).IsRequired(false);
                entity.Property(e => e.CloudLimit).IsRequired(false);
                entity.Property(e => e.CreatedAt).IsRequired();
                entity.Property(e => e.UpdatedAt).IsRequired(false);
                entity.HasIndex("PropertyId").IsUnique(false);
            });

            builder.Entity<MonitoringExecutionEntity>(entity =>
            {
                entity.ToTable("MonitoringExecutions");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
                entity.Property(e => e.PropertyId).IsRequired();
                entity.Property(e => e.ExecutionDate).IsRequired();
                entity.Property(e => e.Status).IsRequired();
                entity.Property(e => e.Satellite).IsRequired(false);
                entity.Property(e => e.CloudCover).IsRequired(false);
                entity.Property(e => e.AverageVegetationIndex).IsRequired(false);
                entity.Property(e => e.AffectedArea).IsRequired(false);
                entity.Property(e => e.Message).IsRequired(false);
                entity.Property(e => e.JsonStatistics).IsRequired(false);
                entity.Property(e => e.CreatedAt).IsRequired();
                entity.HasIndex("PropertyId").IsUnique(false);
            });

            builder.Entity<MonitoringAlertEntity>(entity =>
            {
                entity.ToTable("MonitoringAlerts");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
                entity.Property(e => e.ExecutionId).IsRequired();
                entity.Property(e => e.Severity).IsRequired();
                entity.Property(e => e.Message).IsRequired();
                entity.Property(e => e.Viewed).IsRequired();
                entity.Property(e => e.CreatedAt).IsRequired();
                entity.HasIndex("ExecutionId").IsUnique(false);
            });

            builder.Entity<UsuarioEntity>()
                .Property(u => u.Id)
                .ValueGeneratedOnAdd()
                .HasDefaultValueSql("uuid_generate_v4()");

            builder.Entity<UsuarioCanalEntity>()
                .HasOne(uc => uc.Usuario)
                .WithMany(u => u.UsuarioCanal)
                .HasForeignKey(uc => uc.UsuarioId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<UsuarioCanalEntity>()
                .HasOne(uc => uc.UsuarioLiderado)
                .WithMany(u => u.UsuarioCanalLiderado)
                .HasForeignKey(uc => uc.LideradoId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<UsuarioCanalEntity>()
                .HasOne(uc => uc.Canal)
                .WithMany(c => c.UsuarioCanal)
                .HasForeignKey(uc => uc.CanalId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<UsuarioPontoAtendimentoEntity>()
                .HasOne(up => up.Usuario)
                .WithMany(u => u.UsuarioPontoAtendimento)
                .HasForeignKey(up => up.UsuarioId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<UsuarioPontoAtendimentoEntity>()
                .HasOne(up => up.Liderado)
                .WithMany(u => u.UsuarioPontoAtendimentoLiderado)
                .HasForeignKey(up => up.LideradoId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<UsuarioPontoAtendimentoEntity>()
                .HasOne(up => up.PontoAtendimento)
                .WithMany(pa => pa.UsuarioPontoAtendimento)
                .HasForeignKey(up => up.PontoAtendimentoId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<UsuarioPontoAtendimentoEntity>()
                .HasOne(up => up.Canal)
                .WithMany(c => c.UsuarioPontoAtendimento)
                .HasForeignKey(up => up.CanalId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<UsuarioEstruturaNegocioEntity>()
                .HasOne(ue => ue.Usuario)
                .WithMany(u => u.UsuarioEstruturaNegocio)
                .HasForeignKey(ue => ue.UsuarioId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<UsuarioEstruturaNegocioEntity>()
                .HasOne(ue => ue.UsuarioCriador)
                .WithMany(u => u.UsuarioEstruturaNegocioCriador)
                .HasForeignKey(ue => ue.UsuarioCriadorId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<UsuarioEstruturaNegocioEntity>()
                .HasOne(ue => ue.Canal)
                .WithMany(c => c.UsuarioEstruturaNegocio)
                .HasForeignKey(ue => ue.CanalId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<UsuarioEstruturaNegocioEntity>()
                .HasOne(ue => ue.PontoAtendimento)
                .WithMany(pa => pa.UsuarioEstruturaNegocio)
                .HasForeignKey(ue => ue.PontoAtendimentoId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configurar chave primária para a entidade IdentityUserLogin<string>
            builder.Entity<IdentityUserLogin<string>>()
                .HasKey(l => new { l.LoginProvider, l.ProviderKey });

            /*Multi-Cálculo*/

            builder.Entity<CorretoraEntity>().ToTable("Corretoras");
            builder.Entity<SeguradoraEntity>().ToTable("Seguradoras");
            builder.ApplyConfigurationsFromAssembly(typeof(DbContextMCR).Assembly);
        }

        /*Multi-Cálculo*/

        public class ParametrizacaoMultiCalculoParceiroMap : IEntityTypeConfiguration<ParametrizacaoMultiCalculoParceiroEntity>
        {
            public void Configure(EntityTypeBuilder<ParametrizacaoMultiCalculoParceiroEntity> builder)
            {
                builder.ToTable("ParametrizacaoMultiCalculoParceiro");
                builder.HasKey(a => a.Id);
                builder.Property(a => a.Id).ValueGeneratedOnAdd();
                builder.Property(a => a.HabilitarMapfre).IsRequired();
                builder.Property(a => a.HabilitarPottencial).IsRequired();
                builder.Property(a => a.HabilitarSompo).IsRequired();
                builder.Property(a => a.HabilitarSwissRe).IsRequired();
                builder.Property(a => a.HabilitarSombrero).IsRequired();
                builder.Property(a => a.HabilitarFairFax).IsRequired();
                builder.Property(a => a.HabilitarAllianz).IsRequired();
                builder.Property(a => a.HabilitarTokio).IsRequired();
                builder.Property(a => a.NomeParceiro).IsRequired();
                builder.Property(a => a.LogoParceiro).IsRequired();
                builder.Property(a => a.CorPredominante).IsRequired();
            }
        }
        public class LoginSeguradoraMap : IEntityTypeConfiguration<LoginSeguradoraEntity>
        {
            public void Configure(EntityTypeBuilder<LoginSeguradoraEntity> builder)
            {
                builder.ToTable("LoginSeguradoras");
                builder.HasKey(a => a.Id);
                builder.Property(a => a.Id).ValueGeneratedOnAdd();
                builder.Property(a => a.Seguradora).IsRequired();
                builder.Property(a => a.CorretorLogado).IsRequired();
                builder.Property(a => a.Link).IsRequired(false);
                builder.Property(a => a.Susep).IsRequired(false);
                builder.Property(a => a.CodigoInterno).IsRequired(false);
                builder.Property(a => a.Usuario).IsRequired(false);
                builder.Property(a => a.Senha).IsRequired(false);
                builder.Property(a => a.Token).IsRequired(false);
                builder.Property(a => a.CorretoraId).IsRequired();
                builder.Property(a => a.ChaveAPIKey).IsRequired(false);
                builder.Property(a => a.ChaveAPIValue).IsRequired(false);
            }
        }
        public class CotacaoRetornoSeguradoraMap : IEntityTypeConfiguration<CotacaoRetornoSeguradoraEntity>
        {
            public void Configure(EntityTypeBuilder<CotacaoRetornoSeguradoraEntity> builder)
            {
                builder.ToTable("CotacaoRetornoSeguradoras");
                builder.HasKey(a => a.Id);
                builder.Property(a => a.Id).ValueGeneratedOnAdd();
                builder.Property(a => a.CotacaoId).IsRequired();
                builder.Property(a => a.NumeroCotacao).IsRequired(false);
                builder.Property(a => a.Seguradora).IsRequired();
                builder.Property(a => a.DataHoraRetorno).IsRequired();
                builder.Property(a => a.Efetivada).IsRequired();
                builder.Property(a => a.DataHoraEfetivacao).IsRequired();
                builder.Property(a => a.CorretorEfetivou).IsRequired(false);
                builder.Property(a => a.FormaPagamento).IsRequired(false);
                builder.Property(a => a.DiaPagamento).IsRequired(false);
                builder.Property(a => a.NumeroParcelas).IsRequired(false);
                builder.Property(a => a.Premio).IsRequired(false);
                builder.Property(a => a.Parcelamento).IsRequired(false);
                builder.Property(a => a.MensagemComplementar).IsRequired(false);
                builder.Property(a => a.JsonRetornoPremioCobertura).IsRequired(false);
                builder.HasIndex("CotacaoId").IsUnique(false);
            }
        }
        public class ModeloMap : IEntityTypeConfiguration<ModeloEntity>
        {
            public void Configure(EntityTypeBuilder<ModeloEntity> builder)
            {
                builder.ToTable("Modelos");
                builder.HasKey(a => a.Id);
                builder.Property(a => a.Id).ValueGeneratedOnAdd();
                builder.Property(a => a.CodigoInterno).IsRequired();
                builder.Property(a => a.Nome).IsRequired();
            }
        }
        public class CotacaoPublicacaoRetornoJobMap : IEntityTypeConfiguration<CotacaoPublicacaoRetornoJobEntity>
        {
            public void Configure(EntityTypeBuilder<CotacaoPublicacaoRetornoJobEntity> builder)
            {
                builder.ToTable("CotacoesPublicacoesRetornoJob");
                builder.HasKey(a => a.Id);
                builder.Property(a => a.Id).ValueGeneratedOnAdd();
                builder.Property(a => a.CotacaoId).IsRequired();
                builder.Property(a => a.JsonPublicacao).IsRequired();
                builder.Property(a => a.Publicado).IsRequired();
                builder.Property(a => a.Fila).IsRequired();
                builder.Property(a => a.Seguradora).IsRequired();
                builder.HasIndex("CotacaoId").IsUnique(false);
            }
        }
        public class CotacaoCondicaoComercialSeguradoraMap : IEntityTypeConfiguration<CotacaoCondicaoComercialSeguradoraEntity>
        {
            public void Configure(EntityTypeBuilder<CotacaoCondicaoComercialSeguradoraEntity> builder)
            {
                builder.ToTable("CotacoesCondicaoComercialSeguradoras");
                builder.HasKey(a => a.Id);
                builder.Property(a => a.Id).ValueGeneratedOnAdd();
                builder.Property(a => a.CotacaoId).IsRequired();
                builder.Property(a => a.Seguradora).IsRequired();
                builder.Property(a => a.Comissao).IsRequired();
                builder.Property(a => a.DescontoAgravo).IsRequired();
                builder.Property(a => a.MultiplicadorFranquia).IsRequired();
                builder.HasIndex("CotacaoId").IsUnique(false);
            }
        }
        public class MarcaMap : IEntityTypeConfiguration<MarcaEntity>
        {
            public void Configure(EntityTypeBuilder<MarcaEntity> builder)
            {
                builder.ToTable("Marcas");
                builder.HasKey(a => a.Id);
                builder.Property(a => a.Id).ValueGeneratedOnAdd();
                builder.Property(a => a.CodigoInterno).IsRequired();
                builder.Property(a => a.Nome).IsRequired();
            }
        }
        public class SeguradoraAnteriorMap : IEntityTypeConfiguration<SeguradoraAnteriorEntity>
        {
            public void Configure(EntityTypeBuilder<SeguradoraAnteriorEntity> builder)
            {
                builder.ToTable("SeguradoraAnterior");
                builder.HasKey(a => a.Id);
                builder.Property(a => a.Id).ValueGeneratedOnAdd();
                builder.Property(a => a.CodigoInterno).IsRequired();
                builder.Property(a => a.Nome).IsRequired();
            }
        }
        public class EquipamentoMap : IEntityTypeConfiguration<EquipamentoEntity>
        {
            public void Configure(EntityTypeBuilder<EquipamentoEntity> builder)
            {
                builder.ToTable("Equipamentos");
                builder.HasKey(a => a.Id);
                builder.Property(a => a.Id).ValueGeneratedOnAdd();
                builder.Property(a => a.CodigoInterno).IsRequired();
                builder.Property(a => a.Nome).IsRequired();
            }
        }
        public class BancoMap : IEntityTypeConfiguration<BancoEntity>
        {
            public void Configure(EntityTypeBuilder<BancoEntity> builder)
            {
                builder.ToTable("Bancos");
                builder.HasKey(a => a.Id);
                builder.Property(a => a.Id).ValueGeneratedOnAdd();
                builder.Property(a => a.CodigoInterno).IsRequired();
                builder.Property(a => a.Nome).IsRequired();
            }
        }
        public class CotacaoMap : IEntityTypeConfiguration<CotacaoEntity>
        {
            public void Configure(EntityTypeBuilder<CotacaoEntity> builder)
            {
                builder.ToTable("Cotacoes");
                builder.HasKey(a => a.Id);
                builder.Property(a => a.Id).ValueGeneratedOnAdd();
                builder.Property(a => a.Corretor).IsRequired();
                builder.Property(a => a.DataHoraCotacao).IsRequired();
                builder.Property(a => a.Cancelado).IsRequired();
                builder.Property(a => a.Efetivada).IsRequired();
                builder.Property(a => a.Premio).IsRequired();
                builder.Property(a => a.Seguradora).IsRequired(false);
                builder.Property(a => a.JsonCotacao).IsRequired(false);
                builder.Property(a => a.CodigoCotacao).IsRequired(false);
                builder.Property(a => a.LinkAcessoCotacao).IsRequired(false);
            }
        }
        public class CotacaoCondicaoComercialMap : IEntityTypeConfiguration<CotacaoCondicaoComercialEntity>
        {
            public void Configure(EntityTypeBuilder<CotacaoCondicaoComercialEntity> builder)
            {
                builder.ToTable("CotacaoCondicoesComerciais");
                builder.HasKey(a => a.Id);
                builder.Property(a => a.Id).ValueGeneratedOnAdd();
                builder.Property(a => a.CotacaoId).IsRequired();
                builder.HasIndex("CotacaoId").IsUnique(false);
            }
        }
        public class CotacaoFormularioRiscoMap : IEntityTypeConfiguration<CotacaoFormularioRiscoEntity>
        {
            public void Configure(EntityTypeBuilder<CotacaoFormularioRiscoEntity> builder)
            {
                builder.ToTable("CotacaoFormulariosRiscos");
                builder.HasKey(a => a.Id);
                builder.Property(a => a.Id).ValueGeneratedOnAdd();
                builder.Property(a => a.CotacaoId).IsRequired();
                builder.Property(a => a.BemId).IsRequired();
                builder.HasIndex("CotacaoId").IsUnique(false);
                builder.HasIndex("BemId").IsUnique(false);
            }
        }
        public class CotacaoInformacaoBeneficiarioMap : IEntityTypeConfiguration<CotacaoInformacoesBeneficiarioEntity>
        {
            public void Configure(EntityTypeBuilder<CotacaoInformacoesBeneficiarioEntity> builder)
            {
                builder.ToTable("CotacaoInformacaoBeneficiarios");
                builder.HasKey(a => a.Id);
                builder.Property(a => a.Id).ValueGeneratedOnAdd();
                builder.Property(a => a.CotacaoId).IsRequired();
                builder.Property(a => a.TipoPessoa).IsRequired();
                builder.Property(a => a.NomeBeneficiario).IsRequired();
                builder.Property(a => a.CPFCNPJBeneficiario).IsRequired();
                builder.Property(a => a.BancoBeneficiario).IsRequired(false);
                builder.Property(a => a.AgenciaBeneficiario).IsRequired(false);
                builder.Property(a => a.ContaBeneficiario).IsRequired(false);
                builder.Property(a => a.DigitoContaBeneficiario).IsRequired(false);
                builder.HasIndex("CotacaoId").IsUnique(false);
            }
        }
        public class CotacaoCoberturaMap : IEntityTypeConfiguration<CotacaoCoberturaEntity>
        {
            public void Configure(EntityTypeBuilder<CotacaoCoberturaEntity> builder)
            {
                builder.ToTable("CotacaoCoberturas");
                builder.HasKey(a => a.Id);
                builder.Property(a => a.Id).ValueGeneratedOnAdd();
                builder.Property(a => a.CotacaoId).IsRequired();
                builder.Property(a => a.BemId).IsRequired();
                builder.Property(a => a.AplicarCoberturaTotal).IsRequired();
                builder.Property(a => a.ContratarResponsabilidadeCivilMaquinariaAgricola).IsRequired();
                builder.Property(a => a.ValorResponsabilidadeCivilMaquinariaAgricola).IsRequired();
                builder.Property(a => a.ContratarResponsabilidadeCivilEmpregador).IsRequired();
                builder.Property(a => a.ValorResponsabilidadeCivilEmpregador).IsRequired();
                builder.Property(a => a.ContratarCoberturaRelativaPerdaPagamentoAluguel).IsRequired();
                builder.Property(a => a.ContratarFurtoSimples).IsRequired();
                builder.Property(a => a.ValorFurtoSimples).IsRequired();
                builder.Property(a => a.ContratarDanosEletricos).IsRequired();
                builder.Property(a => a.ValorDanosEletricos).IsRequired();
                builder.Property(a => a.ContratarQuebraVidros).IsRequired();
                builder.Property(a => a.ValorQuebraVidros).IsRequired();
                builder.HasIndex("CotacaoId").IsUnique(false);
                builder.HasIndex("BemId").IsUnique(false);
            }
        }
        public class CotacaoInformacaoSeguroMap : IEntityTypeConfiguration<CotacaoInformacaoSeguroEntity>
        {
            public void Configure(EntityTypeBuilder<CotacaoInformacaoSeguroEntity> builder)
            {
                builder.ToTable("CotacaoInformacaoSeguros");
                builder.HasKey(a => a.Id);
                builder.Property(a => a.Id).ValueGeneratedOnAdd();
                builder.Property(a => a.CotacaoId).IsRequired();
                builder.Property(a => a.TempoVigenciaSeguro).IsRequired();
                builder.Property(a => a.TipoSeguro).IsRequired();
                builder.Property(a => a.QuantidadeParcelas).IsRequired();
                builder.Property(a => a.ApoliceRenovacao).IsRequired(false);
                builder.Property(a => a.SeguradoraAnterior).IsRequired(false);
                builder.Property(a => a.BancoBeneficiarioInformacaoSeguro).IsRequired(false);
                builder.Property(a => a.FormaPagamentoSeguro).IsRequired(false);
                builder.Property(a => a.PrazoSeguro).IsRequired(false);
                builder.HasIndex("CotacaoId").IsUnique(false);
            }
        }
        public class CotacaoInformacaoBemMap : IEntityTypeConfiguration<CotacaoInformacaoBemEntity>
        {
            public void Configure(EntityTypeBuilder<CotacaoInformacaoBemEntity> builder)
            {
                builder.ToTable("CotacaoInformacaoBens");
                builder.HasKey(a => a.Id);
                builder.Property(a => a.Id).ValueGeneratedOnAdd();
                builder.Property(a => a.CotacaoId).IsRequired();
                builder.Property(a => a.TipoEquipamento).IsRequired();
                builder.Property(a => a.AnoFabricacao).IsRequired();
                builder.Property(a => a.ValorEquipamento).IsRequired();
                builder.Property(a => a.MarcaEquipamento).IsRequired();
                builder.Property(a => a.ModeloEquipamento).IsRequired();
                builder.Property(a => a.NumeroSerieEquipamento).IsRequired();
                builder.Property(a => a.NumeroChassiEquipamento).IsRequired();
                builder.HasIndex("CotacaoId").IsUnique(false);
            }
        }
        public class CotacaoInformacaoSeguradoMap : IEntityTypeConfiguration<CotacaoInformacaoSeguradoEntity>
        {
            public void Configure(EntityTypeBuilder<CotacaoInformacaoSeguradoEntity> builder)
            {
                builder.ToTable("CotacaoInformacaoSegurados");
                builder.HasKey(a => a.Id);
                builder.Property(a => a.Id).ValueGeneratedOnAdd();
                builder.Property(a => a.CotacaoId).IsRequired();
                builder.Property(a => a.Nome).IsRequired();
                builder.Property(a => a.TipoPessoa).IsRequired();
                builder.Property(a => a.CPFCNPJ).IsRequired();
                builder.Property(a => a.CEP).IsRequired();
                builder.Property(a => a.Endereco).IsRequired();
                builder.Property(a => a.Bairro).IsRequired();
                builder.Property(a => a.Numero).IsRequired();
                builder.Property(a => a.Estado).IsRequired();
                builder.Property(a => a.Cidade).IsRequired();
                builder.HasIndex("CotacaoId").IsUnique(false);
            }
        }
        public class SeguradoraMap : IEntityTypeConfiguration<SeguradoraEntity>
        {
            public void Configure(EntityTypeBuilder<SeguradoraEntity> builder)
            {
                builder.ToTable("Seguradoras");
                builder.HasKey(a => a.Id);
                builder.Property(a => a.Id).ValueGeneratedOnAdd();
                builder.Property(a => a.RazaoSocial).IsRequired();
                builder.Property(a => a.NomeFantasia).IsRequired();
                builder.Property(a => a.Logo).IsRequired();
                builder.Property(a => a.Ativo).IsRequired();
            }
        }
        public class ParametrizacaoBeneficiarioMap : IEntityTypeConfiguration<ParametrizacaoBeneficiarioEntity>
        {
            public void Configure(EntityTypeBuilder<ParametrizacaoBeneficiarioEntity> builder)
            {
                builder.ToTable("ParametrizacaoBeneficiario");
                builder.HasKey(a => a.Id);
                builder.Property(a => a.Id).ValueGeneratedOnAdd();
                builder.Property(a => a.Banco).IsRequired();
                builder.Property(a => a.Agencia).IsRequired();
                builder.Property(a => a.Conta).IsRequired();
                builder.Property(a => a.DigitoConta).IsRequired();
            }
        }
        public class ParametrizacaoCustomizacaoRelatorioMap : IEntityTypeConfiguration<ParametrizacaoCustomizacaoRelatorioEntity>
        {
            public void Configure(EntityTypeBuilder<ParametrizacaoCustomizacaoRelatorioEntity> builder)
            {
                builder.ToTable("ParametrizacaoCustomizacaoRelatorio");
                builder.HasKey(a => a.Id);
                builder.Property(a => a.Id).ValueGeneratedOnAdd();
                builder.Property(a => a.Titulo1).IsRequired();
                builder.Property(a => a.TamanhoTitulo1).IsRequired();
                builder.Property(a => a.CorTitulo1).IsRequired();
                builder.Property(a => a.Texto1).IsRequired();
                builder.Property(a => a.TamanhoTexto1).IsRequired();
                builder.Property(a => a.CorTexto1).IsRequired();
                builder.Property(a => a.Titulo2).IsRequired(false);
                builder.Property(a => a.TamanhoTitulo2).IsRequired();
                builder.Property(a => a.CorTitulo2).IsRequired(false);
                builder.Property(a => a.Texto2).IsRequired(false);
                builder.Property(a => a.TamanhoTexto2).IsRequired();
                builder.Property(a => a.CorTexto2).IsRequired(false);
                builder.Property(a => a.Imagem1).IsRequired(false);
            }
        }
        public class ParametrizacaoRiscoMap : IEntityTypeConfiguration<ParametrizacaoRiscoEntity>
        {
            public void Configure(EntityTypeBuilder<ParametrizacaoRiscoEntity> builder)
            {
                builder.ToTable("ParametrizacaoRisco");
                builder.HasKey(a => a.Id);
                builder.Property(a => a.Id).ValueGeneratedOnAdd();
                builder.Property(a => a.EquipamentoOperaProximoAgua).IsRequired();
                builder.Property(a => a.SeguradoColaboradorOperador).IsRequired();
            }
        }
        public class CorretoraMap : IEntityTypeConfiguration<CorretoraEntity>
        {
            public void Configure(EntityTypeBuilder<CorretoraEntity> builder)
            {
                builder.ToTable("Corretoras");
                builder.HasKey(a => a.Id);
                builder.Property(a => a.Id).ValueGeneratedOnAdd();
                builder.Property(a => a.RazaoSocial).IsRequired();
                builder.Property(a => a.NomeFantasia).IsRequired();
                builder.Property(a => a.CNPJ).IsRequired();
                builder.Property(a => a.Endereco).IsRequired(false);
                builder.Property(a => a.Bairro).IsRequired(false);
                builder.Property(a => a.Numero).IsRequired(false);
                builder.Property(a => a.Cidade).IsRequired(false);
                builder.Property(a => a.Estado).IsRequired(false);
                builder.Property(a => a.Ativa).IsRequired();
                builder.Property(a => a.EmAtraso).IsRequired();
                builder.Property(a => a.PlanoContratado).IsRequired(false);
                builder.Property(a => a.ImagemLogo).IsRequired(false);
                builder.Property(a => a.DiaVencimento).IsRequired();
                builder.Property(a => a.EmailSeguro).HasMaxLength(256);
                builder.Property(a => a.EmailCopiaSeguro).HasMaxLength(256);
                builder.Property(a => a.EmailSinistro).HasMaxLength(256);
                builder.Property(a => a.EmailCopiaSinistro).HasMaxLength(256);
            }
        }
        public class ParametrizacaoSeguradoraMap : IEntityTypeConfiguration<ParametrizacaoSeguradoraEntity>
        {
            public void Configure(EntityTypeBuilder<ParametrizacaoSeguradoraEntity> builder)
            {
                builder.ToTable("ParametrizacaoSeguradora");
                builder.HasKey(a => a.Id);
                builder.Property(a => a.Id).ValueGeneratedOnAdd();
                builder.Property(e => e.ContratarCoberturaResponsabilidadeCivilParaMaquinaAgricola).IsRequired();
                builder.Property(e => e.ValorCoberturaResponsabilidadeCivilParaMaquinaAgricola).IsRequired();
                builder.Property(e => e.ContratarCoberturaResponsabilidadeCivilParaEmpregador).IsRequired();
                builder.Property(e => e.ValorCoberturaResponsabilidadeCivilParaEmpregador).IsRequired();
                builder.Property(e => e.ContratarCoberturaPerdaPagamentoAluguel).IsRequired();
                builder.Property(e => e.ValorCoberturaPerdaPagamentoAluguel).IsRequired();
                builder.Property(e => e.ContratarCoberturaFurtoSimples).IsRequired();
                builder.Property(e => e.ValorCoberturaFurtoSimples).IsRequired();
                builder.Property(e => e.ContratarCoberturaDanosEletricos).IsRequired();
                builder.Property(e => e.ValorCoberturaDanosEletricos).IsRequired();
                builder.Property(e => e.ContratarCoberturaQuebraVidros).IsRequired();
                builder.Property(e => e.ValorCoberturaQuebraVidros).IsRequired();
                builder.Property(e => e.NumeroParcelamento).IsRequired();
                builder.Property(e => e.ComissaoSeguradoraMapfre).IsRequired();
                builder.Property(e => e.DescontoAgravoSeguradoraMapfre).IsRequired();
                builder.Property(e => e.MultiplicadorFranquiaSeguradoraMapfre).IsRequired();
                builder.Property(e => e.ComissaoSeguradoraSwissRe).IsRequired();
                builder.Property(e => e.DescontoAgravoSeguradoraSwissRe).IsRequired();
                builder.Property(e => e.MultiplicadorFranquiaSeguradoraSwissRe).IsRequired();
                builder.Property(e => e.ComissaoSeguradoraAllianz).IsRequired();
                builder.Property(e => e.DescontoAgravoSeguradoraAllianz).IsRequired();
                builder.Property(e => e.MultiplicadorFranquiaSeguradoraAllianz).IsRequired();
                builder.Property(e => e.ComissaoSeguradoraTokio).IsRequired();
                builder.Property(e => e.DescontoAgravoSeguradoraTokio).IsRequired();
                builder.Property(e => e.MultiplicadorFranquiaSeguradoraTokio).IsRequired();
                builder.Property(e => e.ComissaoSeguradoraSOMPO).IsRequired();
                builder.Property(e => e.DescontoAgravoSeguradoraSOMPO).IsRequired();
                builder.Property(e => e.MultiplicadorFranquiaSeguradoraSOMPO).IsRequired();
                builder.Property(e => e.ComissaoSeguradoraPottencial).IsRequired();
                builder.Property(e => e.DescontoAgravoSeguradoraPottencial).IsRequired();
                builder.Property(e => e.MultiplicadorFranquiaSeguradoraPottencial).IsRequired();
                builder.Property(e => e.ComissaoSeguradoraSombrero).IsRequired();
                builder.Property(e => e.DescontoAgravoSeguradoraSombrero).IsRequired();
                builder.Property(e => e.MultiplicadorFranquiaSeguradoraSombrero).IsRequired();
                builder.Property(e => e.ComissaoSeguradoraFF).IsRequired();
                builder.Property(e => e.DescontoAgravoSeguradoraFF).IsRequired();
                builder.Property(e => e.MultiplicadorFranquiaSeguradoraFF).IsRequired();
            }
        }

        /*Multi-Cálculo*/
    }
}
