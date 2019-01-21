using System;
using System.Reflection;
using System.Reflection.Emit;
using System.Threading;

namespace Biind
{
	public class BindAssembly
	{
		private static string NewGuid() => Guid.NewGuid().ToString();

		private readonly ModuleBuilder _module;
		private readonly AssemblyBuilder _assemblyBuilder;
		private readonly AssemblyName _assemblyName;

		public BindAssembly()
		{
			_assemblyName = new AssemblyName(NewGuid());

			// yeah this is code duplication to an extent,
			// but honestly, the readability is much better this way.
			// i doubt it's changing soon either.
#if NET472
            _assemblyBuilder = Thread.GetDomain() // ms docs use this over AppDomain.CurrentDomain
                .DefineDynamicAssembly
                (
                    name: _assemblyName,
                    access: AssemblyBuilderAccess.RunAndSave
                );
#else
			_assemblyBuilder = AssemblyBuilder
				.DefineDynamicAssembly
				(
					name: _assemblyName,
					access: AssemblyBuilderAccess.Run
				);
#endif

			_module = _assemblyBuilder.DefineDynamicModule
			(
				name: _assemblyName.Name
			);
		}

		public TypeBuilder DefineType
		(
			Type[] interfaces,
			string name = null,
			TypeAttributes typeAttributes =
				TypeAttributes.Public |
				TypeAttributes.Class,
			Type parent = null
		)
			=> _module.DefineType
			(
				name: name ?? NewGuid(),
				attr: typeAttributes,
				parent: parent,
				interfaces: interfaces
			);
	}
}