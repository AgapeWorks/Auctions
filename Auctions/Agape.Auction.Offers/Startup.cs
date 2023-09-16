using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Threading.Tasks;
using Agape.Azure.Cosmos;
using AgapeModelOffers = Agape.Auctions.Models.Offers;

namespace Agape.Auction.Offers
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddControllersWithViews();
            services.AddSingleton<ICosmosRepository<AgapeModelOffers.Offer, AgapeModelOffers.Offer>>(InitializeCosmosRepositaryInstanceAsync(Configuration.GetSection("CosmosDb")).GetAwaiter().GetResult());
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }

        private static async Task<ICosmosRepository<AgapeModelOffers.Offer, AgapeModelOffers.Offer>> InitializeCosmosRepositaryInstanceAsync(IConfigurationSection configurationSection)
        {
            ICosmosRepository<AgapeModelOffers.Offer, AgapeModelOffers.Offer> cosmosRepository;
            var options = new CosmosApiOptions();
            options.Account = configurationSection.GetSection("Account").Value;
            options.Container = configurationSection.GetSection("ContainerName").Value;
            options.Database = configurationSection.GetSection("DatabaseName").Value;
            options.PartitionKey = configurationSection.GetSection("PartitionKey").Value;
            var cosmosDbKey = configurationSection.GetSection("Key").Value;
            cosmosRepository = new CosmosRepository<AgapeModelOffers.Offer, AgapeModelOffers.Offer>(cosmosDbKey, options);
            return cosmosRepository;
        }
    }
}