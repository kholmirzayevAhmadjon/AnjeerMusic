﻿using AnjeerMusic.Domain.Commons;

namespace AnjeerMusic.Data.Repositories;

public interface IRepository<TEntity> where TEntity : Auditable
{
    Task<TEntity> InsertAsync(TEntity entity);
    Task<TEntity> UpdateAsync(TEntity entity);
    Task<TEntity> DeleteAsync(TEntity entity);
    Task<TEntity> SelectByIdAsync(long id);
    IEnumerable<TEntity> SelectAllAsEnumerable();
    IQueryable<TEntity> SelectAllAsQueryable();
    Task SaveAsync();
}
