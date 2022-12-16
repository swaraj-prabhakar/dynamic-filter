namespace DynamicFilter.Test;

public static class DbData
{
    public static List<Department> GetInitialData()
    {
        return new List<Department>()
        {
            new Department()
            {
                Id = 1,
                Name = "Dept1",
                IsActive = true,
                Employees = new List<Employee>()
                {
                    new Employee()
                    {
                        Id = 1,
                        DepartmentId = 1,
                        FirstName = "John",
                        LastName = "Luther",
                        Address = "132, My Street, Bigtown BG23 4YZ England",
                        PhoneNumber = "1234567890",
                        Age = 30,
                        DateOfBirth = DateTime.Parse("1992-05-02"),
                        DateOfJoining = DateTime.Parse("2020-01-10"),
                        IsPermanent = true,
                    },
                    new Employee()
                    {
                        Id = 3,
                        DepartmentId = 1,
                        FirstName = "Marcos",
                        LastName = "Ferrari",
                        Address = "132, My Street, Kingston, New York 12401 United States",
                        PhoneNumber = "6756454323",
                        Age = 58,
                        DateOfBirth = DateTime.Parse("1988-02-20"),
                        DateOfJoining = DateTime.Parse("2021-11-20"),
                        IsPermanent = false,
                    },
                },
            },
            new Department()
            {
                Id = 2,
                Name = "Dept2",
                IsActive = true,
                Employees = new List<Employee>()
                {
                    new Employee()
                    {
                        Id = 2,
                        DepartmentId = 2,
                        FirstName = "Daniel",
                        LastName = "Smith",
                        Address = "8, My Street, Ilassan Lekki, Lagos 105102 Nigeria",
                        PhoneNumber = "8767468567",
                        Age = 55,
                        DateOfBirth = DateTime.Parse("1990-10-03"),
                        DateOfJoining = DateTime.Parse("2010-04-15"),
                        IsPermanent = true,
                    },
                },
            },
        };
    }
    public static List<Employee> GetEmployees()
    {
        return new List<Employee>()
        {
            new Employee()
            {
                Id = 1,
                DepartmentId = 1,
                FirstName = "John",
                LastName = "Luther",
                Address = "132, My Street, Bigtown BG23 4YZ England",
                PhoneNumber = "1234567890",
                Age = 30,
                DateOfBirth = DateTime.Parse("1992-05-02"),
                DateOfJoining = DateTime.Parse("2020-01-10"),
                IsPermanent = true,
                Department = new Department
                {
                    Id = 1,
                    Name = "Dept1",
                    IsActive = true,
                },
            },
            new Employee()
            {
                Id = 2,
                DepartmentId = 2,
                FirstName = "Daniel",
                LastName = "Smith",
                Address = "8, My Street, Ilassan Lekki, Lagos 105102 Nigeria",
                PhoneNumber = "8767468567",
                Age = 55,
                DateOfBirth = DateTime.Parse("1990-10-03"),
                DateOfJoining = DateTime.Parse("2010-04-15"),
                IsPermanent = true,
                Department = new Department
                {
                    Id = 2,
                    Name = "Dept2",
                    IsActive = true,
                },
            },
            new Employee()
            {
                Id = 3,
                DepartmentId = 1,
                FirstName = "Marcos",
                LastName = "Ferrari",
                Address = "132, My Street, Kingston, New York 12401 United States",
                PhoneNumber = "6756454323",
                Age = 58,
                DateOfBirth = DateTime.Parse("1988-02-20"),
                DateOfJoining = DateTime.Parse("2021-11-20"),
                IsPermanent = false,
                Department = new Department
                {
                    Id = 1,
                    Name = "Dept1",
                    IsActive = true,
                },
            },
        };
    }
}
