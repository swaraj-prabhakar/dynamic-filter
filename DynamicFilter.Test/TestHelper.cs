namespace DynamicFilter.Test;

internal class TestHelper
{
    public TestHelper()
    {
        DbContext = CreateDbContext();
    }
    public AppDbContext DbContext { get; }
    private AppDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()).Options;
        var dbContext = new AppDbContext(options);

        dbContext.AddRange(DbData.GetInitialData());
        dbContext.SaveChanges();

        return dbContext;
    }
}
