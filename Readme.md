# ASP.NET Core 1.0 Hosting .NET 4.6

1. Start with a new C# web app using ASP.NET 5 (Core 1.0)
    - Select the Web API template (preview)

2. Open project.json and remove dnxcore50 from the frameworks section.
    - Leave dnx451 under frameworks.

3. Under Entity Framework unser dependencies in the project.json file.
    - This should be the full EF 6.x.

4.  Add a `DbConfig` class that extends `DbConfiguration`

    ```csharp
    public class DbConfig : DbConfiguration
    {
        public DbConfig()
        {
            SetProviderServices("System.Data.SqlClient", SqlProviderServices.Instance);
        }
    }
    ```

5. Add a `Product` entity class.

    ```csharp
    public class Product
    {
        public int Id { get; set; }
        public string ProductName { get; set; }
        public decimal UnitPrice { get; set; }
    }
    ```
6. Add a `SampleContext` inheriting from `DbContext`.
    - Place a `DbConfigurationType` attribute on it with `DbConfig`.
    - Add a `Products` property of type `DbSet<Product>`

    ```chsarp
    [DbConfigurationType(typeof(DbConfig))]
    public class SampleContext : DbContext
    {
        public SampleContext(string connectionName) :
            base(connectionName) { }

        public DbSet<Product> Products { get; set; }
    }
    ```
7. Add a "Data" section to appsettings.json with a connection string
    - Here we specify LocalDb, but SQL Express or full is OK too.

    ```json
    "Data": {
      "NorthwindSlim": {
        "ConnectionString": "Data Source=(localdb)\\MSSQLLocalDB;Database=Sample;Integrated Security=True"
      }
    }
    ```

8. Register `SampleContext` with DI system by supplying a new instance of `SampleContext`
    - Add the following code to the `ConfigureServices` method in `Startup`

    ```csharp
    services.AddScoped(provider =>
    {
        var connectionString = Configuration["Data:NorthwindSlim:ConnectionString"];
        return new SampleContext(connectionString);
    });
    ```
9. Add a `ProductsController` that extends `Controller`
    - Pass `SampleContext` to the ctor
    - Add actions for GET, POST, PUT and DELETE
    - Override `Dispose` to dispose of the context

    ```csharp
    [Route("api/[controller]")]
    public class ProductsController : Controller
    {
        private readonly SampleContext _dbContext;

        public ProductsController(SampleContext dbContext)
        {
            _dbContext = dbContext;
        }

        // GET: api/products
        [HttpGet]
        public async Task<ObjectResult> Get()
        {
            var products = await _dbContext.Products
                .OrderBy(e => e.ProductName)
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
            if (product == null)
                return await Task.FromResult<ObjectResult>(null);

            _dbContext.Entry(product).State = EntityState.Added;

            await _dbContext.SaveChangesAsync();
            return Ok(product);
        }

        // PUT api/products/5
        [HttpPut]
        public async Task<ObjectResult> Put([FromBody]Product product)
        {
            if (product == null)
                return await Task.FromResult<ObjectResult>(null);

            _dbContext.Entry(product).State = EntityState.Modified;

            await _dbContext.SaveChangesAsync();
            return Ok(product);
        }

        // DELETE api/products/5
        [HttpDelete("{id}")]
        public async Task Delete(int id)
        {
            if (id == 0) return;

            var product = await _dbContext.Products
                .SingleOrDefaultAsync(e => e.Id == id);
            if (product == null) return;

            _dbContext.Entry(product).State = EntityState.Deleted;
            await _dbContext.SaveChangesAsync();
        }

        protected override void Dispose(bool disposing)
        {
            _dbContext.Dispose();
        }
    }
    ```

10. Test the controller by running the app and submitting some requests.
    - Use Postman or Fiddler
    - Set Content-Type header to application/json
    - The database should be created automatically

    ```
    GET: http://localhost:49951/api/products
    POST: http://localhost:49951/api/products
      - Body: {"ProductName":"Chai","UnitPrice":10}
      - Body: {"ProductName":"Chang","UnitPrice":10}
    GET: http://localhost:49951/api/products/2
    PUT: http://localhost:49951/api/products
      - Body: {"Id":2,"ProductName":"Chang","UnitPrice":11}
    DELETE: http://localhost:49951/api/products/2
    ```

