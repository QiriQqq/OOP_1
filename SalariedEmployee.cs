public class SalariedEmployee : Employee
{
    public const double MIN_SALARY = 20000.0;
    private double salary;

    public SalariedEmployee(
        string fullName, Position position, Grade grade, WorkFormat workFormat,
        int hoursPerWeek, DateTime hireDate, string employmentType, double hourlyRate,
        double salary, IBonusPolicy bonusPolicy) 
        : base(fullName, position, grade, workFormat, hoursPerWeek, hireDate, 
               employmentType, hourlyRate, bonusPolicy, () => CheckSalary(salary))
    {
        this.salary = salary;
    }

    public double Salary => salary;

    // метод для проверки оклада при создании объекта
    private static void CheckSalary(double salary)
    {
        if (salary < MIN_SALARY)
            throw new ArgumentException($"Оклад не может быть ниже, чем {MIN_SALARY}");
    }

    // повышение оклада на процент, но не больше maxAmount
    public void RaiseSalary(double percent, double maxAmount)
    {
        if (percent <= 0)
            throw new ArgumentException("Процент повышения должен быть положительным");
        if (maxAmount < 0)
            throw new ArgumentException("Максимальная сумма повышения не может быть отрицательной");

        double increase = salary * percent / 100.0;
        if (increase > maxAmount)
            increase = maxAmount;

        salary += increase;
    }

    // перегрузка без ограничения суммы
    public void RaiseSalary(double percent)
    {
        RaiseSalary(percent, double.MaxValue);
    }

    // реализация абстрактных методов из базового класса
    protected override double CalculateBasePayment() => salary;
    protected override string PaymentKind => $"оклад {salary:0.00}";
}
