namespace Yahvol.Services
{
    using Yahvol.Data;

    public class ServiceCommandContextInstanceFactory : IInstanceFactory<ServiceCommandContext>
    {
        public ServiceCommandContext Create()
        {
            return new ServiceCommandContext();
        }
    }
}