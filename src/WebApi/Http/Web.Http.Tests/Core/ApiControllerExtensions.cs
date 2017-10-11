using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrookfieldGrs.Web.Http.Tests.Core
{
    namespace BrookfieldGrs.Claims.Services.WebApi.Tests
    {
        using System.Net.Http;
        using System.Web.Http;
        using System.Web.Http.Controllers;
        using System.Web.Http.Hosting;
        using System.Web.Http.Routing;

        public static class ApiControllerExtensions
        {
            public static void SetupControllerRequest(this ApiController controller, HttpMethod method)
            {
                var controllerName = controller.GetType().Name.Replace("Controller", string.Empty);
                controller.SetupControllerRequest(method, string.Concat("http://10.125.25.169/Claims/api/", controllerName), controllerName);
            }

            public static void SetupControllerRequest(this ApiController controller, HttpMethod method, RepositoryType type)
            {
                var controllerName = controller.GetType().Name.Replace("Controller", string.Empty);
                controller.SetupControllerRequest(method, string.Concat("http://10.125.25.169/Claims/api/", controllerName), controllerName, type);
            }

            public static void SetupControllerRequest(this ApiController controller, HttpMethod method, string url, string controllerName)
            {
                controller.SetupControllerRequest(method, url, controllerName, RepositoryType.Stubbed);
            }

            public static void SetupControllerRequest(this ApiController controller, HttpMethod method, string url, string controllerName, RepositoryType type)
            {
                var config = new HttpConfiguration();
                SetDependencyResolver(config, type);

                var request = new HttpRequestMessage(method, url);
                var route = config.Routes.MapHttpRoute("DefaultApi", "api/{controller}/{id}");
                var routeData = new HttpRouteData(route, new HttpRouteValueDictionary { { "controller", controllerName } });

                request.Properties[HttpPropertyKeys.HttpConfigurationKey] = config;
                request.Properties[HttpPropertyKeys.HttpRouteDataKey] = routeData;

                controller.Configuration = config;
                controller.ControllerContext = new HttpControllerContext(config, routeData, request);
                controller.Request = request;
            }

            private static void SetDependencyResolver(HttpConfiguration config, RepositoryType type)
            {
                //switch (type)
                //{
                //    case RepositoryType.Mocked:
                //        {
                //            config.DependencyResolver = DependencyResolverProvider.MockedResolver;
                //            break;
                //        }
                //    case RepositoryType.Stubbed:
                //        {
                //            config.DependencyResolver = DependencyResolverProvider.StubbedResolver;
                //            break;
                //        }
                //    default:
                //        {
                //            config.DependencyResolver = DependencyResolverProvider.Resolver;
                //            break;
                //        }
                //}
            }
        }
    }

}
