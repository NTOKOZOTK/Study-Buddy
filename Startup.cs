using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Study_Buddy.Startup))]
namespace Study_Buddy
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
