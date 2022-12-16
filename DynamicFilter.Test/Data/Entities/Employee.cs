namespace DynamicFilter.Test.Data.Entities;

public class Employee
{
    public long Id { get; set; }
    public long DepartmentId { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Address { get; set; }
    public string PhoneNumber { get; set; }
    public int Age { get; set; }
    public DateTime DateOfBirth { get; set; }
    public DateTime DateOfJoining { get; set; }
    public bool IsPermanent { get; set; }
    public virtual Department Department { get; set; }
}
