using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DMendez.Domain.Entities;

namespace DMendez.Domain.Interfaces
{
    public interface IComboRepository : IGenericRepository<Combo>
    {
        Task<IReadOnlyList<Combo>> GetActiveCombosWithItemsAsync(CancellationToken cancellationToken = default);
        Task<Combo?> GetComboWithItemsAsync(Guid comboId, CancellationToken cancellationToken = default);
    }
}
