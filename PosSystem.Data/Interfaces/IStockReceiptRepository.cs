using PosSystem.Core.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PosSystem.Data.Interfaces
{
    public interface IStockReceiptRepository
    {
        Task<IEnumerable<StockReceiptModel>> GetAllAsync();
        Task<StockReceiptModel> GetByIdAsync(int id);
        Task<IEnumerable<StockReceiptModel>> GetByProductIdAsync(int productId);
        Task<IEnumerable<StockReceiptModel>> GetByDateRangeAsync(DateTime startDate, DateTime endDate);
        Task AddAsync(StockReceiptModel stockReceipt);
        Task UpdateAsync(StockReceiptModel stockReceipt);
        Task DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
    }
}