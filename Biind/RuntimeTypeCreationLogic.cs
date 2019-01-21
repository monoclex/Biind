using System;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace Biind
{
	internal static class RuntimeTypeCreationLogic
	{
		public static ConstructorInfo ObjectConstructor { get; } = typeof(object).GetConstructor(Type.EmptyTypes);

		/// <summary>
		/// Creates a factory that feeds in a TInput to the ctor specified, and returns a TOutput
		/// </summary>
		public static Func<TInput, TOutput> CreateFactory<TInput, TOutput>
		(
			Type returnType,
			Type[] parameterTypes,
			ConstructorInfo constructor
		)
		{
			var dynMethod = new DynamicMethod
			(
				name: string.Empty,
				returnType: returnType,
				parameterTypes: parameterTypes
			);

			var ilgen = dynMethod.GetILGenerator();

			ilgen.Emit(OpCodes.Ldarg_0);
			ilgen.Emit(OpCodes.Newobj, constructor);
			ilgen.Emit(OpCodes.Ret);

			return (Func<TInput, TOutput>)dynMethod.CreateDelegate(typeof(Func<TInput, TOutput>)); ;
		}

		public static Type Build<TType, TInterface>
		(
			TypeBuilder typeBuilder,
			BindSpecifications<TType, TInterface> bindSpecifications
		)
		{
			var instance = typeBuilder.DefineField("_instance", typeof(TType), FieldAttributes.Private);

			var ctor = CreateConstructor<TType>(typeBuilder, instance);

			foreach (var (targetMethod, interfaceMethod) in bindSpecifications.FunctionMappings)
			{
				BindMethod
				(
					typeBuilder: typeBuilder,
					instance: instance,
					targetMethod: targetMethod,
					interfaceMethod: interfaceMethod
				);
			}

			foreach (var (targetProperty, interfaceProperty) in bindSpecifications.PropertyMappings)
			{
				BindProperty
				(
					typeBuilder: typeBuilder,
					instance: instance,
					targetProperty: targetProperty,
					interfaceProperty: interfaceProperty
				);
			}

			return
#if NET472
				typeBuilder.CreateType();
#else
				typeBuilder.CreateTypeInfo().AsType();
#endif
		}

		private static ConstructorBuilder CreateConstructor<TType>(TypeBuilder typeBuilder, FieldBuilder instanceField)
		{
			var ctor = typeBuilder.DefineConstructor(MethodAttributes.Public, CallingConventions.Standard, new Type[] { typeof(TType) });
			var ctorIl = ctor.GetILGenerator();

			ctorIl.Emit(OpCodes.Ldarg_0);
			ctorIl.Emit(OpCodes.Call, ObjectConstructor);

			ctorIl.Emit(OpCodes.Ldarg_0);
			ctorIl.Emit(OpCodes.Ldarg_1);
			ctorIl.Emit(OpCodes.Stfld, instanceField);
			ctorIl.Emit(OpCodes.Ret);

			return ctor;
		}

		private static void BindMethod
		(
			TypeBuilder typeBuilder,
			FieldBuilder instance,
			MethodInfo targetMethod,
			MethodInfo interfaceMethod
		)
		{
			var methodBuilder = typeBuilder.DefineMethod
			(
				name: interfaceMethod.Name,
				attributes: interfaceMethod.Attributes & ~MethodAttributes.Abstract,
				returnType: interfaceMethod.ReturnType,
				parameterTypes: interfaceMethod.GetParameters().Select(x => x.ParameterType).ToArray()
			);

			var ilg = methodBuilder.GetILGenerator();

			if (!targetMethod.IsStatic)
			{
				ilg.Emit(OpCodes.Ldarg_0);
				ilg.Emit(OpCodes.Ldfld, instance);
			}

			foreach (var parameter in targetMethod.GetParameters())
			{
				ilg.Emit(OpCodes.Ldarg, parameter.Position + 1);
			}

			var callOpcode = OpCodes.Callvirt;

			if (targetMethod.IsStatic)
			{
				callOpcode = OpCodes.Call;
			}

			ilg.Emit(callOpcode, targetMethod);
			ilg.Emit(OpCodes.Ret);
		}

		private static void BindProperty
		(
			TypeBuilder typeBuilder,
			FieldBuilder instance,
			PropertyInfo targetProperty,
			PropertyInfo interfaceProperty
		)
		{
			var propertyBuilder = typeBuilder.DefineProperty
			(
				name: interfaceProperty.Name,
				attributes: interfaceProperty.Attributes,
				returnType: interfaceProperty.PropertyType,
				parameterTypes: null
			);

			if (interfaceProperty.CanRead)
			{
				var get = MakeBoundGetMethod(typeBuilder, interfaceProperty, instance, targetProperty);

				propertyBuilder.SetGetMethod(get);

				typeBuilder.DefineMethodOverride(get, interfaceProperty.GetGetMethod());
			}

			if (interfaceProperty.CanWrite)
			{
				var set = MakeBoundSetMethod(typeBuilder, interfaceProperty, instance, targetProperty);

				propertyBuilder.SetSetMethod(set);

				typeBuilder.DefineMethodOverride(set, interfaceProperty.GetSetMethod());
			}
		}

		private static MethodBuilder MakeBoundGetMethod
		(
			TypeBuilder typeBuilder,
			PropertyInfo interfaceProperty,
			FieldBuilder instance,
			PropertyInfo targetProperty
		)
		{
			var get = typeBuilder.DefineMethod
			(
				name: $"get_{interfaceProperty.Name}",
				attributes: MethodAttributes.Public | MethodAttributes.Virtual | MethodAttributes.HideBySig | MethodAttributes.SpecialName,
				returnType: interfaceProperty.PropertyType,
				parameterTypes: Type.EmptyTypes
			);

			var il = get.GetILGenerator();

			var callOpcode = OpCodes.Call;

			if (!targetProperty.IsStatic())
			{
				callOpcode = OpCodes.Callvirt;

				il.Emit(OpCodes.Ldarg_0);
				il.Emit(OpCodes.Ldfld, instance);
			}

			il.Emit(callOpcode, targetProperty.GetGetMethod());
			il.Emit(OpCodes.Ret);

			typeBuilder.DefineMethodOverride
			(
				methodInfoBody: get,
				methodInfoDeclaration: interfaceProperty.GetGetMethod()
			);

			return get;
		}

		private static MethodBuilder MakeBoundSetMethod
				(
			TypeBuilder typeBuilder,
			PropertyInfo interfaceProperty,
			FieldBuilder instance,
			PropertyInfo typeProperty
		)
		{
			var set = typeBuilder.DefineMethod
			(
				name: $"set_{interfaceProperty.Name}",
				attributes: MethodAttributes.Public | MethodAttributes.Virtual | MethodAttributes.HideBySig | MethodAttributes.SpecialName,
				returnType: typeof(void),
				parameterTypes: new Type[] { interfaceProperty.PropertyType }
			);

			var il = set.GetILGenerator();

			var callOpcode = OpCodes.Call;

			if (!typeProperty.IsStatic())
			{
				callOpcode = OpCodes.Callvirt;

				il.Emit(OpCodes.Ldarg_0);
				il.Emit(OpCodes.Ldfld, instance);
			}

			il.Emit(OpCodes.Ldarg_1);
			il.Emit(callOpcode, typeProperty.GetSetMethod());
			il.Emit(OpCodes.Ret);
			return set;
		}
	}
}