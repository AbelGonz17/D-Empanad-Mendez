using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DMendez.Application.DTOs.Orders;
using DMendez.Application.Interfaces;
using DMendez.Domain.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DMendez.Api.Controllers
{
    /// <summary>
    /// Gestión del ciclo de vida de pedidos, estados de entrega y notificaciones automáticas.
    /// </summary>
    public class OrdersController : BaseApiController
    {
        private readonly IOrderService _orderService;

        public OrdersController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        /// <summary>
        /// Obtiene la lista completa de pedidos realizados en el sistema.
        /// </summary>
        /// <param name="cancellationToken">Token de cancelación.</param>
        /// <response code="200">Lista de pedidos.</response>
        [HttpGet]
        [ProducesResponseType(typeof(IReadOnlyList<OrderDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IReadOnlyList<OrderDto>>> GetAll(CancellationToken cancellationToken)
        {
            var result = await _orderService.GetAllAsync(cancellationToken);
            return HandleResult(result);
        }

        /// <summary>
        /// Obtiene el historial de pedidos de un usuario cliente en particular.
        /// </summary>
        /// <param name="userId">Identificador del usuario.</param>
        /// <param name="cancellationToken">Token de cancelación.</param>
        /// <response code="200">Historial de pedidos del cliente.</response>
        [HttpGet("user/{userId}")]
        [ProducesResponseType(typeof(IReadOnlyList<OrderDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IReadOnlyList<OrderDto>>> GetByUserId(string userId, CancellationToken cancellationToken)
        {
            var result = await _orderService.GetByUserIdAsync(userId, cancellationToken);
            return HandleResult(result);
        }

        /// <summary>
        /// Filtra los pedidos por su estado actual (ej. Pendiente, EnPreparación, Entregado).
        /// </summary>
        /// <param name="status">Estado del pedido a consultar.</param>
        /// <param name="cancellationToken">Token de cancelación.</param>
        /// <response code="200">Pedidos filtrados por estado.</response>
        [HttpGet("status/{status}")]
        [ProducesResponseType(typeof(IReadOnlyList<OrderDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IReadOnlyList<OrderDto>>> GetByStatus(OrderStatus status, CancellationToken cancellationToken)
        {
            var result = await _orderService.GetByStatusAsync(status, cancellationToken);
            return HandleResult(result);
        }

        /// <summary>
        /// Obtiene los detalles de un pedido específico por su identificador único.
        /// </summary>
        /// <param name="id">Identificador único del pedido (GUID).</param>
        /// <param name="cancellationToken">Token de cancelación.</param>
        /// <response code="200">Detalles del pedido y sus ítems.</response>
        /// <response code="404">Si el pedido no fue encontrado.</response>
        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(OrderDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<OrderDto>> GetById(Guid id, CancellationToken cancellationToken)
        {
            var result = await _orderService.GetByIdAsync(id, cancellationToken);
            return HandleResult(result);
        }

        /// <summary>
        /// Registra un nuevo pedido y envía el correo de confirmación al cliente.
        /// </summary>
        /// <param name="dto">Datos del pedido (productos, combos, tipo de entrega y dirección).</param>
        /// <param name="cancellationToken">Token de cancelación.</param>
        /// <response code="201">Pedido registrado exitosamente.</response>
        /// <response code="400">Si los datos del pedido o los ítems son inválidos.</response>
        /// <response code="404">Si alguno de los productos o combos solicitados no existe.</response>
        [HttpPost]
        [ProducesResponseType(typeof(OrderDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<OrderDto>> Create([FromBody] CreateOrderDto dto, CancellationToken cancellationToken)
        {
            var result = await _orderService.CreateAsync(dto, cancellationToken);
            return HandleCreatedResult(result, nameof(GetById), new { id = result.Value?.Id });
        }

        /// <summary>
        /// Actualiza el estado de un pedido y notifica al cliente por correo electrónico.
        /// </summary>
        /// <param name="id">Identificador único del pedido.</param>
        /// <param name="dto">Nuevo estado a asignar al pedido.</param>
        /// <param name="cancellationToken">Token de cancelación.</param>
        /// <response code="200">Estado actualizado exitosamente.</response>
        /// <response code="404">Si el pedido no existe.</response>
        [HttpPatch("{id:guid}/status")]
        [ProducesResponseType(typeof(OrderDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<OrderDto>> UpdateStatus(Guid id, [FromBody] UpdateOrderStatusDto dto, CancellationToken cancellationToken)
        {
            var result = await _orderService.UpdateStatusAsync(id, dto, cancellationToken);
            return HandleResult(result);
        }

        /// <summary>
        /// Cancela un pedido existente y notifica al cliente de la cancelación.
        /// </summary>
        /// <param name="id">Identificador único del pedido a cancelar.</param>
        /// <param name="cancellationToken">Token de cancelación.</param>
        /// <response code="204">Pedido cancelado correctamente.</response>
        /// <response code="404">Si el pedido no existe.</response>
        /// <response code="409">Si el pedido ya fue entregado o ya estaba cancelado.</response>
        [HttpPost("{id:guid}/cancel")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult> Cancel(Guid id, CancellationToken cancellationToken)
        {
            var result = await _orderService.CancelAsync(id, cancellationToken);
            return HandleResult(result);
        }
    }
}
