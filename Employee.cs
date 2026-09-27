public class Employee
{
    // приватные поля (MIN_SALARY - константа, её всё равно не изменить)
    private static int employeesCreated = 0;
    public const double MIN_SALARY = 20000.0;
    private static int nextId = 1;
    private readonly int id;
    private string fullName;
    private string position;
    private double salary;
    private int hoursPerWeek;
    private Contract contract;

    // employeesCreated только для чтения через expression body
    public static int GetEmployeesCreated() => employeesCreated;

    // основной конструктор с проверкой трёх основных условий 
    public Employee(string fullName, string position, double salary, int hoursPerWeek, Contract contract)
    {
        if (string.IsNullOrWhiteSpace(fullName))
            throw new ArgumentException("ФИО не может быть пустым");
        if (salary < MIN_SALARY)
            throw new ArgumentException($"Оклад не может быть ниже, чем {MIN_SALARY}");
        if (hoursPerWeek < 1 || hoursPerWeek > 60)
            throw new ArgumentException("Часов в неделю должно быть от 1 до 60 включительно");

        this.id = nextId++;
        this.fullName = fullName;
        this.position = position;
        this.salary = salary;
        this.hoursPerWeek = hoursPerWeek;
        this.contract = contract;

        employeesCreated++;
    }

    // упрощённый конструктор, вызывающий основной 
    public Employee(string fullName, double salary, Contract contract)
        : this(fullName, "Без должности", salary, 40, contract) { }

    // важные поля только для чтения 
    public int Id => id;
    public string FullName => fullName;
    public double Salary => salary;
    public int HoursPerWeek => hoursPerWeek;
    public Contract GetContract() => contract;

    // отдельный метод для переоформления контракта
    public void RenewContract(Contract newContract)
    {
        // чтобы при вызове ToString для работника программа не упала с NullReferenceException
        if (newContract == null)
            throw new ArgumentException(nameof(newContract), "Контракт не может быть null");
        contract = newContract;
    }

    // 
    public void RaiseSalary(double percent)
    {
        RaiseSalary(percent, double.MaxValue);
    }

    public void RaiseSalary(double percent, double maxAmount)
    {
        // проверки percent и maxAmount на "разумность"
        if (percent <= 0)
            throw new ArgumentException("Процент повышения должен быть положительным");
        if (maxAmount < 0)
            throw new ArgumentException("Максимальная сумма повышения не может быть отрицательной");

        double increase = salary * percent / 100.0;
        if (increase > maxAmount)
            increase = maxAmount;

        salary += increase;
    }

    // переопределённый ToString()
    public override string ToString()
    {
        return $"[{id}] {fullName}, {position}, оклад {salary:0.00}," +
               $"{hoursPerWeek} ч/нед, {contract}";
    }

}