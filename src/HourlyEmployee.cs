public class HourlyEmployee : Employee
{
    private const int WeeksPerMonth = 4;

    public HourlyEmployee(
        string fullName, Position position, Grade grade,
        WorkFormat workFormat, int hoursPerWeek, DateTime hireDate,
        string employmentType, double hourlyRate, IBonusPolicy bonusPolicy)
        : base(fullName, position, grade, workFormat, hoursPerWeek,
               hireDate, employmentType, hourlyRate, bonusPolicy)
    {
    }

    protected override double CalculateBasePayment() => HoursPerWeek * WeeksPerMonth * HourlyRate;
    protected override string PaymentKind => "почасовая оплата";
}
