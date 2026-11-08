public class Contractor : Employee
{
    private readonly List<double> _acts = new List<double>();

    public Contractor(
        string fullName, Position position, Grade grade,
        WorkFormat workFormat, int hoursPerWeek, DateTime hireDate,
        string employmentType, double hourlyRate, IBonusPolicy bonusPolicy)
        : base(fullName, position, grade, workFormat, hoursPerWeek,
            hireDate, employmentType, hourlyRate, bonusPolicy)
    {
    }

    public void AddAct(double amount)
    {
        if (amount <= 0)
            throw new ArgumentException("Сумма акта должна быть больше 0");
        _acts.Add(amount);
    }

    protected override double CalculateBasePayment()
    {
        double sum = 0;
        foreach (double act in _acts)
            sum += act;
        return sum;
    }

    protected override string PaymentKind => $"оплата по актам ({_acts.Count} шт.)";
}
