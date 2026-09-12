using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DMendez.Application.DTOs.Orders;
using DMendez.Domain.Enums;

namespace DMendez.Application.Interfaces
{
    public interface IOrderService
    {
        Task<IReadOnlyList<OrderDto>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<IReadOnlyList<OrderDto>> GetByUserIdAsync(string userId, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<OrderDto>> GetByStatusAsync(OrderStatus status, CancellationToken cancellationToken = default);
        Task<OrderDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<OrderDto> CreateAsync(CreateOrderDto dto, CancellationToken cancellationToken = default);
        Task<bool> UpdateStatusAsync(Guid id, UpdateOrderStatusDto dto, CancellationToken cancellationToken = default);
        Task<bool> CancelAsync(Guid id, CancellationToken cancellationToken = default);
    }
}
