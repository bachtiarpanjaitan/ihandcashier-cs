using IhandCashier.Bepe.Database;
using IhandCashier.Bepe.Dtos;
using IhandCashier.Bepe.Entities;
using IhandCashier.Bepe.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace IhandCashier.Bepe.Services;

public class ProductPriceService : IDataService<ProductPriceDto>
{
    public ProductPriceService(){}
    public int TotalData()
    {
        using var _context = new AppDbContext(); 
        return _context.ProductPrices.AsNoTracking().Count();
    }

    public async Task<List<ProductPriceDto>> GetPagingData(int pageIndex, int pageSize, string sq = null)
    {
        using var _context = new AppDbContext(); 
        IQueryable<ProductPrice> query = _context.ProductPrices
            .AsNoTracking()
            .Include(p => p.Product)
            .Include(b => b.Unit)
            .ThenInclude(bu => bu.BasicUnit);
            
        if (!string.IsNullOrWhiteSpace(sq))
        {
            query = query.Where(item => EF.Functions.Like(item.Product.nama, $"%{sq}%") ||
                                        EF.Functions.Like(item.Unit.nama, $"%{sq}%")
            );
        }
        return await query
            .Skip(pageIndex * pageSize)
            .Take(pageSize)
            .Select(item => new ProductPriceDto()
            {
                id = item.id,
                nama = item.Product.nama,
                harga = (double) item.harga,
                kode = item.Product.kode,
                product_id = item.product_id,
                tanggal_berlaku = item.tanggal_berlaku,
                unit_id = item.Unit.id,
                unit_nama = item.Unit.nama,
                basic_unit_nama = item.Unit.BasicUnit.nama,
                harga_satuan_terkecil = (double) (item.harga/item.Unit.konversi)
            })
            .OrderByDescending(x => x.tanggal_berlaku)
            .ToListAsync();
    }
}