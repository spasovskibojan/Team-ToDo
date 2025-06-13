using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(IT_Proekt_Proba_Teams.Startup))]
namespace IT_Proekt_Proba_Teams
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
