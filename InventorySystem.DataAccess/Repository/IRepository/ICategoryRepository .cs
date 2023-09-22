﻿using InventorySystem.Models;

namespace InventorySystem.DataAccess.Repository.IRepository
{
    public interface ICategoryRepository : IRepository<Category>
    {
        void Update(Category category);
    }
}