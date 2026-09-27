public class Department
{
    // приватные поля
    private Employee[] employees;
    private int count;

    // конструктор с массивом и счётчиком
    public Department(int capacity)
    {
        employees = new Employee[capacity];
        count = 0;
    }

    // нанять сотдруника (если нет мест - выкидывает ошибку)
    public void Hire(Employee employee)
    {
        if (count == employees.Length)
            throw new ArgumentException("В отделе нет свободных мест");
        employees[count++] = employee;
    }

    // увольнение сотрудника по id (false если не найден)
    public bool Fire(int id)
    {
        for (int i = 0; i < count; i++)
        {
            if (employees[i].Id == id)
            {
                for (int j = i; j < count - 1; j++)
                    employees[j] = employees[j + 1];
                count--;
                return true;
            }
        }

        return false;
    }

    // общая сумма всех окладов
    public double TotalPayroll()
    {
        double sum = 0;
        for (int i = 0; i < count; i++)
            sum += employees[i].Salary;
        return sum;
    }

    // средний оклад (если отдел пуст - 0)
    public double AverageSalary()
    {
        if (count == 0) return 0;
        return TotalPayroll() / count;
    }

    // поиск самого загруженного сотрудника (если отдел не пуст)
    public Employee MostLoaded()
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