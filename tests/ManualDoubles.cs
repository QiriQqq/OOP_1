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
    private readonly int _value;

    public FixedRandomSource(int value)
    {
        _value = value;
    }

    public int Next(int maxExclusive) => _value;
}

public sealed class InMemoryEmployeeStorage : IEmployeeStorage
{
    private readonly List<Employee> _employees = new();

    public void Save(Employee employee)
    {
        _employees.Add(employee);
    }

    public IReadOnlyList<Employee> GetAll() => _employees.ToArray();
}
