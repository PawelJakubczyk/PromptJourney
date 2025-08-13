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

// Assuming you have a styles repository, add these methods:
//public async Task<Result<MidjourneyStyleExampleLink>> AddLinkToStyleAsync(
//    string styleName, 
//    string link, 
//    string version, 
//    CancellationToken cancellationToken = default)
//{
//    try
//    {
//        // Check if style exists
//        var style = await _context.Styles.FindAsync(styleName);
//        if (style == null)
//            return Result.Fail<MidjourneyStyleExampleLink>($"Style with name '{styleName}' not found");
        
//        // Check if version exists
//        var versionExists = await _context.VersionMasters.AnyAsync(vm => vm.Version == version);
//        if (!versionExists)
//            return Result.Fail<MidjourneyStyleExampleLink>($"Version '{version}' not found");
        
//        // Check if link already exists
//        var existingLink = await _context.StyleExampleLinks
//            .FirstOrDefaultAsync(el => el.StyleName == styleName && el.Link == link && el.Version == version);
            
//        if (existingLink != null)
//            return Result.Fail<MidjourneyStyleExampleLink>($"Link '{link}' already exists for style '{styleName}' and version '{version}'");
        
//        // Check number of links
//        var linkCount = await _context.StyleExampleLinks.CountAsync(el => el.StyleName == styleName);
//        if (linkCount >= 10)
//            return Result.Fail<MidjourneyStyleExampleLink>($"Style '{styleName}' already has the maximum number of links (10)");
        
//        // Create and add the link
//        var linkResult = MidjourneyStyleExampleLink.Create(link, styleName, version);
//        if (linkResult.IsFailed)
//            return Result.Fail<MidjourneyStyleExampleLink>(linkResult.Errors);
        
//        await _context.StyleExampleLinks.AddAsync(linkResult.Value, cancellationToken);
//        await _context.SaveChangesAsync(cancellationToken);
        
//        return Result.Ok(linkResult.Value);
//    }
//    catch (Exception ex)
//    {
//        return Result.Fail<MidjourneyStyleExampleLink>($"Failed to add link: {ex.Message}");
//    }
//}

//public async Task<Result> RemoveLinkFromStyleAsync(
//    string styleName, 
//    string link, 
//    string version, 
//    CancellationToken cancellationToken = default)
//{
//    try
//    {
//        // Find the link
//        var existingLink = await _context.StyleExampleLinks
//            .FirstOrDefaultAsync(el => el.StyleName == styleName && el.Link == link && el.Version == version);
            
//        if (existingLink == null)
//            return Result.Fail($"Link '{link}' not found for style '{styleName}' and version '{version}'");
        
//        // Remove the link
//        _context.StyleExampleLinks.Remove(existingLink);
//        await _context.SaveChangesAsync(cancellationToken);
        
//        return Result.Ok();
//    }
//    catch (Exception ex)
//    {
//        return Result.Fail($"Failed to remove link: {ex.Message}");
//    }
//}

