using System;
using System.Linq.Expressions;
using System.Reflection;

namespace Biind
{
	public static partial class BindOptionsExtensions
	{
		public static BindOptions<TType, TInterface> MapMethod<TType, TInterface>
		(
			this BindOptions<TType, TInterface> bindOptions,
			Expression<Action<TType>> targetMethodExpression,
			Expression<Action<TInterface>> interfaceMethodExpression
		)
			=> bindOptions
			.MapMethod
			(
				targetMethodExpression,
				(LambdaExpression)interfaceMethodExpression
			);

		public static BindOptions<TType, TInterface> MapMethod<TType, TInterface>
		(
			this BindOptions<TType, TInterface> bindOptions,
			Expression<Action> targetMethodExpression,
			Expression<Action<TInterface>> interfaceMethodExpression
		)
			=> bindOptions
			.MapMethod
			(
				targetMethodExpression,
				(LambdaExpression)interfaceMethodExpression
			);

		private static BindOptions<TType, TInterface> MapMethod<TType, TInterface>
		(
			this BindOptions<TType, TInterface> bindOptions,
			LambdaExpression targetMethodExpression,
			LambdaExpression interfaceMethodExpression
		)
			=> bindOptions.MapChain
			(
				targetMethod: targetMethodExpression.FetchMethodCall(),
				interfaceMethod: interfaceMethodExpression.FetchMethodCall()
			);

		private static MethodInfo FetchMethodCall(this LambdaExpression expression)
			=> (expression.Body as MethodCallExpression).Method;
	}
}