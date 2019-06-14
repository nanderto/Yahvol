using Orleans.Runtime;
using System;

namespace Yahvol.Server
{
    public class Silo : Orleans.Runtime.Silo
    {
        public Silo(ILocalSiloDetails siloDetails, IServiceProvider services) : base(siloDetails, services)
        {
        }
    }
}
