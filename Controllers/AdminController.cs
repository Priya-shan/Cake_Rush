using Cake_Rush.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Net.Http.Headers;
using System.Text.Json;


namespace Cake_Rush.Controllers
{
    public class AdminController : Controller
    {
        // GET: AdminController
        public ActionResult Index()
        {
            return View();
        }

        // GET: AdminController/Details/5
        public async Task<ActionResult> AddProducts()
        {
            ApiRequests<CategoryModel> obj = new ApiRequests<CategoryModel>();
            List<CategoryModel> modelList=new List<CategoryModel>();
            modelList = await obj.getRequest("api/Category");
            ViewBag.categoryNameList= modelList;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddProducts(IFormCollection form)
        {
            var productImage=form.Files.GetFile("file");
            //checking if the file is empty
            if (productImage == null || productImage.Length == 0)
            {
                return RedirectToAction("AddProducts");
            }
            else
            {
                //setting up unique file name
                var originalFileName = productImage.FileName;
                var randomNumber = new Random().Next(100000, 999999);
                var newFileName = $"{Path.GetFileNameWithoutExtension(originalFileName)}_" +
                                  $"{randomNumber}{Path.GetExtension(originalFileName)}";
                newFileName = "Product-" + newFileName;
                //setting up the pathe to save
                var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Images/Products", newFileName);
                
                //saving the file
                using (var stream = new FileStream(path, FileMode.Create))
                {
                    await productImage.CopyToAsync(stream);
                }
                
                //preparing product model
                ApiRequests<ProductModel> obj = new ApiRequests<ProductModel>();
                ProductModel productModel = new ProductModel();
                productModel.productName = form["productName"];
                productModel.categoryId = Convert.ToInt32(form["category"]);
                productModel.productDescription = form["desc"];
                productModel.price = Convert.ToInt32(form["subCatPrice"]);
                productModel.imageid = newFileName;
                productModel.label = form["subCatName"];
                
                //sending post request to add product
                string response = await obj.postRequest("api/Product", productModel);
                Console.WriteLine(response);
                
                //getting product_id from the response returned
                var jsonResponse = JsonSerializer.Deserialize<JsonElement>(response);
                var productId = jsonResponse.GetProperty("productId").GetInt32();
                
                //preparing subcategory model
                ApiRequests<SubCategory> obj1 = new ApiRequests<SubCategory>();
                SubCategory subcategoryModel = new SubCategory();
                subcategoryModel.productId = productId;
                subcategoryModel.categoryName = form["subCatName"];
                subcategoryModel.price = Convert.ToInt32(form["subCatPrice"]);
                
                //sending post request to add subcategory
                Console.WriteLine(await obj1.postRequest("api/SubCategoryMap", subcategoryModel));
                
                //fetching additional fields
                int fieldCount = (Request.Form.Count-6)/2;
                int idx = 1;
                
                //sending post request for additional fields
                while(fieldCount > 0)
                {
                    
                    subcategoryModel.productId = productId;
                    subcategoryModel.categoryName = form[$"subCatName_{idx}"];
                    subcategoryModel.price = Convert.ToInt32(form[$"subCatPrice_{idx}"]);
                    Console.WriteLine(await obj1.postRequest("api/SubCategoryMap", subcategoryModel));
                    idx++;
                    fieldCount--;
                }
                return RedirectToAction("Index");
            }
                
            
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        //[Bind("categoryName")CategoryModel CategoryModel
        public async Task<IActionResult> AddCategory(IFormCollection form)
        {
            ApiRequests<CategoryModel> obj = new ApiRequests<CategoryModel>();
            CategoryModel model = new CategoryModel();
            model.categoryName = form["categoryName"];
            Console.WriteLine(await obj.postRequest("api/Category",model));
            return RedirectToAction("Index");
        }

        // GET: AdminController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: AdminController/Create
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

        // GET: AdminController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: AdminController/Edit/5
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

        // GET: AdminController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: AdminController/Delete/5
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
