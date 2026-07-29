using FluentAssertions;
using Library.ControllerApi.Services;
using Library.Data;
using Library.Data.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

public class UserServerSqlitetests : IDisposable
{
    private class SqliteLibraryDbContext : LibraryDbContext
    {
        public SqliteLibraryDbContext(DbContextOptions<LibraryDbContext> options)
            : base(options) { }
        protected override void OnModelCreating(ModelBuilder b)
        {
            base.OnModelCreating(b);
            b.Entity<InventoryItem>().Property(i => i.RowVersion)
                .HasDefaultValue(Array.Empty<Byte>());
        }
    }
    private readonly SqliteConnection _connection;
    private readonly LibraryDbContext _db;
    private readonly UserService _sut;

    public UserServerSqlitetests()
    {
        _connection = new SqliteConnection("DataSource=:memory:");
        _connection.Open();

        var options = new DbContextOptionsBuilder<LibraryDbContext>()
            .UseSqlite(_connection)
            .Options;

        _db = new SqliteLibraryDbContext(options);
        _db.Database.EnsureCreated();

        _sut = new UserService(_db, new PasswordHasher<User>());
    }

    public void Dispose()
    {
        _db.Dispose();
        _connection.Dispose();
    }

    [Fact]
    public async Task RegisterAsync_NewUser_PersistsAHashedPassword()
    {
        //Act
        var error = await _sut.RegisterAsync("grace", "secure-pass");

        //Assert
        error.Should().BeNull();

        var newUser = await _db.Users.SingleAsync(u => u.UserName == "grace");

        newUser.Role.Should().Be("consumer");
        newUser.PasswordHash.Should().NotBeNullOrEmpty();
        newUser.PasswordHash.Should().NotBe("secure-pass");
    }
}