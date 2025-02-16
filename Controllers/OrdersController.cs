using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SkiServiceAPI.Models;
using System.Linq;
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

        // GET: api/orders
        [HttpGet]
        public async Task<IActionResult> GetOrders()
        {
            _logger.LogInformation("Fetching all orders");
            var orders = await _context.Orders.ToListAsync();
            _logger.LogInformation("Fetched {Count} orders from the database.", orders.Count);
            return Ok(orders);
        }

        // GET: api/orders/{id}
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

            _logger.LogInformation("Order with ID {OrderId} fetched successfully.", id);
            return Ok(order);
        }

        // POST: api/orders
        [HttpPost]
        public async Task<IActionResult> CreateOrder([FromBody] Order order)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid order model received.");
                return BadRequest(ModelState);
            }

            _logger.LogInformation("Creating a new order for CustomerId: {CustomerId}", order.Id);

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Order {OrderId} created successfully.", order.Id);
            return CreatedAtAction(nameof(GetOrderById), new { id = order.Id }, order);
        }

        // PUT: api/orders/{id}
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

        // PATCH: api/orders/{id}
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

        // DELETE: api/orders/{id}
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
