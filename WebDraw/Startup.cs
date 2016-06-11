using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(WebDraw.Startup))]
namespace WebDraw
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
