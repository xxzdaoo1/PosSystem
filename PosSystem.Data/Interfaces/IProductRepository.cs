using PosSystem.Core.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PosSystem.Data.Interfaces
{
    public interface IProductRepository
    {
        Task<IEnumerable<ProductModel>> GetAllAsync();
        Task<IEnumerable<ProductModel>> SearchAsync(string searchText);
        Task<ProductModel> GetByIdAsync(int id);
        Task AddAsync(ProductModel product);
        Task UpdateAsync(ProductModel product);
        Task DeleteAsync(ProductModel product);
    }
}
