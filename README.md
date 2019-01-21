# Biind

![Version][badge_nuget_version] ![Downloads][badge_nuget_downloads]

Bind classes to interfaces, enabling more polymorphic inheritence and extensability of code you don't own, or making using legacy code slightly more enjoyable.

> PM> Package-Install Biind

## Examples

### Two different classes to the same interface
```cs
public class A
{
    public int MyValue { get; set; }
}

public class B
{
    public int StoredValue { get; set; }
}

public interface IIntegerValue
{
    int Value { get; set; }
}

public Bind<A, IIntegerValue> MakeABinder(BindAssembly bindAssembly)
	=> new BindOptions<A, IIntegerValue>()
	.MapProperty(e => e.MyValue, e => e.Value)
	.Build(bindAssembly);

public Bind<B, IIntegerValue> MakeBBinder(BindAssembly bindAssembly)
	=> new BindOptions<B, IIntegerValue>()
	.MapProperty(e => e.StoredValue, e => e.Value)
	.Build(bindAssembly);

public void BindingTest()
{
	var assembly = new BindAssembly();

	var aBinder = MakeABinder(assembly);
	var bBinder = MakeBBinder(assembly);

	var a = new A();
	var b = new B();

	a.MyValue = 8;
	b.StoredValue = 4;

	var boundA = aBinder.NewBind(a);
	var boundB = bBinder.NewBind(b);

	Console.WriteLine(boundA.Value); // 8
	Console.WriteLine(boundB.Value); // 4

	boundA.Value = 6;
	boundB.Value = 6;

	Console.WriteLine(a.MyValue); // 6
	Console.WriteLine(b.StoredValue); // 6
}
```

### Binding Console to an interface
```cs
public interface ILogger
{
    void WriteLine(string message);
    
    void Write(string message);
}

public ILogger MakeConsole(BindAssembly bindAssembly)
{
    return new BindOptions<object, ILogger>()
        .MapMethod(() => Console.WriteLine(default(string)), e => e.WriteLine(default(string)))
        .MapMethod(() => Console.Write(default(string)), e => e.Write(default(string)))
        .Build(bindAssembly)
        .NewBind(null);
}

var console = MakeConsole(new BindAssembly());
```

[badge_nuget_version]: https://img.shields.io/nuget/vpre/Biind.svg
[badge_nuget_downloads]: https://img.shields.io/nuget/dt/Biind.svg