using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DMendez.Application.Common.Models;
using DMendez.Application.DTOs.Combos;

namespace DMendez.Application.Interfaces
{
    public interface IComboService
    {
        Task<Result<IReadOnlyList<ComboDto>>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<Result<IReadOnlyList<ComboDto>>> GetActiveAsync(CancellationToken cancellationToken = default);
        Task<Result<ComboDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<Result<ComboDto>> CreateAsync(CreateComboDto dto, CancellationToken cancellationToken = default);
        Task<Result<ComboDto>> UpdateAsync(Guid id, UpdateComboDto dto, CancellationToken cancellationToken = default);
        Task<Result<ComboDto>> AddItemAsync(Guid id, AddComboItemDto dto, CancellationToken cancellationToken = default);
        Task<Result<ComboDto>> RemoveItemAsync(Guid id, Guid productId, CancellationToken cancellationToken = default);
        Task<Result> ActivateAsync(Guid id, CancellationToken cancellationToken = default);
        Task<Result> DeactivateAsync(Guid id, CancellationToken cancellationToken = default);
        Task<Result> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    }
}
