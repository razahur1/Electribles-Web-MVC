using electrible.Context;
using electrible.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace electrible.Controllers
{
    public class CartController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CartController(ApplicationDbContext context)
        {
            _context = context;
        }
        [HttpGet]
        public async Task<IActionResult> Cart()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetCartItems()
        {
            var user = await GetLoggedInUser();
            if (user == null)
            {
                return Json(new { success = false, message = "User not logged in." });
            }

            var cart = await _context.Cart
                .Include(c => c.CartItems)
                .ThenInclude(ci => ci.Product)
                .FirstOrDefaultAsync(c => c.UserId == user.UserId);

            if (cart == null)
            {
                return Json(new { success = false, message = "Cart not found." });
            }

            var cartItems = cart.CartItems.Select(ci => new
            {
                cartItemId = ci.CartItemId,
                product = new
                {
                    imageUrl = ci.Product.ImageUrl,
                    name = ci.Product.Name,
                    price = ci.Product.Price
                },
                quantity = ci.Quantity
            }).ToList();

            return Json(cartItems);
        }

        [HttpPost]
        public async Task<IActionResult> AddToCart(int productId, int quantity = 1)
        {
            var user = await GetLoggedInUser();
            if (user == null)
            {
                return Json(new { success = false, message = "Please logged in first..." });
            }

            var cart = await _context.Cart.Include(c => c.CartItems)
                                           .FirstOrDefaultAsync(c => c.UserId == user.UserId);

            if (cart == null)
            {
                cart = new Cart
                {
                    UserId = user.UserId,
                    CartItems = new List<CartItem>()
                };
                _context.Cart.Add(cart);
            }

            var product = await _context.Products.FindAsync(productId);
            if (product == null)
            {
                return Json(new { success = false, message = "Product not found." });
            }

            var cartItem = cart.CartItems.FirstOrDefault(ci => ci.ProductId == productId);
            if (cartItem == null)
            {
                cartItem = new CartItem
                {
                    ProductId = productId,
                    Quantity = quantity,
                    Price = product.Price
                };
                cart.CartItems.Add(cartItem);
            }
            else
            {
                cartItem.Quantity += quantity;
            }

            cart.TotalAmount = cart.CartItems.Sum(ci => ci.Price * ci.Quantity);

            await _context.SaveChangesAsync();
            return Json(new { success = true });
        }

        [HttpPost]
        public async Task<IActionResult> UpdateCartItem(int cartItemId, int quantity)
        {
            var cartItem = await _context.CartItems.FindAsync(cartItemId);
            if (cartItem == null)
            {
                return Json(new { success = false, message = "Cart item not found." });
            }

            cartItem.Quantity = quantity;
            await _context.SaveChangesAsync();
            await UpdateCartTotal(cartItem.CartId);
            return Json(new { success = true });
        }

        [HttpPost]
        public async Task<IActionResult> RemoveFromCart(int cartItemId)
        {
            var cartItem = await _context.CartItems.FindAsync(cartItemId);
            if (cartItem == null)
            {
                return Json(new { success = false, message = "Cart item not found." });
            }

            _context.CartItems.Remove(cartItem);
            await _context.SaveChangesAsync();
            await UpdateCartTotal(cartItem.CartId);
            return Json(new { success = true });
        }

        private async Task UpdateCartTotal(int cartId)
        {
            var cart = await _context.Cart.Include(c => c.CartItems)
                                           .FirstOrDefaultAsync(c => c.CartId == cartId);
            if (cart != null)
            {
                cart.TotalAmount = cart.CartItems.Sum(ci => ci.Price * ci.Quantity);
                await _context.SaveChangesAsync();
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetCartItemCount()
        {
            var user = await GetLoggedInUser();
            if (user == null)
            {
                // If user is not logged in or cart is not found, return 0 as item count
                return Json(new { itemCount = 0 });
            }

            var cart = await _context.Cart
                .Include(c => c.CartItems)
                .FirstOrDefaultAsync(c => c.UserId == user.UserId);

            if (cart == null)
            {
                // If cart is not found, return 0 as item count
                return Json(new { itemCount = 0 });
            }

            // Return the count of cart items
            return Json(new { itemCount = cart.CartItems.Count });
        }

        private async Task<User> GetLoggedInUser()
        {
            var username = HttpContext.Session.GetString("LoggedInUser");
            if (string.IsNullOrEmpty(username))
            {
                return null;
            }

            return await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
        }

    }
}
