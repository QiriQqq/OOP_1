public class Department
{
    private Employee[] _employees;
    private int _count;

    public Department(int capacity)
    {
        _employees = new Employee[capacity];
        _count = 0;
    }

    public void Hire(Employee employee)
    {
        if (employee == null)
            throw new ArgumentNullException(nameof(employee), "Сотрудник не может быть null");
        if (_count == _employees.Length)
            throw new ArgumentException("В отделе нет свободных мест");
        _employees[_count++] = employee;
    }

    public bool Fire(int id)
    {
        for (int i = 0; i < _count; i++)
        {
            if (_employees[i].Id == id)
            {
                for (int j = i; j < _count - 1; j++)
                    _employees[j] = _employees[j + 1];
                _count--;
                _employees[_count] = null!;
                return true;
            }
        }
        return false;
    }

    public Employee[] Disband()
    {
        Employee[] released = GetEmployeesSnapshot();
        Array.Clear(_employees, 0, _count);
        _count = 0;
        return released;
    }

    public double TotalPayroll()
    {
        double sum = 0;
        for (int i = 0; i < _count; i++)
            sum += _employees[i].CalculatePayment();
        return sum;
    }

    public double AveragePayment()
    {
        if (_count == 0) return 0;
        return TotalPayroll() / _count;
    }

    public Employee? MostLoaded()
    {
        if (_count == 0) return null;
        Employee result = _employees[0];
        for (int i = 1; i < _count; i++)
            if (_employees[i].HoursPerWeek > result.HoursPerWeek)
                result = _employees[i];
        return result;
    }

    public Employee[] GetEmployeesSnapshot()
    {
        Employee[] copy = new Employee[_count];
        Array.Copy(_employees, copy, _count);
        return copy;
    }
}
