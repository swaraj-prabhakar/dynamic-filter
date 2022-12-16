
namespace DynamicFilter.Test.Tests;

public class SampleServiceTest
{
    [Theory]
    [JsonFileData("testDataEmployee.json", typeof(FilterDto), typeof(List<long>))]
    public void GetEmployees_ValidFilters_ReturnDesiredList(FilterDto filter, List<long> resultIds)
    {
        // Arrange
        var service = new SampleService();

        // Act
        var result = service.GetEmployees(filter);

        // Assert
        Assert.True(resultIds.SequenceEqual(result.Select(s => s.Id)));
    }

    [Theory]
    [JsonFileData("testDataDepartment.json", typeof(FilterDto), typeof(List<long>))]
    public void GetDepartments_ValidFilters_ReturnDesiredList(FilterDto filter, List<long> resultIds)
    {
        // Arrange
        var service = new SampleService();

        // Act
        var result = service.GetDepartments(filter);

        // Assert
        Assert.True(resultIds.SequenceEqual(result.Select(s => s.Id)));
    }
}
