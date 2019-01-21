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
		static void Main(string[] args)
		{
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
}
