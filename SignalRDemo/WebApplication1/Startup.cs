using Owin;
using Microsoft.Owin;
[assembly: OwinStartup(typeof(WebApplication1.Startup))]
namespace WebApplication1
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            // 任何连接或集线器接线和配置都应在这里配置
            app.MapSignalR();
        }
    }
}