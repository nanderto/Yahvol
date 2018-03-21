namespace Yahvol.Data
{
	using System;

	internal class DefaultConstructorInstanceFactory<T> : IInstanceFactory<T>
	{
		public T Create()
		{
			return Activator.CreateInstance<T>();
		}
	}
}
