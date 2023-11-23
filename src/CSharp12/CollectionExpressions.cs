using System.Collections.Concurrent;
using System.Collections.Immutable;
using System.Collections.ObjectModel;

public static class CollectionExpressions
{
    public static void Demo()
    {
        Console.WriteLine("Initialise variables with a type:");
        Console.WriteLine();

        // Collection expressions create the correct target type
        int[] array = [1, 2, 3];
        List<int> list = [4, 5, 6];

        PrintValues("Array", array);
        PrintValues("List", list);

        Console.WriteLine();
        Console.WriteLine("Implicit type:");
        Console.WriteLine();

        // An appropriate type is created to match the method signature
        PrintValues("Enumerable", [7, 8, 9]);
        PrintNumbers("Empty array", []);

        // Type parameter is required where the type is not obvious
        PrintValues<int>("Empty enumerable", []);

        // The appropriate type changes from int[] to List<int> because it needs to be mutable
        AddToCollectionAndPrint("Collection", [7, 8, 9]);

        Console.WriteLine();
        Console.WriteLine("Spread:");
        Console.WriteLine();

        // The spread operator (..) is used to create a collection containing values from other collections
        PrintValues("Spread two sets", [..array, ..list]);
        PrintValues("Spread with literals", [0, ..array, ..list, 7]);

        Console.WriteLine();

        PrintValues("Spread with slice", [..array[..2]]);
        PrintValues("Spread with slice", [..array[1..]]);

        Console.WriteLine();
        Console.WriteLine("Creates target type to match the method signature:");
        Console.WriteLine();

        // With the same syntax, the right type is always created
        PrintArray([1, 2, 3]);
        PrintBlockingCollection([1, 2, 3]);
        PrintCollection([1, 2, 3]);
        PrintConcurrentBag([1, 2, 3]);
        PrintHashSet([1, 2, 3]);
        PrintImmutableArray([1, 2, 3]);
        PrintList([1, 2, 3]);
        PrintSpan([1, 2, 3]);
        PrintReadOnlySpan([1, 2, 3]);

        Console.WriteLine();

        // Collection interfaces are also supported
        PrintReadOnlyCollection([1, 2, 3]);
        PrintReadOnlyList([1, 2, 3]);

        Console.WriteLine();

        PrintICollection([1, 2, 3]);
        PrintIList([1, 2, 3]);

        Console.WriteLine();

        // Custom collection types are supported too
        PrintSomeNumbers([1, 2, 3]);

        Console.WriteLine();

        // Dictionaries are not yet supported, except for when they are empty
        // Future syntax such as ["Kirk": "Captain", "Spock": "Lieutenant"] is proposed for dictionaries
        PrintDictionary("Empty dictionary", []);
        PrintCrewMembers([]);

        Console.WriteLine();
    }

    public class SomeNumbers : List<int>
    {
    }

    public class CrewMembers : Dictionary<string, string>
    {
    }

    static void PrintValues<T>(string prefix, IEnumerable<T> values)
        => Console.WriteLine($"\u001b[32m{prefix}\u001b[0m: [{string.Join(", ", values)}] (\u001b[35m{values.GetType().Name}\u001b[0m)");

    static void AddToCollectionAndPrint<T>(string prefix, ICollection<T?> values)
    {
        values.Add(default);
        PrintValues(prefix, values);
    }

    static void PrintNumbers(string prefix, IEnumerable<int> values) => PrintValues(prefix, values);

    static void PrintArray<T>(T[] value) => PrintValues("T[]", value);
    static void PrintBlockingCollection<T>(BlockingCollection<T> value) => PrintValues("BlockingCollection<T>", value);
    static void PrintCollection<T>(Collection<T> value) => PrintValues("Collection<T>", value);
    static void PrintConcurrentBag<T>(ConcurrentBag<T> value) => PrintValues("ConcurrentBag<T>", value);
    static void PrintHashSet<T>(HashSet<T> value) => PrintValues("HashSet<T>", value);
    static void PrintICollection<T>(ICollection<T> value) => PrintValues("ICollection<T>", value);
    static void PrintIList<T>(IList<T> value) => PrintValues("IList<T>", value);
    static void PrintImmutableArray<T>(ImmutableArray<T> value) => PrintValues("ImmutableArray<T>", value);
    static void PrintList<T>(List<T> value) => PrintValues("List<T>", value);
    static void PrintReadOnlyCollection<T>(IReadOnlyCollection<T> value) => PrintValues("IReadOnlyCollection<T>", value);
    static void PrintReadOnlyList<T>(IReadOnlyList<T> value) => PrintValues("IReadOnlyList<T>", value);
    static void PrintSomeNumbers(SomeNumbers value) => PrintValues("SomeNumbers", value);

    static void PrintReadOnlySpan<T>(ReadOnlySpan<T> values)
    {
        Console.Write("\u001b[32mReadOnlySpan<T>\u001b[0m: [");

        for (int i = 0; i < values.Length; i++)
        {
            Console.Write(values[i]);

            if (i < values.Length - 1)
            {
                Console.Write(", ");
            }
        }

        Console.WriteLine($"] (\u001b[35mReadOnlySpan<{typeof(T).Name}>\u001b[0m)");
    }

    static void PrintSpan<T>(Span<T> values)
    {
        Console.Write("\u001b[32mSpan<T>\u001b[0m: [");

        for (int i = 0; i < values.Length; i++)
        {
            Console.Write(values[i]);

            if (i < values.Length - 1)
            {
                Console.Write(", ");
            }
        }

        Console.WriteLine($"] (\u001b[35mSpan<{typeof(T).Name}>\u001b[0m)");
    }

    static void PrintDictionary(string prefix, Dictionary<string, string> values)
    {
        Console.WriteLine($"\u001b[32m{prefix}\u001b[0m (\u001b[35mCount: {values.Count}, {values.GetType().Name}\u001b[0m)");
        foreach ((var key, var value) in values)
        {
            Console.WriteLine($" [{key}] = {value}");
        }
    }

    static void PrintCrewMembers(CrewMembers values) => PrintDictionary("CrewMembers", values);
}
