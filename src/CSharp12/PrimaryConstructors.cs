public static class PrimaryConstructors
{
    public static void Demo()
    {
        var person11 = new Presenter11("Eli", true);
        person11.StopPresenting();

        var person12 = new Presenter12("Martin");
        person12.Present();

        Console.WriteLine($"Is {person11.Name} presenting? {person11.IsPresenting}");
        Console.WriteLine($"Is {person12.Name} presenting? {person12.IsPresenting}");
    }
}

public class Person11
{
    public Person11(string name)
    {
        Name = name;
    }

    public string Name { get; }
}

public class Presenter11 : Person11
{
    public Presenter11(string name)
        : this(name, false)
    {
    }

    public Presenter11(string name, bool isPresenting)
        : base(name)
    {
        IsPresenting = isPresenting;
    }

    public bool IsPresenting { get; private set; }

    public void Present() => IsPresenting = true;

    public void StopPresenting() => IsPresenting = false;
}

public class Person12(string name)
{
    public string Name { get; } = name;
}

public class Presenter12(string name, bool isPresenting) : Person12(name)
{
    public Presenter12(string name)
        : this(name, false)
    {
    }

    public bool IsPresenting => isPresenting;

    public void Present() => isPresenting = true;

    public void StopPresenting() => isPresenting = false;
}
