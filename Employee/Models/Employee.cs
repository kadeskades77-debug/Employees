using EMPLOYEE.Models;
using EmployeeDomain.Entities;
using Microsoft.EntityFrameworkCore;

public class Employee
{
    public int Id { get; set; }
    public string Position { get; set; } = string.Empty;
    [Precision(18, 2)]
    public decimal Salary { get; set; }
    public bool IsActive { get; set; } = true;

    public int? DepartmentId { get; set; }

    public string? UserId { get; set; }
}
