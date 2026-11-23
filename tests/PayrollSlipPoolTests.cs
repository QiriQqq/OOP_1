using Xunit;

public class PayrollSlipPoolTests
{
    [Fact]
    public void Returned_slip_is_cleared_before_next_rent()
    {
        var pool = new PayrollSlipPool();
        var settings = new FixedPayrollSettings(0.10m, 0.05m);
        Employee employee = TestData.Salaried(60000);

        PayrollSlip first = pool.Rent();
        first.Calculate(employee, settings);
        Assert.Equal("Иванов Иван Иванович", first.EmployeeName);
        Assert.Equal(51000m, first.NetPayment);

        pool.Return(first);
        PayrollSlip second = pool.Rent();

        Assert.Same(first, second);
        Assert.Equal(0, second.EmployeeId);
        Assert.Equal(string.Empty, second.EmployeeName);
        Assert.Equal(0m, second.GrossPayment);
        Assert.Equal(0m, second.NetPayment);
        Assert.Empty(second.TaxDetails);
        Assert.Equal(1, pool.CreatedCount);
    }

    [Fact]
    public void Pool_rejects_double_return()
    {
        var pool = new PayrollSlipPool();
        PayrollSlip slip = pool.Rent();
        pool.Return(slip);

        Assert.Throws<InvalidOperationException>(() => pool.Return(slip));
    }

    [Fact]
    public void Pool_rejects_object_not_rented_from_it()
    {
        var pool = new PayrollSlipPool();

        Assert.Throws<InvalidOperationException>(() => pool.Return(new PayrollSlip()));
    }
}
