public class PieceworkEmployee : Employee
{
    private readonly double ratePerUnit;
    private int unitsProduced;

    public PieceworkEmployee(string fullName, Position position, Grade grade, WorkFormat workFormat,
        int hoursPerWeek, DateTime hireDate, string employmentType, double hourlyRate,
        double ratePerUnit, int unitsProduced, IBonusPolicy bonusPolicy)
        : base(fullName, position, grade, workFormat, hoursPerWeek, hireDate, employmentType,
               hourlyRate, bonusPolicy, () => CheckPieceParams(ratePerUnit, unitsProduced))
    {
        this.ratePerUnit = ratePerUnit;
        this.unitsProduced = unitsProduced;
    }

    private static void CheckPieceParams(double ratePerUnit, int unitsProduced)
    {
        if (ratePerUnit <= 0)
            throw new ArgumentException("Расценка за единицу должна быть больше 0");
        if (unitsProduced < 0)
            throw new ArgumentException("Количество единиц не может быть отрицательным");
    }

    public void AddUnits(int units)
    {
        if (units <= 0)
            throw new ArgumentException("Количество единиц должно быть больше 0");
        unitsProduced += units;
    }

    protected override double CalculateBasePayment() => ratePerUnit * unitsProduced;
    protected override string PaymentKind => $"сдельная оплата ({unitsProduced} ед. по {ratePerUnit:0.00})";
}
