namespace Biind
{
	public static partial class BindOptionsExtensions
	{
		public static Bind<TType, TInterface> Build<TType, TInterface>
		(
			this BindOptions<TType, TInterface> bindOptions,
			BindAssembly bindAssembly
		)
			=> new Bind<TType, TInterface>
			(
				bindSpecifications: bindOptions.AsSpecifications(),
				bindAssembly: bindAssembly
			);
	}
}