namespace DynamicFilter.Test.Tests;

public class SampleRepositoryTest
{
    [Theory]
    [JsonFileData("testDataEmployee.json", typeof(FilterDto), typeof(List<long>))]
    public async void GetEmployees_ValidFilters_ReturnDesiredList(FilterDto filter, List<long> resultIds)
    {
        // Arrange
        var db = new TestHelper().DbContext;
        var repo = new SampleRepository<Employee>(db);

        // Act
        var result = await repo.Get(filter, default);

        // Assert
        Assert.True(resultIds.SequenceEqual(result.Select(s => s.Id)));
    }

    [Theory]
    [JsonFileData("testDataDepartment.json", typeof(FilterDto), typeof(List<long>))]
    public async void GetDepartments_ValidFilters_ReturnDesiredList(FilterDto filter, List<long> resultIds)
    {
        // Arrange
        var db = new TestHelper().DbContext;
        var repo = new SampleRepository<Department>(db);

        // Act
        var result = await repo.Get(filter, default);

        // Assert
        Assert.True(resultIds.SequenceEqual(result.Select(s => s.Id)));
    }
}
