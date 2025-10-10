using Microsoft.EntityFrameworkCore;
using om.servicing.casemanagement.data.Repositories.Shared;

namespace om.servicing.casemanagement.tests.Data.Repositories.Shared;

public class GenericRepositoryTests
{
    [Fact]
    public async Task AddAsync_AddsEntity()
    {
        var repo = CreateRepository();
        var entity = new TestEntity { Name = "Test" };

        await repo.AddAsync(entity);

        var result = await repo.GetAllAsync();
        Assert.Single(result);
        Assert.Equal("Test", result.First().Name);
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsEntity()
    {
        var repo = CreateRepository();
        var entity = new TestEntity { Name = "Test" };
        await repo.AddAsync(entity);

        var result = await repo.GetByIdAsync(entity.Id);
        Assert.NotNull(result);
        Assert.Equal("Test", result!.Name);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsAllEntities()
    {
        var repo = CreateRepository();
        await repo.AddAsync(new TestEntity { Name = "A" });
        await repo.AddAsync(new TestEntity { Name = "B" });

        var result = await repo.GetAllAsync();
        Assert.Equal(2, result.Count());
    }

    [Fact]
    public async Task FindAsync_ReturnsMatchingEntities()
    {
        var repo = CreateRepository();
        await repo.AddAsync(new TestEntity { Name = "A" });
        await repo.AddAsync(new TestEntity { Name = "B" });

        var result = await repo.FindAsync(e => e.Name == "B");
        Assert.Single(result);
        Assert.Equal("B", result.First().Name);
    }

    [Fact]
    public async Task Update_UpdatesEntity()
    {
        var repo = CreateRepository();
        var entity = new TestEntity { Name = "Old" };
        await repo.AddAsync(entity);

        entity.Name = "New";
        repo.Update(entity);

        var result = await repo.GetByIdAsync(entity.Id);
        Assert.Equal("New", result!.Name);
    }

    [Fact]
    public async Task Remove_RemovesEntity()
    {
        var repo = CreateRepository();
        var entity = new TestEntity { Name = "ToRemove" };
        await repo.AddAsync(entity);

        repo.Remove(entity);

        var result = await repo.GetAllAsync();
        Assert.Empty(result);
    }

    private GenericRepository<TestEntity, TestDbContext> CreateRepository()
    {
        var options = new DbContextOptionsBuilder<TestDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        var context = new TestDbContext(options);
        return new GenericRepository<TestEntity, TestDbContext>(context);
    }
}

public class TestEntity
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
}

public class TestDbContext : DbContext
{
    public DbSet<TestEntity> TestEntities { get; set; }

    public TestDbContext(DbContextOptions<TestDbContext> options) : base(options) { }
}
