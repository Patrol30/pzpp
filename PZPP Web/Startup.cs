using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(PZPP_Web.Startup))]
namespace PZPP_Web
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
