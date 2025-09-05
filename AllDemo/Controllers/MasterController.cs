using AllDemo.Data;
using AllDemo.Helper.Log;
using AllDemo.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AllDemo.Controllers
{
    [Authorize(Roles = "ADMIN")]
    public class MasterController : Controller
    {
        private readonly AppDbContext _appDbContext;
        private readonly ErrorLog _errorLog;
        public MasterController(AppDbContext appDbContext, ErrorLog errorLog)
        {
            _appDbContext = appDbContext;
            _errorLog = errorLog;
        }
        [HttpGet]
        public async Task<IActionResult> GetAdmissionType()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PostAdmissionType(AdmissionTypeModel model)
        {
            if (!ModelState.IsValid)
                return View("GetAdmissionType", model);
            try
            {
                var admissionType = await _appDbContext.AdmissionTypes.FirstOrDefaultAsync(a =>
                    a.AdmissionTypeName.ToLower() == model.AdmissionTypeName.ToString());
                if (admissionType == null)
                {
                    await _appDbContext.AdmissionTypes.AddAsync(new AdmissionTypeModel { AdmissionTypeName = model.AdmissionTypeName, IsActive = model.IsActive });
                    await _appDbContext.SaveChangesAsync();
                    TempData["AddAdmissionTypeSuccess"] = "Admission Type Added Successfully";
                    return RedirectToAction("GetAdmissionType");
                }
                else
                {
                    ModelState.AddModelError("", "Admission Type are already Exists");
                }
            }
            catch (Exception ex)
            {
                _errorLog.WriteErrorLog(ex, "Error generate at PostAdmissionTypeTask()");
            }
            return View("GetAdmissionType");
        }

        [HttpGet]
        public async Task<IActionResult> AdmissionTypeList()
        {
            List<AdmissionTypeModel> list = new List<AdmissionTypeModel>();
            list = _appDbContext.AdmissionTypes.ToList();
            return View(list);
        }

        [HttpGet]
        public async Task<IActionResult> EditAdmissionType(int id)
        {
            AdmissionTypeModel getAdmissionType =
                await _appDbContext.AdmissionTypes.FindAsync(id);
            if (getAdmissionType != null)
                return View(getAdmissionType);
            else
            {
                TempData["FailedToFindAdmissionType"] = "Admission Type are not found";
                return View("AdmissionTypeList");
            }
        }

        [HttpPost]
        public async Task<IActionResult> EditAdmissionType(int id, AdmissionTypeModel model)
        {
            if (id != model.AdmissionTypeId)
            {
                ModelState.AddModelError("", "Admission type are not matching");
                return View(model);
            }
            if (!ModelState.IsValid)
                return View(model);
            var existingAdmissionType = await _appDbContext.AdmissionTypes.FindAsync(id);
            if (existingAdmissionType == null)
            {
                ModelState.AddModelError("", "Recording Not found");
                return View(model);
            }

            existingAdmissionType.AdmissionTypeName = model.AdmissionTypeName;
            existingAdmissionType.IsActive = model.IsActive;
            await _appDbContext.SaveChangesAsync();
            TempData["UpdateRole"] = "Admission Type Updated Successfully";
            return RedirectToAction("AdmissionTypeList");
        }

        [HttpGet]
        public IActionResult DeleteAdmissionType(int id)
        {
            if (id == null || id == 0)
            {
                ViewBag.ErrorFindID = "Please Select Admission Type";
                return View("AdmissionTypeList");
            }

            var admissionType =  _appDbContext.AdmissionTypes.Find(id);
            if (admissionType != null)
            {
                _appDbContext.AdmissionTypes.Remove(admissionType);
                _appDbContext.SaveChangesAsync();
                TempData["DeleteSuccessAdmissionType"] = "Delete Admission Type :" + admissionType.AdmissionTypeName;
            }
            return RedirectToAction("AdmissionTypeList");
        }
    }
}
