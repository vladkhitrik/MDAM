using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(MDAM.Startup))]
namespace MDAM
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
