using Cake_Rush.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;

namespace Cake_Rush.Controllers
{
    public class ProductController : Controller
    {
        // GET: ProductController
        public ActionResult Index()
        {
            return View();
        }

        // GET: ProductController/Details/5
        public async Task<ActionResult> Details(int id)
        {
            ApiRequests<ProductModel> obj = new ApiRequests<ProductModel>();
            ProductModel productModel = await obj.getRequestById($"api/Product/{id}",id);
            int productId = productModel.productId;
            int categoryId = productModel.categoryId;
            ViewBag.ProductModel = productModel;

            ApiRequests<SubCategory> obj1 = new ApiRequests<SubCategory>();
            List<SubCategory> subCategoryModelList = new List<SubCategory>();
            subCategoryModelList = await obj1.getRequest("api/SubCategoryMap");
            subCategoryModelList = subCategoryModelList.Where(x => x.productId == productId).ToList();
            ViewBag.subCategoryModelList = subCategoryModelList;

            ApiRequests<CategoryModel> obj2 = new ApiRequests<CategoryModel>();
            CategoryModel categoryModel = await obj2.getRequestById($"api/Category/{categoryId}", categoryId);
            ViewBag.CategoryNameById = categoryModel.categoryName;

            return View();
        }

        public async Task<ActionResult> DetailsBySubCategory(int id)
        {
            ApiRequests<SubCategory> obj1 = new ApiRequests<SubCategory>();
            SubCategory subCategoryModel = new SubCategory();
            Console.WriteLine("map id is :" + id);
            subCategoryModel = await obj1.getRequestById($"api/SubCategoryMap/{id}",id);
            Console.WriteLine("-> "+subCategoryModel.mapId);
            Console.WriteLine(subCategoryModel.categoryName);
            Console.WriteLine(subCategoryModel.productId);
            Console.WriteLine(subCategoryModel.price);
            int productId = subCategoryModel.productId;
            ViewBag.activeSubCatId = id;

            ApiRequests<ProductModel> obj = new ApiRequests<ProductModel>();
            ProductModel productModel = await obj.getRequestById($"api/Product/{productId}", productId);
            int categoryId=productModel.categoryId;
            productModel.label = subCategoryModel.categoryName;
            productModel.price = subCategoryModel.price;
            ViewBag.ProductModel = productModel;

            ApiRequests<SubCategory> obj3 = new ApiRequests<SubCategory>();
            List<SubCategory> subCategoryModelList = new List<SubCategory>();
            subCategoryModelList = await obj3.getRequest("api/SubCategoryMap");
            subCategoryModelList = subCategoryModelList.Where(x => x.productId == productId).ToList();
            ViewBag.subCategoryModelList = subCategoryModelList;

            ApiRequests<CategoryModel> obj2 = new ApiRequests<CategoryModel>();
            CategoryModel categoryModel = await obj2.getRequestById($"api/Category/{categoryId}", categoryId);
            ViewBag.CategoryNameById = categoryModel.categoryName;

            return View();
        }
            // GET: ProductController/Create
        public ActionResult Orders(int id)
        {
            ViewBag.id = id;
            return View();
        }

        // POST: ProductController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: ProductController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: ProductController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: ProductController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: ProductController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
