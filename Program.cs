class Program
{
    static void Main()
    {
        // СОЗДАНИЕ СОТРУДНИКОВ И ДЕПАРТАМЕНТА
        var contract1 = new Contract(new DateTime(2023, 3, 1), "Полная", 500);
        var e1 = new Employee("Иванов Иван Иванович", "Инженер", 60000, 40, contract1);
        var e2 = new Employee("Петров Пётр Петрович", "Аналитик", 55000, 35, contract1);
        var e3 = new Employee("Сидоров Сидор Сидорович", "Менеджер", 50000, 30, contract1);
        var e4 = new Employee("Кузнецов Кузьма Кузьмич", "Тестировщик", 45000, 25, contract1);

        var dept = new Department(10);
        dept.Hire(e1);
        dept.Hire(e2);
        dept.Hire(e3);
        dept.Hire(e4);

        // ТЕСТЫ
        Console.WriteLine("\u001b[33m" + "0. Данные сотрудников \u001b[0m");
        WriteEmployees(dept.GetEmployeesSnapshot());


        Console.WriteLine("\n\u001b[33m" + "1. Демонстрация работы методов класса Department \u001b[0m");
        Console.WriteLine($"Фонд ЗП: {dept.TotalPayroll()}");
        Console.WriteLine($"Средний оклад: {dept.AverageSalary()}");
        Console.WriteLine($"Самый загруженный: {dept.MostLoaded()}");


        Console.WriteLine("\n\u001b[33m" + "2. Демонстрация работы метода, повышающего оклад на заданный процент \u001b[0m");
        e1.RaiseSalary(10);
        Console.WriteLine(e1);


        Console.WriteLine("\n\u001b[33m" + "3. Демонстрация защиты от некорретных параметров (пустое ФИО, оклад, часы) \u001b[0m");
        // передаются три действия через лямбды (' () => ' - функция без параметров)
        TryAndReport(() => new Employee("", "Стажёр", 30000, 20, contract1), 
            "создание с пустым ФИО");
        TryAndReport(() => new Employee("Сидоров С.С.", "Стажёр", 5000, 20, contract1),
            "оклад ниже минимума");
        TryAndReport(() => new Employee("Пивченко Я.З.", "Стажёр", 30000, 80, contract1),
            "80 часов в неделю");
        Console.WriteLine("Сотрудники после попытки создания с некорректными параметрами:");
        WriteEmployees(dept.GetEmployeesSnapshot());


        Console.WriteLine("\n\u001b[33m" + "4. Попытка уволить несуществующего работника \u001b[0m");
        bool fired = dept.Fire(999);
        Console.WriteLine($"Увольнение несуществующего id: успех = {fired}" +
            $"\nОставшиеся сотрудники:");
        WriteEmployees(dept.GetEmployeesSnapshot());


        Console.WriteLine("\n\u001b[33m" + "5. Демонстрация неизменяемости контракта\u001b[0m");
        Contract got = e1.GetContract();
        Contract modified = got.WithNewRate(999);
        Console.WriteLine($"Исходный контракт сотрудника: {e1.GetContract()}");
        Console.WriteLine($"Новый объект: {modified}");


        Console.WriteLine("\n\u001b[33m" + "6. Демонстрация увольнения сотрудника\u001b[0m");
        // вызов через название класса, чтобы показать, что счётчик общий
        Console.WriteLine($"Всего создано сотрудников: {Employee.GetEmployeesCreated()}");

        dept.Fire(e2.Id);
        Console.WriteLine($"Увольнение сотрудника c id {e2.Id} - {e2.FullName}");
        WriteEmployees(dept.GetEmployeesSnapshot());

    }

    // метод для ловли ошибок при попытке создать новый объект сотрудника 
    static void TryAndReport(Action action, string description)
    {
        try
        {
            action(); // делегат (выполнит переданную лямбду)
            Console.WriteLine($"Ошибка: {description} не была отбита");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Перехвачено ({description}): {ex.Message}");
        }
    }

    // метод для вывода списка сотрудников
    static void WriteEmployees(Employee[] employees)
    {
        foreach (Employee emp in employees)
        {
            Console.WriteLine(emp);
        }
    }    
}


// Поле / параметр        | Правило                                   | Метод, где проверяется
// -----------------------|-------------------------------------------|---------------------------------
// Employee.fullName      | ФИО не пустое и не из одних пробелов      | Employee(...) - основной конструктор
// Employee.salary        | Salary >= MIN_SALARY                      | Employee(...) - основной конструктор
//                        |                                           | (в RaiseSalary оклад только растёт,
//                        |                                           |  поэтому ниже минимума уйти не может)
// Employee.hoursPerWeek  | 1 <= часы <= 60                           | Employee(...) - основной конструктор
// Employee.contract      | не null при переоформлении                | RenewContract(...)
// Employee.id            | задаётся один раз (readonly), снаружи     | Employee(...) - присваивается только
//                        | изменить нельзя                           | в конструкторе, сеттера нет
// percent                | процент повышения > 0                     | RaiseSalary(percent, maxAmount)
// maxAmount              | предел прибавки >= 0                      | RaiseSalary(percent, maxAmount)
// Contract.employmentType| тип занятости не пустой                   | Contract(...)
// Contract.hourlyRate    | ставка за час > 0                         | Contract(...) и WithNewRate(...)
//                        |                                           | (через конструктор)
// Contract (все поля)    | после создания не меняются                | { get; } без set: изменение
//                        |                                           | возможно только через WithNewRate
// Department.employees   | нельзя нанять сверх вместимости           | Hire(...)
// Department.employees   | массив не отдаётся наружу, только копия   | GetEmployeesSnapshot()
// id при увольнении      | несуществующий id не роняет программу     | Fire(...) - возвращает false