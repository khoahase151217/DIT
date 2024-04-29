using DIT.Core.Entities;

namespace DIT.Application.Interfaces
{
    public interface ICategoryRepository
    {
        Task<IEnumerable<Category>> GetAllAsync();
    }
}
