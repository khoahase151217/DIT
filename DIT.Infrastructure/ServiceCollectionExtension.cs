using DIT.Application.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace DIT.Infrastructure
{
	public static class ServiceCollectionExtension
	{
		public static void RegisterServices(this IServiceCollection services)
		{
			services.AddTransient<IProductRepository, ProductRepository>();
			services.AddTransient<ICategoryRepository, CategoryRepository>();
			services.AddTransient<IUnitOfWork, UnitOfWork>();
		}
	}
}
