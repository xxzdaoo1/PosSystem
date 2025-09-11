using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PosSystem.Core.Models;

namespace PosSystem.Data.Interfaces
{
    public interface ISaleItemRepository
    {
        Task<IEnumerable<SaleItemModel>> GetBySaleIdAsync(int saleId);
        Task AddAsync(SaleItemModel saleItem);
        Task AddRangeAsync(IEnumerable<SaleItemModel> saleItems);
        Task UpdateAsync(SaleItemModel saleItem);
        Task DeleteAsync(int id);
        Task DeleteBySaleIdAsync(int saleId);
    }
}
