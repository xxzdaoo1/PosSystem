using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PosSystem.Core.Models;

namespace PosSystem.Data.Interfaces
{
    public interface ISaleRepository
    {
        Task<IEnumerable<SaleModel>> GetAllAsync();
        Task<SaleModel> GetByIdAsync(int id);
        Task<SaleModel> GetByIdWithItemsAsync(int id);
        Task AddAsync(SaleModel sale);
        Task UpdateAsync(SaleModel sale);
        Task DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
        Task<IEnumerable<SaleModel>> GetSalesByDateRangeAsync(DateTime startDate, DateTime endDate);
        Task<IEnumerable<SaleModel>> GetRecentSalesAsync(int count = 10);
        Task<decimal> GetTotalSalesByDateAsync(DateTime date);
    }
}
