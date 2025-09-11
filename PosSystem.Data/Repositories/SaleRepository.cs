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
    public class SaleRepository : ISaleRepository
    {
        private readonly AppDBContext _context;

        public SaleRepository(AppDBContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<SaleModel>> GetAllAsync()
        {
            return await _context.Sales
                .OrderByDescending(s => s.SaleDate)
                .ToListAsync();
        }

        public async Task<SaleModel> GetByIdAsync(int id)
        {
            return await _context.Sales.FindAsync(id);
        }

        public async Task<SaleModel> GetByIdWithItemsAsync(int id)
        {
            return await _context.Sales
                .Include(s => s.SaleItems)
                .ThenInclude(si => si.Product)
                .FirstOrDefaultAsync(s => s.SaleID == id);
        }

        public async Task AddAsync(SaleModel sale)
        {
            await _context.Sales.AddAsync(sale);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(SaleModel sale)
        {
            _context.Sales.Update(sale);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var sale = await GetByIdAsync(id);
            if (sale != null)
            {
                _context.Sales.Remove(sale);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Sales.AnyAsync(s => s.SaleID == id);
        }

        public async Task<IEnumerable<SaleModel>> GetSalesByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            return await _context.Sales
                .Include(s => s.SaleItems)
                .Where(s => s.SaleDate >= startDate && s.SaleDate <= endDate)
                .OrderByDescending(s => s.SaleDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<SaleModel>> GetRecentSalesAsync(int count = 10)
        {
            return await _context.Sales
                .Include(s => s.SaleItems)
                .ThenInclude(si => si.Product)
                .OrderByDescending(s => s.SaleDate)
                .Take(count)
                .ToListAsync();
        }

        public async Task<decimal> GetTotalSalesByDateAsync(DateTime date)
        {
            return await _context.Sales
                .Where(s => s.SaleDate.Date == date.Date)
                .SumAsync(s => s.TotalAmount);
        }
    }
}