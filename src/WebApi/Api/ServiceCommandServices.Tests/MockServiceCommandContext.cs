namespace TransfereeService.Tests
{
    using System.Data.Entity;
    using System.Linq;

    using BrookfieldGrs.Services;

    public class MockServiceCommandContext : DbContext, IServiceCommandContext
    {

        private readonly IDbSet<ServiceCommand> db = new InMemoryDbSet<ServiceCommand>();
        private readonly IDbSet<Subscriber> db2 = new InMemoryDbSet<Subscriber>();

        public IDbSet<ServiceCommand> ServiceCommands
        {
            get
            {
                return this.db;
            }

            set
            {
                var dbSet = value;
            } 
        }


        public IDbSet<Subscriber> Subscribers
        {
            get
            {
                return this.db2;
            }

            set
            {
                var dbSet = value;
            }
        }

        public int SaveChanges()
        {
            this.db.Last().Id = this.db.Count();
            return this.db.Count();
        }
    }
}