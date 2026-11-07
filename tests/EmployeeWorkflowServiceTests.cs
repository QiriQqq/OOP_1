using Xunit;

public class EmployeeWorkflowServiceTests
{
    [Fact]
    public void Seniority_bonus_uses_injected_time_and_completed_years()
    {
        var employee = TestData.Salaried(60000, new NoBonus(), new DateTime(2020, 3, 10));
        var service = new EmployeeWorkflowService(
            new FixedTimeProvider(new DateTime(2025, 3, 10)),
            new FixedRandomSource(0),
            new InMemoryEmployeeStorage());

        Assert.Equal(6000, service.CalculateSeniorityBonus(employee, 2));
    }

    [Fact]
    public void Seniority_bonus_does_not_count_incomplete_year()
    {
        var employee = TestData.Salaried(60000, new NoBonus(), new DateTime(2020, 3, 11));
        var service = new EmployeeWorkflowService(
            new FixedTimeProvider(new DateTime(2025, 3, 10)),
            new FixedRandomSource(0),
            new InMemoryEmployeeStorage());

        Assert.Equal(4800, service.CalculateSeniorityBonus(employee, 2));
    }

    [Fact]
    public void Random_task_assignment_returns_preselected_task()
    {
        var service = new EmployeeWorkflowService(
            new FixedTimeProvider(new DateTime(2025, 1, 1)),
            new FixedRandomSource(1),
            new InMemoryEmployeeStorage());

        Assert.Equal("Проверить отчёт", service.AssignTask(new[] { "Позвонить клиенту", "Проверить отчёт", "Подготовить план" }));
    }

    [Fact]
    public void Employee_registration_is_saved_in_memory_storage()
    {
        var storage = new InMemoryEmployeeStorage();
        var service = new EmployeeWorkflowService(
            new FixedTimeProvider(new DateTime(2025, 1, 1)),
            new FixedRandomSource(0),
            storage);
        var employee = TestData.Salaried();

        service.RegisterEmployee(employee);

        Assert.Single(storage.GetAll());
        Assert.Same(employee, storage.GetAll()[0]);
    }

    [Fact]
    public void Assigning_task_from_empty_list_is_rejected()
    {
        var service = new EmployeeWorkflowService(
            new FixedTimeProvider(new DateTime(2025, 1, 1)),
            new FixedRandomSource(0),
            new InMemoryEmployeeStorage());

        Assert.Throws<ArgumentException>(() => service.AssignTask(Array.Empty<string>()));
    }
}
