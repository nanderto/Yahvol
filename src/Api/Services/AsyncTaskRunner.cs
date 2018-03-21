namespace Yahvol.Services
{
	using System.Collections.Generic;
	using System.Threading.Tasks;
	using Akka.Actor;

	public class AsyncTaskRunner
	{
		private readonly IEnumerable<Subscriber> subscribers;

		private readonly ActorSelection throttle;

		public AsyncTaskRunner(IEnumerable<Subscriber> subscribers, ActorSelection throttle = null)
		{
			this.subscribers = subscribers;
			this.throttle = throttle;
		}

		public void StartTasks()
		{
			foreach (var subscriber in this.subscribers)
			{
				var restarter = new ReStarter(subscriber, this.throttle);
				Task.Run(async () => await restarter.StartAsync());
			}
		}
	}
}

