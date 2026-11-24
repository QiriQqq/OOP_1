class Program
{
    static void Main()
    {
        // разовое создание payrollSettings
        IPayrollSettings payrollSettings = new PayrollSettings(0.13m, 0.03m);
        var payrollCalculator = new PayrollCalculator(payrollSettings);
        IEmployeeFactory employeeFactory = new EmployeeFactory();

        var engineer = new Position("Инженер");
        var analyst = new Position("Аналитик");
        var middle = new Grade("Middle", 2);
        var junior = new Grade("Junior", 1);
        var office = new WorkFormat("Офис", false);
        var remote = new WorkFormat("Удалённый", true);
        var hireDate = new DateTime(2022, 4, 1);


        Console.WriteLine("\u001b[32m1. Зависимость от настроек налогов через интерфейс\u001b[0m");
        PayrollCalculation calculation = payrollCalculator.Calculate(60000m);
        Console.WriteLine($"Начислено: {calculation.GrossPayment:0.00}; удержания: " +
            $"{calculation.IncomeTax + calculation.SocialContribution:0.00}; к выплате: {calculation.NetPayment:0.00}");


        Console.WriteLine("\u001b[32m\n2. Фабрика сотрудников\u001b[0m");
        Employee salaried = employeeFactory.Create(EmployeeTypes.Salaried,
            new EmployeeCreationData("Иванов Иван Иванович", engineer, middle, office,
                40, hireDate, "Окладный", 500, new PercentBonus(5), salary: 60000));
        Employee hourly = employeeFactory.Create(EmployeeTypes.Hourly,
            new EmployeeCreationData("Петров Пётр Петрович", analyst, junior, remote,
                35, hireDate, "Почасовой", 500, new NoBonus()));
        Employee contractor = employeeFactory.Create(EmployeeTypes.Contractor,
            new EmployeeCreationData("Сидоров Сидор Сидорович", engineer, middle, remote,
                30, hireDate, "Подрядчик", 500, new NoBonus(), actAmounts: new[] { 20000d, 15000d }));
        Employee piecework = employeeFactory.Create(EmployeeTypes.Piecework,
            new EmployeeCreationData("Орлов Олег Олегович", engineer, junior, office,
                40, hireDate, "Сдельный", 500, new NoBonus(), ratePerUnit: 250, unitsProduced: 120));
        foreach (Employee employee in new[] { salaried, hourly, contractor, piecework })
            Console.WriteLine($"{employee.FullName}: {employee.GetType().Name}, выплата {employee.CalculatePayment():0.00}");

        TryAndReport(() => employeeFactory.Create("67", new EmployeeCreationData(
            "Тестовый Сотрудник", engineer, junior, office, 40, hireDate,
            "Неизвестный", 500, new NoBonus())), "неизвестный тип занятости");


        Console.WriteLine("\u001b[32m\n3. Строитель трудового договора\u001b[0m");
        // текучий интерфейс
        EmploymentAgreement agreement = new EmploymentAgreementBuilder()
            .ForEmployee(salaried)
            .ForPosition(engineer)
            .WithSalary(60000m)
            .WithWorkSchedule("5/2, 09:00–18:00")
            .WithProbationPeriod(3)
            .AddAllowance("Питание", 3000m)
            .AddAllowance("Транспорт", 1500m)
            .Build();
        Console.WriteLine($"{agreement.EmployeeFullName}, должность: {agreement.Position}, " +
            $"оклад: {agreement.Salary:0.00}, график: {agreement.WorkSchedule}, " +
            $"испытательный срок: {agreement.ProbationPeriodMonths} мес., надбавки: {agreement.TotalAllowances:0.00}");
        TryAndReport(() => new EmploymentAgreementBuilder()
            .ForEmployee(salaried).ForPosition(engineer).WithWorkSchedule("5/2")
            .WithProbationPeriod(3).Build(), "сборка договора без оклада");


        Console.WriteLine("\u001b[32m\n4. Прототип вакансии\u001b[0m");
        var vacancyPrototype = new VacancyTemplate("Разработка", "Разработка внутренних сервисов",
            engineer, middle, remote, 40, 65000);
        VacancyTemplate vacancyCopy = vacancyPrototype.Copy();
        vacancyCopy.ChangePosition(new Position("Ведущий инженер"));
        vacancyCopy.ChangeSalary(90000);
        Console.WriteLine($"Образец: {vacancyPrototype.Position}, {vacancyPrototype.Salary:0.00}");
        Console.WriteLine($"Копия:   {vacancyCopy.Position}, {vacancyCopy.Salary:0.00}");


        Console.WriteLine("\u001b[32m\n5.1. Пул и измерение для 1000 сотрудников\u001b[0m");
        var employeesForBenchmark = new List<Employee>(PayrollPoolBenchmark.EmployeesPerRound);
        for (int i = 0; i < PayrollPoolBenchmark.EmployeesPerRound; i++)
        {
            employeesForBenchmark.Add(employeeFactory.Create(EmployeeTypes.Salaried,
                new EmployeeCreationData($"Сотрудник {i + 1}", engineer, middle, office,
                    40, hireDate, "Окладный", 500, new NoBonus(), salary: 40000 + (i % 10) * 1000)));
        }

        PayrollBenchmarkResult benchmark = PayrollPoolBenchmark.Run(employeesForBenchmark, payrollSettings);
        Console.WriteLine($"Без пула: {benchmark.WithoutPoolMilliseconds:0.000} мс на 1000 сотрудников");
        Console.WriteLine($"С пулом:  {benchmark.WithPoolMilliseconds:0.000} мс на 1000 сотрудников");
        Console.WriteLine($"Разница:  {benchmark.PoolSavingsPercent:+0.00;-0.00;0.00}% (положительное значение — пул быстрее)");
        Console.WriteLine(benchmark.PoolIsWorthwhile
            ? "Вывод: в этом запуске пул дал преимущество не менее 5%; повторите замер несколько раз."
            : "Вывод: преимущество пула не достигло 5%; для такой нагрузки усложнение может быть не оправдано.");


        Console.WriteLine("\u001b[32m\n5.2 Демонстрация очистки объекта в пуле\u001b[0m");
        var pool = new PayrollSlipPool();
        PayrollSlip slip = pool.Rent();
        slip.Calculate(salaried, payrollSettings);
        Console.WriteLine($"До возврата: {slip.EmployeeName}, к выплате {slip.NetPayment:0.00}");
        pool.Return(slip);
        PayrollSlip reused = pool.Rent();
        Console.WriteLine($"После повторной выдачи: имя='{reused.EmployeeName}', начислено {reused.GrossPayment:0.00}");
        pool.Return(reused);
    }

    private static void TryAndReport(Action action, string description)
    {
        try
        {
            action();
            Console.WriteLine($"Ошибка: {description} не была отклонена");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ожидаемое отклонение ({description}): {ex.Message}");
        }
    }
}
