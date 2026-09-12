using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MCR.API.Repository;

namespace MCR.API.Migrations
{
    public static class TxnApplyMigrations
    {
        public static void ApplyMigrations(WebApplication app)
        {
            bool canApplyInserts = false;
            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;

                // Lista de todos os DbContexts que precisam ser migrados
                var contextTypes = new[]
                {
                    typeof(DbContextMCR),
                };

                Console.WriteLine("Starting database migrations process...");
                foreach (var contextType in contextTypes)
                {
                    try
                    {
                        Console.WriteLine($"\nProcessing migrations for {contextType.Name}:");
                        var context = services.GetRequiredService(contextType) as DbContext;

                        if (context == null)
                        {
                            Console.WriteLine($"Error: Could not resolve {contextType.Name}");
                            continue;
                        }

                        // Verifica se há migrações pendentes
                        var pendingMigrations = context.Database.GetPendingMigrations();
                        var pendingList = pendingMigrations.ToList();

                        if (pendingList.Any())
                        {
                            Console.WriteLine($"Found {pendingList.Count} pending migrations:");

                            Console.WriteLine("Applying migrations...");
                            foreach (var migration in pendingList)
                            {
                                Console.WriteLine($"  - {migration}");
                                if (migration == "20250213140421_BranchInicial")
                                {
                                    CreateBranchInitial(context);
                                    canApplyInserts = true;
                                }
                            }

                            context.Database.Migrate();


                            if (canApplyInserts)
                            {
                                CreateInsertInitialData(context);
                            }

                            Console.WriteLine("Migrations applied successfully.");
                        }
                        else
                        {
                            Console.WriteLine("No pending migrations found. Database is up to date.");
                        }

                        // Verifica se as migrações foram aplicadas corretamente
                        var appliedMigrations = context.Database.GetAppliedMigrations();
                        Console.WriteLine($"Total applied migrations: {appliedMigrations.Count()}");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error during migration of {contextType.Name}:");
                        Console.WriteLine($"  - {ex.Message}");

                        if (ex.InnerException != null)
                        {
                            Console.WriteLine($"  - Inner Exception: {ex.InnerException.Message}");
                        }
                    }
                }

                Console.WriteLine("\nMigration process completed for all contexts.");
            }
        }

        private static void CreateBranchInitial(DbContext context)
        {
            var baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            // Usar Path.Combine que se ajusta automaticamente à plataforma
            string[] pathComponents = { baseDirectory, "Repository", "ScriptsSql", "ScriptCreateDatabaseDefault.sql" };
            var sqlFile = Path.Combine(pathComponents);

            // Verificar também em caminhos alternativos que podem existir no contêiner Linux
            if (!File.Exists(sqlFile))
            {
                // Tenta com caminho alternativo em minúsculas
                pathComponents = new[] { baseDirectory, "repository", "scriptssql", "ScriptCreateDatabaseDefault.sql" };
                sqlFile = Path.Combine(pathComponents);

                // Tenta procurar em outros diretórios comuns no contêiner
                if (!File.Exists(sqlFile))
                {
                    // Tenta encontrar o arquivo em qualquer lugar abaixo do diretório base
                    var possibleFiles = Directory.GetFiles(baseDirectory, "ScriptCreateDatabaseDefault.sql", SearchOption.AllDirectories);
                    if (possibleFiles.Length > 0)
                    {
                        sqlFile = possibleFiles[0];
                    }
                }
            }

            if (File.Exists(sqlFile))
            {
                var sqlScript = File.ReadAllText(sqlFile);
                context.Database.ExecuteSqlRaw(sqlScript);
            }
            else
            {
                // Log mais detalhado para diagnóstico
                var availableFiles = Directory.GetFiles(baseDirectory, "*.sql", SearchOption.AllDirectories);
                var availableDirectories = Directory.GetDirectories(baseDirectory, "*", SearchOption.AllDirectories);

                throw new Exception($"ScriptCreateDatabaseDefault.sql não encontrado. Diretório base: {baseDirectory}. " +
                                  $"Arquivos SQL disponíveis: {string.Join(", ", availableFiles)}. " +
                                  $"Diretórios disponíveis: {string.Join(", ", availableDirectories)}");
            }
        }

        private static void CreateInsertInitialData(DbContext context)
        {
            var baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            var connectionString = context.Database.GetConnectionString();
            var isLocalhost = connectionString?.Contains("localhost", StringComparison.OrdinalIgnoreCase) ?? false;

            // Usar Path.Combine que se ajusta automaticamente à plataforma
            string[] pathComponents = { baseDirectory, "Repository", "ScriptsSql", "ScriptInsertInitialData.sql" };
            var sqlFile = Path.Combine(pathComponents);

            // Verificar também em caminhos alternativos que podem existir no contêiner Linux
            if (!File.Exists(sqlFile))
            {
                // Tenta com caminho alternativo em minúsculas
                pathComponents = new[] { baseDirectory, "repository", "scriptssql", "ScriptInsertInitialData.sql" };
                sqlFile = Path.Combine(pathComponents);

                // Tenta procurar em outros diretórios comuns no contêiner
                if (!File.Exists(sqlFile))
                {
                    // Tenta encontrar o arquivo em qualquer lugar abaixo do diretório base
                    var possibleFiles = Directory.GetFiles(baseDirectory, "ScriptInsertInitialData.sql", SearchOption.AllDirectories);
                    if (possibleFiles.Length > 0)
                    {
                        sqlFile = possibleFiles[0];
                    }
                }
            }

            if (File.Exists(sqlFile))
            {
                var sqlScript = File.ReadAllText(sqlFile);
                context.Database.ExecuteSqlRaw(sqlScript);
            }
            else
            {
                // Log mais detalhado para diagnóstico
                var availableFiles = Directory.GetFiles(baseDirectory, "*.sql", SearchOption.AllDirectories);
                var availableDirectories = Directory.GetDirectories(baseDirectory, "*", SearchOption.AllDirectories);

                throw new Exception($"ScriptInsertInitialData.sql não encontrado. Diretório base: {baseDirectory}. " +
                                  $"Arquivos SQL disponíveis: {string.Join(", ", availableFiles)}. " +
                                  $"Diretórios disponíveis: {string.Join(", ", availableDirectories)}");
            }
        }

    }
}