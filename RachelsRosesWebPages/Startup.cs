using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(RachelsRosesWebPages.Startup))]
namespace RachelsRosesWebPages
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
