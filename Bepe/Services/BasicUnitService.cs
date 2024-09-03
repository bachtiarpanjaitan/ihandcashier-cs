using IhandCashier.Bepe.Database;
using IhandCashier.Bepe.Dtos;
using IhandCashier.Bepe.Entities;
using IhandCashier.Bepe.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace IhandCashier.Bepe.Services;

public class BasicUnitService : IDataService<BasicUnitDto>
{
    private readonly AppDbContext _context;

    public BasicUnitService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<BasicUnit>> GetAsync()
    {
        return await _context.BasicUnits.AsNoTracking().ToListAsync();
    }
    
    public List<BasicUnit> Get()
    {
        return _context.BasicUnits.AsNoTracking().ToList();
    }
    
    public IQueryable<BasicUnit> Query()
    {
        return _context.BasicUnits.AsNoTracking().AsQueryable();
    }
    
    public int TotalData()
    {
        return _context.BasicUnits.AsNoTracking().Count();
    }

    public async Task<List<BasicUnitDto>> GetPagingData(int pageIndex, int pageSize, string searchQuery)
    {
        IQueryable<BasicUnit> query = _context.BasicUnits.AsNoTracking();
        if (!string.IsNullOrWhiteSpace(searchQuery))
        {
            query = query.Where(item => EF.Functions.Like(item.nama, $"%{searchQuery}%")
            );
        }
        return await query
            .Select(d => new BasicUnitDto()
            {
                id = d.id,
                nama = d.nama,
            })
            .Skip(pageIndex * pageSize).Take(pageSize).ToListAsync();
    }

    public async Task AddAsync(BasicUnit item)
    {
        _context.BasicUnits.Add(item);
        await _context.SaveChangesAsync();
        _context.BasicUnits.Entry(item).State = EntityState.Detached;
    }
    
    public async Task UpdateAsync(BasicUnit item)
    {
        var entity = await _context.BasicUnits.AsNoTracking().FirstOrDefaultAsync(e => e.id == item.id);
        _context.Entry(entity).CurrentValues.SetValues(item);
        _context.Update(entity);
        await _context.SaveChangesAsync();
        
        _context.Entry(entity).State = EntityState.Detached;
    }

    public async Task DeleteAsync(BasicUnit unit)
    {
        _context.BasicUnits.Remove(unit);    
        await _context.SaveChangesAsync();
    }
}