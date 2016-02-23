using System.Data.Entity;
using System.Threading.Tasks;
using Experimental.AspNetCore.EF6.Contexts;
using Experimental.AspNetCore.EF6.Models;
using Microsoft.AspNet.Mvc;

namespace Experimental.AspNetCore.EF6.Controllers
{
    [Route("api/[controller]")]
    public class ProductsController : Controller
    {
        private readonly SampleDbContext _dbContext;

        public ProductsController(SampleDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        // GET: api/products
        [HttpGet]
        public async Task<ObjectResult> Get()
        {
            var products = await _dbContext.Products
                .ToListAsync();
            return Ok(products);
        }

        // GET api/products/5
        [HttpGet("{id}")]
        public async Task<ObjectResult> Get(int id)
        {
            var product = await _dbContext.Products
                .SingleOrDefaultAsync(e => e.Id == id);
            return Ok(product);
        }

        // POST api/products
        [HttpPost]
        public async Task<ObjectResult> Post([FromBody]Product product)
        {
            _dbContext.Entry(product).State = EntityState.Added;
            await _dbContext.SaveChangesAsync();
            return Ok(product);
        }

        // PUT api/products/5
        [HttpPut]
        public async Task<ObjectResult> Put([FromBody]Product product)
        {
            _dbContext.Entry(product).State = EntityState.Modified;
            await _dbContext.SaveChangesAsync();
            return Ok(product);
        }

        // DELETE api/products/5
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var product = await _dbContext.Products
                .SingleOrDefaultAsync(e => e.Id == id);
            if (product == null) return Ok();

            _dbContext.Entry(product).State = EntityState.Deleted;
            await _dbContext.SaveChangesAsync();
            return Ok();
        }
    }
}
