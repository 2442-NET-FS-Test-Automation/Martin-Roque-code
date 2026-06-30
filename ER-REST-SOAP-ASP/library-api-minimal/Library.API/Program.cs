using Microsoft.EntityFrameworkCore;
using Library.Data;

var builder = WebApplication.CreateBuilder(args);

//The first thing we need is to give our builder a connection string to our database
var conn_string = "Server=localhost,1433;Database=LibraryMinimalDb;User ID=sa;Password=LibPass123;TrustServerCertificate=true";

//Tell the builder to use our LibraryDbContext with the connection string above
builder.Services.AddDbContext<LibraryDbContext>(options => options.UseSqlServer(conn_string));

var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.Run();
