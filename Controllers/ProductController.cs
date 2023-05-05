using Cake_Rush.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using SendGrid;
using SendGrid.Helpers.Mail;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Security.Claims;

namespace Cake_Rush.Controllers
{
    public class ProductController : Controller
    {
        public static int activeSubCatId = 0;

        // GET: ProductController
        public ActionResult Index()
        {
            //initialize active sub category
            activeSubCatId = 0;
            ViewBag.activeSubCatId = activeSubCatId;
            return View();
        }

        // GET: ProductController/Details/5
        public async Task<ActionResult> Details(int id)
        {

           
            //fetching the product using product id
            ProductModel productModel = await new ApiRequests<ProductModel>().getRequestById($"api/Product/{id}",id);
            int productId = productModel.productId;
            int categoryId = productModel.categoryId;
            

            //fetching all its subcategories
            List<SubCategory> subCategoryModelList = new List<SubCategory>();
            subCategoryModelList = await new ApiRequests<SubCategory>().getRequest("api/SubCategoryMap");
            subCategoryModelList = subCategoryModelList.Where(x => x.productId == productId).ToList();

            ViewBag.subCategoryModelList = subCategoryModelList;
            Console.WriteLine("\n\n\nsubCategoryModelList");
            foreach(var item in subCategoryModelList)
            {
                Console.WriteLine(item.mapId);
            }
            //fetching its category name
            CategoryModel categoryModel = await new ApiRequests<CategoryModel>().getRequestById($"api/Category/{categoryId}", categoryId);
            ViewBag.CategoryNameById = categoryModel.categoryName;


            ViewBag.activeSubCatId = activeSubCatId;

            if (activeSubCatId > 0)
            {
                SubCategory subCategory = await new ApiRequests<SubCategory>().getRequestById($"api/SubCategoryMap/{activeSubCatId}", activeSubCatId);
                productModel.price = subCategory.price;
                productModel.label = subCategory.categoryName;
            }

            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == "6b154caa-87b6-49d0-909f-c25134ee797e")
            {
                ViewBag.ShowAddToCart = 0;
            }
            else
            {
                ViewBag.ShowAddToCart = 1;
            }
            ViewBag.ProductModel = productModel;
            return View();
        }

        public async Task<ActionResult> DetailsBySubCategory(int id)
        {
            //fetching corresponding subcategory
            SubCategory subCategoryModel = new SubCategory();
            Console.WriteLine("map id is :" + id);
            subCategoryModel = await new ApiRequests<SubCategory>().getRequestById($"api/SubCategoryMap/{id}", id);
            int productId = subCategoryModel.productId;
            activeSubCatId = id;
            ViewBag.activeSubCatId = id;
            return RedirectToAction("Details", "Product", new { id = productId });
        }
        // GET: ProductController/Create

    }
}
