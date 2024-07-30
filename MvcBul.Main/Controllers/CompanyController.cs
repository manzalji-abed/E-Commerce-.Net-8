using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using MvcBul.DataAccess.Data;
using MvcBul.DataAccess.Repository.IRepository;
using MvcBul.Models;
using MvcBul.Models.ViewModels;
using MvcBul.Utility;

namespace MvcBul.Main.Controllers
{
    [Authorize(Roles = SD.Role_User_Admin)]
    public class CompanyController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public CompanyController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            List<Company> objCompanyList = _unitOfWork.Company.GetAll().ToList();
            return View(objCompanyList);
        }
        [HttpPost]
        public async Task<IActionResult> Upsert(Company CompanyObj)
        {
            if (ModelState.IsValid)
            {
                
                if (CompanyObj.Id == 0)
                {
                    _unitOfWork.Company.Add(CompanyObj);
                }
                else
                {
                    _unitOfWork.Company.Update(CompanyObj);
                }
                _unitOfWork.Save();
                TempData["success"] = "Company created successfully";
                return RedirectToAction("Index");
            }
            else
            {
                return View(CompanyObj);
            }
        }

        [HttpPost, ActionName("Delete")]
        public IActionResult DeletePost(int? id)
        {
            Company? Company = _unitOfWork.Company.Get(c => c.Id == id);

            if (Company == null)
            {
                return NotFound();
            }
            _unitOfWork.Company.Delete(Company);
            _unitOfWork.Save();
            TempData["success"] = "Company deleted successfully";
            return RedirectToAction("Index");
        }
        public async Task<IActionResult> Upsert(int? id)
        {

            if(id==null || id == 0)
            {
                return View(new Company());
            }
            else
            {
                Company company = _unitOfWork.Company.Get(u=>u.Id==id);
                return View(company);
            }
        }


        #region API CALLS
        [HttpGet]
        public IActionResult GetAll()
        {
            List<Company> objCompanyList = _unitOfWork.Company.GetAll().ToList();
            return Json(new {data=objCompanyList});
        }

        [HttpDelete]
        public IActionResult Delete(int ?id)
        {
            var CompanyToBeDeleted = _unitOfWork.Company.Get(u=>u.Id==id);
            if (CompanyToBeDeleted == null)
            {
                return Json(new { success = false, message = "Error while deleting" });
            }
            _unitOfWork.Company.Delete(CompanyToBeDeleted);
            _unitOfWork.Save();
            return Json(new { success = true, message=" Company deleted successfully" });
        }
        #endregion
    }
}
