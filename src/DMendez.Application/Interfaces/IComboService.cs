using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DMendez.Application.DTOs.Combos;

namespace DMendez.Application.Interfaces
{
    public interface IComboService
    {
        Task<IReadOnlyList<ComboDto>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<IReadOnlyList<ComboDto>> GetActiveAsync(CancellationToken cancellationToken = default);
        Task<ComboDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<ComboDto> CreateAsync(CreateComboDto dto, CancellationToken cancellationToken = default);
        Task<ComboDto?> UpdateAsync(Guid id, UpdateComboDto dto, CancellationToken cancellationToken = default);
        Task<ComboDto?> AddItemAsync(Guid id, AddComboItemDto dto, CancellationToken cancellationToken = default);
        Task<ComboDto?> RemoveItemAsync(Guid id, Guid productId, CancellationToken cancellationToken = default);
        Task<bool> ActivateAsync(Guid id, CancellationToken cancellationToken = default);
        Task<bool> DeactivateAsync(Guid id, CancellationToken cancellationToken = default);
        Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    }
}
