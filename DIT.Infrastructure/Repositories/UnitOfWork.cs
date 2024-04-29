using DIT.Application.Interfaces;

namespace DIT.Infrastructure
{
	public class UnitOfWork : IUnitOfWork
	{
		public UnitOfWork(IProductRepository productRepository, ICategoryRepository categoryRepository)
		{
			Product = productRepository;
			Category = categoryRepository;
		}

		public IProductRepository Product { get; set; }
		public ICategoryRepository Category { get; set; }
	}
}
