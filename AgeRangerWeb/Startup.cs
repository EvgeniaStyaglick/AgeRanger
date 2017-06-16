using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(AgeRangerWeb.Startup))]

namespace AgeRangerWeb
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
        }
    }
}
