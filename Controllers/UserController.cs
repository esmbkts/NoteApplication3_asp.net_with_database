using Microsoft.AspNetCore.Mvc;
using asp.net.Models;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using asp.net.Data;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace asp.net.Controllers
{
    //[Authorize(Roles = "Admin")]
    public class UserController : Controller
    {
        private readonly IUserAction _userAction;
        private readonly INoteAction _noteAction;
        private readonly IAccountAction _accountAction;
        private readonly ApplicationDbContext _context;
        private readonly ILogger<UserController> _logger;

        public UserController(IUserAction userAction, IAccountAction accountAction, ILogger<UserController> logger, ApplicationDbContext context)
        {
            _userAction = userAction;
            _accountAction = accountAction;
            _logger = logger;
            _context = context;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Giriş yapan kullanıcının hesap bilgilerini veritabanından al
                var account = _accountAction.GetAccountByFilter(a => a.Username == model.Username && a.Password == model.Password && a.IsActive == 1);
                _logger.LogInformation("Login attempt with Username: {Username}, Account found: {@Account}", model.Username, account);

                if (account == null)
                {
                    ViewBag.ErrorMessage = "Account not found or inactive, or incorrect password.";
                    return View(model);
                }


                // Kullanıcı bilgilerini al
                var user = _userAction.GetUserByFilter(u => u.Id == account.UserId && u.IsActive == 1);
                _logger.LogInformation("User found for AccountId: {AccountId}, User: {@User}", account.UserId, user);

                if (user == null)
                {
                    ViewBag.ErrorMessage = "User not found or inactive.";
                    return View(model);
                }

                // Claims oluştur
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, account.Username),
                    new Claim(ClaimTypes.NameIdentifier, account.Id.ToString()), // account yerine user
                    new Claim("AccountId", account.Id.ToString()), //
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(ClaimTypes.Role, user.IsAdmin == 1 ? "Admin" : "User")
                };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var authProperties = new AuthenticationProperties
                {
                    IsPersistent = true
                };

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity), authProperties);

                if (user.IsAdmin == 1)
                {
                    return RedirectToAction("AdminMenu", "User");
                }
                return RedirectToAction("UserMenu", "User");
            }
            ViewBag.ErrorMessage = "Invalid login attempt.";
            return View(model);
        }

        [HttpGet]
        public IActionResult Signup()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Signup(SignupViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Kullanıcı adının var olup olmadığını kontrol et
                var existingUsername = _accountAction.GetAccountByFilter(a => a.Username == model.UserName);
                if (existingUsername != null)
                {
                    ViewBag.ErrorMessage = "This username is already taken. Please choose a different username.";
                    return View(model);
                }

                var existingUser = _userAction.GetUserByFilter(u => u.Email == model.Email);
                if (existingUser == null)
                {
                    var user = new User
                    {
                        Firstname = model.FirstName,
                        Surname = model.LastName,
                        Email = model.Email,
                        IsActive = 1,
                        IsAdmin = 0,
                        CreateTime = DateTime.Now,
                        LastUpdateTime = DateTime.Now
                    };
                    _userAction.AddUser(user);

                    var account = new Account
                    {
                        UserId = user.Id,
                        Username = model.UserName,
                        Password = model.Password,
                        IsActive = 1
                    };
                    _accountAction.AddAccount(account);
                }

                else
                {
                    var account = new Account
                    {
                        UserId = existingUser.Id,
                        Username = model.UserName,
                        Password = model.Password,
                        IsActive = 1
                    };
                    _accountAction.AddAccount(account);
                }
                    return RedirectToAction("Login", "User");
                
            }
            return View(model);
        }

        public IActionResult UserMenu()
        {
            return View();
        }

        [Authorize]
        [HttpGet]
        public IActionResult UpdateUser()
        {
            var userEmail = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
            var user = _userAction.GetUserByFilter(u => u.Email == userEmail);
            if (user == null)
            {
                return RedirectToAction("Login");
            }

            var model = new UpdateUserViewModel
            {
                FirstName = user.Firstname,
                LastName = user.Surname,
                Email = user.Email
            };

            return View(model);
        }


        [Authorize]
        [HttpPost]
        public async Task<IActionResult> UpdateUser(UpdateUserViewModel model)
        {
            if (ModelState.IsValid)
            {
                var userEmail = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
                var user = _userAction.GetUserByFilter(u => u.Email == userEmail);

                if (user != null)
                {
                    // User tablosundaki bilgileri güncelle
                    user.Firstname = model.FirstName;
                    user.Surname = model.LastName;
                    user.Email = model.Email;
                    user.LastUpdateTime = DateTime.Now;
                    _userAction.UpdateUser(user);

                    // Şifre güncellemesi varsa, Account tablosundaki şifreyi güncelle
                    if (!string.IsNullOrEmpty(model.Password))
                    {
                        var account = _accountAction.GetAccountByFilter(a => a.UserId == user.Id);
                        if (account != null)
                        {
                            account.Password = model.Password; // Şifre güncellenmeli, şifreleme burada yapılmalı
                            _accountAction.UpdateAccount(account);
                        }
                    }

                    // Kullanıcı güncellendikten sonra oturum bilgilerini güncelle
                    var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Email),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Role, user.IsAdmin == 1 ? "Admin" : "User")
            };

                    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                        new ClaimsPrincipal(claimsIdentity));

                    return RedirectToAction("UserMenu");
                }
            }
            return View(model);
        }

        // Admin işlemleri

        [Authorize(Roles = "Admin")]
        public IActionResult AdminMenu()
        {
            var users = _userAction.GetUserList().Where(u => u.IsActive == 1).ToList();

           var adminUserViewModels = users.Select(user => new AdminUserViewModel
            {
                Email = user.Email,
                Firstname = user.Firstname,
                Surname = user.Surname,
                Usernames = _context.Accounts.Where(a => a.UserId == user.Id && a.IsActive == 1)
                .Select(a => a.Username).ToList()
           }).ToList();

            return View(adminUserViewModels);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public IActionResult DeleteAccount(string username)
        {
            var account = _accountAction.GetAccountByFilter(a => a.Username == username);
            if (account == null)
            {
                return NotFound();
            }
            return View(account); // Account modelini DeleteAccount.cshtml dosyasına gönderiyoruz
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteAccountConfirmed(string username)
        {
            var account = _accountAction.GetAccountByFilter(a => a.Username == username);
            _accountAction.DeleteAccount(account); // hesabı silmek yerine isActive flag'ini 0 yapıyor
            var userAccounts = _accountAction.GetAccountList().Where(a => a.UserId == account.UserId && a.IsActive == 1).ToList();
            if (userAccounts.Count == 0)
            {
                var user = _userAction.GetUserByFilter(u => u.Id == account.UserId);
                _userAction.DeleteUser(user); // eğer kullanıcının başka aktif hesabı yoksa kullanıcının da isActive flag'ini 0 yapıyoruz
            }            
            return RedirectToAction("AdminMenu");
        }
    }
}
