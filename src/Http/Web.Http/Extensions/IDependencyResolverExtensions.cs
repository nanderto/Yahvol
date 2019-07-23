namespace Yahvol.Web.Http
{
	using System.Collections.Generic;
	using System.Linq;
	using System.Web.Http.Dependencies;

	public static class DependencyResolverExtensions
	{
		public static T GetService<T>(this IDependencyResolver resolver)
		{
			var service = resolver.GetService(typeof(T));

			if (service != null && typeof(T).IsAssignableFrom(service.GetType()))
			{
				return (T)service;
			}

			return default(T);
		}

		public static IEnumerable<T> GetServices<T>(this IDependencyResolver resolver)
		{
			return resolver.GetServices(typeof(T)).OfType<T>();
		}
	}
}
