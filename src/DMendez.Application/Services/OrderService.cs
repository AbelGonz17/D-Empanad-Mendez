using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DMendez.Application.Common.Models;
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

        public async Task<Result<IReadOnlyList<OrderDto>>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            var orders = await _orderRepository.GetAllAsync(cancellationToken);
            return Result.Success(orders.ToDtoList());
        }

        public async Task<Result<IReadOnlyList<OrderDto>>> GetByUserIdAsync(string userId, CancellationToken cancellationToken = default)
        {
            var orders = await _orderRepository.GetOrdersByUserIdAsync(userId, cancellationToken);
            return Result.Success(orders.ToDtoList());
        }

        public async Task<Result<IReadOnlyList<OrderDto>>> GetByStatusAsync(OrderStatus status, CancellationToken cancellationToken = default)
        {
            var orders = await _orderRepository.GetOrdersByStatusAsync(status, cancellationToken);
            return Result.Success(orders.ToDtoList());
        }

        public async Task<Result<OrderDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var order = await _orderRepository.GetOrderWithItemsAsync(id, cancellationToken);
            if (order == null)
                return Result.NotFound<OrderDto>($"No se encontró el pedido con ID '{id}'.");

            return Result.Success(order.ToDto());
        }

        public async Task<Result<OrderDto>> CreateAsync(CreateOrderDto dto, CancellationToken cancellationToken = default)
        {
            if (dto.Items == null || dto.Items.Count == 0)
                return Result.Failure<OrderDto>("El pedido debe contener al menos un ítem.");

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
                        return Result.NotFound<OrderDto>($"El producto con ID '{itemDto.ProductId}' no existe.");
                    }
                    unitPrice = product.Price;
                }
                else if (itemDto.ComboId.HasValue)
                {
                    var combo = await _comboRepository.GetByIdAsync(itemDto.ComboId.Value, cancellationToken);
                    if (combo == null)
                    {
                        return Result.NotFound<OrderDto>($"El combo con ID '{itemDto.ComboId}' no existe.");
                    }
                    unitPrice = combo.Price;
                }
                else
                {
                    return Result.Failure<OrderDto>("Cada ítem debe tener un producto o un combo asignado.");
                }

                order.AddItem(itemDto.ProductId, itemDto.ComboId, itemDto.Quantity, unitPrice);
            }

            await _orderRepository.AddAsync(order, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // Notificación externa (correo) con plantilla HTML
            try
            {
                var htmlBody = $@"
                    <div style='font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto; padding: 20px; border: 1px solid #e0e0e0; border-radius: 8px;'>
                        <h2 style='color: #d32f2f;'>¡Gracias por tu pedido en D'Empanadas Méndez!</h2>
                        <p>Hola,</p>
                        <p>Hemos recibido tu pedido correctamente. A continuación los detalles:</p>
                        <div style='background-color: #f9f9f9; padding: 15px; border-radius: 6px; margin: 15px 0;'>
                            <p style='margin: 5px 0;'><strong>Número de Pedido:</strong> #{order.Id}</p>
                            <p style='margin: 5px 0;'><strong>Dirección de Entrega:</strong> {order.DeliveryAddress}</p>
                            <p style='margin: 5px 0;'><strong>Costo de Envío:</strong> ${order.DeliveryFee:F2}</p>
                            <p style='margin: 5px 0; font-size: 1.1em; color: #d32f2f;'><strong>Total a Pagar:</strong> ${order.TotalAmount:F2}</p>
                        </div>
                        <p>Te notificaremos cuando tu pedido esté en camino.</p>
                    </div>";

                await _emailService.SendEmailAsync(
                    dto.UserId, 
                    $"Confirmación de Pedido #{order.Id}", 
                    htmlBody, 
                    isHtml: true, 
                    cancellationToken: cancellationToken);
            }
            catch
            {
                // La falla en el envío de correo no debe anular la creación de la orden
            }

            return Result.Success(order.ToDto());
        }

        public async Task<Result<OrderDto>> UpdateStatusAsync(Guid id, UpdateOrderStatusDto dto, CancellationToken cancellationToken = default)
        {
            var order = await _orderRepository.GetOrderWithItemsAsync(id, cancellationToken);
            if (order == null)
                return Result.NotFound<OrderDto>($"No se encontró el pedido con ID '{id}'.");

            order.UpdateStatus(dto.Status);
            _orderRepository.Update(order);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            try
            {
                var htmlBody = $@"
                    <div style='font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto; padding: 20px; border: 1px solid #e0e0e0; border-radius: 8px;'>
                        <h2 style='color: #d32f2f;'>Actualización de Pedido</h2>
                        <p>El estado de tu pedido <strong>#{order.Id}</strong> ha cambiado a:</p>
                        <div style='text-align: center; margin: 20px 0;'>
                            <span style='background-color: #d32f2f; color: white; padding: 10px 18px; border-radius: 20px; font-weight: bold; font-size: 1.1em;'>{dto.Status}</span>
                        </div>
                        <p>¡Gracias por preferir D'Empanadas Méndez!</p>
                    </div>";

                await _emailService.SendEmailAsync(
                    order.UserId, 
                    $"Actualización de Pedido #{order.Id}", 
                    htmlBody, 
                    isHtml: true, 
                    cancellationToken: cancellationToken);
            }
            catch
            {
                // La falla en el correo no debe anular la actualización de la orden
            }

            return Result.Success(order.ToDto());
        }

        public async Task<Result> CancelAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var order = await _orderRepository.GetOrderWithItemsAsync(id, cancellationToken);
            if (order == null)
                return Result.NotFound($"No se encontró el pedido con ID '{id}'.");

            if (order.Status == OrderStatus.Delivered || order.Status == OrderStatus.Cancelled)
                return Result.Conflict($"No se puede cancelar un pedido con estado '{order.Status}'.");

            order.UpdateStatus(OrderStatus.Cancelled);
            _orderRepository.Update(order);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            try
            {
                var htmlBody = $@"
                    <div style='font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto; padding: 20px; border: 1px solid #e0e0e0; border-radius: 8px;'>
                        <h2 style='color: #d32f2f;'>Cancelación de Pedido</h2>
                        <p>Tu pedido <strong>#{order.Id}</strong> ha sido cancelado.</p>
                        <p>Si tienes alguna consulta, por favor contáctanos directamente.</p>
                    </div>";

                await _emailService.SendEmailAsync(
                    order.UserId, 
                    $"Cancelación de Pedido #{order.Id}", 
                    htmlBody, 
                    isHtml: true, 
                    cancellationToken: cancellationToken);
            }
            catch
            {
                // La falla en el correo no debe anular la cancelación de la orden
            }

            return Result.Success();
        }
    }
}
