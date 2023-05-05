using Cake_Rush.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Cake_Rush.Controllers
{
    public class UserController : Controller
    {
        // GET: UserController
        public static string userId;

        public async Task<ActionResult> Index()
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            Console.WriteLine("user id " + userId);
            UserModel user = await new ApiRequests<UserModel>().getRequestById($"api/User/{userId}", 0);
            //ViewBag.UserModel = user;
            return View(user);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> UpdateProfile(IFormCollection form)
        {
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
            Console.WriteLine(await new ApiRequests<UserModel>().putRequest($"api/User/{userId}", 0, userModel));
            return RedirectToAction("Index");
        }

        public async Task<ActionResult> MyOrders()
        {
            List<OrderModel> ordersList = await new ApiRequests<OrderModel>().getRequest("api/Order");
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            List<OrderModel> currentUserOrderList = ordersList.FindAll(x => x.Cart.userId == userId);

            Dictionary<string, List<OrderModel>> plansDictionary = new Dictionary<string, List<OrderModel>>();

            List<OrderModel> activeOrdersList = ordersList.Where(x => x.orderStatus == "Pending").ToList();

            List<OrderModel> completedOrdersList = ordersList.Where(x => x.orderStatus == "Completed").ToList();
            
            activeOrdersList = activeOrdersList.OrderByDescending(x => x.dateOrdered).ToList();
            completedOrdersList = completedOrdersList.OrderByDescending(x => x.dateOrdered).ToList();
            
            plansDictionary.Add("Active", activeOrdersList);
            plansDictionary.Add("Completed", completedOrdersList);

            ViewBag.PlansDictionary = plansDictionary;
            //currentUserOrderList
            return View(currentUserOrderList);
        }
    }
}
