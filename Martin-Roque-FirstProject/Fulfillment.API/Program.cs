using VideoGameStore.Data;
using VideoGameStore.Data.Entities;
using VideoGameStore.API.Fulfillment;
using Microsoft.EntityFrameworkCore;
using Serilog;
using System.Diagnostics;

var builder = WebApplication.CreateBuilder(args);

var conn_string = "Server=localhost,1433;Database=VideoGameStoreMinimalAPI;User Id=sa;Password=LibPass123;TrustServerCertificate=true";

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("logs/fulfillment-log-.log", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();

builder.Services.AddDbContext<VideoGameStoreDbContext>(options => options.UseSqlServer(conn_string),
    ServiceLifetime.Scoped, ServiceLifetime.Singleton);

builder.Services.AddDbContextFactory<VideoGameStoreDbContext>(options => options.UseSqlServer(conn_string));

builder.Services.AddScoped<IFulfillmentService, FulfillmentService>();
builder.Services.AddScoped<ISeeder, Seeder>();
builder.Services.AddScoped<BurstPlanner>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.MapGet("/inventory", async (VideoGameStoreDbContext db) =>
{
    return await db.Inventory.ToListAsync();
});

app.MapGet("/videogames", async (VideoGameStoreDbContext db) =>
{
    return await db.Videogames.ToListAsync();
});

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
            default:
                break;
        }

    }

    db.SaveChanges();
    logger.LogInformation("Stock reset");
    return Results.Ok("stock reset");

});

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
        sequentialMs = sw1.ElapsedMilliseconds,
        concurrentMs = sw2.ElapsedMilliseconds
    };

});

app.MapGet("/reports/by-fulfillment", (VideoGameStoreDbContext db) =>
{
    return db.Buyings
         .Where(b => b.Stauts == Status.Fulfilled)
         .OrderBy(b => b.CompletedUtc)
         .Select(b => new { b.Id, b.Priority, b.CompletedUtc })
         .ToList();

});

app.MapGet("/reports/top-videogames", (VideoGameStoreDbContext db) =>
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

app.MapGet("/reports/top-customers", (VideoGameStoreDbContext db) =>
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
Log.CloseAndFlush();