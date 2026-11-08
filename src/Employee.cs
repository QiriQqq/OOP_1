public abstract class Employee
{
    private static int _employeesCreated = 0;
    private static int _nextId = 1;

    private readonly int _id;
    private readonly string _fullName;
    private int _hoursPerWeek;
    private Position _position;
    private Grade _grade;
    private WorkFormat _workFormat;
    private Contract _contract;
    private IBonusPolicy _bonusPolicy;

    public static int GetEmployeesCreated() => _employeesCreated;

    protected Employee(
        string fullName, Position position, Grade grade,
        WorkFormat workFormat, int hoursPerWeek, DateTime hireDate,
        string employmentType, double hourlyRate, IBonusPolicy bonusPolicy,
        Action? extraValidation = null)
    {
        if (string.IsNullOrWhiteSpace(fullName))
            throw new ArgumentException("ФИО не может быть пустым");
        CheckHours(hoursPerWeek);
        CheckNotNull(position, nameof(position));
        CheckNotNull(grade, nameof(grade));
        CheckNotNull(workFormat, nameof(workFormat));
        CheckNotNull(bonusPolicy, nameof(bonusPolicy));
        extraValidation?.Invoke();
        Contract newContract = new Contract(hireDate, employmentType, hourlyRate);
        _id = _nextId++;
        _fullName = fullName;
        _hoursPerWeek = hoursPerWeek;
        _position = position;
        _grade = grade;
        _workFormat = workFormat;
        _contract = newContract;
        _bonusPolicy = bonusPolicy;
        _employeesCreated++;
    }

    public int Id => _id;
    public string FullName => _fullName;
    public int HoursPerWeek => _hoursPerWeek;
    public Position Position => _position;
    public Grade Grade => _grade;
    public WorkFormat WorkFormat => _workFormat;
    public double HourlyRate => _contract.HourlyRate;
    public DateTime HireDate => _contract.HireDate;

    public double CalculatePayment()
    {
        double basePayment = CalculateBasePayment();
        return basePayment + _bonusPolicy.CalculateBonus(basePayment);
    }

    protected abstract double CalculateBasePayment();
    protected abstract string PaymentKind { get; }

    public void SetBonusPolicy(IBonusPolicy newPolicy)
    {
        CheckNotNull(newPolicy, nameof(newPolicy));
        _bonusPolicy = newPolicy;
    }

    public void ChangeGrade(Grade newGrade)
    {
        CheckNotNull(newGrade, nameof(newGrade));
        _grade = newGrade;
    }

    public void ChangeWorkFormat(WorkFormat newFormat)
    {
        CheckNotNull(newFormat, nameof(newFormat));
        _workFormat = newFormat;
    }

    public void ChangeHoursPerWeek(int newHours)
    {
        CheckHours(newHours);
        _hoursPerWeek = newHours;
    }

    public void RenewContract(double newHourlyRate)
    {
        _contract = _contract.WithNewRate(newHourlyRate);
    }

    public sealed override string ToString()
    {
        return $"[{_id}] {_fullName}, {_position}, {_grade}, {_workFormat}, {PaymentKind}, " +
               $"{_hoursPerWeek} ч/нед, {_contract}";
    }

    private static void CheckHours(int hours)
    {
        if (hours < 1 || hours > 60)
            throw new ArgumentException("Часов в неделю должно быть от 1 до 60 включительно");
    }

    private static void CheckNotNull(object? value, string paramName)
    {
        if (value == null)
            throw new ArgumentNullException(paramName, $"{paramName} не может быть null");
    }
}
