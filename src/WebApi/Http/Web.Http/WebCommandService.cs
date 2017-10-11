namespace BrookfieldGrs.Web.Http
{
    using BrookfieldGrs.Services;
    using Microsoft.AspNet.SignalR;

    public class WebCommandService : ServiceCommandService
    {
        private readonly IHubContext serviceCommandHub = null;

        public WebCommandService()
            : base()
        {
            if (GlobalHost.ConnectionManager != null)
            {
                this.serviceCommandHub = GlobalHost.ConnectionManager.GetHubContext<ServiceCommandHub>();
            }     
        }

        public WebCommandService(IServiceCommandRepository serviceCommandRepository) : base(serviceCommandRepository)
        {
            if (GlobalHost.ConnectionManager != null)
            {
                this.serviceCommandHub = GlobalHost.ConnectionManager.GetHubContext<ServiceCommandHub>();
            }   
        }

        public override void RaiseServiceCommandsCompletedEvent()
        {
            if (this.Command.ConnectionId != null)
            {
                this.serviceCommandHub.Clients.Client(this.Command.ConnectionId).CommandCompleted(this.Command);
            }
        }

        public override void RaiseWorkloadCompletedEvent(string name)
        {
            if (this.Command.ConnectionId != null)
            {
                this.serviceCommandHub.Clients.Client(this.Command.ConnectionId).WorkloadCompleted(name);
            }
        }
    }
}
