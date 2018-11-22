using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ProjetoT.Security.Infra.Data
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            //services.AddDbContext<ProjetoTIdentityDbContext>(options =>
            //    options.UseSqlServer(
            //        Configuration.GetConnectionString("ProjetoTIdentityConnection")));

        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env) { }
    }
}
