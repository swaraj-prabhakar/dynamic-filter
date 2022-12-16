
namespace DynamicFilter.Test.Repositories;

public class SampleRepository<T> where T : class
{
    private readonly AppDbContext db;
    public SampleRepository(AppDbContext dbContext)
    {
        db = dbContext;
    }
    public async Task<List<T>> Get(FilterDto filter, CancellationToken ct)
    {
        return await db.Set<T>().AsAsyncEnumerable().ApplyFilter(filter).ToListAsync(ct).ConfigureAwait(false);
    }
}
