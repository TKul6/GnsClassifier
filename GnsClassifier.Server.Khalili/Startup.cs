using Microsoft.AspNet.SignalR;
using Microsoft.Owin;
using Microsoft.Owin.Cors;
using Owin;

[assembly: OwinStartup(typeof(GnsClassifier.Server.Khalili.Startup))]
namespace GnsClassifier.Server.Khalili
{  
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.UseCors(CorsOptions.AllowAll);
            app.MapSignalR("/signalr",new HubConfiguration() {EnableDetailedErrors = true});
        }
    }
}