namespace Yahvol.Web.Http
{
	using System.Web.Http;

	using Yahvol.Data;

	public class RepositoryApiController : ApiController
	{
		public RepositoryApiController(IRepository repository)
		{
			this.Repository = repository;
		}

		protected IRepository Repository { get; private set; }
	}
}