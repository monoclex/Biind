using System.Reflection;

namespace Biind
{
	public static partial class BindOptionsExtensions
	{
		public static BindOptions<TType, TInterface> MapChain<TType, TInterface>
		(
			this BindOptions<TType, TInterface> bindOptions,
			MethodInfo targetMethod,
			MethodInfo interfaceMethod
		)
		{
			bindOptions.Map
			(
				targetMethod,
				interfaceMethod
			);

			return bindOptions;
		}

		public static BindOptions<TType, TInterface> MapChain<TType, TInterface>
		(
			this BindOptions<TType, TInterface> bindOptions,
			PropertyInfo targetProperty,
			PropertyInfo interfaceProperty
		)
		{
			bindOptions.Map
			(
				targetProperty,
				interfaceProperty
			);

			return bindOptions;
		}
	}
}