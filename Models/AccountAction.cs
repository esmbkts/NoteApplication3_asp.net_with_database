using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using asp.net.Data;
using Microsoft.Extensions.Logging;

namespace asp.net.Models
{
    public class AccountAction : IAccountAction
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<AccountAction> _logger;            //

        public AccountAction(ApplicationDbContext context, ILogger<AccountAction> logger)
        {
            _context = context;
            _logger = logger;
        }

        public void AddAccount(Account account)
        {
            _context.Accounts.Add(account);
            _context.SaveChanges();
        }

        public List<Account> GetAccountList()  // olmalı mı olmamalı mı...
        {
            return _context.Accounts.Include(a => a.User).ToList();
        }

        public void DeleteAccount(Account account) // olmalı mı olmamalı mı...
        {
            var existingAccount = _context.Accounts.FirstOrDefault(a => a.Id == account.Id);
            if (existingAccount != null)
            {
                existingAccount.IsActive = 0;
                _context.Accounts.Update(existingAccount);
                _context.SaveChanges();

                // Eğer kullanıcının aktif başka hesabı yoksa, kullanıcıyı da inaktif yapalım
                var userAccounts = _context.Accounts.Where(a => a.UserId == existingAccount.UserId && a.IsActive == 1).ToList();
                if (!userAccounts.Any())
                {
                    var user = _context.Users.FirstOrDefault(u => u.Id == existingAccount.UserId);
                    if (user != null)
                    {
                        user.IsActive = 0;
                        _context.Users.Update(user);
                        _context.SaveChanges();
                    }
                }
            }
        }

        public Account GetAccountByFilter(Func<Account, bool> filter)
        {
            var account = _context.Accounts.FirstOrDefault(filter);
            _logger.LogInformation("GetAccountByFilter result: {@Account}", account);
            return account;
        }

        public void UpdateAccount(Account account)
        {
            _context.Accounts.Update(account);
            _context.SaveChanges();
        }
    }
}
