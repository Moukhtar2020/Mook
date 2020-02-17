using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Student_Mgt_System_2019.Startup))]
namespace Student_Mgt_System_2019
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
