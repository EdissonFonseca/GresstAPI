namespace Gresstt.Domain.Interfaces
{
    public interface IAccountRepository
    {
        IEnumerable<Account> GetAll();
        Account Get(long id);
    }
}
