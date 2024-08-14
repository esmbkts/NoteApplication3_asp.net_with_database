namespace asp.net.Models
{
    public interface IUserAction
    {
        void AddUser(User user);
        List<User> GetUserList();
        void DeleteUser(User user);
        User GetUserByFilter(Func<User, bool> filter);
        void UpdateUser(User user);
        List<Account> GetAccountsByUserId(int userId);
    }
}