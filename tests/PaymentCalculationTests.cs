using Xunit;

public class PaymentCalculationTests
{
    [Fact]
    public void Hourly_employee_payment_uses_hours_weeks_and_hourly_rate()
    {
        var employee = TestData.Hourly(35);
        Assert.Equal(70000, employee.CalculatePayment());
    }

    [Fact]
    public void Salaried_employee_payment_includes_percentage_bonus()
    {
        var employee = TestData.Salaried(60000, new PercentBonus(10));
        Assert.Equal(66000, employee.CalculatePayment());
    }

    [Fact]
    public void Contractor_payment_is_sum_of_acts_and_bonus()
    {
        var employee = new Contractor("Сидоров Сидор Сидорович", TestData.Position,
            TestData.Grade, TestData.WorkFormat, 30, TestData.HireDate,
            "Договор подряда", 500, new FixedPlanBonus(8000, true));
        employee.AddAct(20000);
        employee.AddAct(25000);
        Assert.Equal(53000, employee.CalculatePayment());
    }

    [Fact]
    public void Contractor_without_acts_has_zero_base_payment()
    {
        var employee = new Contractor("Сидоров Сидор Сидорович", TestData.Position,
            TestData.Grade, TestData.WorkFormat, 30, TestData.HireDate,
            "Договор подряда", 500, new NoBonus());
        Assert.Equal(0, employee.CalculatePayment());
    }

    [Fact]
    public void Percentage_bonus_is_not_ignored()
    {
        Assert.Equal(6000, new PercentBonus(10).CalculateBonus(60000));
    }
}
