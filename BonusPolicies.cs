public interface IBonusPolicy
{
    double CalculateBonus(double basePayment);
}


// Политика 1: премии нет
public class NoBonus : IBonusPolicy
{
    public double CalculateBonus(double basePayment) => 0;
}


// Политика 2: процент от базовой выплаты/оклада
public class PercentBonus : IBonusPolicy
{
    private readonly double percent;

    public PercentBonus(double percent)
    {
        if (percent < 0)
            throw new ArgumentException("Процент премии не может быть отрицательным");
        this.percent = percent;
    }

    public double CalculateBonus(double basePayment) => basePayment * percent / 100.0;
}


// Политика 3: фиксированная сумма за выполненный план
public class FixedPlanBonus : IBonusPolicy
{
    private readonly double amount;
    private readonly bool planCompleted;

    public FixedPlanBonus(double amount, bool planCompleted)
    {
        if (amount < 0)
            throw new ArgumentException("Сумма премии не может быть отрицательной");
        this.amount = amount;
        this.planCompleted = planCompleted;
    }

    public double CalculateBonus(double basePayment) => planCompleted ? amount : 0;
}
