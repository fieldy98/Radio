using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(radio.Startup))]
namespace radio
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
