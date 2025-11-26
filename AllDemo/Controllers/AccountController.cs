using System.Runtime.CompilerServices;
using System.Security.Claims;
using AllDemo.Helper.Log;
using AllDemo.Models;
using AllDemo.ViewModels;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using AllDemo.Data.context;

namespace AllDemo.Controllers
{
    public class AccountController : Controller
    {
        private readonly AppDbContext _dbContext;
        private readonly ErrorLog _errorLog;
        public AccountController(AppDbContext dbContext,ErrorLog errorLog)
        {
            this._dbContext=dbContext;
            this._errorLog = errorLog;
        }
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel loginViewModel)
        {
            if (!ModelState.IsValid)
                return View(loginViewModel);
            _errorLog.WriteErrorLog(new Exception("Login method entered"),"Use login()");
            try
            {
                var users = await _dbContext.UserModels.FirstOrDefaultAsync(u => u.UserId == loginViewModel.UserId);
            }
            catch (Exception ex)
            {
                _errorLog.WriteErrorLog(ex,"error");
            }
            var user = await _dbContext.UserModels.FirstOrDefaultAsync(u => u.UserId == loginViewModel.UserId);
            _errorLog.WriteErrorLog(new Exception("did not fetch the user"), "Use login()");

            if (user == null)
            {
                ModelState.AddModelError("", "Invalid Credentials.");
                return View(loginViewModel);
            }

            var hasher = new PasswordHasher<UserModel>();
            var result = hasher.VerifyHashedPassword(null, user.HashPassword, loginViewModel.Password);

            if (result != PasswordVerificationResult.Success)
            {
                ModelState.AddModelError("", "Invalid Credentials.");
                return View(loginViewModel);
            }

            // Get role
            var roleType = await _dbContext.RoleModels.FirstOrDefaultAsync(r => r.RollId == user.RoleId);
            if (roleType == null)
            {
                ModelState.AddModelError("", "User has no role assigned.");
                return View(loginViewModel);
            }

            // ✅ Store UserId in Session
            HttpContext.Session.SetInt32("UserId", user.UserId);

            // ✅ Claims-based cookie auth
            var claims = new List<Claim>
            {
                 new Claim(ClaimTypes.Name, user.UserName),
                 new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                 new Claim(ClaimTypes.Email, user.UserEmail),
                 new Claim(ClaimTypes.Role, roleType.RoleName)//added role claim here, means this is going to return role type called ADMIN,SUPERADMIN,TEACHER etc
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));

            // ✅ Redirect based on role
            if (roleType.RoleName.ToLower() == "admin")
                return RedirectToAction("Index", "Admin");
            //else if (roleType.RoleName == "SuperAdmin")
            //    return RedirectToAction("Index", "SuperAdmin");
            else
                return RedirectToAction("Index", "Home");

            return View();
        }
        [HttpGet]
        public IActionResult Register()
        {
            RegisterViewModel model = new RegisterViewModel();
            model.Roles =_dbContext.RoleModels.ToList();
            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel registerViewModel)
        {
            try
            {
                //validate model state
                if (!ModelState.IsValid)
                {
                    registerViewModel.Roles = _dbContext.RoleModels.ToList();
                    return View(registerViewModel);
                }
                // Check if user email address already exists
                if (await _dbContext.UserModels.AnyAsync(u => u.UserEmail == registerViewModel.UserEmail))
                {
                    ModelState.AddModelError("UserEmail", "This email is already registered.");
                    registerViewModel.Roles = _dbContext.RoleModels.ToList();
                    return View(registerViewModel);
                }
                // ✅ Custom UserId generation
                int newUserId;
                var lastUser = await _dbContext.UserModels.OrderByDescending(u => u.UserId).FirstOrDefaultAsync();
                newUserId = lastUser == null ? 1260000 : lastUser.UserId + 1;
                var hasher = new PasswordHasher<UserModel>();

                var user = new UserModel
                {
                    UserId = newUserId,
                    UserName = registerViewModel.UserName,
                    UserEmail = registerViewModel.UserEmail,
                    RoleId = registerViewModel.RoleId,
                    CreatedDate = DateTime.UtcNow,
                };

                user.HashPassword = hasher.HashPassword(user, registerViewModel.Password);

                _dbContext.UserModels.Add(user);
                await _dbContext.SaveChangesAsync();
                TempData["RegisterUserId"] = "User Registered Successful, Your Registered UserId: " + newUserId;
                ModelState.Clear();// if i return view to the same page this code not going to work for that i have to use the below code
                var model = new RegisterViewModel
                {
                    Roles = _dbContext.RoleModels.ToList()
                };
                return View(model);
            }
            catch (Exception ex)
            {
                _errorLog.WriteErrorLog(ex,"Error In Register() POST");
                registerViewModel.Roles = _dbContext.RoleModels.ToList();
                ViewData["RegisterFailed"] = "Registration Failed!";
                return View(registerViewModel);
            }
            
        }

        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            HttpContext.Session.Clear();
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login","Account");
        }
    }
}