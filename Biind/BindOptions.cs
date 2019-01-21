using System.Collections.Generic;
using System.Reflection;

namespace Biind
{
	public class BindOptions<TType, TInterface>
	{
		private readonly ICollection<FunctionMapping> _mapFunctions = new List<FunctionMapping>();
		private readonly ICollection<PropertyMapping> _mapProperties = new List<PropertyMapping>();

		public BindSpecifications<TType, TInterface> AsSpecifications()
			=> new BindSpecifications<TType, TInterface>
			(
				functionMappings: new List<FunctionMapping>(_mapFunctions),
				propertyMappings: new List<PropertyMapping>(_mapProperties)
			);

		public void Map(MethodInfo targetMethod, MethodInfo interfaceMethod)
			=> _mapFunctions.Add
			(
				new FunctionMapping
				{
					Target = targetMethod,
					Interface = interfaceMethod
				}
			);

		public void Map(PropertyInfo targetProperty, PropertyInfo interfaceProperty)
			=> _mapProperties.Add
			(
				new PropertyMapping
				{
					Target = targetProperty,
					Interface = interfaceProperty
				}
			);
	}
}