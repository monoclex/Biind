using System.Reflection;

namespace Biind
{
	internal static class PropertyExtensions
	{
		public static bool IsStatic(this PropertyInfo property)
			=> (property.GetMethod ?? property.SetMethod).IsStatic;
	}
}