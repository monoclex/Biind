using System;
using System.Linq.Expressions;
using System.Reflection;

namespace Biind
{
	public static partial class BindOptionsExtensions
	{
		public static BindOptions<TType, TInterface> MapProperty<TType, TInterface, TResult>
		(
			this BindOptions<TType, TInterface> bindOptions,
			Expression<Func<TType, TResult>> targetMethodExpression,
			Expression<Func<TInterface, TResult>> interfaceMethodExpression
		)
			=> bindOptions.MapProperty<TType, TInterface, TResult>
			(
				targetMethodExpression,
				(LambdaExpression)interfaceMethodExpression
			);

		public static BindOptions<TType, TInterface> MapProperty<TType, TInterface, TResult>
		(
			this BindOptions<TType, TInterface> bindOptions,
			Expression<Func<TResult>> targetMethodExpression,
			Expression<Func<TInterface, TResult>> interfaceMethodExpression
		)
			=> bindOptions.MapProperty<TType, TInterface, TResult>
			(
				targetMethodExpression,
				(LambdaExpression)interfaceMethodExpression
			);

		private static BindOptions<TType, TInterface> MapProperty<TType, TInterface, TResult>
		(
			this BindOptions<TType, TInterface> bindOptions,
			LambdaExpression targetMethodExpression,
			LambdaExpression interfaceMethodExpression
		)
			=> bindOptions.MapChain
			(
				targetProperty: targetMethodExpression.FetchMemberInfo<PropertyInfo>(),
				interfaceProperty: interfaceMethodExpression.FetchMemberInfo<PropertyInfo>()
			);

		private static T FetchMemberInfo<T>(this LambdaExpression lambdaExpression)
			where T : MemberInfo
			=> (lambdaExpression.Body as MemberExpression).Member as T;
	}
}