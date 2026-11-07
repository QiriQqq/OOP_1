// Сотрудник на сдельной оплате: платят за каждую изготовленную единицу продукции.
// «Является» сотрудником: есть Id, ФИО, контракт, часы, политика премии;
// отличается только способом расчёта базовой части выплаты.
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

    // проверки выполняются до выдачи Id и увеличения счётчика (см. extraValidation в Employee)
    private static void CheckPieceParams(double ratePerUnit, int unitsProduced)
    {
        if (ratePerUnit <= 0)
            throw new ArgumentException("Расценка за единицу должна быть больше 0");
        if (unitsProduced < 0)
            throw new ArgumentException("Количество единиц не может быть отрицательным");
    }

    // учесть ещё изготовленные единицы
    public void AddUnits(int units)
    {
        if (units <= 0)
            throw new ArgumentException("Количество единиц должно быть больше 0");
        unitsProduced += units;
    }

    // базовая выплата = расценка * количество изготовленных единиц
    protected override double CalculateBasePayment() => ratePerUnit * unitsProduced;

    protected override string PaymentKind =>
        $"сдельная оплата ({unitsProduced} ед. по {ratePerUnit:0.00})";
}
