using Xunit;

public class VacancyPrototypeTests
{
    [Fact]
    public void Copy_can_change_position_and_salary_without_changing_prototype()
    {
        var prototype = new VacancyTemplate("Разработка", "Внутренние сервисы",
            TestData.Position, TestData.Grade, TestData.WorkFormat, 40, 60000);

        VacancyTemplate copy = prototype.Copy();
        copy.ChangePosition(new Position("Ведущий инженер"));
        copy.ChangeSalary(90000);

        Assert.NotSame(prototype, copy);
        Assert.Equal("Инженер", prototype.Position.Title);
        Assert.Equal(60000, prototype.Salary);
        Assert.Equal("Ведущий инженер", copy.Position.Title);
        Assert.Equal(90000, copy.Salary);
    }

    [Fact]
    public void Prototype_and_copy_can_share_immutable_reference_fields()
    {
        var prototype = new VacancyTemplate("Разработка", "Внутренние сервисы",
            TestData.Position, TestData.Grade, TestData.WorkFormat, 40, 60000);

        VacancyTemplate copy = prototype.Copy();

        Assert.Same(prototype.Grade, copy.Grade);
        Assert.Same(prototype.WorkFormat, copy.WorkFormat);
    }
}
