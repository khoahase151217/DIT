namespace DIT.Application.Interfaces
{
    public interface IUnitOfWork
    {
        IProductRepository Product { get; }
        ICategoryRepository Category { get; }
    }
}
