using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SmartAssistant.Shared.Interfaces
{
	public interface IRepository<T>
	{
		Task<T> GetByIdAsync(int id);
		Task<IEnumerable<T>> GetAllAsync();
        System.Threading.Tasks.Task AddAsync(T entity);
		System.Threading.Tasks.Task UpdateAsync(T entity);
        System.Threading.Tasks.Task DeleteAsync(T entity);
	}
}
