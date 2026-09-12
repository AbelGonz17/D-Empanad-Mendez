using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DMendez.Domain.Entities;
using DMendez.Domain.Enums;

namespace DMendez.Domain.Interfaces
{
    public interface IOrderRepository : IGenericRepository<Order>
    {
        Task<Order?> GetOrderWithItemsAsync(Guid orderId, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<Order>> GetOrdersByUserIdAsync(string userId, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<Order>> GetOrdersByStatusAsync(OrderStatus status, CancellationToken cancellationToken = default);
    }
}
