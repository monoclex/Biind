using System;

namespace Biind
{
	public class Bind<TType, TInterface>
	{
		private static string NewGuid() => Guid.NewGuid().ToString();

		private readonly BindSpecifications<TType, TInterface> _specs;
		private readonly BindAssembly _bindAssembly;

		private readonly Func<TType, TInterface> _newDelegate;
		private readonly Type _generatedType;

		public Bind(BindSpecifications<TType, TInterface> bindSpecifications, BindAssembly bindAssembly)
		{
			_specs = bindSpecifications;
			_bindAssembly = bindAssembly;

			_generatedType = RuntimeTypeCreationLogic.Build
			(
				_bindAssembly.DefineType
				(
					interfaces: new[] { typeof(TInterface) }
				),
				_specs
			);

			_newDelegate = RuntimeTypeCreationLogic.CreateFactory<TType, TInterface>
			(
				returnType: typeof(TInterface),
				parameterTypes: new[] { typeof(TType) },
				constructor: _generatedType.GetConstructor(new[] { typeof(TType) })
			);
		}

		public TInterface NewBind(TType instance) => _newDelegate(instance);
	}
}