using PosSystem.Core.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PosSystem.Data.Interfaces
{
    public interface IStockAdjustmentRepository
    {
        Task<IEnumerable<StockAdjustmentModel>> GetAllAsync();
        Task<StockAdjustmentModel> GetByIdAsync(int id);
        Task<IEnumerable<StockAdjustmentModel>> GetByProductIdAsync(int productId);
        Task<IEnumerable<StockAdjustmentModel>> GetByDateRangeAsync(DateTime startDate, DateTime endDate);
        Task<IEnumerable<StockAdjustmentModel>> GetByTypeAsync(string adjustmentType);
        Task AddAsync(StockAdjustmentModel stockAdjustment);
        Task AddRangeAsync(IEnumerable<StockAdjustmentModel> stockAdjustments);
        Task UpdateAsync(StockAdjustmentModel stockAdjustment);
        Task DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
    }
}