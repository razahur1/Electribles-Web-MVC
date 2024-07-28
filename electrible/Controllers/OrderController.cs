using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using electrible.Context;
using electrible.Models;

namespace electrible.Controllers
{
    public class OrderController : Controller
    {
        private readonly ApplicationDbContext _context;

        public OrderController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Checkout()
        {
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> PlaceOrder(string address, string mobile)
        {
            try
            {
                // Validate address and mobile
                if (string.IsNullOrWhiteSpace(address) || string.IsNullOrWhiteSpace(mobile))
                {
                    return Json(new { success = false, message = "Address and mobile are required." });
                }

                var user = await GetLoggedInUser();
                if (user == null)
                {
                    return Json(new { success = false, message = "User not logged in." });
                }

                var cart = await _context.Cart
                    .Include(c => c.CartItems)
                    .ThenInclude(ci => ci.Product)
                    .FirstOrDefaultAsync(c => c.UserId == user.UserId);

                if (cart == null || cart.CartItems.Count == 0)
                {
                    return Json(new { success = false, message = "Cart is empty." });
                }

                // Create a new Order
                var order = new Order
                {
                    UserId = user.UserId,
                    OrderDate = DateTime.Now,
                    TotalAmount = cart.TotalAmount,
                    Address = address,
                    Mobile = mobile,
                    OrderItems = cart.CartItems.Select(ci => new OrderItem
                    {
                        ProductId = ci.ProductId,
                        Quantity = ci.Quantity,
                        Price = ci.Product.Price
                    }).ToList()
                };

                // Save the order
                _context.Orders.Add(order);
                await _context.SaveChangesAsync();

                // Clear the cart after placing the order
                _context.CartItems.RemoveRange(cart.CartItems);
                _context.Cart.Remove(cart);
                await _context.SaveChangesAsync();

                return Json(new { success = true, orderId = order.OrderId });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "An error occurred while placing the order." });
            }
        }

        [HttpGet]
        public async Task<IActionResult> OrderConfirmation(int orderId)
        {
            var order = await _context.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .FirstOrDefaultAsync(o => o.OrderId == orderId);

            if (order == null)
            {
                return NotFound();
            }

            return View(order);
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
