using VideoGameStore.Data;
using VideoGameStore.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Serilog;



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

app.MapPost("/inventory/rest", (VideoGameStoreDbContext db, ILogger<Program> logger) =>
{
    // We just ask for an ILogger like we do our dbcontext
    // then use it as normal
    logger.LogInformation("Started seeing database");

    // What I want to do is reset the items that I know I stuck into the db.
    foreach (InventoryItem inv in db.Inventory) // for each item in my db Inventory table... do something
    {
        // I only want to do something if the primary key is 1, 2, or 3.... 
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

    db.SaveChanges(); // persisting to db
    logger.LogInformation("Stock reset");
    return Results.Ok("stock reset");

});

app.Run();
