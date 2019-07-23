namespace Yahvol.Web.Http
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Net;
    using System.Net.Http;
    using System.Threading;
    using System.Web.Http;

    using Yahvol.Data;
    using Yahvol.Linq.Expressions;
    using Yahvol.Services;

    public abstract class ServiceCommandApiController : ApiController
	{
		private readonly IServiceCommandService serviceCommandService;

		protected ServiceCommandApiController(IServiceCommandService serviceCommandService)
		{
			this.serviceCommandService = serviceCommandService;
		}

        protected ServiceCommandApiController(IInstanceFactory<IServiceCommandContext> serviceCommandContext)
        {
            this.serviceCommandService = new WebCommandService(serviceCommandContext);
        }

		protected IServiceCommandService CommandService
		{
			get
			{
				return this.serviceCommandService;
			}
		}

		public virtual IEnumerable<ServiceCommand> Get()
		{
			return this.CommandService.Get<ServiceCommand>(sc => sc.User == Thread.CurrentPrincipal.Identity.Name);
		}

        public virtual ServiceCommand Get(int id)
        {
            var command = this.serviceCommandService.Get(id);
            if (command == null)
            {
                throw new HttpResponseException(this.Request.CreateResponse(HttpStatusCode.NotFound));
            }

            return command;
        }

        public IEnumerable<ServiceCommand> Get(Expression<Func<ServiceCommand, bool>> predicate)
        {
            var user = Thread.CurrentPrincipal.Identity.Name;
            Expression<Func<ServiceCommand, bool>> userPredicate = sc => sc.User == user;
            var expressions = new List<Expression<Func<ServiceCommand, bool>>> { predicate, userPredicate };

            var combinedExpressions = expressions.AndAll();
            return this.serviceCommandService.Get(combinedExpressions);
        }
	}
}
