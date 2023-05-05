using Cake_Rush.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Json;


namespace Cake_Rush.Controllers
{
    public class AdminController : Controller
    {
        // GET: AdminController
        public static string userId;
        public async Task<ActionResult> Index()
        {
            //get user data
            userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            Console.WriteLine("user id " + userId);
            UserModel user = await new ApiRequests<UserModel>().getRequestById($"api/User/{userId}", 0);
            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> UpdateProfile(IFormCollection form)
        {
            //get user id and user details
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            Console.WriteLine("user id " + userId);
            UserModel userModel = new UserModel();
            userModel.userId = userId;
            userModel.userName = form["userName"];
            userModel.mobile = form["mobile"];
            userModel.address = form["address"];
            userModel.city = form["city"];
            userModel.pincode = form["pincode"];
            userModel.email = form["email"];
            //update user table
            Console.WriteLine(await new ApiRequests<UserModel>().putRequest($"api/User/{userId}", 0, userModel));
            return RedirectToAction("Index");
        }

        // GET: AdminController/Details/5
        public async Task<ActionResult> AddProducts()
        {
            List<CategoryModel> categoryNameList = await new ApiRequests<CategoryModel>().getRequest("api/Category");
            return View(categoryNameList);
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
                ProductModel productModel = new ProductModel();
                productModel.productName = form["productName"];
                productModel.categoryId = Convert.ToInt32(form["category"]);
                productModel.productDescription = form["desc"];
                productModel.price = Convert.ToInt32(form["subCatPrice_0"]);
                productModel.imageid = newFileName;
                productModel.label = form["subCatName_0"];
                
                //sending post request to add product
                string response = await new ApiRequests<ProductModel>().postRequest("api/Product", productModel);
                //Console.WriteLine(response);
                
                //getting product_id from the response returned
                var jsonResponse = JsonSerializer.Deserialize<JsonElement>(response);
                var productId = jsonResponse.GetProperty("productId").GetInt32();

                //fetching additional fields
                int fieldCount = (Request.Form.Count-4)/2;
                int idx = 0;
                Console.WriteLine("\n\n\n\nRequest.Form.Count " + Request.Form.Count);
                Console.WriteLine("\n\n\n\n\n\n enetered field count"+ fieldCount+ " "+ productId);
                //sending post request for additional fields
                while (fieldCount > 0)
                {
                    Console.WriteLine("entered field count");
                    SubCategory subcategoryModel = new SubCategory();
                    subcategoryModel.productId = productId;
                    subcategoryModel.categoryName = form[$"subCatName_{idx}"];
                    subcategoryModel.price = Convert.ToInt32(form[$"subCatPrice_{idx}"]);
                    Console.WriteLine("\n\n\n"+fieldCount + " " + idx);
                    Console.WriteLine("gonna posttttttttttttt !!!!!");
                    Console.WriteLine(subcategoryModel.productId + " " + subcategoryModel.categoryName + " " + subcategoryModel.price);
                    Console.WriteLine(await new ApiRequests<SubCategory>().postRequest("api/SubCategoryMap", subcategoryModel));
                    idx++;
                    fieldCount--;
                   
                }
                return RedirectToAction("Index","Home");
            }
                
            
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        //[Bind("categoryName")CategoryModel CategoryModel
        public async Task<IActionResult> AddCategory(IFormCollection form)
        {
            CategoryModel model = new CategoryModel();
            model.categoryName = form["categoryName"];
            Console.WriteLine(await new ApiRequests<CategoryModel>().postRequest("api/Category",model));
            return RedirectToAction("AddProducts");
        }

        public async Task<ActionResult> Orders()
        {

            //get all orders
            List<OrderModel> ordersList = await new ApiRequests<OrderModel>().getRequest("api/Order");

            Dictionary<string, List<OrderModel>> plansDictionary = new Dictionary<string, List<OrderModel>>();

            //get active and completed orders
            List<OrderModel> activeOrdersList = ordersList.Where(x => x.orderStatus == "Pending").ToList();
            List<OrderModel> completedOrdersList = ordersList.Where(x => x.orderStatus == "Completed").ToList();

            //sort it by date descending
            activeOrdersList = activeOrdersList.OrderByDescending(x => x.dateOrdered).ToList();
            completedOrdersList = completedOrdersList.OrderByDescending(x => x.dateOrdered).ToList();

            //add to dictionary
            plansDictionary.Add("Active", activeOrdersList);
            plansDictionary.Add("Completed", completedOrdersList);

            ViewBag.PlansDictionary = plansDictionary;
            return View(ordersList);
        }

        public async Task<IActionResult> CompleteOrder(int id)
        {
            int orderId = id;
            //get current order and update its status
            OrderModel orderModel = await new ApiRequests<OrderModel>().getRequestById($"api/Order/{orderId}", orderId);
            orderModel.orderStatus = "Completed";
            Console.WriteLine(await new ApiRequests<OrderModel>().putRequest($"api/Order/{orderId}", orderId, orderModel));
            return RedirectToAction("Orders");
        }

    }
}
