/// <summary>
/// Абстракция настроек удержаний. Клиенты зависят от интерфейса,
/// а не от глобального экземпляра конкретного класса.
/// </summary>
public interface IPayrollSettings
{
    decimal IncomeTaxRate { get; }
    decimal SocialContributionRate { get; }
}

/// <summary>
/// Обычный объект настроек. Он намеренно не является Singleton:
/// единственный экземпляр при необходимости создаётся в точке сборки приложения.
/// Ставки задаются долями: 0.13m означает 13 процентов.
/// </summary>
public sealed class PayrollSettings : IPayrollSettings
{
    public decimal IncomeTaxRate { get; }
    public decimal SocialContributionRate { get; }

    public PayrollSettings(decimal incomeTaxRate, decimal socialContributionRate)
    {
        ValidateRate(incomeTaxRate, nameof(incomeTaxRate));
        ValidateRate(socialContributionRate, nameof(socialContributionRate));
        if (incomeTaxRate + socialContributionRate > 1m)
            throw new ArgumentOutOfRangeException(nameof(socialContributionRate),
                "Сумма ставок удержаний не может превышать 100 процентов");

        IncomeTaxRate = incomeTaxRate;
        SocialContributionRate = socialContributionRate;
    }

    private static void ValidateRate(decimal rate, string parameterName)
    {
        if (rate < 0m || rate > 1m)
            throw new ArgumentOutOfRangeException(parameterName,
                "Ставка должна находиться в диапазоне от 0 до 1");
    }
}

public sealed record PayrollCalculation(
    decimal GrossPayment,
    decimal IncomeTax,
    decimal SocialContribution,
    decimal NetPayment);

/// <summary>
/// Клиентский сервис. Настройки получает извне через конструктор.
/// </summary>
public sealed class PayrollCalculator
{
    private readonly IPayrollSettings _settings;

    public PayrollCalculator(IPayrollSettings settings)
    {
        _settings = settings ?? throw new ArgumentNullException(nameof(settings));
    }

    public PayrollCalculation Calculate(decimal grossPayment)
    {
        if (grossPayment < 0m)
            throw new ArgumentOutOfRangeException(nameof(grossPayment), "Начисление не может быть отрицательным");

        decimal incomeTax = grossPayment * _settings.IncomeTaxRate;
        decimal socialContribution = grossPayment * _settings.SocialContributionRate;
        decimal netPayment = grossPayment - incomeTax - socialContribution;

        return new PayrollCalculation(grossPayment, incomeTax, socialContribution, netPayment);
    }
}
