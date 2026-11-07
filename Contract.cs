public class Contract
{
    // автоматическое свойство без сеттера для последующей инициализации в конструкторе
    public DateTime HireDate { get; }
    public string EmploymentType { get; }
    public double HourlyRate { get; }

    // конструктор с проверкой условий
    public Contract(DateTime hireDate, string employmentType, double hourlyRate)
    {
        if (string.IsNullOrWhiteSpace(employmentType))
            throw new ArgumentException("Тип занятости не может быть пустым");
        if (hourlyRate <= 0.0)
            throw new ArgumentException("Ставка за час должна быть больше 0");

        HireDate = hireDate;
        EmploymentType = employmentType;
        HourlyRate = hourlyRate;
    }

    // создаёт новый контракт, сохраняя все поля, кроме ставки - она новая
    public Contract WithNewRate(double newHourlyRate)
    {
        return new Contract(HireDate, EmploymentType, newHourlyRate);
    }

    public override string ToString() 
    {
        return $"Контракт от {HireDate:d} ({EmploymentType}), ставка {HourlyRate:0.00}/ч";
    }
}