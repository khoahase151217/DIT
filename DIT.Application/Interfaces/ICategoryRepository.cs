using DIT.Core.Dtos;
using DIT.Core.Entities;

namespace DIT.Application.Interfaces
{
    public interface ICategoryRepository
    {
        Task<IEnumerable<Category>> GetAllAsync(string q);
        Task<Category> GetByIdAsync(Guid id);
        Task InsertOrUpdateAsync(PostCategoryRequest request);
        Task DeleteByIdAsync(Guid id);
    }
}
