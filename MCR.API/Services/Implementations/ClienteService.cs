using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using MCR.API.Entities;
using MCR.API.Models;
using MCR.API.Repository;
using MCR.API.Services.Interfaces;

namespace MCR.API.Services.Implementations
{
    public class ClienteService : IClienteService
    {
        private readonly DbContextMCR _context;

        public ClienteService(DbContextMCR context)
        {
            _context = context;
        }

        private ValidationResult Validar(ClienteEntity entity, bool isUpdate = false)
        {
            var errors = new List<string>();

            if (string.IsNullOrEmpty(entity.Nome))
                errors.Add("Informe o nome.");
            if (string.IsNullOrEmpty(entity.CPF))
                errors.Add("Informe o CPF.");
            if (!isUpdate && string.IsNullOrEmpty(entity.ImagemCPF))
                errors.Add("Informe a imagem do CPF.");
            if (string.IsNullOrEmpty(entity.DataNascimento))
                errors.Add("Informe a data de nascimento.");
            if (string.IsNullOrEmpty(entity.RG))
                errors.Add("Informe o RG.");
            if (!isUpdate && string.IsNullOrEmpty(entity.ImagemRG))
                errors.Add("Informe a imagem do RG.");
            if (string.IsNullOrEmpty(entity.DataExpedicaoRG))
                errors.Add("Informe a data de expedição do RG.");
            if (string.IsNullOrEmpty(entity.OrgaoExpeditorRG))
                errors.Add("Informe o órgão expeditor do RG.");
            if (string.IsNullOrEmpty(entity.EstadoCivil))
                errors.Add("Informe o estado civil.");
            if (string.IsNullOrEmpty(entity.Sexo))
                errors.Add("Informe o sexo.");
            if (string.IsNullOrEmpty(entity.Telefone))
                errors.Add("Informe o telefone.");
            if (string.IsNullOrEmpty(entity.Celular))
                errors.Add("Informe o celular.");
            if (string.IsNullOrEmpty(entity.Profissao))
                errors.Add("Informe a profissão.");
            if (string.IsNullOrEmpty(entity.Banco))
                errors.Add("Informe banco.");
            if (string.IsNullOrEmpty(entity.Agencia))
                errors.Add("Informe a agência.");
            if (string.IsNullOrEmpty(entity.Conta))
                errors.Add("Informe a conta.");
            if (string.IsNullOrEmpty(entity.ChavePIX))
                errors.Add("Informe a chave PIX.");
            if (string.IsNullOrEmpty(entity.Endereco))
                errors.Add("Informe o endereço.");
            if (string.IsNullOrEmpty(entity.Bairro))
                errors.Add("Informe o bairro.");
            if (string.IsNullOrEmpty(entity.Cidade))
                errors.Add("Informe a cidade.");
            if (string.IsNullOrEmpty(entity.Estado))
                errors.Add("Informe o estado.");
            if (string.IsNullOrEmpty(entity.CEP))
                errors.Add("Informe o CEP.");
            if (string.IsNullOrEmpty(entity.Email))
                errors.Add("Informe o e-mail.");
            if (!IsValidCPF(entity.CPF))
                errors.Add("CPF inválido.");
            if (!IsValidEmail(entity.Email))
                errors.Add("E-mail inválido.");

            return errors.Any() ? new ValidationResult(false, errors) : new ValidationResult(true, null);
        }

        public async Task<ClienteEntity> ObterPorIdAsync(Guid id)
        {
            var cliente = await _context.Clientes
                .FirstOrDefaultAsync(c => c.Id == id && !c.Excluido);

            if (cliente == null)
            {
                return new ClienteEntity
                {
                    Sucesso = false,
                    Mensagem = "Cliente não encontrado ou excluído."
                };
            }

            cliente.VinculosFamiliares = await _context.VinculosFamiliares
                .Where(c => c.ClienteId == cliente.Id)
                .ToListAsync();

            cliente.VinculosPropriedads = await _context.VinculosPropriedadesClientes
                .Where(c => c.ClienteId == cliente.Id)
                .ToListAsync();

            var listaPropriedades = new List<PropriedadeEntity>();
            foreach (var vinculo in cliente.VinculosPropriedads)
            {
                var addPropriedade = await _context.Propriedades.FindAsync(vinculo.PropriedadeId);
                if (addPropriedade != null)
                    listaPropriedades.Add(addPropriedade);
            }

            cliente.Propriedades = listaPropriedades;
            cliente.Sucesso = true;
            return cliente;
        }

        public async Task<IEnumerable<ClienteEntity>> ObterTodosPaginadoAsync(
            string nome = null, string cpf = null,
            string cidade = null, string estado = null,
            bool? ativo = null, int page = 1, int pageSize = 20)
        {
            var query = _context.Clientes
                .Where(c => !c.Excluido)
                .AsQueryable();

            if (!string.IsNullOrEmpty(nome))
                query = query.Where(c => c.Nome.Contains(nome));
            if (!string.IsNullOrEmpty(cpf))
                query = query.Where(c => c.CPF.Replace(".", "").Replace("-", "").Contains(cpf.Replace(".", "").Replace("-", "")));
            if (!string.IsNullOrEmpty(cidade))
                query = query.Where(c => c.Cidade.Contains(cidade));
            if (!string.IsNullOrEmpty(estado))
                query = query.Where(c => c.Estado.Contains(estado));
            if (ativo.HasValue)
                query = query.Where(c => c.Ativo == ativo.Value);

            var totalItems = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            var results = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            foreach (var cliente in results)
            {
                cliente.TotalItems = totalItems;
                cliente.TotalPages = totalPages;
                cliente.Sucesso = true;
                cliente.Mensagem = "Dados carregados com sucesso!";
            }

            return results;
        }

        public async Task<ClienteEntity> CadastrarAsync(ClienteEntity entity)
        {
            entity.Ativo = true;
            entity.Excluido = false;

            var validationResult = Validar(entity);
            if (!validationResult.IsValid)
            {
                entity.Sucesso = false;
                entity.Mensagem = string.Join("; ", validationResult.Errors);
                return entity;
            }

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                _context.Clientes.Add(entity);
                await _context.SaveChangesAsync();

                if (entity.VinculosFamiliares != null && entity.VinculosFamiliares.Any())
                {
                    entity.VinculosFamiliares.ToList().ForEach(c => c.ClienteId = entity.Id);
                    _context.VinculosFamiliares.AddRange(entity.VinculosFamiliares);
                    await _context.SaveChangesAsync();
                }

                if (entity.Propriedades != null && entity.Propriedades.Any())
                {
                    var listaVinculoPropriedadeCliente = new List<VinculoPropriedadeClienteEntity>();
                    foreach (var propriedade in entity.Propriedades)
                    {
                        listaVinculoPropriedadeCliente.Add(new VinculoPropriedadeClienteEntity { ClienteId = entity.Id, PropriedadeId = propriedade.Id });
                    }
                    _context.VinculosPropriedadesClientes.AddRange(listaVinculoPropriedadeCliente);
                    await _context.SaveChangesAsync();
                }

                await transaction.CommitAsync();
                entity.Sucesso = true;
                entity.Mensagem = "Cadastrado com sucesso.";
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                entity.Sucesso = false;
                entity.Mensagem = $"Erro ao cadastrar cliente: {ex.Message}";
            }

            return entity;
        }

        public async Task<ClienteEntity> AtualizarAsync(ClienteEntity entity)
        {
            var validationResult = Validar(entity, isUpdate: true);
            if (!validationResult.IsValid)
            {
                entity.Sucesso = false;
                entity.Mensagem = string.Join("; ", validationResult.Errors);
                return entity;
            }

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var vinculosExistentes = await _context.VinculosFamiliares
                    .Where(v => v.ClienteId == entity.Id)
                    .ToListAsync();
                _context.VinculosFamiliares.RemoveRange(vinculosExistentes);
                await _context.SaveChangesAsync();

                if (entity.VinculosFamiliares != null && entity.VinculosFamiliares.Any())
                {
                    entity.VinculosFamiliares.ToList().ForEach(c => c.ClienteId = entity.Id);
                    _context.VinculosFamiliares.AddRange(entity.VinculosFamiliares);
                    await _context.SaveChangesAsync();
                }

                var vinculosPropriedades = await _context.VinculosPropriedadesClientes
                    .Where(v => v.ClienteId == entity.Id)
                    .ToListAsync();
                _context.VinculosPropriedadesClientes.RemoveRange(vinculosPropriedades);
                await _context.SaveChangesAsync();

                if (entity.Propriedades != null && entity.Propriedades.Any())
                {
                    var listaVinculoPropriedadeCliente = new List<VinculoPropriedadeClienteEntity>();
                    foreach (var propriedade in entity.Propriedades)
                    {
                        listaVinculoPropriedadeCliente.Add(new VinculoPropriedadeClienteEntity { ClienteId = entity.Id, PropriedadeId = propriedade.Id });
                    }
                    _context.VinculosPropriedadesClientes.AddRange(listaVinculoPropriedadeCliente);
                    await _context.SaveChangesAsync();
                }

                _context.Clientes.Update(entity);
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();
                entity.Sucesso = true;
                entity.Mensagem = "Atualizado com sucesso.";
            }
            catch
            {
                await transaction.RollbackAsync();
                entity.Sucesso = false;
                entity.Mensagem = "Erro ao atualizar cliente. Tente novamente.";
            }

            return entity;
        }

        public async Task<ClienteEntity> ObterPorCpfAsync(string cpf)
        {
            var cliente = await _context.Clientes
                .FirstOrDefaultAsync(c => c.CPF.Replace(".", "").Replace("-", "").Contains(cpf.Replace(".", "").Replace("-", "")) && !c.Excluido);

            if (cliente == null)
            {
                return new ClienteEntity
                {
                    Sucesso = false,
                    Mensagem = "Cliente não encontrado."
                };
            }

            cliente.Sucesso = true;
            return cliente;
        }

        public async Task<IEnumerable<ClienteEntity>> PesquisarPorNomeAsync(string nome)
        {
            var clientes = await _context.Clientes
                .Where(c => c.Nome.Contains(nome) && !c.Excluido)
                .ToListAsync();

            clientes.ForEach(c => c.Sucesso = true);
            return clientes;
        }

        public async Task<IEnumerable<PropriedadeEntity>> ObterPropriedadesDoClienteAsync(Guid clienteId)
        {
            var vinculos = await _context.VinculosPropriedadesClientes
                .Where(v => v.ClienteId == clienteId)
                .ToListAsync();

            var propriedades = new List<PropriedadeEntity>();
            foreach (var vinculo in vinculos)
            {
                var propriedade = await _context.Propriedades
                    .FirstOrDefaultAsync(p => p.Id == vinculo.PropriedadeId && !p.Excluido);
                if (propriedade != null)
                {
                    propriedade.Sucesso = true;
                    propriedades.Add(propriedade);
                }
            }

            return propriedades;
        }

        public Task<List<FaixaRendaDTO>> ObterFaixasRendaAsync()
        {
            var faixasDeRenda = new List<FaixaRendaDTO>
            {
                new FaixaRendaDTO { Descricao = "Até R$ 1.320,00", ValorMinimo = 0, ValorMaximo = 1320 },
                new FaixaRendaDTO { Descricao = "De R$ 1.321,00 a R$ 2.640,00", ValorMinimo = 1321, ValorMaximo = 2640 },
                new FaixaRendaDTO { Descricao = "De R$ 2.641,00 a R$ 4.400,00", ValorMinimo = 2641, ValorMaximo = 4400 },
                new FaixaRendaDTO { Descricao = "De R$ 4.401,00 a R$ 8.800,00", ValorMinimo = 4401, ValorMaximo = 8800 },
                new FaixaRendaDTO { Descricao = "De R$ 8.801,00 a R$ 22.000,00", ValorMinimo = 8801, ValorMaximo = 22000 },
                new FaixaRendaDTO { Descricao = "Acima de R$ 22.001,00", ValorMinimo = 22001, ValorMaximo = decimal.MaxValue }
            };
            return Task.FromResult(faixasDeRenda);
        }

        public async Task<IEnumerable<TalhaoEntity>> ObterTalhoesDaPropriedadeAsync(Guid propriedadeId)
        {
            var talhoes = await _context.Talhoes
                .Where(t => t.PropriedadeId == propriedadeId && !t.Excluido)
                .ToListAsync();

            talhoes.ForEach(t => { t.Sucesso = true; t.Mensagem = "Dados carregados com sucesso!"; });
            return talhoes;
        }

        private static bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            try
            {
                email = Regex.Replace(email, @"(@)(.+)$", match =>
                {
                    var idn = new System.Globalization.IdnMapping();
                    var domainName = idn.GetAscii(match.Groups[2].Value);
                    return match.Groups[1].Value + domainName;
                }, RegexOptions.None, TimeSpan.FromMilliseconds(200));
            }
            catch
            {
                return false;
            }

            try
            {
                return Regex.IsMatch(email,
                    @"^[^@\s]+@[^@\s]+\.[^@\s]+$",
                    RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250));
            }
            catch
            {
                return false;
            }
        }

        private static bool IsValidCPF(string cpf)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(cpf))
                    return false;

                cpf = cpf.Replace(".", "").Replace("-", "");

                if (cpf.Length != 11 || !long.TryParse(cpf, out _))
                    return false;

                if (new string(cpf[0], 11) == cpf)
                    return false;

                int[] multiplicador1 = { 10, 9, 8, 7, 6, 5, 4, 3, 2 };
                int[] multiplicador2 = { 11, 10, 9, 8, 7, 6, 5, 4, 3, 2 };

                string tempCpf = cpf.Substring(0, 9);
                int soma = 0;

                for (int i = 0; i < 9; i++)
                    soma += int.Parse(tempCpf[i].ToString()) * multiplicador1[i];

                int resto = soma % 11;
                int digito1 = resto < 2 ? 0 : 11 - resto;

                tempCpf += digito1;
                soma = 0;

                for (int i = 0; i < 10; i++)
                    soma += int.Parse(tempCpf[i].ToString()) * multiplicador2[i];

                resto = soma % 11;
                int digito2 = resto < 2 ? 0 : 11 - resto;

                return cpf.EndsWith($"{digito1}{digito2}");
            }
            catch
            {
                return false;
            }
        }

        private class ValidationResult
        {
            public bool IsValid { get; }
            public List<string> Errors { get; }

            public ValidationResult(bool isValid, List<string> errors)
            {
                IsValid = isValid;
                Errors = errors ?? new List<string>();
            }
        }
    }
}
