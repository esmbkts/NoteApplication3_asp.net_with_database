using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using asp.net.Data;

namespace asp.net.Models
{
    public class UserAction : IUserAction
    {
        private readonly ApplicationDbContext _context;

        public UserAction(ApplicationDbContext context)
        {
            _context = context;
        }

        public void AddUser(User user)
        {
           _context.Users.Add(user);
           _context.SaveChanges();
        }

        public List<User> GetUserList()
        {
            return _context.Users.ToList();
        }

        public void DeleteUser(User user)                
        {
            var existingUser = _context.Users.FirstOrDefault(u => u.Id == user.Id);
            if (existingUser != null)
            {
                existingUser.IsActive = 0;
                _context.Users.Update(existingUser);
                _context.SaveChanges();
            }
        }

        public User GetUserByFilter(Func<User, bool> filter)
        {
            return _context.Users.FirstOrDefault(filter);
        }

        public List<Account> GetAccountsByUserId(int userId)
        {
            return _context.Accounts.Where(a => a.UserId == userId && a.IsActive == 1).ToList();
        }

        public void UpdateUser(User user)
        {
            _context.Users.Update(user);
            _context.SaveChanges();
        }
    }
}

