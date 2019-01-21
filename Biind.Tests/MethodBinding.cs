using FluentAssertions;
using System;
using System.IO;
using System.Linq;
using System.Text;
using Xunit;

namespace Biind.Tests
{
	public class Instance
	{
		public class BindSimpleMethod
		{
			public interface IGlue
			{
				void GlueMe();
			}

			public class Gluee
			{
				public bool MethodCalled { get; protected set; }

				public void NeedsGlueing()
				{
					MethodCalled = true;
				}
			}

			[Fact]
			public void Binds()
			{
				var instance = new Gluee();

				var glue = new BindOptions<Gluee, IGlue>()
					.MapMethod(e => e.NeedsGlueing(), e => e.GlueMe())
					.Build(new BindAssembly())

					.NewBind(instance);

				glue.GlueMe();

				instance.MethodCalled
					.Should()
					.BeTrue();
			}
		}

		public class BindParameterizedMethod
		{
			public interface IGlue
			{
				void GlueMe(int value);
			}

			public class Gluee
			{
				public bool MethodCalled { get; protected set; }
				public int WithParameter { get; protected set; }

				public void NeedsGlueing(int parameter)
				{
					MethodCalled = true;
					WithParameter = parameter;
				}
			}

			[Fact]
			public void Binds()
			{
				var instance = new Gluee();

				var glue = new BindOptions<Gluee, IGlue>()
					.MapMethod(e => e.NeedsGlueing(default), e => e.GlueMe(default))
					.Build(new BindAssembly())

					.NewBind(instance);

				const int paramValue = 42178;

				glue.GlueMe(paramValue);

				instance.MethodCalled
					.Should()
					.BeTrue();

				instance.WithParameter
					.Should()
					.Be(paramValue);
			}
		}
	}

	public class Static
	{
		public class BindsSimpleStaticMethod
		{
			public interface IGlue
			{
				void GlueMe();
			}

			public static class Gluee
			{
				public static bool MethodCalled { get; private set; }

				public static void NeedsGlueing()
				{
					MethodCalled = true;
				}
			}

			[Fact]
			public void Binds()
			{
				var glue = new BindOptions<object, IGlue>()
					.MapMethod(() => Gluee.NeedsGlueing(), e => e.GlueMe())
					.Build(new BindAssembly())

					.NewBind(null);

				glue.GlueMe();

				Gluee.MethodCalled
					.Should()
					.BeTrue();
			}
		}

		public class BindsParameterizedStaticMethod
		{
			public interface IGlue
			{
				void GlueMe(int value);
			}

			public static class Gluee
			{
				public static bool MethodCalled { get; private set; }
				public static int WithParameter { get; private set; }

				public static void NeedsGlueing(int value)
				{
					MethodCalled = true;
					WithParameter = value;
				}
			}

			[Fact]
			public void Binds()
			{
				var glue = new BindOptions<object, IGlue>()
					.MapMethod(() => Gluee.NeedsGlueing(default), e => e.GlueMe(default))
					.Build(new BindAssembly())

					.NewBind(null);

				const int paramValue = 984127;

				glue.GlueMe(paramValue);

				Gluee.MethodCalled
					.Should()
					.BeTrue();

				Gluee.WithParameter
					.Should()
					.Be(paramValue);
			}
		}
	}
}
