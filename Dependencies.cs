using Microsoft.Extensions.DependencyInjection;
using sellercatalogue.BAL;
using sellercatalogue.DAL;

namespace sellercatalogue
{
    public static class Dependencies
    {
        /// <summary>
        /// Extension method to inject all dependencies to make Startup.cs cleaner
        /// </summary>
        /// <param name="services"></param>
        public static void AddDependencies(this IServiceCollection services)
        {
            //DAL
            services.AddSingleton<IDbFactory, DbFactory>();
            services.AddSingleton<ICatalogueRepository, CatalogueRepository>();

            //BAL
            services.AddSingleton<ICatalogue, Catalogue>(); 
            
        }
    }
}