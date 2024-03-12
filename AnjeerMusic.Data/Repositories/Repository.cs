using AnjeerMusic.Data.DbContexts;
using AnjeerMusic.Domain.Commons;
using Microsoft.EntityFrameworkCore;

namespace AnjeerMusic.Data.Repositories;

public class Repository<TEntity> : IRepository<TEntity> where TEntity : Auditable
{
    private readonly AppDbContext context;
    private readonly DbSet<TEntity> entities;
    public Repository(AppDbContext context)
    {
        this.context = context;
        entities = context.Set<TEntity> ();
    }

    public async Task<TEntity> InsertAsync(TEntity entity)
    {
        return (await entities.AddAsync(entity)).Entity;
    }

    public async Task<TEntity> UpdateAsync(TEntity entity)
    {
        entities.Entry(entity).State = EntityState.Modified;
        return await Task.FromResult(entity);
    }

    public async Task<TEntity> DeleteAsync(TEntity entity)
    {
        entities.Entry(entity).State = EntityState.Modified;
        return await Task.FromResult(entity);
    }

    public async Task<TEntity> SelectByIdAsync(long id)
    {
#pragma warning disable CS8603 // Possible null reference return.
        return await entities.FirstOrDefaultAsync(entity => entity.Id == id && !entity.IsDeleted);
#pragma warning restore CS8603 // Possible null reference return.
    }

    public IEnumerable<TEntity> SelectAllAsEnumerable()
    {
        return  entities.AsEnumerable();
    }

    public IQueryable<TEntity> SelectAllAsQueryable()
    {
        return entities.AsQueryable();
    }


    public async Task SaveAsync()
    {
        await context.SaveChangesAsync();
    }
}