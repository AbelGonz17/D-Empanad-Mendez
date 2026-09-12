using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DMendez.Application.DTOs.Orders;
using DMendez.Application.Interfaces;
using DMendez.Application.Interfaces.External;
using DMendez.Application.Mappings;
using DMendez.Domain.Entities;
using DMendez.Domain.Enums;
using DMendez.Domain.Interfaces;

namespace DMendez.Application.Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IProductRepository _productRepository;
        private readonly IComboRepository _comboRepository;
        private readonly IDeliveryZoneRepository _deliveryZoneRepository;
        private readonly IEmailService _emailService;
        private readonly IUnitOfWork _unitOfWork;

        public OrderService(
            IOrderRepository orderRepository,
            IProductRepository productRepository,
            IComboRepository comboRepository,
            IDeliveryZoneRepository deliveryZoneRepository,
            IEmailService emailService,
            IUnitOfWork unitOfWork)
        {
            _orderRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));
            _productRepository = productRepository ?? throw new ArgumentNullException(nameof(productRepository));
            _comboRepository = comboRepository ?? throw new ArgumentNullException(nameof(comboRepository));
            _deliveryZoneRepository = deliveryZoneRepository ?? throw new ArgumentNullException(nameof(deliveryZoneRepository));
            _emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        public async Task<IReadOnlyList<OrderDto>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            var orders = await _orderRepository.GetAllAsync(cancellationToken);
            return orders.ToDtoList();
        }

        public async Task<IReadOnlyList<OrderDto>> GetByUserIdAsync(string userId, CancellationToken cancellationToken = default)
        {
            var orders = await _orderRepository.GetOrdersByUserIdAsync(userId, cancellationToken);
            return orders.ToDtoList();
        }

        public async Task<IReadOnlyList<OrderDto>> GetByStatusAsync(OrderStatus status, CancellationToken cancellationToken = default)
        {
            var orders = await _orderRepository.GetOrdersByStatusAsync(status, cancellationToken);
            return orders.ToDtoList();
        }

        public async Task<OrderDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var order = await _orderRepository.GetOrderWithItemsAsync(id, cancellationToken);
            return order?.ToDto();
        }

        public async Task<OrderDto> CreateAsync(CreateOrderDto dto, CancellationToken cancellationToken = default)
        {
            decimal deliveryFee = 0;
            if (dto.Type == OrderType.Delivery && dto.DeliveryZoneId.HasValue)
            {
                var zone = await _deliveryZoneRepository.GetByIdAsync(dto.DeliveryZoneId.Value, cancellationToken);
                if (zone != null)
                {
                    deliveryFee = zone.DeliveryFee;
                }
            }

            var order = new Order(
                dto.UserId, 
                dto.Type, 
                dto.PaymentMethod, 
                dto.DeliveryAddress, 
                dto.DeliveryZoneId, 
                deliveryFee);

            foreach (var itemDto in dto.Items)
            {
                decimal unitPrice = 0;
                if (itemDto.ProductId.HasValue)
                {
                    var product = await _productRepository.GetByIdAsync(itemDto.ProductId.Value, cancellationToken);
                    if (product == null)
                    {
                        throw new InvalidOperationException($"El producto con id {itemDto.ProductId} no existe.");
                    }
                    unitPrice = product.Price;
                }
                else if (itemDto.ComboId.HasValue)
                {
                    var combo = await _comboRepository.GetByIdAsync(itemDto.ComboId.Value, cancellationToken);
                    if (combo == null)
                    {
                        throw new InvalidOperationException($"El combo con id {itemDto.ComboId} no existe.");
                    }
                    unitPrice = combo.Price;
                }
                else
                {
                    throw new InvalidOperationException("Cada ítem debe tener un producto o un combo asignado.");
                }

                order.AddItem(itemDto.ProductId, itemDto.ComboId, itemDto.Quantity, unitPrice);
            }

            await _orderRepository.AddAsync(order, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // Notificación externa (correo)
            _ = _emailService.SendEmailAsync(
                dto.UserId, 
                $"Confirmación de Pedido #{order.Id}", 
                $"Tu pedido ha sido creado con éxito. Total: ${order.TotalAmount}", 
                false, 
                cancellationToken);

            return order.ToDto();
        }

        public async Task<bool> UpdateStatusAsync(Guid id, UpdateOrderStatusDto dto, CancellationToken cancellationToken = default)
        {
            var order = await _orderRepository.GetOrderWithItemsAsync(id, cancellationToken);
            if (order == null)
                return false;

            order.UpdateStatus(dto.Status);
            _orderRepository.Update(order);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _ = _emailService.SendEmailAsync(
                order.UserId, 
                $"Actualización de Pedido #{order.Id}", 
                $"El estado de tu pedido ahora es: {dto.Status}", 
                false, 
                cancellationToken);

            return true;
        }

        public async Task<bool> CancelAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var order = await _orderRepository.GetOrderWithItemsAsync(id, cancellationToken);
            if (order == null)
                return false;

            order.UpdateStatus(OrderStatus.Cancelled);
            _orderRepository.Update(order);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}
