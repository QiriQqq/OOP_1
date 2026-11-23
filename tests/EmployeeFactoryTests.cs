using Xunit;

public class EmployeeFactoryTests
{
    [Fact]
    public void Factory_creates_all_registered_employee_variants()
    {
        var factory = new EmployeeFactory();

        Employee hourly = factory.Create(EmployeeTypes.Hourly, TestData.CreationData());
        Employee salaried = factory.Create(EmployeeTypes.Salaried, TestData.CreationData(salary: 50000));
        Employee contractor = factory.Create(EmployeeTypes.Contractor,
            TestData.CreationData(actAmounts: new[] { 1000d, 2000d }));
        Employee piecework = factory.Create(EmployeeTypes.Piecework,
            TestData.CreationData(ratePerUnit: 10, unitsProduced: 15));

        Assert.IsType<HourlyEmployee>(hourly);
        Assert.IsType<SalariedEmployee>(salaried);
        Assert.IsType<Contractor>(contractor);
        Assert.Equal(3000, contractor.CalculatePayment());
        Assert.IsType<PieceworkEmployee>(piecework);
        Assert.Equal(150, piecework.CalculatePayment());
    }

    [Fact]
    public void Factory_rejects_unknown_type_with_available_values()
    {
        var factory = new EmployeeFactory();

        KeyNotFoundException exception = Assert.Throws<KeyNotFoundException>(
            () => factory.Create("intern", TestData.CreationData()));

        Assert.Contains("intern", exception.Message);
        Assert.Contains(EmployeeTypes.Hourly, exception.Message);
    }

    [Fact]
    public void Factory_rejects_duplicate_registration()
    {
        var factory = new EmployeeFactory();

        Assert.Throws<InvalidOperationException>(() =>
            factory.Register(EmployeeTypes.Hourly, _ => TestData.Hourly()));
    }

    [Fact]
    public void Factory_checks_variant_specific_fields()
    {
        var factory = new EmployeeFactory();

        Assert.Throws<ArgumentException>(() =>
            factory.Create(EmployeeTypes.Salaried, TestData.CreationData()));
        Assert.Throws<ArgumentException>(() =>
            factory.Create(EmployeeTypes.Piecework, TestData.CreationData(ratePerUnit: 10)));
    }
}
