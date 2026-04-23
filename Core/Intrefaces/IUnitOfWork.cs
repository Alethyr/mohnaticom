using Core.Entities;

namespace Core.Intrefaces;

public interface IUnitOfWork
{
  IGenericRepository<TEntity> Repository<TEntity>() where TEntity : BaseEntity;
  Task<bool> Complete();
}
