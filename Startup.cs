using AspNetCore.Identity.PostgreSQL.Context;
using AspNetCore.Identity.PostgreSQL.Stores;
using BodyCore.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using IdentityRole = AspNetCore.Identity.PostgreSQL.IdentityRole;

namespace BodyCore
{
	public class Startup
	{
		public Startup( IConfiguration configuration )
		{
			Configuration = configuration;
		}

		public IConfiguration Configuration { get; }

		// This method gets called by the runtime. Use this method to add services to the container.
		public void ConfigureServices( IServiceCollection services )
		{
			services.AddMvc(option => option.EnableEndpointRouting = false);

			/*// получаем строку подключения из файла конфигурации
			string connection = Configuration.GetConnectionString("DefaultConnection");
			services.AddDbContext<ApplicationContext>(options =>
		options.UseNpgsql(connection));
			services.AddIdentity<ApplicationUser, IdentityRole>()
				.AddEntityFrameworkStores<ApplicationContext>()
				.AddDefaultTokenProviders();
			//services.AddControllersWithViews();*/
			services.AddIdentity<ApplicationUser, IdentityRole>()
			   .AddUserStore<UserStore<ApplicationUser>>()
			   .AddRoleStore<RoleStore<IdentityRole>>()
			   .AddRoleManager<RoleManager<IdentityRole>>()
				   .AddDefaultTokenProviders();

			// Add application services.
			IdentityDbConfig.StringConnectionName = "DefaultCon";
			services.AddMvc();
			services.AddSingleton(_ => Configuration);
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure( IApplicationBuilder app, IWebHostEnvironment env )
		{


			app.UseDeveloperExceptionPage();
			app.UseStatusCodePages();
			app.UseHttpsRedirection();
			app.UseStaticFiles();
			app.UseMvcWithDefaultRoute();

			app.UseAuthentication();
			app.UseAuthorization();
		}
	}
}
