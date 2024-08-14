namespace asp.net.Models
{
    public interface IAccountAction
    {
        void AddAccount(Account account);
        List<Account> GetAccountList();
        void DeleteAccount(Account account);
        void UpdateAccount(Account account);
        Account GetAccountByFilter(Func<Account, bool> filter);
    }
}
