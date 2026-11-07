public class Contract
{
    public DateTime HireDate { get; }
    public double HourlyRate { get; }
    public string EmploymentType { get; }

    public Contract(DateTime hireDate, string employmentType, double hourlyRate)
    {
        if (string.IsNullOrWhiteSpace(employmentType))
            throw new ArgumentException("Тип занятости не может быть пустым");
        if (hourlyRate <= 0.0)
            throw new ArgumentException("Ставка за час должна быть больше 0");

        // инициализация свойств
        HireDate = hireDate;
        EmploymentType = employmentType;
        HourlyRate = hourlyRate;
    }

    // метод для создания нового контракта с изменённой ставкой
    public Contract WithNewRate(double newHourlyRate)
    {
        return new Contract(HireDate, EmploymentType, newHourlyRate);
    }

    public override string ToString() 
    {
        return $"Контракт от {HireDate:d} ({EmploymentType}), ставка {HourlyRate:0.00}/ч";
    }
}