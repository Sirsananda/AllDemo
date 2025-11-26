using System.Collections.Immutable;
using AllDemo.Data.context;
using AllDemo.Helper.Log;
using AllDemo.Models;
using AllDemo.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AllDemo.Controllers
{
    [Authorize(Roles = "ADMIN")]
    public class AdminController : Controller
    {
        private readonly AppDbContext _context;
        private readonly ErrorLog _errorLog;
        public AdminController(AppDbContext context, ErrorLog errorLog)
        {
            _context = context;
            _errorLog = errorLog;
        }
        public IActionResult Index()//this is the dashboard page
        {
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> AddRole()
        {
            var roleList = new RoleViewModel()
            {
                RoleList = await _context.RoleModels.ToListAsync()
            };
            return View(roleList);
        }

        [HttpPost]
        [ValidateAntiForgeryToken] /*protect against CSRF attacks*/
        public async Task<IActionResult> AddRole(RoleViewModel roleViewModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var role = await _context.RoleModels.FirstOrDefaultAsync(m =>
                        m.RoleName.ToLower() == roleViewModel.RoleName.ToLower());
                    if (role == null)
                    {
                        _context.RoleModels.Add(new RoleModel { RoleName = roleViewModel.RoleName });
                        await _context.SaveChangesAsync();
                        //ModelState.Clear();
                        //var modelRole = new RoleViewModel()
                        //{
                        //};
                        TempData["AddRoleSuccess"] = "Role Added Successfully";
                        return RedirectToAction("AddRole");
                    }
                    else
                    {
                        ModelState.AddModelError("", "Role Type Already exists");
                        //return View(roleViewModel);
                    }
                }
                roleViewModel.RoleList = await _context.RoleModels.ToListAsync();
                return View(roleViewModel);
            }
            catch (Exception ex)
            {
                _errorLog.WriteErrorLog(ex, "Error generate at POST AddRole()");
                TempData["AddRoleFailed"] = "Role Added Failed!";
                return View(roleViewModel);
            }
        }

        [HttpGet]
        public async Task<IActionResult> EditRole(int id)
        {
            RoleModel model = await _context.RoleModels.FindAsync(id);
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> EditRole(int id, RoleModel model)
        {
            if (id != model.RollId)
            {
                ModelState.AddModelError("","RoleId does not match!");
                return View(model);
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var existingRole =await _context.RoleModels.FindAsync(id);
            if (existingRole == null)
            {
                return NotFound();
            }
            
            existingRole.RoleName = model.RoleName;
            await _context.SaveChangesAsync();
            TempData["UpdateRole"] = "Role Updated Successfully";
            return RedirectToAction("AddRole");
        }
    }
}
