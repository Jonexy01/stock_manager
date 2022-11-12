using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using stockmanager.Data;
using stockmanager.Models;
using System.Security.Claims;

namespace stockmanager.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class StockController : ControllerBase
    {
        private readonly StockManagerDbContext _dbContext;
        public StockController(StockManagerDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet]
        public async Task<IEnumerable<Stock>> GetStocks()
        {
            return await _dbContext.Stocks.ToListAsync();
        }

        [HttpGet("id")]
        [ProducesResponseType(typeof(Stock), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(Guid id)
        {
            var stock = await _dbContext.Stocks.FindAsync(id);
            return stock == null ? NotFound() : Ok(stock);
        }

        [HttpPost]
        [ProducesResponseType(typeof(Response), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Response), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> CreateStock(AddStockRequest stock)
        {
            string? userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            Guid.TryParse(userId, out var userGuid);
            User? user = await _dbContext.Users.FindAsync(userGuid);

            Response response = new Response() { detail = "Only managers can create stock" };
            if (user!.Role != "manager") return Unauthorized(response);

            var stockToAdd = new Stock()
            {
                Id = Guid.NewGuid(),
                Name = stock.Name,
                Type = stock.Type,
                Price = stock.Price,
                Quantity = stock.Quantity,
                Created = DateTime.Now,
            };
            await _dbContext.Stocks.AddAsync(stockToAdd);
            await _dbContext.SaveChangesAsync();
            var responseSuccess = new Response()
            {
                detail = "Product added successfuly",
                stock = stockToAdd,
            };

            return Ok(responseSuccess);
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(Response), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(Response), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Delete(Guid id)
        {
            string? userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            Guid.TryParse(userId, out var userGuid);
            User? user = await _dbContext.Users.FindAsync(userGuid);

            Response response1 = new Response() { detail = "Only managers can create stock" };
            if (user!.Role != "manager") return Unauthorized(response1);

            var stockToDelete = await _dbContext.Stocks.FindAsync(id);
            if (stockToDelete == null) return NotFound();

            _dbContext.Stocks.Remove(stockToDelete);
            await _dbContext.SaveChangesAsync();

            var response = new Response() { detail = "Product Deleted Successfuly" };
            return Ok(response);
        }

        [HttpPut]
        [ProducesResponseType(typeof(Response), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> updateStockCount(StockCount stockCount)
        {
            var stockToUpdate = await _dbContext.Stocks.FindAsync(stockCount.Id);
            if (stockToUpdate == null) return NotFound();
            if (stockCount.operation == "add")
            {
                stockToUpdate.Quantity = stockToUpdate.Quantity + stockCount.QuantityToOperate;
            } else
            {
                if (stockToUpdate.Quantity < stockCount.QuantityToOperate)
                {
                    Response response = new Response() { detail = "You don't have enough stock" };
                    return Ok(response);
                } else
                {
                    stockToUpdate.Quantity = stockToUpdate.Quantity - stockCount.QuantityToOperate;
                }
            }
            await _dbContext.SaveChangesAsync();
            var response2 = new Response() { detail = "Stock quantity updated successfuly" };
            return Ok(response2);
        }

        [HttpPut("updatePrice")]
        [ProducesResponseType(typeof(Response), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(Response), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> updateStockPrice(StockPriceUpdate stockPriceUpdate) {
            string? userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            Guid.TryParse(userId, out var userGuid);
            User? user = await _dbContext.Users.FindAsync(userGuid);

            Response response1 = new Response() { detail = "Only managers can create stock" };
            if (user!.Role != "manager") return Unauthorized(response1);

            var stockToUpdate = await _dbContext.Stocks.FindAsync(stockPriceUpdate.Id);
            if (stockToUpdate == null) return NotFound();
            stockToUpdate.Price = stockPriceUpdate.NewPrice;
            await _dbContext.SaveChangesAsync();
            var response = new Response() { detail = "Stock Price updated successfuly" };
            return Ok(response);
        }
    }
}
