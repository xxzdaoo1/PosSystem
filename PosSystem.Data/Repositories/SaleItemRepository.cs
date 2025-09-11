using Microsoft.EntityFrameworkCore;
using PosSystem.Core.Models;
using PosSystem.Data.Context;
using PosSystem.Data.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PosSystem.Data.Repositories
{
    public class SaleItemRepository : ISaleItemRepository
    {
        private readonly AppDBContext _context;

        public SaleItemRepository(AppDBContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<SaleItemModel>> GetBySaleIdAsync(int saleId)
        {
            return await _context.SaleItems
                .Include(si => si.Product)
                .Where(si => si.SaleID == saleId)
                .ToListAsync();
        }

        public async Task AddAsync(SaleItemModel saleItem)
        {
            await _context.SaleItems.AddAsync(saleItem);
            await _context.SaveChangesAsync();
        }

        public async Task AddRangeAsync(IEnumerable<SaleItemModel> saleItems)
        {
            await _context.SaleItems.AddRangeAsync(saleItems);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(SaleItemModel saleItem)
        {
            _context.SaleItems.Update(saleItem);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var saleItem = await _context.SaleItems.FindAsync(id);
            if (saleItem != null)
            {
                _context.SaleItems.Remove(saleItem);
                await _context.SaveChangesAsync();
            }
        }

        public async Task DeleteBySaleIdAsync(int saleId)
        {
            var saleItems = await GetBySaleIdAsync(saleId);
            _context.SaleItems.RemoveRange(saleItems);
            await _context.SaveChangesAsync();
        }
    }
}