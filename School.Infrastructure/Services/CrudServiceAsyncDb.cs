using School.Common;
using School.Infrastructure.Repositories;

namespace School.Infrastructure.Services;

/// <summary>
/// Асинхронний CRUD сервіс, що використовує Repository для доступу до бази даних
/// </summary>
public class CrudServiceAsyncDb<T> : ICrudServiceAsync<T> where T : class
{
    private readonly IRepository<T> _repository;
    private readonly Func<T, Guid> _idSelector;

    public CrudServiceAsyncDb(IRepository<T> repository, Func<T, Guid> idSelector)
    {
        _repository = repository;
        _idSelector = idSelector;
    }

    public async Task<bool> CreateAsync(T element)
    {
        if (element == null)
            return false;

        try
        {
            await _repository.AddAsync(element);
            await _repository.SaveChangesAsync();
            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<T> ReadAsync(Guid id)
    {
        var result = await _repository.GetByIdAsync(id);
        if (result == null)
            throw new KeyNotFoundException($"Елемент з ID {id} не знайдено");
        return result;
    }

    public async Task<IEnumerable<T>> ReadAllAsync()
    {
        return await _repository.GetAllAsync();
    }

    public async Task<IEnumerable<T>> ReadAllAsync(int page, int amount)
    {
        if (page < 1 || amount < 1)
            throw new ArgumentException("Page and amount must be greater than 0");

        var allItems = await _repository.GetAllAsync();
        return allItems
            .Skip((page - 1) * amount)
            .Take(amount)
            .ToList();
    }

    public async Task<bool> UpdateAsync(T element)
    {
        if (element == null)
            return false;

        try
        {
            await _repository.Update(element);
            await _repository.SaveChangesAsync();
            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> RemoveAsync(T element)
    {
        if (element == null)
            return false;

        try
        {
            await _repository.Delete(element);
            await _repository.SaveChangesAsync();
            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> SaveAsync()
    {
        try
        {
            await _repository.SaveChangesAsync();
            return true;
        }
        catch
        {
            return false;
        }
    }

    // Реалізація IEnumerable<T>
    public IEnumerator<T> GetEnumerator()
    {
        return ReadAllAsync().GetAwaiter().GetResult().GetEnumerator();
    }

    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}

