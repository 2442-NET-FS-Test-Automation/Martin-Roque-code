using System.Diagnostics;
using Library.ControllerApi.Filters;
using Library.ControllerApi.Mapping;
using Library.ControllerApi.Middleware;
using Library.ControllerApi.Services;
using Library.Data;
using Microsoft.EntityFrameworkCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
//Adding connection string
var conn_string = "Server=localhost,1433;Database=LibraryMinimalDb;User ID=sa;Password=LibPass123;TrustServerCertificate=true";

builder.Services.AddDbContextFactory<LibraryDbContext>(o => o.UseSqlServer(conn_string));

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("logs/fulfillment-log-.log", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();

//Registring our custom Repo and Service Layer
builder.Services.AddScoped<IInventoryRepository, InventoryRepository>();
builder.Services.AddScoped<IInventoryService, InventoryService>();

// Adding our mapping profile for AutoMapper
builder.Services.AddAutoMapper(cfg => cfg.AddMaps(typeof(MappingProfile).Assembly));

// having our filter apply ot every controlles
builder.Services.AddControllers(o => o.Filters.Add<TimingFilter>());
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseSwagger();
app.UseSwaggerUI();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.Use(async (ctx, next) =>
{
    var sw = Stopwatch.StartNew();

    await next();

    sw.Stop();
    Log.Information("{Method} {Path} -> {StatusCode} in {Elapsed}ms",
    ctx.Request.Method, ctx.Request.Path, ctx.Response.StatusCode, sw.ElapsedMilliseconds);

});

app.Use(async (ctx, next) =>
{
    if (ctx.Request.Headers.ContainsKey("X-Maintenance"))
    {
        ctx.Response.StatusCode = StatusCodes.Status503ServiceUnavailable;
        await ctx.Response.WriteAsync("Down for maintenance");
        return;
    }
    await next(ctx);
});

app.UseSerilogRequestLogging();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
