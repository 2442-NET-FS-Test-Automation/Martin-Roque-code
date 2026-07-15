using System.Diagnostics;
using System.Text;
using Library.ControllerApi.Filters;
using Library.ControllerApi.Mapping;
using Library.ControllerApi.Middleware;
using Library.ControllerApi.Services;
using Library.Data;
using Library.Data.Entities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
//Adding connection string
var conn_string = "Server=localhost,1433;Database=LibraryMinimalDb;User ID=sa;Password=LibPass123;TrustServerCertificate=true";

builder.Services.AddDbContextFactory<LibraryDbContext>(o => o.UseSqlServer(conn_string));
builder.Services.AddSingleton<IPasswordHasher<User>, PasswordHasher<User>>();

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("logs/fulfillment-log-.log", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();


const string SpaCorsPolicy = "spa";

builder.Services.AddCors(o => o.AddPolicy(SpaCorsPolicy, p => p
    .WithOrigins("http://127.0.0.1:5500", "http://localhost:5500")
    .AllowAnyHeader()
    .AllowAnyMethod()));

// Validation side of JWT. Issuance lives in TokenService
var jwtKey = builder.Configuration["Jwt:Key"];

//Harcoding the issuer and audience - these hace to match the ones we set on the token
const string jwtIssuer = "library-fulfillment";
const string jwtAudience = "library-fulfillment-clients";

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(o => o.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidIssuer = jwtIssuer,
        ValidateAudience = true,
        ValidAudience = jwtAudience,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey!)),
        ValidateLifetime = true
    });

builder.Services.AddAuthorization();

builder.Services.AddSingleton<ITokenService, TokenService>();

// Adding our httpClient
builder.Services.AddHttpClient<ISupplierClient, SupplierClient>(c =>
    c.BaseAddress = new Uri("https://dummyjson.com/"));

// Registring our custom Repo and Service Layer
builder.Services.AddScoped<IInventoryRepository, InventoryRepository>();
builder.Services.AddScoped<IInventoryService, InventoryService>();
builder.Services.AddScoped<IUserService, UserService>();

// Adding our mapping profile for AutoMapper
builder.Services.AddAutoMapper(cfg => cfg.AddMaps(typeof(MappingProfile).Assembly));

// having our filter apply ot every controlles
builder.Services.AddControllers(o => o.Filters.Add<TimingFilter>());
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddSwaggerGen();

// Adding caching
builder.Services.AddMemoryCache();
builder.Services.AddResponseCaching();

var app = builder.Build();

// Seeding admins - cant do a plain INSERT INTO using SQl because I won't have a hashed password
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<LibraryDbContext>();
    if (!db.Users.Any(u => u.Role == "admin"))
    {
        var hasher = new PasswordHasher<User>();
        var admin = new User { UserName = "ada", Role = "admin" };
        admin.PasswordHash = hasher.HashPassword(admin, "pass123!");

        db.Users.Add(admin);
        db.SaveChanges();
    }
}

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

app.UseResponseCaching();

app.UseCors(SpaCorsPolicy);

app.UseAuthentication();
app.UseAuthorization();

app.UseSerilogRequestLogging();

app.UseHttpsRedirection();

app.MapControllers();

app.Run();

Log.CloseAndFlush();