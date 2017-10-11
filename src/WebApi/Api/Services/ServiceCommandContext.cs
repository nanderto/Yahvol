namespace Yahvol.Services
{
    using System.Data.Entity;

    public class ServiceCommandContext : DbContext, IServiceCommandContext
    {
        // You can add custom code to this file. Changes will not be overwritten.
        // 
        // If you want Entity Framework to drop and regenerate your database
        // automatically whenever you change your model schema, add the following
        // code to the Application_Start method in your Global.asax file.
        // Note: this will destroy and re-create your database with every model change.
        // 
       
        public ServiceCommandContext()
            : base("name=ServiceCommandContext")
        {
            this.Configuration.ProxyCreationEnabled = false;
        }

        public IDbSet<ServiceCommand> ServiceCommands { get; set; }

        public IDbSet<Subscriber> Subscribers { get; set; }

        public IDbSet<LogEntry> LogEntries { get; set; }
    }
}