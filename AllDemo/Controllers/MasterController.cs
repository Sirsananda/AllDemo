using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AllDemo.Data.context;
using AllDemo.Helper.Log;
using AllDemo.Models;
using AllDemo.ViewModels;
using Ganss.Xss;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ValueGeneration.Internal;
using MimeKit.Tnef;

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
        #region start Admission type
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
        #endregion
        #region start academic year

        [HttpGet("AcademicYears")]
        public IActionResult GetAcademicYear()
        {
            List<AcademicYearModel> listAcademicYearModel = new List<AcademicYearModel>();
            listAcademicYearModel = _appDbContext.AcademicYears.ToList();
            return View(listAcademicYearModel);
        }

        [HttpGet("SaveAcademicYear")]
        public IActionResult InsertAcademicYear()
        {
            return View();
        }

        [HttpPost("SaveAcademicYear")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> InsertAcademicYear(AcademicYearModel model)
        {
            if (!ModelState.IsValid)
                return View(model);
            try
            {
                var admissionType =
                    await _appDbContext.AcademicYears.FirstOrDefaultAsync(m =>
                        m.Year_Name.ToLower() == model.Year_Name.ToLower());
                if (admissionType != null)
                {
                    ModelState.AddModelError("",
                        "Academic Year " + model.Year_Name + " already exists! please change Academic Year");
                    return View(model);
                }
                else
                {
                    await _appDbContext.AcademicYears.AddAsync(model);
                    await _appDbContext.SaveChangesAsync();
                    TempData["Success"] = "Academic Year("+model.Year_Name+") Save Successfully";
                    //clear the input field
                    ModelState.Clear();
                    return View(new AcademicYearModel());
                }
            }catch (Exception ex)
            {
                _errorLog.WriteErrorLog(ex, "Error Occur: Error generate at InsertAcademicYear()");
                ModelState.AddModelError("","Academic Year ("+model.Year_Name+") Save failed !");
                return View();
            }
        }

        [HttpGet("EditAcademicYear")]
        public async Task<IActionResult> ModifyAcademicYear(int id)
        {
            var academicYear = await _appDbContext.AcademicYears.FindAsync(id);
            if (academicYear == null)
            {
                TempData["FetchFailedAcademicYear"] = "Academic Year does not exists";
                return View("GetAcademicYear");
            }
            else
            {
                var model = new AcademicYearModel()
                {
                    Year_Id = academicYear.Year_Id,
                    Year_Name = academicYear.Year_Name,
                    Start_Date = academicYear.Start_Date,
                    End_Date = academicYear.End_Date
                };
                return View(model);
            }
        }

        [HttpPost("EditAcademicYear")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ModifyAcademicYear(int id, AcademicYearModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var academicYear = await _appDbContext.AcademicYears.FirstOrDefaultAsync(m => m.Year_Id == model.Year_Id);
            if (academicYear == null)
            {
                TempData["AcademicYearFindFailed"] = "Record are not match!";
                return View("ModifyAcademicYear", model);
            }

            academicYear.Year_Name = model.Year_Name;
            academicYear.Start_Date = model.Start_Date;
            academicYear.End_Date = model.End_Date;
            _appDbContext.Update(academicYear);
            _appDbContext.SaveChanges();
            TempData["AcademicYearChangeSuccess"] = "Academic Year("+model.Year_Name+") Changes Successfully";
            return RedirectToAction("GetAcademicYear");
        }
        #endregion
        #region CRUD Semester
        [HttpGet("Semesters")]
        public IActionResult GetSemesterList()
        {
            List<SemesterModel> lstModel = _appDbContext.Semesters.Include(s => s.AcademicYear).ToList();
            //List<SemesterModel> lstModel=new List<SemesterModel>();
            //lstModel=_appDbContext.Semesters.ToList();
            return View(lstModel);
        }
        [HttpGet("CreateSemester")]
        public IActionResult CreateSemester()
        {
            SemesterViewModel semesterViewModel = new SemesterViewModel();
            semesterViewModel.AcademicYears = _appDbContext.AcademicYears.ToList();
            return View(semesterViewModel);
        }
        [HttpPost("CreateSemester")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateSemester(SemesterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.AcademicYears =await _appDbContext.AcademicYears.ToListAsync();
                return View(model);
            }
            // 🧹 Step 1: Sanitize user input (before database checks)
            var sanitizer = new HtmlSanitizer();
            model.SemesterName = sanitizer.Sanitize(model.SemesterName ?? string.Empty);

            // Step 2: Handle sanitization result gracefully (don’t throw or mark invalid)
            if (string.IsNullOrWhiteSpace(model.SemesterName))
            {
                // If it becomes empty after sanitization, assign a fallback or just return
                ModelState.AddModelError("SemesterName", "Invalid or empty semester name.");
                model.AcademicYears = await _appDbContext.AcademicYears.ToListAsync();
                return View(model);
            }

            var semester = await _appDbContext.Semesters.FirstOrDefaultAsync(s =>
                s.SemesterName.ToLower()==model.SemesterName.ToLower() &&
                s.Year_Id == model.Academic_Year_id);
            if (semester != null)
            {
                ModelState.AddModelError("", "Semester Name Already Exists !");
                model.AcademicYears = await _appDbContext.AcademicYears.ToListAsync();
                return View(model);
            }
            else
            {
                semester = new SemesterModel
                {
                    SemesterName = model.SemesterName,
                    Year_Id = model.Academic_Year_id??0,
                    StartDate = model.StartDate,
                    EndDate = model.EndDate
                };
                await _appDbContext.Semesters.AddAsync(semester);
                await _appDbContext.SaveChangesAsync();
                TempData["SemesterSuccess"] = "Semester Added Successfully";
                model.AcademicYears = await _appDbContext.AcademicYears.ToListAsync();
                return View(model);
            }
        }

        [HttpGet("ModifySemester")]
        public async Task<IActionResult> EditSemester(int? id)
        {
            SemesterViewModel model = new SemesterViewModel();
            if (id is not null and not 0)
            {
                var semester =await _appDbContext.Semesters.FindAsync(id);
                if (semester != null)
                {
                    model.SemesterId = semester.SemesterId;
                    model.Academic_Year_id = semester.Year_Id;
                    model.SemesterName = semester.SemesterName;
                    model.StartDate = semester.StartDate;
                    model.EndDate = semester.EndDate;
                    model.AcademicYears = await _appDbContext.AcademicYears.ToListAsync();
                    return View(model);
                }
                else
                {
                    TempData["SemesterDoesNotFind"] = "Semester Does Not Exists !";
                    return RedirectToAction("GetSemesterList");
                }
            }
            else
            {
                TempData["SemesterDoesNotFind"] = "SemesterId Does Not Match!";
                return RedirectToAction("GetSemesterList");
            }
        }

        [HttpPost("ModifySemester")]
        public async Task<IActionResult> EditSemester(int? id, SemesterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.AcademicYears = await _appDbContext.AcademicYears.ToListAsync();
                return View(model);
            }

            var sanitize = new HtmlSanitizer();
            model.SemesterName = sanitize.Sanitize(model.SemesterName ?? string.Empty);
            if (string.IsNullOrEmpty(model.SemesterName))
            {
                ModelState.AddModelError("SemesterName","Invalid or empty semester name");
                model.AcademicYears = await _appDbContext.AcademicYears.ToListAsync();
                return View(model);
            }
            var semester = await _appDbContext.Semesters.FirstOrDefaultAsync(s=>s.SemesterId==model.SemesterId);
            if (semester == null)
            {
                ModelState.AddModelError("", "Semester Does not Exists !");
                model.AcademicYears = await _appDbContext.AcademicYears.ToListAsync();
                return View(model);
            }
            else
            {
                semester.SemesterId = model.SemesterId??0;
                semester.Year_Id = model.Academic_Year_id??0;
                semester.SemesterName = model.SemesterName;
                semester.StartDate = model.StartDate;
                semester.EndDate = model.EndDate;
                 _appDbContext.Update(semester);
                await _appDbContext.SaveChangesAsync();
                TempData["UpdateSemesterSuccess"] = "Semester Updated Successfully";
                return RedirectToAction("GetSemesterList");
            }
        }
        #endregion

    }
}
