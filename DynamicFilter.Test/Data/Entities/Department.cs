namespace DynamicFilter.Test.Data.Entities;

public class Department
{
    public long Id { get; set; }
    public string Name { get; set; }
    public bool IsActive { get; set; }
    public virtual List<Employee> Employees { get; set; }
}
