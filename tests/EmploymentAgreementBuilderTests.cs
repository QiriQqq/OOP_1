using Xunit;

public class EmploymentAgreementBuilderTests
{
    [Fact]
    public void Builder_creates_contract_with_all_fields_and_allowances()
    {
        EmploymentAgreement agreement = new EmploymentAgreementBuilder()
            .ForEmployee(TestData.Salaried())
            .ForPosition(TestData.Position)
            .WithSalary(55000m)
            .WithWorkSchedule("5/2")
            .WithProbationPeriod(3)
            .AddAllowance("Питание", 2500m)
            .AddAllowance("Транспорт", 1500m)
            .Build();

        Assert.Equal("Иванов Иван Иванович", agreement.EmployeeFullName);
        Assert.True(agreement.EmployeeId > 0);
        Assert.Equal(55000m, agreement.Salary);
        Assert.Equal("5/2", agreement.WorkSchedule);
        Assert.Equal(3, agreement.ProbationPeriodMonths);
        Assert.Equal(4000m, agreement.TotalAllowances);
        Assert.Equal(2, agreement.Allowances.Count);
    }

    [Fact]
    public void Builder_rejects_contract_without_employee()
    {
        Assert.Throws<InvalidOperationException>(() => new EmploymentAgreementBuilder()
            .ForPosition(TestData.Position).WithSalary(50000m)
            .WithWorkSchedule("5/2").WithProbationPeriod(3).Build());
    }

    [Fact]
    public void Builder_rejects_contract_without_salary()
    {
        Assert.Throws<InvalidOperationException>(() => new EmploymentAgreementBuilder()
            .ForEmployee(TestData.Salaried()).ForPosition(TestData.Position)
            .WithWorkSchedule("5/2").WithProbationPeriod(3).Build());
    }

    [Fact]
    public void Builder_rejects_salary_below_domain_minimum()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new EmploymentAgreementBuilder()
            .ForEmployee(TestData.Salaried()).ForPosition(TestData.Position)
            .WithSalary(19999m).WithWorkSchedule("5/2")
            .WithProbationPeriod(3).Build());
    }

    [Fact]
    public void Built_agreement_does_not_share_mutable_allowance_list_with_builder()
    {
        var builder = new EmploymentAgreementBuilder()
            .ForEmployee(TestData.Salaried()).ForPosition(TestData.Position)
            .WithSalary(50000m).WithWorkSchedule("5/2")
            .WithProbationPeriod(3).AddAllowance("Питание", 1000m);

        EmploymentAgreement agreement = builder.Build();
        builder.AddAllowance("Транспорт", 500m);

        Assert.Single(agreement.Allowances);
        Assert.Equal(1000m, agreement.TotalAllowances);
    }
    [Fact]
    public void Builder_validates_allowances_inside_build()
    {
        var builder = new EmploymentAgreementBuilder()
            .ForEmployee(TestData.Salaried()).ForPosition(TestData.Position)
            .WithSalary(50000m).WithWorkSchedule("5/2")
            .WithProbationPeriod(3).AddAllowance("", 100m);

        Assert.Throws<ArgumentException>(() => builder.Build());
    }

    [Fact]
    public void Builder_rejects_missing_work_schedule()
    {
        Assert.Throws<InvalidOperationException>(() => new EmploymentAgreementBuilder()
            .ForEmployee(TestData.Salaried()).ForPosition(TestData.Position)
            .WithSalary(50000m).WithProbationPeriod(3).Build());
    }

}
