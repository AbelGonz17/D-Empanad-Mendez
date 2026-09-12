using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DMendez.Domain.Entities;

namespace DMendez.Domain.Interfaces
{
    public interface ICategoryRepository : IGenericRepository<Category>
    {
        Task<IReadOnlyList<Category>> GetActiveCategoriesAsync(CancellationToken cancellationToken = default);
    }
}
