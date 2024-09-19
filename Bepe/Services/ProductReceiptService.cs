using IhandCashier.Bepe.Constants;
using IhandCashier.Bepe.Database;
using IhandCashier.Bepe.Dtos;
using IhandCashier.Bepe.Dtos.Details;
using IhandCashier.Bepe.Entities;
using IhandCashier.Bepe.Interfaces;
using IhandCashier.Bepe.Statics;
using Microsoft.EntityFrameworkCore;
using Syncfusion.Maui.DataSource.Extensions;

namespace IhandCashier.Bepe.Services;

public class ProductReceiptService : IDataService<ProductReceiptDto>
{
    private readonly AppDbContext _context;

    public ProductReceiptService(AppDbContext context)
    {
        _context = context;
    }
    public int TotalData()
    {
        return _context.ProductReceipts.AsNoTracking().Where(x => x.deleted_at == null).Count();
    }

    public async Task<List<ProductReceiptDto>> GetPagingData(int pageIndex, int pageSize, string searchQuery = null)
    {
        IQueryable<ProductReceipt> query = _context.ProductReceipts.AsNoTracking()
            .Include(s => s.Supplier)
            .Where(x => x.deleted_at == null);
        if (!string.IsNullOrWhiteSpace(searchQuery))
        {
            query = query.Where(item => EF.Functions.Like(item.kode_transaksi, $"%{searchQuery}%") ||
                                        EF.Functions.Like(item.penerima, $"%{searchQuery}%") ||
                                        EF.Functions.Like(item.Supplier.nama, $"%{searchQuery}%")
            );
        }
        var result = await query.AsNoTracking().Skip(pageIndex * pageSize).Take(pageSize)
            .Include(d => d.Details).ThenInclude(x => x.Product)
            .Include(d => d.Details).ThenInclude(x => x.Unit)
            .Select(item => new ProductReceiptDto()
            {
                Id = item.id,
                KodeTransaksi = item.kode_transaksi,
                SupplierId = item.supplier_id,
                Penerima = item.penerima,
                Tanggal = item.tanggal,
                Keterangan = item.keterangan,
                SupplierName = item.Supplier.nama,
                ItemCount = item.Details.Count(),
                StatusName = Helper.SplitCamelCase(AppEnumeration.GetEnumName<ReceiptStatus>(item.status)),
                Status = item.status,
                Total = item.Details.Sum(x=> (double)(x.jumlah * x.harga_satuan)),
                CreatedAt = item.created_at,
                UpdatedAt = item.updated_at,
                DeletedAt = item.deleted_at,
                Details = item.Details.Select(d => new ProductReceiptDetailDto()
                {
                    Id = d.id,
                    ProductReceiptId = d.product_receipt_id,
                    ProductId = d.product_id,
                    ProductName = d.Product != null ? d.Product.nama : null,
                    Jumlah = d.jumlah,
                    UnitId = d.unit_id,
                    UnitName = d.Unit != null ? d.Unit.nama : null,
                    HargaSatuan = d.harga_satuan,
                    Total = (d.harga_satuan * d.jumlah),
                }).ToList(),
                Views = item.Details.Select(d => new ProductReceiptDetailGrid()
                {
                    Id = d.id,
                    NamaBarang = d.Product != null ? d.Product.nama : null,
                    Satuan = d.Unit != null ? d.Unit.nama : null,
                    Jumlah = d.jumlah,
                    HargaSatuan = d.harga_satuan,
                    Total = (d.harga_satuan * d.jumlah)
                }).ToList()
            })
            .ToListAsync();
        return result;
    }
    
    public async Task AddAsync(ProductReceipt item)
    {
        using (var transaction = _context.Database.BeginTransaction())
        {
            try
            {
                var newItem = new ProductReceipt()
                {
                    kode_transaksi = item.kode_transaksi,
                    supplier_id = item.supplier_id,
                    penerima = item.penerima,
                    tanggal = item.tanggal,
                    created_at = DateTime.Now,
                    status = item.status,
                    keterangan = item.keterangan,
                };
                _context.ProductReceipts.Add(newItem);
                _context.SaveChanges();
                var details = item.Details.Select(d =>
                {
                    d.product_receipt_id = newItem.id;
                    return d;
                }).ToList();
                _context.ProductReceiptDetails.AddRange(details);

                //Calculate Stock
                foreach (var detail in details)
                {
                    if (newItem.status == (int)ReceiptStatus.Selesai) CalculateStock(detail);
                }

                await _context.SaveChangesAsync();
                _context.ProductReceipts.Entry(item).State = EntityState.Detached;
                transaction.Commit();
            }catch (Exception ex)
            {
                transaction.Rollback();
                Console.WriteLine($" Error: {ex.Message}");
            }
        } 
    }

    public async Task UpdateAsync(ProductReceipt item)
    {
        using (var transaction = _context.Database.BeginTransaction())
        {
            try
            {
                var entity = await _context.ProductReceipts.AsNoTracking().FirstOrDefaultAsync(e => e.id == item.id);
                _context.Entry(entity).State = EntityState.Detached;
                var editItem = new ProductReceipt()
                {
                    id = item.id,
                    kode_transaksi = item.kode_transaksi,
                    supplier_id = item.supplier_id,
                    penerima = item.penerima,
                    tanggal = item.tanggal,
                    updated_at = DateTime.Now,
                    created_at = item.created_at,
                    status = item.status,
                    keterangan = item.keterangan,
                };
                _context.Entry(entity).CurrentValues.SetValues(editItem);
                _context.Update(entity);

                foreach (var detail in item.Details)
                {
                    var en = await _context.ProductReceiptDetails.AsNoTracking()
                        .FirstOrDefaultAsync(e => e.id == detail.id);
                    if (editItem.status == (int)ReceiptStatus.Selesai) CalculateStock(detail, StockStatus.Adjustment,en);

                    _context.Entry(en).CurrentValues.SetValues(detail);
                    _context.Update(en);
                }

                await _context.SaveChangesAsync();

                _context.Entry(entity).State = EntityState.Detached;
                transaction.Commit();
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                Console.WriteLine($" Error: {ex.Message}");
            }
        }
    }

    public async Task SoftDeleteAsync(ProductReceipt item)
    {
        using (var transaction = _context.Database.BeginTransaction())
        {
            try
            {
                var entity = await _context.ProductReceipts.AsNoTracking().FirstOrDefaultAsync(e => e.id == item.id);
                var editItem = new ProductReceipt()
                {
                    id = item.id,
                    kode_transaksi = item.kode_transaksi,
                    supplier_id = item.supplier_id,
                    penerima = item.penerima,
                    tanggal = item.tanggal,
                    updated_at = item.updated_at,
                    created_at = item.created_at,
                    status = item.status,
                    keterangan = item.keterangan,
                    deleted_at = DateTime.Now
                };

                if (item.Details.Count > 0)
                {
                    foreach (var detail in item.Details)
                    {
                        var en = await _context.ProductReceiptDetails.AsNoTracking()
                            .FirstOrDefaultAsync(e => e.id == detail.id);
                        if (editItem.status == (int)ReceiptStatus.Selesai) CalculateStock(detail, StockStatus.Deletion);
                    }
                }

                _context.Entry(entity).CurrentValues.SetValues(editItem);
                _context.Update(entity);
                await _context.SaveChangesAsync();
                _context.Entry(entity).State = EntityState.Detached;
                
                transaction.Commit();
            }catch (Exception ex)
            {
                transaction.Rollback();
                Console.WriteLine($" Error: {ex.Message}");
            }
        }
    }

    private async void CalculateStock(ProductReceiptDetail newData, StockStatus status = StockStatus.Addition, ProductReceiptDetail oldData = null)
    {
        ProductStock oldStock = null;
        oldStock = _context.ProductStocks
            .AsNoTracking()
            .Where(x => x.product_id == newData.product_id)
            .Where(x => x.unit_id == newData.unit_id)
            .FirstOrDefault();

        if (oldStock != null)
        {
            if(status == StockStatus.Addition) oldStock.jumlah = oldStock.jumlah + newData.jumlah;
            else if (status == StockStatus.Adjustment && oldData != null) oldStock.jumlah = oldStock.jumlah + (newData.jumlah - oldData.jumlah);
            else if(status == StockStatus.Deletion) oldStock.jumlah = oldStock.jumlah - newData.jumlah;
            _context.Update(oldStock);
        }
        else
        {
            var newStock = new ProductStock()
            {
                product_id = newData.product_id,
                unit_id = newData.unit_id,
                jumlah = newData.jumlah,
            };
            _context.ProductStocks.Add(newStock);
        }
    }
}