namespace Yahvol.Services
{
	using System;
	using System.Threading.Tasks;
	using Akka.Actor;

	public class Throttle : ReceiveActor
	{
		public Throttle()
		{
			this.ReceiveAsync<Func<Task>>(async startFunction => { await startFunction(); });
		}
	}
}