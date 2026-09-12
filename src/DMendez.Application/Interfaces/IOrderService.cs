using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DMendez.Application.Common.Models;
using DMendez.Application.DTOs.Orders;
using DMendez.Domain.Enums;

namespace DMendez.Application.Interfaces
{
    public interface IOrderService
    {
        Task<Result<IReadOnlyList<OrderDto>>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<Result<IReadOnlyList<OrderDto>>> GetByUserIdAsync(string userId, CancellationToken cancellationToken = default);
        Task<Result<IReadOnlyList<OrderDto>>> GetByStatusAsync(OrderStatus status, CancellationToken cancellationToken = default);
        Task<Result<OrderDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<Result<OrderDto>> CreateAsync(CreateOrderDto dto, CancellationToken cancellationToken = default);
        Task<Result<OrderDto>> UpdateStatusAsync(Guid id, UpdateOrderStatusDto dto, CancellationToken cancellationToken = default);
        Task<Result> CancelAsync(Guid id, CancellationToken cancellationToken = default);
    }
}
