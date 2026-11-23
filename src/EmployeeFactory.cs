public static class EmployeeTypes
{
    public const string Hourly = "hourly";
    public const string Salaried = "salaried";
    public const string Contractor = "contractor";
    public const string Piecework = "piecework";
}

public sealed class EmployeeCreationData
{
    public string FullName { get; }
    public Position Position { get; }
    public Grade Grade { get; }
    public WorkFormat WorkFormat { get; }
    public int HoursPerWeek { get; }
    public DateTime HireDate { get; }
    public string EmploymentType { get; }
    public double HourlyRate { get; }
    public IBonusPolicy BonusPolicy { get; }
    public double? Salary { get; }
    public double? RatePerUnit { get; }
    public int? UnitsProduced { get; }
    public IReadOnlyList<double> ActAmounts { get; }

    public EmployeeCreationData(
        string fullName,
        Position position,
        Grade grade,
        WorkFormat workFormat,
        int hoursPerWeek,
        DateTime hireDate,
        string employmentType,
        double hourlyRate,
        IBonusPolicy bonusPolicy,
        double? salary = null,
        double? ratePerUnit = null,
        int? unitsProduced = null,
        IEnumerable<double>? actAmounts = null)
    {
        FullName = fullName;
        Position = position;
        Grade = grade;
        WorkFormat = workFormat;
        HoursPerWeek = hoursPerWeek;
        HireDate = hireDate;
        EmploymentType = employmentType;
        HourlyRate = hourlyRate;
        BonusPolicy = bonusPolicy;
        Salary = salary;
        RatePerUnit = ratePerUnit;
        UnitsProduced = unitsProduced;
        ActAmounts = Array.AsReadOnly((actAmounts ?? Array.Empty<double>()).ToArray());
    }
}

public interface IEmployeeFactory
{
    Employee Create(string employmentType, EmployeeCreationData data);
}

public sealed class EmployeeFactory : IEmployeeFactory
{
    private readonly Dictionary<string, Func<EmployeeCreationData, Employee>> _registry =
        new(StringComparer.OrdinalIgnoreCase);

    public EmployeeFactory()
    {
        Register(EmployeeTypes.Hourly, data => new HourlyEmployee(
            data.FullName, data.Position, data.Grade, data.WorkFormat,
            data.HoursPerWeek, data.HireDate, data.EmploymentType,
            data.HourlyRate, data.BonusPolicy));

        Register(EmployeeTypes.Salaried, data => new SalariedEmployee(
            data.FullName, data.Position, data.Grade, data.WorkFormat,
            data.HoursPerWeek, data.HireDate, data.EmploymentType,
            data.HourlyRate, Require(data.Salary, "salary"), data.BonusPolicy));

        Register(EmployeeTypes.Contractor, CreateContractor);

        Register(EmployeeTypes.Piecework, data => new PieceworkEmployee(
            data.FullName, data.Position, data.Grade, data.WorkFormat,
            data.HoursPerWeek, data.HireDate, data.EmploymentType,
            data.HourlyRate,
            data.RatePerUnit ?? throw new ArgumentException("Для сдельного сотрудника необходимо указать ratePerUnit"),
            data.UnitsProduced ?? throw new ArgumentException("Для сдельного сотрудника необходимо указать unitsProduced"),
            data.BonusPolicy));
    }

    public void Register(string employmentType, Func<EmployeeCreationData, Employee> creator)
    {
        if (string.IsNullOrWhiteSpace(employmentType))
            throw new ArgumentException("Ключ типа занятости не может быть пустым", nameof(employmentType));
        if (creator == null)
            throw new ArgumentNullException(nameof(creator));

        string key = employmentType.Trim();
        if (_registry.ContainsKey(key))
            throw new InvalidOperationException($"Тип занятости '{key}' уже зарегистрирован");

        _registry.Add(key, creator);
    }

    public Employee Create(string employmentType, EmployeeCreationData data)
    {
        if (string.IsNullOrWhiteSpace(employmentType))
            throw new ArgumentException("Не указан тип занятости", nameof(employmentType));
        if (data == null)
            throw new ArgumentNullException(nameof(data));

        string key = employmentType.Trim();
        if (!_registry.TryGetValue(key, out Func<EmployeeCreationData, Employee>? creator))
        {
            string available = string.Join(", ", _registry.Keys.OrderBy(x => x));
            throw new KeyNotFoundException(
                $"Неизвестный тип занятости '{key}'. Зарегистрированные типы: {available}");
        }

        return creator(data);
    }

    private static Employee CreateContractor(EmployeeCreationData data)
    {
        var contractor = new Contractor(
            data.FullName, data.Position, data.Grade, data.WorkFormat,
            data.HoursPerWeek, data.HireDate, data.EmploymentType,
            data.HourlyRate, data.BonusPolicy);

        foreach (double actAmount in data.ActAmounts)
            contractor.AddAct(actAmount);

        return contractor;
    }

    private static double Require(double? value, string parameterName)
    {
        if (value is null)
            throw new ArgumentException($"Для данного типа сотрудника необходимо указать {parameterName}");
        return value.Value;
    }
}
