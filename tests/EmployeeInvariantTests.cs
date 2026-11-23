using Xunit;

public class EmployeeInvariantTests
{
    [Theory]
    [InlineData(1)]
    [InlineData(60)]
    public void Hours_at_allowed_limits_are_accepted(int hours)
    {
        var employee = TestData.Hourly(hours);
        Assert.Equal(hours, employee.HoursPerWeek);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(61)]
    [InlineData(100)]
    public void Hours_outside_allowed_range_are_rejected(int hours)
    {
        Assert.Throws<ArgumentException>(() => TestData.Hourly(hours));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Empty_employee_name_is_rejected(string? fullName)
    {
        Assert.Throws<ArgumentException>(() => new HourlyEmployee(fullName!, TestData.Position,
            TestData.Grade, TestData.WorkFormat, 40, TestData.HireDate, "Полная", 500, TestData.NoBonus));
    }

    [Fact]
    public void Non_empty_employee_name_is_accepted()
    {
        var employee = new HourlyEmployee("Иванов Иван Иванович", TestData.Position,
            TestData.Grade, TestData.WorkFormat, 40, TestData.HireDate, "Полная", 500, TestData.NoBonus);
        Assert.Equal("Иванов Иван Иванович", employee.FullName);
    }

    [Fact]
    public void Minimum_salary_is_accepted()
    {
        var employee = TestData.Salaried(SalariedEmployee.MIN_SALARY);
        Assert.Equal(SalariedEmployee.MIN_SALARY, employee.Salary);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(19999.99)]
    public void Salary_below_minimum_is_rejected(double salary)
    {
        Assert.Throws<ArgumentException>(() => TestData.Salaried(salary));
    }
}
