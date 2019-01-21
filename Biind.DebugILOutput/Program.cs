using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Biind.DebugILOutput
{
	public class IntegerHolder : IIntegerHolder
	{
		public int Integer { get; set; }
	}

	public class GenericHolder<T>
	{
		public T Value { get; set; }
	}

	public interface IIntegerHolder
	{
		int Integer { get; set; }
	}

	public class Program
	{
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

		static void Main(string[] args)
		{
			new Program().BindingTest();

			var bindAssembly = new BindAssembly();

			var binder = new BindOptions<GenericHolder<int>, IIntegerHolder>()
				.MapProperty
				(
					targetMethodExpression: e => e.Value,
					interfaceMethodExpression: e => e.Integer
				)
				.Build(bindAssembly);

			var myGenericHolder = new GenericHolder<int>();

			var integerHolder = binder.NewBind(myGenericHolder);

			myGenericHolder.Value = 5;

			WriteValues();

			integerHolder.Integer = 8;

			WriteValues();

			Console.ReadLine();

			void WriteValues()
			{
				Console.WriteLine($"Generic Holder: {myGenericHolder.Value}");
				Console.WriteLine($"Integer Holder: {integerHolder.Integer}");
			}
		}
	}

	public interface ILogger
	{
		void WriteLine(string message);

		void Write(string message);
	}

	public class Test
	{
		public Test()
		{
			var console = MakeConsole(new BindAssembly());

			console.WriteLine("Hello, bounnd world!");
		}

		public ILogger MakeConsole(BindAssembly bindAssembly)
		{
			return new BindOptions<object, ILogger>()
				.MapMethod(() => Console.WriteLine(default(string)), e => e.WriteLine(default(string)))
				.MapMethod(() => Console.Write(default(string)), e => e.Write(default(string)))
				.Build(bindAssembly)
				.NewBind(null);
		}

	}
}
