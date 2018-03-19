namespace Yahvol.Services
{
    using Yahvol.Data;

    public class ServiceCommandLoggerInstanceFactory : IInstanceFactory<ServiceCommandLogger>
    {
        public ServiceCommandLogger Create()
        {
            return new ServiceCommandLogger();
        }
    }
}