using Cake_Rush.Models;
using EllipticCurve.Utils;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Net;
using System.Security.Claims;

namespace Cake_Rush.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            ProductController.activeSubCatId = 0;
            ViewBag.activeSubCatId = ProductController.activeSubCatId;
            //inital setup of user profile
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId!=null)
            {
                ApiRequests<UserModel> obj = new ApiRequests<UserModel>();
                UserModel user = await obj.getRequestById($"api/User/{userId}", 0);
                if (user == null && userId != null)
                {
                    UserModel userModel = new UserModel();
                    userModel.userId = userId;
                    userModel.email = User.Identity.Name;
                    string userName = User.Identity.Name;
                    userModel.userName = userName.Substring(0, userName.IndexOf("@"));
                    userModel.mobile = "";
                    userModel.address = "";
                    userModel.city = "";
                    userModel.pincode = "";

                    Console.WriteLine(await obj.postRequest("api/User", userModel));
                }
            }
            //get all catagories
            List<CategoryModel> modelList = new List<CategoryModel>();
            modelList = await new ApiRequests<CategoryModel>().getRequest("api/Category");
            ViewBag.categoryNameList = modelList;

            //get all products
            List<ProductModel> productModelList =await new ApiRequests<ProductModel>().getRequest("api/Product");
            ViewBag.ProductModelList = productModelList;
            return View();
        }
       
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}