using System.Collections.ObjectModel;
using System.Diagnostics;

public readonly record struct TaxDetail(string Name, decimal Rate, decimal Amount);

// рассчётный лист одного сотрудника
public sealed class PayrollSlip
{
    private readonly List<TaxDetail> _taxDetails = new(2);
    private readonly ReadOnlyCollection<TaxDetail> _readOnlyTaxDetails;

    public int EmployeeId { get; private set; }
    public string EmployeeName { get; private set; } = string.Empty;
    public decimal GrossPayment { get; private set; }
    public decimal IncomeTax { get; private set; }
    public decimal SocialContribution { get; private set; }
    public decimal NetPayment { get; private set; }
    public IReadOnlyList<TaxDetail> TaxDetails => _readOnlyTaxDetails;

    public PayrollSlip()
    {
        _readOnlyTaxDetails = _taxDetails.AsReadOnly();
    }

    public void Calculate(Employee employee, IPayrollSettings settings)
    {
        if (employee == null)
            throw new ArgumentNullException(nameof(employee));
        if (settings == null)
            throw new ArgumentNullException(nameof(settings));

        Reset();
        EmployeeId = employee.Id;
        EmployeeName = employee.FullName;
        GrossPayment = Convert.ToDecimal(employee.CalculatePayment());
        IncomeTax = GrossPayment * settings.IncomeTaxRate;
        SocialContribution = GrossPayment * settings.SocialContributionRate;
        NetPayment = GrossPayment - IncomeTax - SocialContribution;
        _taxDetails.Add(new TaxDetail("Подоходный налог", settings.IncomeTaxRate, IncomeTax));
        _taxDetails.Add(new TaxDetail("Социальный взнос", settings.SocialContributionRate, SocialContribution));
    }

    public void Reset()
    {
        EmployeeId = 0;
        EmployeeName = string.Empty;
        GrossPayment = 0m;
        IncomeTax = 0m;
        SocialContribution = 0m;
        NetPayment = 0m;
        _taxDetails.Clear();
    }
}

// склад листов
public sealed class PayrollSlipPool
{
    private readonly Stack<PayrollSlip> _available = new();
    private readonly HashSet<PayrollSlip> _leased = new();

    public int AvailableCount => _available.Count;
    public int LeasedCount => _leased.Count;
    public int CreatedCount { get; private set; }

    public PayrollSlip Rent()
    {
        PayrollSlip slip;
        if (_available.Count > 0)
        {
            slip = _available.Pop();
        }
        else
        {
            slip = new PayrollSlip();
            CreatedCount++;
        }

        if (!_leased.Add(slip))
            throw new InvalidOperationException("Объект уже выдан из пула");
        return slip;
    }

    public void Return(PayrollSlip slip)
    {
        if (slip == null)
            throw new ArgumentNullException(nameof(slip));
        if (!_leased.Remove(slip))
            throw new InvalidOperationException("Нельзя вернуть объект, который не был выдан этим пулом");

        slip.Reset();
        _available.Push(slip);
    }
}

public sealed record PayrollBenchmarkResult(
    double WithoutPoolMilliseconds,
    double WithPoolMilliseconds,
    double PoolSavingsPercent,
    int Rounds,
    int EmployeesPerRound)
{
    public bool PoolIsWorthwhile => PoolSavingsPercent >= 5.0;
}

// бенчмарк (есть ли ваще смысл в использовании пула)
public static class PayrollPoolBenchmark
{
    public const int EmployeesPerRound = 1000;

    public static PayrollBenchmarkResult Run(
        IReadOnlyList<Employee> employees,
        IPayrollSettings settings,
        int rounds = 30)
    {
        if (employees == null)
            throw new ArgumentNullException(nameof(employees));
        if (employees.Count != EmployeesPerRound)
            throw new ArgumentException($"Для сравнения требуется ровно {EmployeesPerRound} сотрудников", nameof(employees));
        if (settings == null)
            throw new ArgumentNullException(nameof(settings));
        if (rounds < 1)
            throw new ArgumentOutOfRangeException(nameof(rounds));

        MeasureWithoutPool(employees, settings, 2);
        MeasureWithPool(employees, settings, 2);

        double withoutPool = MeasureWithoutPool(employees, settings, rounds);
        double withPool = MeasureWithPool(employees, settings, rounds);
        double savings = withoutPool <= 0
            ? 0
            : (withoutPool - withPool) / withoutPool * 100.0;

        return new PayrollBenchmarkResult(withoutPool, withPool, savings, rounds, EmployeesPerRound);
    }

    private static double MeasureWithoutPool(
        IReadOnlyList<Employee> employees, IPayrollSettings settings, int rounds)
    {
        decimal checksum = 0m;
        var stopwatch = Stopwatch.StartNew();

        for (int round = 0; round < rounds; round++)
        {
            foreach (Employee employee in employees)
            {
                var slip = new PayrollSlip();
                slip.Calculate(employee, settings);
                checksum += slip.NetPayment;
            }
        }

        stopwatch.Stop();
        GC.KeepAlive(checksum);
        return stopwatch.Elapsed.TotalMilliseconds / rounds;
    }

    private static double MeasureWithPool(
        IReadOnlyList<Employee> employees, IPayrollSettings settings, int rounds)
    {
        decimal checksum = 0m;
        var pool = new PayrollSlipPool();
        var stopwatch = Stopwatch.StartNew();

        for (int round = 0; round < rounds; round++)
        {
            foreach (Employee employee in employees)
            {
                PayrollSlip slip = pool.Rent();
                try
                {
                    slip.Calculate(employee, settings);
                    checksum += slip.NetPayment;
                }
                finally
                {
                    pool.Return(slip);
                }
            }
        }

        stopwatch.Stop();
        GC.KeepAlive(checksum);
        return stopwatch.Elapsed.TotalMilliseconds / rounds;
    }
}
