namespace Yahvol.Services
{
    using System;
    using System.Collections.Generic;

    public class ServiceCommand
    {
        private ICollection<Subscriber> subscribers;

        public ServiceCommand()
        {
            this.ReceivedDate = DateTime.Now;
            this.Completed = false;
            this.CommandType = this.GetType().FullName;
        }

        public ServiceCommand(string serializedCommand, DateTime receivedDate, string createdBy, string commandType, string user, string uniqueKey, string endpointId)
        {
            this.ReceivedDate = receivedDate;
            this.Completed = false;
            this.SerializedCommand = serializedCommand;
            this.CreatedBy = createdBy;
            this.CommandType = commandType;
            this.UniqueKey = uniqueKey;
            this.User = user;
            this.EndpointId = endpointId;
        }

        public string CommandType { get; set; }

        public bool Completed { get; set; }

        public DateTime? CompletedDate { get; set; }

        public string CreatedBy { get; set; }

        public string EndpointId { get; set; }

        public int Id { get; set; }

        public DateTime ReceivedDate { get; set; }
       
        public string SerializedCommand { get; set; }
        
        public virtual ICollection<Subscriber> Subscribers
        {
            get
            {
                if (this.subscribers == null)
                {
                    this.subscribers = new List<Subscriber>();
                }

                return this.subscribers;
            }
        }

        public string UniqueKey { get; set; }

        public string User { get; set; }

        public ServiceCommand AddSubscriber(Subscriber subscriber)
        {
            this.Subscribers.Add(subscriber);
            subscriber.ServiceCommand = this;
            return this;
        }
    }
}
