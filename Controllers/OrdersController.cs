using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SkiServiceAPI.Models;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SkiServiceAPI.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class OrdersController : ControllerBase
    {
        private readonly SkiServiceDbContext _context;
        private readonly ILogger<OrdersController> _logger;

        public OrdersController(SkiServiceDbContext context, ILogger<OrdersController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> GetOrders([FromQuery] string priority = null)
        {
            _logger.LogInformation("Fetching orders{PriorityFilter}", !string.IsNullOrEmpty(priority) ? $" with priority {priority}" : "");

            IQueryable<Order> query = _context.Orders;

            if (!string.IsNullOrEmpty(priority))
            {
                query = query.Where(o => o.Priority.ToLower() == priority.ToLower());
            }

            var orders = await query.ToListAsync();
            _logger.LogInformation("Fetched {Count} orders from the database.", orders.Count);

            return Ok(orders);
        }

        [Authorize(Roles = "Admin,User")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetOrderById(int id)
        {
            _logger.LogInformation("Fetching order with ID: {OrderId}", id);

            var order = await _context.Orders.FindAsync(id);
            if (order == null)
            {
                _logger.LogWarning("Order with ID {OrderId} not found.", id);
                return NotFound();
            }

            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
            var username = User.Identity.Name;

            if (userRole == "User" && order.Name != username)
            {
                _logger.LogWarning("Unauthorized access attempt by user {Username} for order {OrderId}", username, id);
                return Forbid();
            }

            _logger.LogInformation("Order with ID {OrderId} fetched successfully.", id);
            return Ok(order);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CreateOrder([FromBody] Order order)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Order creation failed due to invalid input.");
                return BadRequest(ModelState);
            }

            _logger.LogInformation("Creating a new order for {CustomerName} ({Email})", order.Name, order.Email);

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Order {OrderId} created successfully.", order.Id);
            return CreatedAtAction(nameof(GetOrderById), new { id = order.Id }, order);
        }

        [Authorize(Roles = "Admin,Employee")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateOrder(int id, [FromBody] Order order)
        {
            _logger.LogInformation("Updating order with ID: {OrderId}", id);

            if (id != order.Id)
            {
                _logger.LogWarning("Order ID mismatch for ID: {OrderId}", id);
                return BadRequest("Order ID mismatch.");
            }

            if (!await _context.Orders.AnyAsync(o => o.Id == id))
            {
                _logger.LogWarning("Order with ID {OrderId} not found for update.", id);
                return NotFound();
            }

            _context.Entry(order).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                _logger.LogInformation("Order with ID {OrderId} updated successfully.", id);
            }
            catch (DbUpdateConcurrencyException)
            {
                _logger.LogError("Error updating order with ID {OrderId}.", id);
                return StatusCode(500, "Error updating order.");
            }

            return NoContent();
        }

        [Authorize(Roles = "Admin,Employee")]
        [HttpPatch("{id}")]
        public async Task<IActionResult> PatchOrder(int id, [FromBody] JsonPatchDocument<Order> patchDoc)
        {
            if (patchDoc == null)
            {
                _logger.LogWarning("Invalid patch document received.");
                return BadRequest("Invalid patch document.");
            }

            _logger.LogInformation("Patching order with ID: {OrderId}", id);

            var order = await _context.Orders.FindAsync(id);
            if (order == null)
            {
                _logger.LogWarning("Order with ID {OrderId} not found for patching.", id);
                return NotFound();
            }

            patchDoc.ApplyTo(order, ModelState);

            if (!TryValidateModel(order))
            {
                _logger.LogWarning("Patch validation failed for order with ID: {OrderId}", id);
                return ValidationProblem(ModelState);
            }

            try
            {
                await _context.SaveChangesAsync();
                _logger.LogInformation("Order with ID {OrderId} patched successfully.", id);
            }
            catch (DbUpdateConcurrencyException)
            {
                _logger.LogError("Error patching order with ID {OrderId}.", id);
                return StatusCode(500, "Error updating order.");
            }

            return NoContent();
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrder(int id)
        {
            _logger.LogInformation("Deleting order with ID: {OrderId}", id);

            var order = await _context.Orders.FindAsync(id);
            if (order == null)
            {
                _logger.LogWarning("Order with ID {OrderId} not found for deletion.", id);
                return NotFound();
            }

            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Order with ID {OrderId} deleted successfully.", id);
            return NoContent();
        }
    }
}
