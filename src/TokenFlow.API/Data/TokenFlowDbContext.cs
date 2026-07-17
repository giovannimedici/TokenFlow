using Microsoft.EntityFrameworkCore;
using TokenFlow.API.Entities;
using MongoDB.Driver;
using MongoDB.EntityFrameworkCore.Extensions;

namespace TokenFlow.API.Data;

public class TokenFlowDbContext(DbContextOptions<TokenFlowDbContext> options) : DbContext(options)
{
    public DbSet<User> Users => Set<User>();

    public static TokenFlowDbContext Create(IMongoDatabase database) => 
            new (new DbContextOptionsBuilder<TokenFlowDbContext>()
            .UseMongoDB(database.Client, database.DatabaseNamespace.DatabaseName)
            .Options);

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>().ToCollection("users");
    }
}