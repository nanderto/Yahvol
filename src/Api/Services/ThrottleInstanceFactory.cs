namespace Yahvol.Services
{
	using System.Configuration;
	using Akka.Actor;
	using Akka.Routing;
	using Yahvol.Data;

	public class ThrottleInstanceFactory : IInstanceFactory<ActorSelection>
	{
		private readonly ActorSystem actorSystem = ActorSystem.Create("ServiceCommandActorSystem");

		private ActorPath actorPath;

		public ActorSelection Create()
		{
			if (this.actorPath == null)
			{
				this.actorPath = this.actorSystem.ActorOf(Props.Create<Throttle>().WithRouter(new RoundRobinPool(this.MaxExecutingSubscribers))).Path;
			}

			return this.actorSystem.ActorSelection(this.actorPath);
		}

		public int MaxExecutingSubscribers { get; set; } = int.Parse(ConfigurationManager.AppSettings.Get("MaxExecutingSubscribers") ?? "1000000");
	}
}