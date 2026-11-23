using NSubstitute;
using Xunit;

public class PayrollSettingsTests
{
    [Fact]
    public void Calculator_uses_injected_fake_settings()
    {
        // До отказа от Singleton тест не мог безопасно подставить отдельные ставки:
        // клиент самостоятельно обращался бы к глобальному статическому объекту.
        IPayrollSettings fake = new FixedPayrollSettings(0.10m, 0.05m);
        var calculator = new PayrollCalculator(fake);

        PayrollCalculation result = calculator.Calculate(1000m);

        Assert.Equal(1000m, result.GrossPayment);
        Assert.Equal(100m, result.IncomeTax);
        Assert.Equal(50m, result.SocialContribution);
        Assert.Equal(850m, result.NetPayment);
    }

    [Fact]
    public void Calculator_uses_nsubstitute_configuration()
    {
        var settings = Substitute.For<IPayrollSettings>();
        settings.IncomeTaxRate.Returns(0.20m);
        settings.SocialContributionRate.Returns(0.05m);
        var calculator = new PayrollCalculator(settings);

        Assert.Equal(750m, calculator.Calculate(1000m).NetPayment);
    }

    [Fact]
    public void Null_settings_are_rejected_by_constructor()
    {
        Assert.Throws<ArgumentNullException>(() => new PayrollCalculator(null!));
    }

    [Fact]
    public void Settings_reject_rates_outside_zero_to_one()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new PayrollSettings(1.01m, 0m));
        Assert.Throws<ArgumentOutOfRangeException>(() => new PayrollSettings(0.5m, 0.6m));
    }
}
