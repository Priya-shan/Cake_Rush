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
        public async Task<ActionResult> AddToCart(int id)
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            int subCatMapId = id;
            int quantity = 1;
            //check if the item gfor the user has been already ordered if so check if its pending
            //=> pending means dont add & redirect to orders page
            List<OrderModel> orderModelList = await new ApiRequests<OrderModel>().getRequest("api/Order");
            foreach (var order in orderModelList)
            {
                if (order.Cart.SubCatMap.mapId == subCatMapId &&
                    order.userId == userId &&
                    order.orderStatus == "Pending"
                    )
                {
                    return RedirectToAction("MyOrders", "User");
                }
            }
            //check if the item is already present in cart and cart is not expired then add +1 to it 

            List<CartModel> cartModelList = await new ApiRequests<CartModel>().getRequest("api/Cart");
            List<CartModel> currentUserscartList = cartModelList.Where(x => x.userId == userId).ToList();
            var cart = currentUserscartList.FirstOrDefault(x => x.mapId == subCatMapId);
            if (cart != null && cart.expiry == 0)
            {
                return RedirectToAction("AddCartItemCount", "Product", new { id = cart.cartId });
            }

            // else adding as new entry
            SubCategory subCategory = await new ApiRequests<SubCategory>().getRequestById($"api/SubCategoryMap/{subCatMapId}", subCatMapId);
            int price = subCategory.price;

            CartModel cartModel = new CartModel()
            {
                userId = userId,
                mapId = subCatMapId,
                quantity = quantity,
                price = price,
                expiry = 0
            };
            Console.WriteLine(await new ApiRequests<CartModel>().postRequest("api/Cart", cartModel));
            return RedirectToAction("Cart");
        }
        public async Task<List<CartModel>> getAllCarts()
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            //fetching all carts
            List<CartModel> AllCarts = await new ApiRequests<CartModel>().getRequest("api/Cart");
            //filtering out current user's cart
            List<CartModel> currentUserCarts = AllCarts.FindAll(x => x.userId == userId && x.expiry == 0);
            currentUserCarts.Reverse();
            return currentUserCarts;
        }
        public async Task<IActionResult> Cart()
        {
            //get all carts
            List<CartModel> cartContentModelList = new List<CartModel>();
            cartContentModelList = await getAllCarts();
            return View(cartContentModelList);
        }


        public async Task<IActionResult> AddCartItemCount(int id)
        {
            //get current cart model
            CartModel cartModel = await new ApiRequests<CartModel>().getRequestById($"api/Cart/{id}", id);
            //update quantity by 1
            int updatedQuantity = cartModel.quantity + 1;
            cartModel.quantity = updatedQuantity;
            //get original price from subcategory model
            SubCategory subCategory = await new ApiRequests<SubCategory>().getRequestById($"api/SubCategoryMap/{cartModel.mapId}", cartModel.mapId);
            //calculate total price and update the table
            cartModel.price = subCategory.price * updatedQuantity;
            Console.WriteLine(await new ApiRequests<CartModel>().putRequest($"api/Cart/{id}", id, cartModel));
            return RedirectToAction("Cart");
        }

        public async Task<IActionResult> MinusCartItemCount(int id)
        {
            CartModel cartModel = await new ApiRequests<CartModel>().getRequestById($"api/Cart/{id}", id);

            int updatedQuantity = cartModel.quantity - 1;
            if (updatedQuantity == 0)
            {
                return RedirectToAction("RemoveCartItem", "Product", new { id = cartModel.cartId });
            }
            cartModel.quantity = updatedQuantity;
            SubCategory subCategory = await new ApiRequests<SubCategory>().getRequestById($"api/SubCategoryMap/{cartModel.mapId}", cartModel.mapId);
            cartModel.price = subCategory.price * updatedQuantity;
            Console.WriteLine(await new ApiRequests<CartModel>().putRequest($"api/Cart/{id}", id, cartModel));
            return RedirectToAction("Cart");
        }
        public async Task<IActionResult> RemoveCartItem(int id)
        {
            Console.WriteLine(await new ApiRequests<CartModel>().deleteRequest($"api/Cart/{id}", id));
            return RedirectToAction("Cart");
        }

        public async Task<IActionResult> RemoveAllCartItems()
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            List<CartModel> cartModels = await new ApiRequests<CartModel>().getRequest("api/Cart");
            cartModels = cartModels.Where(x => x.userId == userId && x.expiry == 0).ToList();
            foreach (var cart in cartModels)
            {
                Console.WriteLine(await new ApiRequests<CartModel>().deleteRequest($"api/Cart/{cart.cartId}", cart.cartId));
            }
            return RedirectToAction("Cart");
        }

    }
}
