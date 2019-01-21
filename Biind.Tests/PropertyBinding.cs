using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Biind.Tests
{
	public class BindsGetProperty
	{
		public interface IGlue
		{
			int Property { get; }
		}

		public class Gluee
		{
			public int BackingField = 0;

			public int GlueProperty { get => BackingField; }
		}

		[Fact]
		public void Binds()
		{
			var instance = new Gluee();

			var glue = new BindOptions<Gluee, IGlue>()
				.MapProperty(e => e.GlueProperty, e => e.Property)
				.Build(new BindAssembly())

				.NewBind(instance);

			instance.BackingField = 893;
			TestProperty();

			void TestProperty()
			{
				glue.Property
					.Should()
					.Be(instance.GlueProperty);
			}
		}
	}

	public class BindsSetProperty
	{
		public interface IGlue
		{
			int Property { get; set; }
		}

		public class Gluee
		{
			public int BackingField = 0;

			public int GlueProperty { get => BackingField; set => BackingField = value; }
		}

		[Fact]
		public void Binds()
		{
			var instance = new Gluee();

			var glue = new BindOptions<Gluee, IGlue>()
				.MapProperty(e => e.GlueProperty, e => e.Property)
				.Build(new BindAssembly())

				.NewBind(instance);

			TestProperty(37);

			void TestProperty(int value)
			{
				glue.Property = value;

				instance.GlueProperty
					.Should()
					.Be(value);
			}
		}
	}

	public class BindsStaticGetProperty
	{
		public interface IGlue
		{
			int Property { get; }
		}

		public static class Gluee
		{
			public static int BackingField = 0;

			public static int GlueProperty { get => BackingField; }
		}

		[Fact]
		public void Binds()
		{
			var glue = new BindOptions<object, IGlue>()
				.MapProperty(() => Gluee.GlueProperty, e => e.Property)
				.Build(new BindAssembly())

				.NewBind(null);

			Gluee.BackingField = 893;
			TestProperty();

			void TestProperty()
			{
				glue.Property
					.Should()
					.Be(Gluee.GlueProperty);
			}
		}
	}

	public class BindsStaticSetProperty
	{
		public interface IGlue
		{
			int Property { get; set; }
		}

		public static class Gluee
		{
			public static int BackingField = 0;

			public static int GlueProperty { get => BackingField; set => BackingField = value; }
		}

		[Fact]
		public void Binds()
		{
			var glue = new BindOptions<object, IGlue>()
				.MapProperty(() => Gluee.GlueProperty, e => e.Property)
				.Build(new BindAssembly())

				.NewBind(null);

			TestProperty(37);

			void TestProperty(int value)
			{
				glue.Property = value;

				Gluee.GlueProperty
					.Should()
					.Be(value);
			}
		}
	}
}
