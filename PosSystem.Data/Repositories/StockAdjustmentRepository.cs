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
    public class StockAdjustmentRepository : IStockAdjustmentRepository
    {
        private readonly AppDBContext _context;

        public StockAdjustmentRepository(AppDBContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<StockAdjustmentModel>> GetAllAsync()
        {
            return await _context.StockAdjustments
                .Include(sa => sa.Product)
                .OrderByDescending(sa => sa.AdjustmentDate)
                .ToListAsync();
        }

        public async Task<StockAdjustmentModel> GetByIdAsync(int id)
        {
            return await _context.StockAdjustments
                .Include(sa => sa.Product)
                .FirstOrDefaultAsync(sa => sa.StockAdjustmentID == id);
        }

        public async Task<IEnumerable<StockAdjustmentModel>> GetByProductIdAsync(int productId)
        {
            return await _context.StockAdjustments
                .Include(sa => sa.Product)
                .Where(sa => sa.ProductID == productId)
                .OrderByDescending(sa => sa.AdjustmentDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<StockAdjustmentModel>> GetByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            return await _context.StockAdjustments
                .Include(sa => sa.Product)
                .Where(sa => sa.AdjustmentDate >= startDate && sa.AdjustmentDate <= endDate)
                .OrderByDescending(sa => sa.AdjustmentDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<StockAdjustmentModel>> GetByTypeAsync(string adjustmentType)
        {
            return await _context.StockAdjustments
                .Include(sa => sa.Product)
                .Where(sa => sa.AdjustmentType == adjustmentType)
                .OrderByDescending(sa => sa.AdjustmentDate)
                .ToListAsync();
        }

        public async Task AddAsync(StockAdjustmentModel stockAdjustment)
        {
            await _context.StockAdjustments.AddAsync(stockAdjustment);
            await _context.SaveChangesAsync();
        }

        public async Task AddRangeAsync(IEnumerable<StockAdjustmentModel> stockAdjustments)
        {
            await _context.StockAdjustments.AddRangeAsync(stockAdjustments);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(StockAdjustmentModel stockAdjustment)
        {
            _context.StockAdjustments.Update(stockAdjustment);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var stockAdjustment = await GetByIdAsync(id);
            if (stockAdjustment != null)
            {
                _context.StockAdjustments.Remove(stockAdjustment);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.StockAdjustments.AnyAsync(sa => sa.StockAdjustmentID == id);
        }
    }
}