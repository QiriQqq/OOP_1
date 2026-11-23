public sealed class Allowance
{
    public string Name { get; }
    public decimal Amount { get; }

    public Allowance(string name, decimal amount)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Название надбавки не может быть пустым", nameof(name));
        if (amount < 0m)
            throw new ArgumentOutOfRangeException(nameof(amount), "Сумма надбавки не может быть отрицательной");
        Name = name;
        Amount = amount;
    }
}

/// <summary>
/// Неизменяемый результат сборки трудового договора.
/// Коллекция надбавок копируется и закрывается для изменения.
/// </summary>
public sealed class EmploymentAgreement
{
    public int EmployeeId { get; }
    public string EmployeeFullName { get; }
    public Position Position { get; }
    public decimal Salary { get; }
    public string WorkSchedule { get; }
    public int ProbationPeriodMonths { get; }
    public IReadOnlyList<Allowance> Allowances { get; }

    internal EmploymentAgreement(
        Employee employee,
        Position position,
        decimal salary,
        string workSchedule,
        int probationPeriodMonths,
        IEnumerable<Allowance> allowances)
    {
        EmployeeId = employee.Id;
        EmployeeFullName = employee.FullName;
        Position = position;
        Salary = salary;
        WorkSchedule = workSchedule;
        ProbationPeriodMonths = probationPeriodMonths;
        Allowances = Array.AsReadOnly(allowances.ToArray());
    }

    public decimal TotalAllowances => Allowances.Sum(x => x.Amount);
}

/// <summary>
/// Пошаговая сборка договора с проверкой обязательных полей в Build.
/// </summary>
public sealed class EmploymentAgreementBuilder
{
    private Employee? _employee;
    private Position? _position;
    private decimal? _salary;
    private string? _workSchedule;
    private int? _probationPeriodMonths;
    private readonly List<(string Name, decimal Amount)> _allowances = new();

    public EmploymentAgreementBuilder ForEmployee(Employee employee)
    {
        _employee = employee ?? throw new ArgumentNullException(nameof(employee));
        return this;
    }

    public EmploymentAgreementBuilder ForPosition(Position position)
    {
        _position = position ?? throw new ArgumentNullException(nameof(position));
        return this;
    }

    public EmploymentAgreementBuilder WithSalary(decimal salary)
    {
        _salary = salary;
        return this;
    }

    public EmploymentAgreementBuilder WithWorkSchedule(string workSchedule)
    {
        _workSchedule = workSchedule;
        return this;
    }

    public EmploymentAgreementBuilder WithProbationPeriod(int months)
    {
        _probationPeriodMonths = months;
        return this;
    }

    public EmploymentAgreementBuilder AddAllowance(string name, decimal amount)
    {
        // Проверка значения откладывается до Build вместе с остальными инвариантами.
        _allowances.Add((name, amount));
        return this;
    }

    public EmploymentAgreement Build()
    {
        if (_employee == null)
            throw new InvalidOperationException("Не указан сотрудник договора");
        if (_position == null)
            throw new InvalidOperationException("Не указана должность");
        if (_salary == null)
            throw new InvalidOperationException("Не указан оклад: договор без оклада создать нельзя");
        if (_salary.Value < (decimal)SalariedEmployee.MIN_SALARY)
            throw new ArgumentOutOfRangeException(nameof(_salary),
                $"Оклад не может быть ниже {SalariedEmployee.MIN_SALARY:0.00}");
        if (string.IsNullOrWhiteSpace(_workSchedule))
            throw new InvalidOperationException("Не указан рабочий график");
        if (_probationPeriodMonths == null)
            throw new InvalidOperationException("Не указан испытательный срок");
        if (_probationPeriodMonths.Value < 0)
            throw new ArgumentOutOfRangeException(nameof(_probationPeriodMonths),
                "Испытательный срок не может быть отрицательным");

        Allowance[] validatedAllowances = _allowances
            .Select(x => new Allowance(x.Name, x.Amount))
            .ToArray();

        return new EmploymentAgreement(
            _employee, _position, _salary.Value, _workSchedule,
            _probationPeriodMonths.Value, validatedAllowances);
    }
}
