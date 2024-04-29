﻿using DIT.Core;
using DIT.Core.Entities;

namespace DIT.Application.Interfaces
{
    public interface IProductRepository
    {
        Task<GetResponse<Product>> GetAllAsync(Guid? category, int? page, int? size, string? q);
        Task<Product> GetByIdAsync(Guid id);
    }
}
