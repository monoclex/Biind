using System.Collections.Generic;
using System.Reflection;

namespace Biind
{
	public class BindSpecifications<TType, TInterface>
	{
		public BindSpecifications
		(
			ICollection<FunctionMapping> functionMappings,
			ICollection<PropertyMapping> propertyMappings
		)
		{
			FunctionMappings = functionMappings;
			PropertyMappings = propertyMappings;
		}

		public ICollection<FunctionMapping> FunctionMappings { get; }
		public ICollection<PropertyMapping> PropertyMappings { get; }
	}

	public class FunctionMapping : IMapping<MethodInfo>
	{
		public MethodInfo Target { get; set; }
		public MethodInfo Interface { get; set; }
	}

	public class PropertyMapping : IMapping<PropertyInfo>
	{
		public PropertyInfo Target { get; set; }
		public PropertyInfo Interface { get; set; }
	}

	internal interface IMapping<T>
	{
		T Target { get; set; }
		T Interface { get; set; }
	}

	internal static class MappingExtensions
	{
		public static void Deconstruct<T>(this IMapping<T> mapping, out T target, out T @interface)
		{
			@target = mapping.Target;
			@interface = mapping.Interface;
		}
	}
}