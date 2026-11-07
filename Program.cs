class Program
{
    static void Main()
    {
        // ============ СБОРКА ОБЪЕКТОВ ============
        // Единственное место, где упоминаются конкретные классы.
        // Дальше клиентский код работает только через Employee и IBonusPolicy.

        // справочные объекты неизменяемы, поэтому их безопасно делить между сотрудниками
        var engineer = new Position("Инженер");
        var analyst = new Position("Аналитик");
        var manager = new Position("Менеджер");
        var tester = new Position("Тестировщик");

        var junior = new Grade("Junior", 1);
        var middle = new Grade("Middle", 2);
        var senior = new Grade("Senior", 3);

        var office = new WorkFormat("Офис", false);
        var hybrid = new WorkFormat("Гибрид", false);
        var remote = new WorkFormat("Удалённый", true);

        var hireDate = new DateTime(2023, 3, 1);
        IBonusPolicy noBonus = new NoBonus();

        // конкретные типы нужны только здесь: у каждого свои специфичные методы
        SalariedEmployee e1 = new SalariedEmployee("Иванов Иван Иванович", engineer, middle, office,
            40, hireDate, "Полная", 500, 60000, new PercentBonus(10));
        HourlyEmployee e2 = new HourlyEmployee("Петров Пётр Петрович", analyst, junior, hybrid,
            35, hireDate, "Полная", 500, noBonus);
        SalariedEmployee e3 = new SalariedEmployee("Сидоров Сидор Сидорович", manager, senior, remote,
            30, hireDate, "Полная", 500, 50000, new FixedPlanBonus(15000, true));
        Contractor e4 = new Contractor("Кузнецов Кузьма Кузьмич", tester, middle, remote,
            25, hireDate, "Договор подряда", 500, new FixedPlanBonus(8000, false));
        e4.AddAct(20000);
        e4.AddAct(25000);

        var dept = new Department(10);
        dept.Hire(e1);
        dept.Hire(e2);
        dept.Hire(e3);
        dept.Hire(e4);

        // ЗАДАНИЕ 5: новая разновидность - единственная правка в существующем коде
        dept.Hire(new PieceworkEmployee("Орлов Олег Олегович", engineer, junior, office,
            40, hireDate, "Полная", 500, 400, 150, new PercentBonus(5)));

        // ============ ДЕМОНСТРАЦИЯ ============
        Console.WriteLine("\u001b[33m" + "0. Данные сотрудников (разные разновидности, один вывод) \u001b[0m");
        WriteEmployees(dept.GetEmployeesSnapshot());


        Console.WriteLine("\n\u001b[33m" + "1. Выплаты: один цикл, без проверок типа \u001b[0m");
        WritePayments(dept.GetEmployeesSnapshot());


        Console.WriteLine("\n\u001b[33m" + "2. Демонстрация работы методов класса Department \u001b[0m");
        Console.WriteLine($"Фонд оплаты труда: {dept.TotalPayroll():0.00}");
        Console.WriteLine($"Средняя выплата: {dept.AveragePayment():0.00}");
        Console.WriteLine($"Самый загруженный: {dept.MostLoaded()}");


        Console.WriteLine("\n\u001b[33m" + "3. Подмена политики премии «на лету» (реализация приходит извне) \u001b[0m");
        Console.WriteLine($"{e1.FullName}, до: {e1.CalculatePayment():0.00}");
        e1.SetBonusPolicy(new FixedPlanBonus(5000, true));
        Console.WriteLine($"{e1.FullName}, после (фикс. премия за план): {e1.CalculatePayment():0.00}");
        e1.SetBonusPolicy(new PercentBonus(10));


        Console.WriteLine("\n\u001b[33m" + "4. Повышение оклада на заданный процент (только для сотрудника на окладе) \u001b[0m");
        e1.RaiseSalary(10);
        Console.WriteLine(e1);
        e1.RaiseSalary(10, 1000);
        Console.WriteLine($"С ограничением надбавки 1000: оклад {e1.Salary:0.00}");


        Console.WriteLine("\n\u001b[33m" + "5. Защита от некорректных параметров: старые объекты не страдают \u001b[0m");
        int createdBefore = Employee.GetEmployeesCreated();
        TryAndReport(() => new SalariedEmployee("", engineer, middle, office, 20, hireDate, "Полная", 500, 30000, noBonus),
            "создание с пустым ФИО");
        TryAndReport(() => new SalariedEmployee("Сидоров С.С.", engineer, middle, office, 20, hireDate, "Полная", 500, 5000, noBonus),
            "оклад ниже минимума");
        TryAndReport(() => new HourlyEmployee("Пивченко Я.З.", engineer, middle, office, 80, hireDate, "Полная", 500, noBonus),
            "80 часов в неделю");
        TryAndReport(() => new Contractor("Орлов О.О.", engineer, middle, office, 20, hireDate, "Полная", 500, null),
            "политика премии = null");
        TryAndReport(() => new HourlyEmployee("Белов Б.Б.", null, middle, office, 20, hireDate, "Полная", 500, noBonus),
            "должность = null");
        TryAndReport(() => new HourlyEmployee("Громов Г.Г.", engineer, middle, office, 20, hireDate, "Полная", 0, noBonus),
            "ставка в контракте = 0");
        Console.WriteLine($"Счётчик сотрудников: было {createdBefore}, стало {Employee.GetEmployeesCreated()}");
        Console.WriteLine("Сотрудники в отделе не изменились:");
        WriteEmployees(dept.GetEmployeesSnapshot());


        Console.WriteLine("\n\u001b[33m" + "6. Изменение часов с проверкой \u001b[0m");
        e2.ChangeHoursPerWeek(30);
        Console.WriteLine($"{e2.FullName}: {e2.HoursPerWeek} ч/нед, выплата {e2.CalculatePayment():0.00}");
        TryAndReport(() => e2.ChangeHoursPerWeek(100), "100 часов в неделю");
        Console.WriteLine($"Часы после неудачной попытки: {e2.HoursPerWeek}");


        Console.WriteLine("\n\u001b[33m" + "7. Контракт: композиция и неизменяемость \u001b[0m");
        var standalone = new Contract(hireDate, "Полная", 500);
        Contract modified = standalone.WithNewRate(999);
        Console.WriteLine($"Исходный контракт: {standalone}");
        Console.WriteLine($"Новый объект: {modified}");
        // у каждого сотрудника СВОЙ контракт: переоформление одного не затрагивает других
        e1.RenewContract(600);
        Console.WriteLine($"Ставка у {e1.FullName}: {e1.HourlyRate}, у {e3.FullName}: {e3.HourlyRate}");
        TryAndReport(() => e1.RenewContract(0), "переоформление со ставкой 0");


        Console.WriteLine("\n\u001b[33m" + "8. Композиция вместо иерархии Manager → SeniorManager → RemoteSeniorManager \u001b[0m");
        Console.WriteLine("Было (удалённый старший менеджер):");
        Console.WriteLine(e3);
        e3.ChangeWorkFormat(office);
        e3.ChangeGrade(new Grade("Lead", 4));
        Console.WriteLine("Стал (вышел в офис, повышен) - ни одного нового класса:");
        Console.WriteLine(e3);


        Console.WriteLine("\n\u001b[33m" + "9. Увольнение: попытка с несуществующим id и реальное увольнение \u001b[0m");
        bool fired = dept.Fire(999);
        Console.WriteLine($"Увольнение несуществующего id: успех = {fired}");
        dept.Fire(e2.Id);
        Console.WriteLine($"Уволен сотрудник c id {e2.Id} - {e2.FullName}. Оставшиеся:");
        WriteEmployees(dept.GetEmployeesSnapshot());
        Console.WriteLine($"Всего создано сотрудников: {Employee.GetEmployeesCreated()}");


        Console.WriteLine("\n\u001b[33m" + "10. Агрегация: расформирование отдела не уничтожает сотрудников \u001b[0m");
        var newDept = new Department(5);
        Employee[] released = dept.Disband();
        Console.WriteLine($"Отдел расформирован. В нём осталось: {dept.GetEmployeesSnapshot().Length}");
        Console.WriteLine("Но объекты сотрудников живы и их можно перевести в другой отдел:");
        foreach (Employee emp in released)
            newDept.Hire(emp);
        WriteEmployees(newDept.GetEmployeesSnapshot());
        Console.WriteLine($"ФОТ нового отдела: {newDept.TotalPayroll():0.00}");
    }

    // метод для ловли ошибок при попытке создать объект / выполнить операцию
    static void TryAndReport(Action action, string description)
    {
        try
        {
            action();
            Console.WriteLine($"Ошибка: {description} не была отбита");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Перехвачено ({description}): {ex.Message}");
        }
    }

    // клиентский код: знает только базовый тип Employee
    static void WriteEmployees(IEnumerable<Employee> employees)
    {
        foreach (Employee emp in employees)
        {
            Console.WriteLine(emp);
        }
    }

    // клиентский код: один и тот же вызов - разное поведение. Ни switch, ни is
    static void WritePayments(IEnumerable<Employee> employees)
    {
        foreach (Employee emp in employees)
        {
            Console.WriteLine($"{emp.FullName}: {emp.CalculatePayment():0.00}");
        }
    }
}


// Поле         | Правило                           | Метод, где проверяется
// -------------|-----------------------------------|-----------------------------------
// fullName     | ФИО не пустое                     | Employee(...)
// hoursPerWeek | 1 <= часы <= 60                   | Employee(...), ChangeHoursPerWeek(...)
// salary       | Salary >= MIN_SALARY              | SalariedEmployee(...) (через extraValidation)
// contract     | ставка > 0, тип не пустой         | Contract(...), вызывается из Employee(...)
// position, grade, workFormat, bonusPolicy | не null | Employee(...), ChangeGrade/ChangeWorkFormat/SetBonusPolicy
