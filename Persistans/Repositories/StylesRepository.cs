//using System;
//using System.Collections.Generic;
//using System.ComponentModel;
//using System.Linq;
//using System.Reflection;
//using System.Text;
//using System.Threading.Tasks;
//using Microsoft.EntityFrameworkCore;
//using Microsoft.EntityFrameworkCore.Metadata.Builders;
//using NUlid;
//using Domain.ValueObjects;
//using Domain.Entities.MidjourneyStyles;

//namespace Persistans.Repositories;


//public class StylesRepository : IStylesRepository
//{
//    private readonly AppDbContext _context;

//    public StylesRepository(AppDbContext context)
//    {
//        _context = context;
//    }

//    public async Task<MidjourneyStyle?> GetByIdAsync(Ulid id)
//    {
//        return await _context.Styles.FindAsync(id);
//    }

//    public async Task AddAsync(MidjourneyStyle style)
//    {
//        await _context.Styles.AddAsync(style);
//    }

//    public async Task UpdateAsync(MidjourneyStyle style)
//    {
//        _context.Styles.Update(style);
//    }

//    public async Task DeleteAsync(Ulid id)
//    {
//        var entity = await _context.Styles.FindAsync(id);
//        if (entity != null)
//            _context.Styles.Remove(entity);
//    }

//    public async Task<IEnumerable<MidjourneyStyle>> GetAllAsync()
//    {
//        return await _context.Styles.ToListAsync();
//    }
//}

