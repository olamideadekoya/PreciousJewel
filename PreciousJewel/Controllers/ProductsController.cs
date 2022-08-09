using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApiLearning.Models;

namespace WebApiLearning.Controllers
{
    [ApiVersion("1.0")]
    //[Route("api/[controller]")]
    [Route("v{v:ApiVersion}/products")]
    [Route("/products")]
    [Authorize]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        
        private readonly ShopContext _context;
        public ProductsController(ShopContext context)
        {
            _context = context;
            _context.Database.EnsureCreated();
        }

        [HttpGet]
        public async Task<IActionResult> GetAllProduct([FromQuery]ProductQueryParameters queryParameters)
        {
            IQueryable<Product> products = _context.Products;
            if (queryParameters != null)
            {
                // Filtering
                if (queryParameters.MinPrice != null)
                {
                    products = products.Where(
                        p => p.Price >= queryParameters.MinPrice.Value);
                }

                if (queryParameters.MaxPrice != null)
                {
                    products = products.Where(
                        p => p.Price <= queryParameters.MaxPrice.Value);
                }

                // Searching
                if (!string.IsNullOrEmpty(queryParameters.Sku))
                {
                    products = products.Where(
                        p => p.Sku == queryParameters.Sku);
                }

                if (!string.IsNullOrEmpty(queryParameters.Name))
                {

                    products = products.Where(
                        p => p.Name.ToLower().Contains(queryParameters.Name.ToLower()));
                }

                // Sorting
                if (!string.IsNullOrEmpty(queryParameters.SortBy))
                {
                    if(typeof(Product).GetProperty(queryParameters.SortBy) != null)
                    {
                        products = products.OrderByCustom(
                        queryParameters.SortBy,
                        queryParameters.SortOrder);
                    }
                }
            }

            // Paging
            products = products
                .Skip(queryParameters.Size * (queryParameters.Page - 1))
                .Take(queryParameters.Size);

            return Ok(await products.ToArrayAsync());
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetProduct(int id)
        {
            if(id == 0) { return BadRequest(); }
             var product = await _context.Products.FindAsync(id);
            if(product == null) { return NotFound(); }
            return Ok(product);
        }

        [HttpPost]
        public async Task<IActionResult> PostProduct(Product product)
        {
            _context.Products.Add(product);
            await _context.SaveChangesAsync();
            return CreatedAtAction("GetProduct", new { id = product.Id }, product);

        }

        [HttpPut]
        public async Task<IActionResult> PutProduct(int id, [FromBody]Product product)
        {
            if(product == null) { return BadRequest(); }

            if(id != product.Id) { return BadRequest(); }

            _context.Entry(product).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                if(!_context.Products.Any(p =>p.Id == id))
                {
                    return NotFound();
                }
                else
                {
                    throw ex;
                }
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<Product>> DeleteProduct(int id)
        {
            var product = await _context.Products.FindAsync(id);    

            if(product == null) { return NotFound(); }

            _context.Products.Remove(product);
            _context.SaveChanges();

            return product;
        }

        [HttpPost]
        [Route("Delete")]
        public async Task<ActionResult<List<Product>>> DeleteMultiple([FromQuery]int[] ids)
        {
            if (ids == null) { return BadRequest(); }
            var products = new List<Product>();

            foreach(int id in ids)
            {
                var product = await _context.Products.FindAsync(id);
                if(product == null) { return NotFound(); }

                products.Add(product);
            }


            _context.Products.RemoveRange(products);
            await _context.SaveChangesAsync();

            return Ok(products);
        }

    }


    [ApiVersion("2.0")]
    //[Route("api/[controller]")]
    //[Route("v{v:ApiVersion}/products")]
    [Route("/products")]
    [ApiController]
    public class ProductsV2Controller : ControllerBase
    {

        private readonly ShopContext _context;
        public ProductsV2Controller(ShopContext context)
        {
            _context = context;
            _context.Database.EnsureCreated();
        }

        [HttpGet]
        public async Task<IActionResult> GetAllProduct([FromQuery] ProductQueryParameters queryParameters)
        {
            IQueryable<Product> products = _context.Products.Where(p => p.IsAvailable == true);
            if (queryParameters != null)
            {
                // Filtering
                if (queryParameters.MinPrice != null)
                {
                    products = products.Where(
                        p => p.Price >= queryParameters.MinPrice.Value);
                }

                if (queryParameters.MaxPrice != null)
                {
                    products = products.Where(
                        p => p.Price <= queryParameters.MaxPrice.Value);
                }

                // Searching
                if (!string.IsNullOrEmpty(queryParameters.Sku))
                {
                    products = products.Where(
                        p => p.Sku == queryParameters.Sku);
                }

                if (!string.IsNullOrEmpty(queryParameters.Name))
                {

                    products = products.Where(
                        p => p.Name.ToLower().Contains(queryParameters.Name.ToLower()));
                }

                // Sorting
                if (!string.IsNullOrEmpty(queryParameters.SortBy))
                {
                    if (typeof(Product).GetProperty(queryParameters.SortBy) != null)
                    {
                        products = products.OrderByCustom(
                        queryParameters.SortBy,
                        queryParameters.SortOrder);
                    }
                }

                // Paging
                products = products
                    .Skip(queryParameters.Size * (queryParameters.Page - 1))
                    .Take(queryParameters.Size);
            }
           

            return Ok(await products.ToArrayAsync());
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetProduct(int id)
        {
            if (id == 0) { return BadRequest(); }
            var product = await _context.Products.FindAsync(id);
            if (product == null) { return NotFound(); }
            return Ok(product);
        }

        [HttpPost]
        public async Task<IActionResult> PostProduct(Product product)
        {
            _context.Products.Add(product);
            await _context.SaveChangesAsync();
            return CreatedAtAction("GetProduct", new { id = product.Id }, product);

        }

        [HttpPut]
        public async Task<IActionResult> PutProduct(int id, [FromBody] Product product)
        {
            if (product == null) { return BadRequest(); }

            if (id != product.Id) { return BadRequest(); }

            _context.Entry(product).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                if (!_context.Products.Any(p => p.Id == id))
                {
                    return NotFound();
                }
                else
                {
                    throw ex;
                }
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<Product>> DeleteProduct(int id)
        {
            var product = await _context.Products.FindAsync(id);

            if (product == null) { return NotFound(); }

            _context.Products.Remove(product);
            _context.SaveChanges();

            return product;
        }

        [HttpPost]
        [Route("Delete")]
        public async Task<ActionResult<List<Product>>> DeleteMultiple([FromQuery] int[] ids)
        {
            if (ids == null) { return BadRequest(); }
            var products = new List<Product>();

            foreach (int id in ids)
            {
                var product = await _context.Products.FindAsync(id);
                if (product == null) { return NotFound(); }

                products.Add(product);
            }


            _context.Products.RemoveRange(products);
            await _context.SaveChangesAsync();

            return Ok(products);
        }

    }
}
