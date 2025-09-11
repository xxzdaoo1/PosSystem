using Microsoft.EntityFrameworkCore;
using PosSystem.Core.Models;
using PosSystem.Data.Context;
using PosSystem.Data.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PosSystem.Data.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly AppDBContext _context;

        public ProductRepository(AppDBContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ProductModel>> GetAllAsync()
        {
            return await _context.Products.ToListAsync();
        }

        public async Task<IEnumerable<ProductModel>> SearchAsync(string searchText)
        {
            if (string.IsNullOrWhiteSpace(searchText))
                return await GetAllAsync();

            return await _context.Products
                .Where(p => (p.Name != null && EF.Functions.Like(p.Name, $"%{searchText}%")) ||
                            (p.Barcode != null && EF.Functions.Like(p.Barcode, $"%{searchText}%")))
                .ToListAsync();
        }

        public async Task<ProductModel> GetByIdAsync(int id)
        {
            return await _context.Products.FindAsync(id);
        }

        public async Task AddAsync(ProductModel product)
        {
            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(ProductModel product)
        {
            _context.Products.Update(product);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(ProductModel product)
        {
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
        }
    }
}
