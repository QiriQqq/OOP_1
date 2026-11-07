public interface ITimeProvider
{
    DateTime Now { get; }
}

public interface IRandomSource
{
    int Next(int maxExclusive);
}

public interface IEmployeeStorage
{
    void Save(Employee employee);
    IReadOnlyList<Employee> GetAll();
}

public class EmployeeWorkflowService
{
    private readonly ITimeProvider timeProvider;
    private readonly IRandomSource randomSource;
    private readonly IEmployeeStorage storage;

    public EmployeeWorkflowService(ITimeProvider timeProvider, IRandomSource randomSource, IEmployeeStorage storage)
    {
        this.timeProvider = timeProvider ?? throw new ArgumentNullException(nameof(timeProvider));
        this.randomSource = randomSource ?? throw new ArgumentNullException(nameof(randomSource));
        this.storage = storage ?? throw new ArgumentNullException(nameof(storage));
    }

    public double CalculateSeniorityBonus(Employee employee, double annualPercent)
    {
        if (employee == null)
            throw new ArgumentNullException(nameof(employee));
        if (annualPercent < 0)
            throw new ArgumentOutOfRangeException(nameof(annualPercent));

        DateTime now = timeProvider.Now;
        int years = now.Year - employee.HireDate.Year;
        if (now.Date < employee.HireDate.AddYears(years).Date)
            years--;
        if (years < 0)
            years = 0;

        return employee.CalculatePayment() * years * annualPercent / 100.0;
    }

    public string AssignTask(IReadOnlyList<string> tasks)
    {
        if (tasks == null)
            throw new ArgumentNullException(nameof(tasks));
        if (tasks.Count == 0)
            throw new ArgumentException("Список задач пуст", nameof(tasks));

        int index = randomSource.Next(tasks.Count);
        if (index < 0 || index >= tasks.Count)
            throw new InvalidOperationException("Источник случайности вернул недопустимый индекс");
        return tasks[index];
    }

    public void RegisterEmployee(Employee employee)
    {
        if (employee == null)
            throw new ArgumentNullException(nameof(employee));
        storage.Save(employee);
    }

    public IReadOnlyList<Employee> GetEmployees() => storage.GetAll();
}
