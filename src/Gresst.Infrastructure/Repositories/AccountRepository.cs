using System.Text.Json;

namespace Gresst.Infrastructure.Repositories
{
    public class AccountRepository: IAccountRepository
    {
        private readonly Entities _entities;

        public AccountRepository(Entities db)
        {
            _entities = db ?? throw new ArgumentNullException(nameof(db));
        }
        public IEnumerable<Account> GetAll()
        {
            var accounts = _entities.Cuenta;
            return accounts.Select(account => new Account() { 
                Id = account.IdCuenta, 
                Name = account.Nombre, 
                Role = account.IdRol, 
                PersonId = account.IdPersona, 
                AdministratorId = account.IdUsuario 
            });
        }
        public Account Get(long id)
        {
            var account = _entities.Cuenta.FirstOrDefault(x => x.IdCuenta == id);
            if (account == null)
                return null;
            return new Account() { 
                Id = account.IdCuenta, 
                Name = account.Nombre, 
                Role = account.IdRol, 
                PersonId = account.IdPersona, 
                Settings = !String.IsNullOrEmpty(account.Ajustes) ? JsonSerializer.Deserialize<Dictionary<string, string>>(account.Ajustes) : null, 
                AdministratorId = account.IdUsuario };
        }
        public Account GetForUser(long userId)
        {
            var account = (from a in _entities.Cuenta
                           join u in _entities.Usuario on a.IdCuenta equals u.IdCuenta
                           where u.IdUsuario == userId
                           select a).FirstOrDefault();
            if (account == null)
                return null;
            return new Account() { Id = account.IdCuenta, Name = account.Nombre, Role = account.IdRol, PersonId = account.IdPersona, Settings = !String.IsNullOrEmpty(account.Ajustes) ? JsonSerializer.Deserialize<Dictionary<string, string>>(account.Ajustes) : null, AdministratorId = account.IdUsuario};
        }
    }
}
