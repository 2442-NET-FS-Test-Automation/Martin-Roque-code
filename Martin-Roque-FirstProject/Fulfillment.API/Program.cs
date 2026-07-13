using VideoGameStore.Data;
using VideoGameStore.Data.Entities;
using VideoGameStore.API.Fulfillment;
using Microsoft.EntityFrameworkCore;
using Serilog;
using System.Diagnostics;

var builder = WebApplication.CreateBuilder(args);

//Connection string needed to access to database (SQL Server in docker) - in production we need to hide it
var conn_string = "Server=localhost,1433;Database=VideoGameStoreMinimalAPI;User Id=sa;Password=LibPass123;TrustServerCertificate=true";

//Configuration to use Serilog in this project in .Net.
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("logs/fulfillment-log-.log", rollingInterval: RollingInterval.Day)
    .CreateLogger();

//Bring access to Serilog to this application
builder.Host.UseSerilog();

//Use normal dbContext to process that it not necessary optimize all the calls to a dbcontext
builder.Services.AddDbContext<VideoGameStoreDbContext>(options => options.UseSqlServer(conn_string),
    ServiceLifetime.Scoped, ServiceLifetime.Singleton);

//Use a Factory when is necessary keep the same dbContext instance
builder.Services.AddDbContextFactory<VideoGameStoreDbContext>(options => options.UseSqlServer(conn_string));

//Declaring own services to be used in the application
builder.Services.AddScoped<IFulfillmentService, FulfillmentService>();
builder.Services.AddScoped<ISeeder, Seeder>();
builder.Services.AddScoped<BurstPlanner>();


//Starting swagger to use it in the presentation
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

//Using Swagget for UI for presentation
app.UseSwagger();
app.UseSwaggerUI();

//Show all inventory items information in database
app.MapGet("/inventory", async (VideoGameStoreDbContext db) =>
{
    return await db.Inventory.ToListAsync();
});

//Show all videogames information in database (for demostration)
app.MapGet("/videogames", async (VideoGameStoreDbContext db) =>
{
    return await db.Videogames.ToListAsync();
});

//Show all customer information in database (for demostration)
app.MapGet("/customers", async (VideoGameStoreDbContext db) =>
{
    return await db.Customers.ToListAsync();
});

//Seed all information for starting values (defined before in this code)
app.MapPost("/inventory/seed", (VideoGameStoreDbContext db, ILogger<Program> logger) =>
{
    logger.LogInformation("Started seeing database");

    foreach (InventoryItem inv in db.Inventory)
    {

        switch (inv.Id)
        {
            case 1:
                inv.CurrentStock = 5;
                break;
            case 2:
                inv.CurrentStock = 3;
                break;
            case 3:
                inv.CurrentStock = 9;
                break;
            case 4:
                inv.CurrentStock = 7;
                break;
            case 5:
                inv.CurrentStock = 4;
                break;
            case 6:
                inv.CurrentStock = 11;
                break;
            default:
                break;
        }

    }

    db.SaveChanges();
    logger.LogInformation("Stock reset");
    return Results.Ok("Stock Reset");

});

app.MapPost("/buyings", async (BuyingPayload buyingRequest, IDbContextFactory<VideoGameStoreDbContext> factory,
            CancellationToken ct, IFulfillmentService fs) =>
{
    await using var db = await factory.CreateDbContextAsync(ct); // ask for db context to place order

    var newBuying = new Buying
    {
        CustomerId = buyingRequest.CustomerId,
        Priority = Priority.Normal,
        // Using the orderRequest from the HTTP request body to create my order
        Lines = { new BuyingLine { GameId = buyingRequest.ProductId, Quantity = buyingRequest.Quantity } }
    };

    db.Buyings.Add(newBuying); // add new Buying 
    await db.SaveChangesAsync(ct); // save that order to db


    FulfillmentResult result = await fs.FulfillOneAsync(newBuying.Id, ct);
    return Results.Ok(new { buyingId = newBuying.Id, result = result.ToString() });
});

//simulating a burst of buyings for the shop
app.MapPost("/buyings/burst", (int n, bool expedited, ISeeder seeder,
    IServiceScopeFactory scopes, IHostApplicationLifetime lifetime) =>
{
    var ids = seeder.SeedBuyings(n, expedited);
    var appStopping = lifetime.ApplicationStopping;

    _ = Task.Run(async () =>
    {
        try
        {
            using var scope = scopes.CreateScope();
            var service = scope.ServiceProvider.GetRequiredService<IFulfillmentService>();
            await service.FulfillBurstAsync(ids, appStopping);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Burst fulfillment failed");
        }
    }, appStopping);

});


//Compare behavior between the sequential mode and a parallel mode to fulfilll the same number of buyings
app.MapPost("/benchmark", async (int n, IFulfillmentService fs, ISeeder seeder, CancellationToken ct) =>
{
    var ids1 = seeder.ResetAndCreateBuyings(n);

    // First, sequential
    var sw1 = Stopwatch.StartNew();

    foreach (var id in ids1)
        await fs.FulfillOneAsync(id, ct);

    sw1.Stop();

    // Next concurrent
    var ids2 = seeder.ResetAndCreateBuyings(n);

    var sw2 = Stopwatch.StartNew();
    await fs.FulfillBurstAsync(ids2, ct);
    sw2.Stop();

    return new
    {
        //Returning the time in milliseconds
        sequentialMs = sw1.ElapsedMilliseconds,
        concurrentMs = sw2.ElapsedMilliseconds
    };

});

//Show how many buyings where fulfilled in total
app.MapGet("/reports/by-fulfillment", (VideoGameStoreDbContext db) =>
{
    return db.Buyings
         .Where(b => b.Stauts == Status.Fulfilled)
         .OrderBy(b => b.CompletedUtc)
         .Select(b => new { b.Id, b.Priority, b.CompletedUtc })
         .ToList();

});


//How many videogames were bought in total
app.MapGet("/reports/videogames", (VideoGameStoreDbContext db) =>
{
    var ranked = db.FulfillmentEvents
        .Where(e => e.Type == "Fulfilled")
        .Join(db.BuyingLines, e => e.BuyingId, l => l.BuyingId, (e, l) => l)
        .GroupBy(l => l.GameId)
        .Select(g => new { ProductId = g.Key, Units = g.Sum(l => l.Quantity) })
        .OrderByDescending(x => x.Units)
        .ToList();

    return ranked;
});

//How many buys do each customer in total
app.MapGet("/reports/customers", (VideoGameStoreDbContext db) =>
{
    var ranked = db.FulfillmentEvents
        .Where(e => e.Type == "Fulfilled")
        .Join(db.BuyingLines, e => e.BuyingId, l => l.BuyingId, (e, l) => l)
        .Join(db.Buyings, l => l.BuyingId, b => b.Id, (l, b) => b)
        .Join(db.Customers, b => b.CustomerId, c => c.Id, (b, c) => c)
        .GroupBy(c => c.Name)
        .Select(g => new { CustomerName = g.Key, Buys = g.Sum(c => c.Id) })
        .OrderByDescending(x => x.Buys)
        .ToList();

    return ranked;
});

//Verify if the stock of each videogame does not be less that 0
app.MapGet("/verify/no-oversell", (VideoGameStoreDbContext db) =>
{
    var rows = db.Inventory.Include(i => i.Videogame).ToList();
    var negative = rows.Where(i => i.CurrentStock < 0).ToList();
    var fulfilled = db.FulfillmentEvents.Count(e => e.Type == "Fulfilled");

    return new
    {
        anyNegative = negative.Any(),
        onHand = rows.Select(i => new { i.ProductId, i.CurrentStock }),
        unitsFulfilled = fulfilled
    };

});

app.Run();
//Close Serilog and free memory and the session for the next time
Log.CloseAndFlush();

public record BuyingPayload(int ProductId, int Quantity, int CustomerId);