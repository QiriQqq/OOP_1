public class Department
{
    private Employee[] employees;
    private int count;
    
    public Department(int capacity)
    {
        employees = new Employee[capacity];
        count = 0;
    }

    public void Hire(Employee employee)
    {
        if (employee == null)
            throw new ArgumentNullException(nameof(employee), "Сотрудник не может быть null");
        if (count == employees.Length)
            throw new ArgumentException("В отделе нет свободных мест");
        employees[count++] = employee;
    }

    public bool Fire(int id)
    {
        for (int i = 0; i < count; i++)
        {
            if (employees[i].Id == id)
            {
                for (int j = i; j < count - 1; j++)
                    employees[j] = employees[j + 1];
                count--;
                employees[count] = null; 
                return true;
            }
        }

        return false;
    }

    // метод для расформирования отдела и возврата массива сотрудников
    public Employee[] Disband()
    {
        Employee[] released = GetEmployeesSnapshot();
        Array.Clear(employees, 0, count);
        count = 0;
        return released;
    }

    public double TotalPayroll()
    {
        double sum = 0;
        for (int i = 0; i < count; i++)
            sum += employees[i].CalculatePayment();
        return sum;
    }

    public double AveragePayment()
    {
        if (count == 0) return 0;
        return TotalPayroll() / count;
    }

    public Employee? MostLoaded()
    {
        if (count == 0) return null;
        Employee result = employees[0];
        for (int i = 1; i < count; i++)
            if (employees[i].HoursPerWeek > result.HoursPerWeek)
                result = employees[i];
        return result;
    }

    public Employee[] GetEmployeesSnapshot()
    {
        Employee[] copy = new Employee[count];
        Array.Copy(employees, copy, count);
        return copy;
    }
}
