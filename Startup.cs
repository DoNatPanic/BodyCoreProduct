using BodyCore.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

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

			// получаем строку подключения из файла конфигурации
			string connection = Configuration.GetConnectionString("DefaultConnection");
			services.AddDbContext<ApplicationContext>(options =>
		options.UseNpgsql(connection));
			services.AddIdentity<User, IdentityRole>()
				.AddEntityFrameworkStores<ApplicationContext>()
				.AddDefaultTokenProviders();
			//services.AddControllersWithViews();
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
