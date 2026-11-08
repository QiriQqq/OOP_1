public interface IBonusPolicy
{
    double CalculateBonus(double basePayment);
}

public class NoBonus : IBonusPolicy
{
    public double CalculateBonus(double basePayment) => 0;
}

public class PercentBonus : IBonusPolicy
{
    private readonly double _percent;

    public PercentBonus(double percent)
    {
        if (percent < 0)
            throw new ArgumentException("Процент премии не может быть отрицательным");
        _percent = percent;
    }

    public double CalculateBonus(double basePayment) => basePayment * _percent / 100.0;
}

public class FixedPlanBonus : IBonusPolicy
{
    private readonly double _amount;
    private readonly bool _planCompleted;

    public FixedPlanBonus(double amount, bool planCompleted)
    {
        if (amount < 0)
            throw new ArgumentException("Сумма премии не может быть отрицательной");
        _amount = amount;
        _planCompleted = planCompleted;
    }

    public double CalculateBonus(double basePayment) => _planCompleted ? _amount : 0;
}
