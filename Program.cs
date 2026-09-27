using System.Diagnostics.Contracts;
using System.Reflection;
using System.Security.AccessControl;
using System.Security.Permissions;

class Programm
{
    static void Main()
    {
        var contract1 = new Contract(new DateTime(2023, 3, 1), "Полная", 500);
        var e1 = new Employee("Иванов Иван Иванович", "Инженер", 60000, 40, contract1);
        var e2 = new Employee("Петров Пётр Петрович", "Аналитик", 55000, 35, contract1);

        var dept = new Department(10);
        dept.Hire(e1);
        dept.Hire(e2);


        Console.WriteLine("\u001b[33m" + "1. Демонстрация работы методов класса Department \u001b[0m");
        Console.WriteLine($"Фонд ЗП: {dept.TotalPayroll()}");
        Console.WriteLine($"Средний оклад: {dept.AverageSalary()}");
        Console.WriteLine($"Самый загруженный: {dept.MostLoaded()}");


        Console.WriteLine("\n\u001b[33m" + "2. Демонстрация работы метода, повышающего оклад на заданный процент \u001b[0m");
        e1.RaiseSalary(10);
        Console.WriteLine(e1);


        Console.WriteLine("\n\u001b[33m" + "3. Демонстрация защиты от некорретных параметров (пустое ФИО, оклад, часы) \u001b[0m");
        TryAndReport(() => new Employee("", "Стажёр", 30000, 20, contract1), 
            "создание с пустым ФИО");
        TryAndReport(() => new Employee("Сидоров С.С.", "Стажёр", 5000, 20, contract1),
            "оклад ниже минимума");
        TryAndReport(() => new Employee("Пивченко Я.З.", "Стажёр", 30000, 80, contract1),
            "80 часов в неделю");

        Console.WriteLine("\n\u001b[33m" + "4. Попытка уволить несуществующего работника \u001b[0m");
        bool fired = dept.Fire(999);
        Console.WriteLine($"Увольнение несуществующего id: успех = {fired}");


        Console.WriteLine("\n\u001b[33m" + "5. Демонстрация неизменяемости контракта\u001b[0m");
        Contract got = e1.GetContract();
        Contract modified = got.WithNewRate(999);
        Console.WriteLine($"Исходный контракт сотрудника: {e1.GetContract()}");
        Console.WriteLine($"Новый объект: {modified}");

        Console.WriteLine("\n\u001b[33m" + "6. Количество созданных сотрудников\u001b[0m");
        // Вызов через название класса, чтобы показать, что счётчик общий
        Console.WriteLine($"Всего создано сотрудников: {Employee.GetEmployeesCreated()}");
    }

    // метод для ловли ошибок при попытке создать новый объект сотрудника 
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
    
}
