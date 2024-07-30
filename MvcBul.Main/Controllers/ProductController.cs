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
    public class ProductController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly string wwwRootPath;
        public ProductController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _webHostEnvironment = webHostEnvironment;
             wwwRootPath = _webHostEnvironment.WebRootPath;

        }
        public IActionResult Index()
        {
            List<Product> objProductList = _unitOfWork.Product.GetAll(includeProps:"Category").ToList();
            return View(objProductList);
        }
        [HttpPost]
        public async Task<IActionResult> Upsert(ProductVM productVM, List<IFormFile> files)
        {
            if (ModelState.IsValid)
            {
                if (productVM.Product.Id == 0)
                {
                    _unitOfWork.Product.Add(productVM.Product);
                }
                else
                {
                    _unitOfWork.Product.Update(productVM.Product);
                }
                if (files != null)
                {

                    foreach (IFormFile file in files)
                    {
                        string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                        string productPath = @"images\products\product-" + productVM.Product.Id;
                        string finalPath = Path.Combine(wwwRootPath, productPath);

                        if (!Directory.Exists(finalPath))
                        {
                            Directory.CreateDirectory(finalPath);
                        }

                        using (var fileStream = new FileStream(Path.Combine(finalPath, fileName), FileMode.Create))
                        {
                            file.CopyTo(fileStream);
                        }

                        ProductImage productImage = new()
                        {
                            ImageUrl = @"\" + productPath + @"\" + fileName,
                            ProductId = productVM.Product.Id
                        };
                        if (productVM.Product.ProductImages == null)
                        {
                            productVM.Product.ProductImages = new();

                        }
                        productVM.Product.ProductImages.Add(productImage);

                    }
                }
                    _unitOfWork.Product.Update(productVM.Product);
               
                _unitOfWork.Save();
                TempData["success"] = "Product created/updated successfully";
                return RedirectToAction("Index");
            }
            else
            {
                productVM.CategoryList = await _unitOfWork.Category.GetAllAsSelect();

            }
            return View();
        }
        public IActionResult Edit(int? id)
        {
            if(id == null || id==0)
            {
                return NotFound();
            }
            Product? product = _unitOfWork.Product.Get(c => c.Id == id);
            if(product == null)
            {
                return NotFound();
            }
            return View(product);
        }
        [HttpPost]
        public IActionResult Edit(Product obj)
        {
            if (ModelState.IsValid)
            {
                _unitOfWork.Product.Update(obj);
                _unitOfWork.Save();
                TempData["success"] = "Product updated successfully";
                return RedirectToAction("Index");
            }
            return View();
        }

        [HttpPost, ActionName("Delete")]
        public IActionResult DeletePost(int? id)
        {
            Product? product = _unitOfWork.Product.Get(c => c.Id == id);

            if (product == null)
            {
                return NotFound();
            }
            _unitOfWork.Product.Delete(product);
            _unitOfWork.Save();
            TempData["success"] = "Product deleted successfully";
            return RedirectToAction("Index");
        }
        public async Task<IActionResult> Upsert(int? id)
        {
            var objCategoryList = await _unitOfWork.Category.GetAllAsSelect();

            var productVM = new ProductVM();
            productVM.Product = new Product();
            productVM.CategoryList = objCategoryList;

            if(id==null || id == 0)
            {
                return View(productVM);
            }
            else
            {
                productVM.Product = _unitOfWork.Product.Get(u=>u.Id==id,includeProps:"ProductImages");
                return View(productVM);
            }
        }

        public IActionResult DeleteImage(int imageId)
        {
            var objFromDb = _unitOfWork.ProductImage.Get(u => u.Id == imageId);

            int productId = objFromDb.ProductId;
            if (objFromDb != null)
            {
                var oldImagePath = Path.Combine(wwwRootPath,
                    objFromDb.ImageUrl.TrimStart('\\'));

                if (System.IO.File.Exists(oldImagePath))
                {
                    System.IO.File.Delete(oldImagePath);
                }
                _unitOfWork.ProductImage.Delete(objFromDb);
                _unitOfWork.Save();

                TempData["success"] = "Image deleted successfully";
            }



            return RedirectToAction("Upsert", new {id=productId});
        }


        #region API CALLS
        [HttpGet]
        public IActionResult GetAll()
        {
            List<Product> objProductList = _unitOfWork.Product.GetAll(includeProps: "Category").ToList();
            return Json(new {data=objProductList});
        }

        [HttpDelete]
        public IActionResult Delete(int ?id)
        {
            var productToBeDeleted = _unitOfWork.Product.Get(u=>u.Id==id);
            if (productToBeDeleted == null)
            {
                return Json(new { success = false, message = "Error while deleting" });
            }

            string productPath = @"images\products\product-" + id;
            string finalPath = Path.Combine(wwwRootPath, productPath);

            if (Directory.Exists(finalPath))
            {
                string[] filePaths = Directory.GetFiles(finalPath);
                foreach(string path in filePaths)
                {
                    System.IO.File.Delete(path);
                }

                Directory.Delete(finalPath);
            }


            _unitOfWork.Product.Delete(productToBeDeleted);
            _unitOfWork.Save();
            return Json(new { success = true, message=" Product deleted successfully" });
        }
        #endregion
    }
}
