public abstract class Employee
{
    private static int employeesCreated = 0;
    private static int nextId = 1;

    private readonly int id;
    private readonly string fullName;
    private Position position;      
    private Grade grade;     
    private WorkFormat workFormat; 
    private int hoursPerWeek;
    private Contract contract;          
    private IBonusPolicy bonusPolicy;   

    public static int GetEmployeesCreated() => employeesCreated;

    protected Employee(
        string fullName, Position position, Grade grade, 
        WorkFormat workFormat, int hoursPerWeek, DateTime hireDate, 
        string employmentType, double hourlyRate, IBonusPolicy bonusPolicy,
        Action? extraValidation = null
    )
    {
        if (string.IsNullOrWhiteSpace(fullName))
            throw new ArgumentException("ФИО не может быть пустым");
        CheckHours(hoursPerWeek);

        // проверка на null для ссылочных типов
        CheckNotNull(position, nameof(position));
        CheckNotNull(grade, nameof(grade));
        CheckNotNull(workFormat, nameof(workFormat));
        CheckNotNull(bonusPolicy, nameof(bonusPolicy));

        extraValidation?.Invoke();

        Contract newContract = new Contract(hireDate, employmentType, hourlyRate);

        id = nextId++;
        this.fullName = fullName;
        this.position = position;
        this.grade = grade;
        this.workFormat = workFormat;
        this.hoursPerWeek = hoursPerWeek;
        this.contract = newContract;
        this.bonusPolicy = bonusPolicy;

        employeesCreated++;
    }

    // свойства для доступа к полям
    public int Id => id;
    public string FullName => fullName;
    public int HoursPerWeek => hoursPerWeek;
    public Position Position => position;
    public Grade Grade => grade;
    public WorkFormat WorkFormat => workFormat;
    public double HourlyRate => contract.HourlyRate;


    // шаблонный метод для расчёта полной оплаты с учётом бонусов
    public double CalculatePayment()
    {
        double basePayment = CalculateBasePayment();
        return basePayment + bonusPolicy.CalculateBonus(basePayment);
    }

    // абстрактные методы, которые должны быть реализованы в наследниках
    protected abstract double CalculateBasePayment();
    protected abstract string PaymentKind { get; }


    // методы для изменения состояния сотрудника
    public void SetBonusPolicy(IBonusPolicy newPolicy)
    {
        CheckNotNull(newPolicy, nameof(newPolicy));
        bonusPolicy = newPolicy;
    }

    public void ChangeGrade(Grade newGrade)
    {
        CheckNotNull(newGrade, nameof(newGrade));
        grade = newGrade;
    }

    public void ChangeWorkFormat(WorkFormat newFormat)
    {
        CheckNotNull(newFormat, nameof(newFormat));
        workFormat = newFormat;
    }

    public void ChangeHoursPerWeek(int newHours)
    {
        CheckHours(newHours);
        hoursPerWeek = newHours;
    }

    public void RenewContract(double newHourlyRate)
    {
        contract = contract.WithNewRate(newHourlyRate);
    }

    // переопределённый ToString с защитой от дальнейшего переопределения
    public sealed override string ToString()    
    {
        return $"[{id}] {fullName}, {position}, {grade}, {workFormat}, {PaymentKind}, " +
               $"{hoursPerWeek} ч/нед, {contract}";
    }

    // вспомогательные методы для проверки аргументов
    private static void CheckHours(int hours)
    {
        if (hours < 1 || hours > 60)
            throw new ArgumentException("Часов в неделю должно быть от 1 до 60 включительно");
    }

    private static void CheckNotNull(object value, string paramName)
    {
        if (value == null)
            throw new ArgumentNullException(paramName, $"{paramName} не может быть null");
    }
}
