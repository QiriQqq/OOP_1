public sealed class FixedTimeProvider : ITimeProvider
{
    public DateTime Now { get; }

    public FixedTimeProvider(DateTime now)
    {
        Now = now;
    }
}

public sealed class FixedRandomSource : IRandomSource
{
    private readonly int value;

    public FixedRandomSource(int value)
    {
        this.value = value;
    }

    public int Next(int maxExclusive) => value;
}

public sealed class InMemoryEmployeeStorage : IEmployeeStorage
{
    private readonly List<Employee> employees = new();

    public void Save(Employee employee)
    {
        employees.Add(employee);
    }

    public IReadOnlyList<Employee> GetAll() => employees.ToArray();
}
