public static class TestData
{
    public static Position Position => new Position("Инженер");
    public static Grade Grade => new Grade("Middle", 2);
    public static WorkFormat WorkFormat => new WorkFormat("Офис", false);
    public static DateTime HireDate => new DateTime(2020, 1, 1);
    public static IBonusPolicy NoBonus => new NoBonus();

    public static SalariedEmployee Salaried(double salary = 60000, IBonusPolicy? bonus = null, DateTime? hireDate = null)
    {
        return new SalariedEmployee("Иванов Иван Иванович", Position, Grade, WorkFormat,
            40, hireDate ?? HireDate, "Полная", 500, salary, bonus ?? NoBonus);
    }

    public static HourlyEmployee Hourly(int hours = 40, IBonusPolicy? bonus = null)
    {
        return new HourlyEmployee("Петров Пётр Петрович", Position, Grade, WorkFormat,
            hours, HireDate, "Полная", 500, bonus ?? NoBonus);
    }
}
