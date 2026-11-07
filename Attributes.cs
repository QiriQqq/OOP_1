public class Position
{
    public string Title { get; }

    public Position(string title)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Название должности не может быть пустым");
        Title = title;
    }

    public override string ToString() => Title;
}

public class Grade
{
    public string Name { get; }
    public int Level { get; }

    public Grade(string name, int level)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Название грейда не может быть пустым");
        if (level < 1)
            throw new ArgumentException("Уровень грейда должен быть не меньше 1");
        Name = name;
        Level = level;
    }

    public override string ToString() => $"{Name} (ур. {Level})";
}


public class WorkFormat
{
    public string Name { get; }
    public bool IsRemote { get; }

    public WorkFormat(string name, bool isRemote)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Название формата работы не может быть пустым");
        Name = name;
        IsRemote = isRemote;
    }

    public override string ToString() => Name;
}
