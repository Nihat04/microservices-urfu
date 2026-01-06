using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UserService.Data;
using UserService.Models;

namespace UserService.Repository;

public class RepositoryBase<T> : ControllerBase, IRepository<T> where T : BaseEntity
{
    protected readonly DbContext _context;
    protected DbSet<T> dbSet;
    private readonly IUnitOfWork _unitOfWork;

    public RepositoryBase(IUnitOfWork unitOfwork)
    {
        _unitOfWork = unitOfwork;
        dbSet = _unitOfWork.Context.Set<T>();
    }

    //Get Request
    public async Task<ActionResult<T>> Get(Guid id)
    {
        var data = await dbSet.FindAsync(id);
        return Ok(data);
    }

    //Create Request
    public async Task<ActionResult<T>> Create(T entity)
    {
        dbSet.Add(entity);
        await _unitOfWork.SaveChangesAsync();
        return entity;
    }

    //Update Request
    public async Task<ActionResult> Update(Guid id, T entity)
    {
        var existingOrder = await dbSet.FindAsync(entity.Id);
        if (existingOrder == null)
        {
            return NotFound();
        }

        _unitOfWork.Context.Entry(existingOrder).CurrentValues.SetValues(entity);

        try
        {
            await _unitOfWork.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            throw;
        }

        return NoContent();
    }

    //Delete Request
    public async Task<ActionResult> Delete(Guid id)
    {
        var data = await dbSet.FindAsync(id);
        if (data == null)
        {
            return NotFound();
        }

        dbSet.Remove(data);
        await _unitOfWork.SaveChangesAsync();
        return NoContent();
    }
    
}