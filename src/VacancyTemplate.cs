public sealed class VacancyTemplate
{
    public string DepartmentName { get; }
    public string Description { get; }
    public Grade Grade { get; }
    public WorkFormat WorkFormat { get; }
    public int HoursPerWeek { get; }
    public Position Position { get; private set; }
    public double Salary { get; private set; }

    public VacancyTemplate(
        string departmentName,
        string description,
        Position position,
        Grade grade,
        WorkFormat workFormat,
        int hoursPerWeek,
        double salary)
    {
        if (string.IsNullOrWhiteSpace(departmentName))
            throw new ArgumentException("Не указано название отдела", nameof(departmentName));
        if (string.IsNullOrWhiteSpace(description))
            throw new ArgumentException("Не указано описание вакансии", nameof(description));
        Position = position ?? throw new ArgumentNullException(nameof(position));
        Grade = grade ?? throw new ArgumentNullException(nameof(grade));
        WorkFormat = workFormat ?? throw new ArgumentNullException(nameof(workFormat));
        if (hoursPerWeek < 1 || hoursPerWeek > 60)
            throw new ArgumentOutOfRangeException(nameof(hoursPerWeek));
        ValidateSalary(salary);

        DepartmentName = departmentName;
        Description = description;
        HoursPerWeek = hoursPerWeek;
        Salary = salary;
    }

    public VacancyTemplate Copy()
    {
        return new VacancyTemplate(DepartmentName, Description, Position,
            Grade, WorkFormat, HoursPerWeek, Salary);
    }

    public void ChangePosition(Position newPosition)
    {
        Position = newPosition ?? throw new ArgumentNullException(nameof(newPosition));
    }

    public void ChangeSalary(double newSalary)
    {
        ValidateSalary(newSalary);
        Salary = newSalary;
    }

    private static void ValidateSalary(double salary)
    {
        if (!double.IsFinite(salary) || salary < SalariedEmployee.MIN_SALARY)
            throw new ArgumentOutOfRangeException(nameof(salary),
                $"Оклад не может быть ниже {SalariedEmployee.MIN_SALARY:0.00}");
    }
}
