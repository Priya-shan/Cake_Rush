using Cake_Rush.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Net;

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
            ApiRequests<CategoryModel> obj = new ApiRequests<CategoryModel>();
            List<CategoryModel> modelList = new List<CategoryModel>();
            modelList = await obj.getRequest("api/Category");
            ViewBag.categoryNameList = modelList;

            ApiRequests<ProductModel> obj1 = new ApiRequests<ProductModel>();
            List<ProductModel> productModelList =await obj1.getRequest("api/Product");
            ViewBag.ProductModelList = productModelList;
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}