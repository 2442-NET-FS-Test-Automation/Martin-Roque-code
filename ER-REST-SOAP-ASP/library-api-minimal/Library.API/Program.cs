using Microsoft.EntityFrameworkCore;
using Library.Data;

var builder = WebApplication.CreateBuilder(args);

//The first thing we need is to give our builder a connection string to our database
var conn_string = "Server=localhost,1433;Database=LibraryMinimalDb;User ID=sa;Password=LibPass123;TrustServerCertificate=true";

//Tell the builder to use our LibraryDbContext with the connection string above
builder.Services.AddDbContext<LibraryDbContext>(options => options.UseSqlServer(conn_string));

//Swagger added to builder
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//App area
var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

//Endpoint area
app.MapGet("/", () => "Hello World!");

app.MapGet("/inventory", async (LibraryDbContext db) =>
{
    return await db.Inventory.ToListAsync();
});

//Using LINQ - Language Integrated Query
//LINQ is a library that just lets us query collections - Logic based from SQL DQL
app.MapGet("/inventory/by-value", (LibraryDbContext db) =>
{
    return db.Inventory.Include(i => i.Product)
    .GroupBy(i => i.CurrentStock >= 5 ? "well-stocked" : "low")
    .Select(g => new { tier = g.Key, count = g.Count(), units = g.Sum(i => i.CurrentStock) })
    .ToList();
});

//Where file ends
app.Run();
