namespace Yahvol.Data
{
	public interface IInstanceFactory<out T>
	{
		T Create();
	}
}