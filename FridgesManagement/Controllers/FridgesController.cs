using FridgesManagement.Models;
using FridgesManagement.Services.Base;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace FridgesManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FridgesController : ControllerBase
    {
        private readonly IAsyncQueryExecutor _queryExecutor;

        private readonly IConfigurationSection _storedProcedures;

        private readonly IReadEnumerable _readEnumerable;

        public FridgesController(IAsyncQueryExecutor queryExecutor,
             IConfiguration configuration,
             IReadEnumerable readEnumerable)
        {
            _readEnumerable = readEnumerable;
            _queryExecutor = queryExecutor;
            _storedProcedures = configuration.GetSection("StoredProcedures");
        }

        [HttpGet("GetAll")]
        public async Task<IEnumerable<Fridge>> GetAll()
        {

            return await _queryExecutor.ExecuteQueryAsync(_storedProcedures?["GetFridges"], 
                r => _readEnumerable.Read(r, r => new Fridge
                {
                    Id = r.GetGuid(0),
                    Name = r.GetString(1),
                    OwnerName = r.GetString(2),
                    ModelId = r.GetGuid(3)
                })
                .ToList());
        }

        [HttpGet("GetFridgeProducts/{stringGuid}")]
        public async Task<ActionResult<IEnumerable<FridgeProduct>>> GetFridgeProducts(string stringGuid)
        {
            bool guidParsed = Guid.TryParse(stringGuid, out Guid guid);

            if (!guidParsed)
                return BadRequest(new { Message = "Invalid guid" });

            return Ok(await _queryExecutor.ExecuteQueryAsync(_storedProcedures?["GetFridgeProducts"], 
                r => _readEnumerable.Read(r, r => new FridgeProduct
                    { 
                        Id = r.GetGuid(0),
                        ProductId = r.GetGuid(1),
                        ProductName = r.GetString(2),
                        DefaultQuantity = r.GetInt32(3),
                        Quantity = r.GetInt32(4)
                    })
                .ToList(),
                new SqlParameter("FridgeId", guid)));
        }

        [HttpDelete("DeleteProductFromFridge/{stringGuid}")]
        public async Task<IActionResult> DeleteProductFromFridge(string stringGuid)
        {
            bool guidParsed = Guid.TryParse(stringGuid, out Guid guid);

            if (!guidParsed)
                return BadRequest(new { Message = "Invalid guid" });

            return await _queryExecutor.ExecuteQueryAsync<ActionResult>(_storedProcedures?["RemoveProduct"], 
                r => r.RecordsAffected == 1 ? Ok(new { Message = "value appended" }) 
                                            : NotFound(new { Message = "value wasn't appended" }), 
                new SqlParameter("Id", guid));
        }

        [HttpGet("GetProducts")]
        public async Task<IEnumerable<Product>> GetProducts()
        {
            return await _queryExecutor.ExecuteQueryAsync(_storedProcedures?["GetProducts"], 
                r => _readEnumerable.Read(r, r => new Product
                {
                    Id = r.GetGuid(0),
                    Name = r.GetString(1),
                    DefaultQuantity = r.GetInt32(2)
                })
                .ToList());
        }

        [HttpPost("AppendProduct")]
        public async Task<IActionResult> AppendProduct(ProductPostData product)
        {
            return await _queryExecutor.ExecuteQueryAsync<ActionResult>(_storedProcedures?["AddProductToFridge"], 
                r => r.RecordsAffected == 1 ? Ok(new { Message = "value appended" }) 
                                            : NotFound(new { Message = "value wasn't appended" }), 
                new ("ProductId", product.ProductId),
                new ("FridgeId", product.FridgeId),
                new ("Quantity", product.Quantity));
        }

        [HttpPatch("ChangeFridgeOwner")]
        public async Task<IActionResult> ChangeFridgeOwner(FridgePatchParams fridge)
        {
            return await _queryExecutor.ExecuteQueryAsync<ActionResult>(_storedProcedures?["UpdateFridgeOwnerName"], 
                r => r.RecordsAffected == 1 ? Ok(new { Message = "name updated" })
                                            : BadRequest(new { Message = "name wasn't updated" }),
                new("Id", fridge.Id),
                new("OwnerName", fridge.OwnerName));
        }
    }
}
