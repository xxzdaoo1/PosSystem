using Microsoft.EntityFrameworkCore;
using PosSystem.Core.Models;
using PosSystem.Data.Context;
using PosSystem.Data.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PosSystem.Data.Repositories
{
    public class StockReceiptRepository : IStockReceiptRepository
    {
        private readonly AppDBContext _context;

        public StockReceiptRepository(AppDBContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<StockReceiptModel>> GetAllAsync()
        {
            return await _context.StockReceipts
                .Include(sr => sr.Product)
                .OrderByDescending(sr => sr.ReceiveDate)
                .ToListAsync();
        }

        public async Task<StockReceiptModel> GetByIdAsync(int id)
        {
            return await _context.StockReceipts
                .Include(sr => sr.Product)
                .FirstOrDefaultAsync(sr => sr.StockReceiptID == id);
        }

        public async Task<IEnumerable<StockReceiptModel>> GetByProductIdAsync(int productId)
        {
            return await _context.StockReceipts
                .Include(sr => sr.Product)
                .Where(sr => sr.ProductID == productId)
                .OrderByDescending(sr => sr.ReceiveDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<StockReceiptModel>> GetByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            return await _context.StockReceipts
                .Include(sr => sr.Product)
                .Where(sr => sr.ReceiveDate >= startDate && sr.ReceiveDate <= endDate)
                .OrderByDescending(sr => sr.ReceiveDate)
                .ToListAsync();
        }

        public async Task AddAsync(StockReceiptModel stockReceipt)
        {
            await _context.StockReceipts.AddAsync(stockReceipt);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(StockReceiptModel stockReceipt)
        {
            _context.StockReceipts.Update(stockReceipt);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var stockReceipt = await GetByIdAsync(id);
            if (stockReceipt != null)
            {
                _context.StockReceipts.Remove(stockReceipt);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.StockReceipts.AnyAsync(sr => sr.StockReceiptID == id);
        }
    }
}