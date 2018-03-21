// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Subscriber.cs" company="Anderton Engineered Solutions">
// Copyright © 2014 All Rights Reserved
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Yahvol.Services
{
    using System;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    public class Subscriber
    {
        private ServiceCommandLogger logger;

        public Subscriber()
        {
            this.TypeName = this.GetType().FullName;
            this.RetryPolicy = new RetryPolicy(); // default retry policy
        }

        public Subscriber(TimeSpan timeToExpire, IWorkload workload, string workloadType)
        {
            this.TypeName = this.GetType().FullName;
            this.RetryPolicy = workload.RetryPolicy ?? new RetryPolicy();
	        this.RetriesAllowed = this.RetryPolicy.NumberOfReTrys;
            this.TimeToExpire = timeToExpire;
            this.Workload = workload;
            this.WorkloadType = workloadType;
        }

        public byte[] BinarySerializedCommand { get; set; }

        public byte[] BinarySerializedWorkload { get; set; }

        public bool Completed { get; set; }

        public int Id { get; set; }

        public DateTime? LastUpdatedDate { get; set; }

        [NotMapped]
        public ServiceCommandLogger Logger
        {
            get
            {
                if (this.logger.ServiceCommandRepository == null)
                {
                    this.logger.ServiceCommandRepository = this.ServiceCommandRepository;
                    this.logger.ServiceCommandId = this.ServiceCommandId;
                    this.logger.SubscriberId = this.Id;
                }

                this.logger.RetryCount = this.RetryCount;

                return this.logger;
            }

            set
            {
                this.logger = value;
            }
        }

        public int RetryCount { get; set; }

		public int RetriesAllowed { get; set; }

		[NotMapped]
        public RetryPolicy RetryPolicy { get; internal set; }

        public string SerializedCommand { get; set; }

        public string SerializedWorkload { get; set; }

        public virtual ServiceCommand ServiceCommand { get; set; }

        public virtual int ServiceCommandId { get; set; }

        [NotMapped]
        public IServiceCommandRepository ServiceCommandRepository { get; set; }

        public TimeSpan TimeToExpire { get; set; }

        public string TypeName { get; set; }

        [NotMapped]
        public IWorkload Workload { get; set; }

        public string WorkloadType { get; set; }

        public async virtual Task<Subscriber> Execute(CancellationToken cancellationToken)
        {
			Exception exception = null;
            try
            {
                await this.Workload.BeforeExecute(cancellationToken);
                await this.Workload.Execute(cancellationToken);
                await this.Workload.OnCompletion(cancellationToken);
                this.Completed = true;
            }
            catch (Exception ex)
            {
                exception = ex;
            }

            await this.ServiceCommandRepository.Update(this.Id, this.ServiceCommandId, this.RetryCount, this.Completed);

            if (exception != null)
            {
                this.Logger.Log(exception, $"{this.WorkloadType} (Subscriber)");
                if (this.RetryPolicy?.ExemptionsFromRetryingExceptions == null 
					|| this.RetryPolicy.ExemptionsFromRetryingExceptions.All(e => e != exception.GetType()))
                {
                    throw exception;
                }
            }

            return this;
        }
    }
}