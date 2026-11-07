using Xunit;

public class DepartmentTests
{
    [Fact]
    public void Average_payment_uses_only_occupied_employee_slots()
    {
        var department = new Department(8);
        department.Hire(TestData.Salaried(20000));
        department.Hire(TestData.Salaried(40000));
        Assert.Equal(30000, department.AveragePayment());
    }

    [Fact]
    public void Empty_department_has_zero_average_payment()
    {
        var department = new Department(4);
        Assert.Equal(0, department.AveragePayment());
    }
}
