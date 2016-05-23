using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Inliner.Sample.Web.Startup))]
namespace Inliner.Sample.Web
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
