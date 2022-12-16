namespace DynamicFilter.Test.Services;

public class SampleService
{
    public List<Employee> GetEmployees(FilterDto filter)
    {
        return DbData.GetEmployees().ApplyFilter(filter).ToList();
    }
    public List<Department> GetDepartments(FilterDto filter)
    {
        return DbData.GetInitialData().ApplyFilter(filter).ToList();
    }
}
