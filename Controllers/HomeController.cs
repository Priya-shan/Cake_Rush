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

        public async Task<IActionResult> Search()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Search(IFormCollection form)
        {
            List<CategoryModel> modelList = new List<CategoryModel>();
            modelList = await new ApiRequests<CategoryModel>().getRequest("api/Category");
            ViewBag.categoryNameList = modelList;

            //get data from search form
            string formName = form["form-name"];
            if (formName == "search-form")
            {
                string keywordToSearch = form["searchBar"].ToString().ToLower();
                List<ProductModel> productModelList = await new ApiRequests<ProductModel>().getRequest("api/Product");
                List<ProductModel> fiteredList = productModelList.FindAll(x => x.productName.ToLower().Contains(keywordToSearch));
                ViewBag.ProductModelList = fiteredList;
                ViewBag.SearchBarValue = keywordToSearch;
                return View();
            }
            if (formName == "Filters-form")
            {
                string category = form["category"];
                int category_;
                if (category == "")
                {
                    category_ = 0;
                }
                else
                {
                    category_ = Convert.ToInt32(category);
                }
                string minPrice = form["minPrice"];
                int minPrice_;
                if (minPrice == "")
                {
                    minPrice_ = 0;
                }
                else
                {
                    minPrice_ = Convert.ToInt32(minPrice);
                    ViewBag.maxPrice = minPrice_;
                }
                string maxPrice = form["maxPrice"];
                int maxPrice_;
                if (maxPrice == "")
                {
                    maxPrice_ = int.MaxValue;
                }
                else
                {
                    maxPrice_ = Convert.ToInt32(maxPrice);
                    ViewBag.minPrice = minPrice_;
                }
                //get product list and perform filtering or sorting
                List<ProductModel> productModelList = await new ApiRequests<ProductModel>().getRequest("api/Product");
                List<ProductModel> filteredList;
                if (category_ == 0)
                {
                    filteredList = productModelList.Where(x => x.price > minPrice_ && x.price < maxPrice_).ToList();

                }
                else
                {
                    filteredList = productModelList.Where(x => x.categoryId == category_ && x.price > minPrice_ && x.price < maxPrice_).ToList();

                }
                string sortby = form["sortby"];
                if (sortby == "asc")
                {
                    filteredList = filteredList.OrderBy(x => x.price).ToList();
                }
                else if (sortby == "desc")
                {
                    filteredList = filteredList.OrderByDescending(x => x.price).ToList();
                }
                ViewBag.ProductModelList = filteredList;
                ViewBag.category = category;
                return View();
            }
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}