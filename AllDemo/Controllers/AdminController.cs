using AllDemo.Data;
using AllDemo.Helper.Log;
using AllDemo.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AllDemo.Controllers
{
    //[Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly AppDbContext _context;
        private readonly ErrorLog _errorLog;
        public AdminController(AppDbContext context,ErrorLog errorLog)
        {
            _context = context;
            _errorLog = errorLog;
        }
        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public IActionResult AddRole()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddRole(RoleModel model)

        {
            try
            {
                if (ModelState.IsValid)
                {
                    var role=await _context.RoleModels.FirstOrDefaultAsync(m =>
                        m.RoleName.ToLower() == model.RoleName.ToLower());
                    if (role == null)
                    {
                        _context.RoleModels.Add(model);
                        await _context.SaveChangesAsync();
                        ModelState.Clear();
                        var modelRole = new RoleModel()
                        {
                        };
                        TempData["AddRoleSuccess"] = "Role Added Successfully";
                        return View(modelRole);
                    }
                    else
                    {
                        ModelState.AddModelError("","Role Type Already exists");
                        return View(model);
                    }
                    
                }
                else
                {
                    return View(model);
                }
            }
            catch (Exception ex)
            {
                _errorLog.WriteErrorLog(ex,"Error generate at POST AddRole()");
                TempData["AddRoleFailed"] = "Role Added Failed!";
                return View(model);
            }
        }

    }
}
