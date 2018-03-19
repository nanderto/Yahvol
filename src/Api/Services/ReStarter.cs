// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ReStarter.cs" company="Anderton Engineered Solutions">
// Copyright © 2014 All Rights Reserved
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Yahvol.Services
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Akka.Actor;

	public class ReStarter : IStarter
    {
        private readonly Subscriber subscriber;

		private readonly ActorSelection throttle;

		public ReStarter(Subscriber subscriber, ActorSelection throttle = null)
        {
            this.subscriber = subscriber;
			this.throttle = throttle;
		}
	
		public int InitialBackOffPeriod { get; set; }

        public int InitialDelay { get; set; }

        public int TimeToWait { get; set; }

        public async Task<Subscriber> RunAsync(CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            return await this.subscriber.Execute(cancellationToken);
        }

		public async Task StartAsync()
		{
			if (this.throttle != null)
			{
				this.throttle.Tell(new Func<Task>(async () => await this.ReStartAsync()));
			}
			else
			{
				await this.ReStartAsync();
			}
		}

		private async Task ReStartAsync()
		{
            var cancellationTokenSource = new CancellationTokenSource(this.subscriber.TimeToExpire);
            this.subscriber.RetryCount = ++this.subscriber.RetryCount;
            var retryPolicy = this.subscriber.RetryPolicy;

            // ReSharper disable once MethodSupportsCancellation
            await this.RunAsync(cancellationTokenSource.Token).ContinueWith(async anticedant =>
                {
                    if (anticedant.Status == TaskStatus.Faulted || anticedant.Status == TaskStatus.Canceled)
                    {
                        var timeToWait = retryPolicy.TimeToWait(this.subscriber.RetryCount);

                        if (timeToWait != TimeSpan.MaxValue)
                        {
                            await Task.Delay(timeToWait);
                            await this.StartAsync();
                        }
                    }

                    cancellationTokenSource.Dispose();
                });
        }
    }
}