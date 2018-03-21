namespace Yahvol.Services
{
	using System.Threading;
    using System.Threading.Tasks;

    public interface IStarter
    {
		Task StartAsync();

		Task<Subscriber> RunAsync(CancellationToken cancellationToken);
	}
}