using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Http;
using Rebus;
using ScriptCs.Rebus;

namespace ScriptCs.Hosting.WebApi.IIS
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
            var builder = new WebApiConfigurationBuilder(config, HttpContext.Current.Server.MapPath("bin"));
            builder.Build();

            var bus = new MsmqBus("controllerBus");
            bus.Container.Register(() => new ScriptedControllerHandler());
            bus.Start();
        }
    }

    public class ScriptedControllerHandler : IHandleMessages<Script>
    {
        public void Handle(Script message)
        {
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"bin\Scripts\ScriptController.csx");
            File.WriteAllText(path, message.ScriptContent);
        }
    }
}
