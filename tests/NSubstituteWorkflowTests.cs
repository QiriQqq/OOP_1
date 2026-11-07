using NSubstitute;
using Xunit;

public class NSubstituteWorkflowTests
{
    [Fact]
    public void Seniority_bonus_uses_time_returned_by_substitute()
    {
        var time = Substitute.For<ITimeProvider>();
        var random = Substitute.For<IRandomSource>();
        var storage = Substitute.For<IEmployeeStorage>();
        time.Now.Returns(new DateTime(2025, 3, 10));
        var employee = TestData.Salaried(60000, new NoBonus(), new DateTime(2020, 3, 10));
        var service = new EmployeeWorkflowService(time, random, storage);

        Assert.Equal(6000, service.CalculateSeniorityBonus(employee, 2));
    }

    [Fact]
    public void Random_task_assignment_uses_substitute_value()
    {
        var time = Substitute.For<ITimeProvider>();
        var random = Substitute.For<IRandomSource>();
        var storage = Substitute.For<IEmployeeStorage>();
        random.Next(3).Returns(2);
        var service = new EmployeeWorkflowService(time, random, storage);

        Assert.Equal("Подготовить план", service.AssignTask(new[] { "Позвонить клиенту", "Проверить отчёт", "Подготовить план" }));
        random.Received(1).Next(3);
    }

    [Fact]
    public void Employee_is_saved_exactly_once_with_expected_reference()
    {
        var time = Substitute.For<ITimeProvider>();
        var random = Substitute.For<IRandomSource>();
        var storage = Substitute.For<IEmployeeStorage>();
        var service = new EmployeeWorkflowService(time, random, storage);
        var employee = TestData.Salaried();

        service.RegisterEmployee(employee);

        storage.Received(1).Save(employee);
    }
}
