# Using Entity Framework 6 with ASP.NET Core 1.0

1. Start with a new C# web app using ASP.NET 5 (Core 1.0)
    - Select the Web API template (preview)

2. Open project.json and remove dnxcore50 from the frameworks section.
    - Leave dnx451 under frameworks.

3. Add Entity Framework under dependencies in the project.json file.
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

6. Add a `SampleDbContext` inheriting from `DbContext`.
    - Place a `DbConfigurationType` attribute on it with `DbConfig`.
    - Add a `Products` property of type `DbSet<Product>`

    ```chsarp
    [DbConfigurationType(typeof(DbConfig))]
    public class SampleDbContext : DbContext
    {
        public SampleDbContext(string connectionName) :
            base(connectionName) { }

        public DbSet<Product> Products { get; set; }
    }
    ```
7. Optionally create a `SampleDbInitializer` class which inherits from `DropCreateDatabaseIfModelChanges<SampleDbContext>`.
    - Override the `Seed` method to see the database with data.

    ```csharp
    public class SampleDbInitializer : DropCreateDatabaseIfModelChanges<SampleDbContext>
    {
        protected override void Seed(SampleDbContext context)
        {
            var products = new List<Product>
            {
                new Product { Id = 1, ProductName = "Chai", UnitPrice = 10 },
                new Product { Id = 2, ProductName = "Chang", UnitPrice = 11 },
                new Product { Id = 3, ProductName = "Aniseed Syrup", UnitPrice = 12 },
            };

            context.Products.AddRange(products);

            context.SaveChanges();
        }
    }    
    ```

8. Add a static ctor to `SampleDbContext` to set the context initializer.

    ```csharp
    static SampleDbContext()
    {
        Database.SetInitializer(new SampleDbInitializer());
    }    
    ```

9. Add a "Data" section to appsettings.json with a connection string
    - Here we specify LocalDb, but SQL Express or full is OK too.

    ```json
    "Data": {
      "SampleDb": {
        "ConnectionString": "Data Source=(localdb)\\MSSQLLocalDB;AttachDbFilename=|DataDirectory|\\SampleDb.mdf;Integrated Security=True; MultipleActiveResultSets=True"
      }
    }
    ```

10. Update the `Startup` ctor to set the "DataDirectory" for the current `AppDomain`.

    ```csharp
    public Startup(IHostingEnvironment env, IApplicationEnvironment appEnv)
    {
        // Set up configuration sources.
        var builder = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .AddEnvironmentVariables();
        Configuration = builder.Build();

        // Set up data directory
        string appRoot = appEnv.ApplicationBasePath;
        AppDomain.CurrentDomain.SetData("DataDirectory", Path.Combine(appRoot, "App_Data"));
    }
    ```

11. Register `SampleDbContext` with DI system by supplying a new instance of `SampleDbContext`
    - Add the following code to the `ConfigureServices` method in `Startup`

    ```csharp
    services.AddScoped(provider =>
    {
        var connectionString = Configuration["Data:SampleDb:ConnectionString"];
        return new SampleDbContext(connectionString);
    });
    ```
12. Add a `ProductsController` that extends `Controller`
    - Pass `SampleDbContext` to the ctor
    - Add actions for GET, POST, PUT and DELETE
    - Override `Dispose` to dispose of the context

    ```csharp
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
    }
    ```

13. Test the controller by running the app and submitting some requests.
    - Use Postman or Fiddler
    - Set Content-Type header to application/json
    - The database should be created automatically

    ```
    GET: http://localhost:49951/api/products
    POST: http://localhost:49951/api/products
      - Body: {"ProductName":"Ikura","UnitPrice":12}
    GET: http://localhost:49951/api/products/4
    PUT: http://localhost:49951/api/products
      - Body: {"Id":4,"ProductName":"Ikura","UnitPrice":13}
    DELETE: http://localhost:49951/api/products/4
    ```

