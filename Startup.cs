using AspNetCore.Identity.PostgreSQL.Context;
using AspNetCore.Identity.PostgreSQL.Stores;
using BodyCore.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
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

			string connection = Configuration.GetConnectionString("DefaultCon");
			services.AddDbContext<ApplicationContext>(options =>
			options.UseNpgsql(connection));

			services.AddIdentity<ApplicationUser, IdentityRole>()
			   .AddUserStore<UserStore<ApplicationUser>>()
			   .AddRoleStore<RoleStore<IdentityRole>>()
			   .AddRoleManager<RoleManager<IdentityRole>>()
				   .AddDefaultTokenProviders();

			IdentityDbConfig.StringConnectionName = "DefaultCon";

			services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
				.AddCookie(options => 
				{
					options.LoginPath = new PathString("/Account/Login");
				});

			services.AddControllersWithViews();
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure( IApplicationBuilder app, IWebHostEnvironment env )
		{
			if(env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}
			else
			{
				app.UseExceptionHandler("/Home/Error");
			}
			app.UseStaticFiles();
			app.UseRouting();
			app.UseAuthentication();
			app.UseAuthorization();

			app.UseEndpoints(endpoints =>
			{
				endpoints.MapControllerRoute(
					name: "default",
					pattern: "{controller=Home}/{action=Index}/{id?}");
			});
		}
	}
}
