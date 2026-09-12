using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DMendez.Application.DTOs.DeliveryZones;
using DMendez.Application.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DMendez.Api.Controllers
{
    /// <summary>
    /// Gestión de zonas geográficas de cobertura y tarifas de entrega a domicilio.
    /// </summary>
    public class DeliveryZonesController : BaseApiController
    {
        private readonly IDeliveryZoneService _deliveryZoneService;

        public DeliveryZonesController(IDeliveryZoneService deliveryZoneService)
        {
            _deliveryZoneService = deliveryZoneService;
        }

        /// <summary>
        /// Obtiene todas las zonas de entrega registradas.
        /// </summary>
        /// <param name="cancellationToken">Token de cancelación.</param>
        /// <response code="200">Lista completa de zonas de entrega.</response>
        [HttpGet]
        [ProducesResponseType(typeof(IReadOnlyList<DeliveryZoneDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IReadOnlyList<DeliveryZoneDto>>> GetAll(CancellationToken cancellationToken)
        {
            var result = await _deliveryZoneService.GetAllAsync(cancellationToken);
            return HandleResult(result);
        }

        /// <summary>
        /// Obtiene únicamente las zonas de entrega activas para pedidos a domicilio.
        /// </summary>
        /// <param name="cancellationToken">Token de cancelación.</param>
        /// <response code="200">Lista de zonas activas.</response>
        [HttpGet("active")]
        [ProducesResponseType(typeof(IReadOnlyList<DeliveryZoneDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IReadOnlyList<DeliveryZoneDto>>> GetActive(CancellationToken cancellationToken)
        {
            var result = await _deliveryZoneService.GetActiveAsync(cancellationToken);
            return HandleResult(result);
        }

        /// <summary>
        /// Obtiene los detalles de una zona de entrega por su identificador único.
        /// </summary>
        /// <param name="id">Identificador único de la zona (GUID).</param>
        /// <param name="cancellationToken">Token de cancelación.</param>
        /// <response code="200">Zona de entrega encontrada.</response>
        /// <response code="404">Si la zona no fue encontrada.</response>
        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(DeliveryZoneDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<DeliveryZoneDto>> GetById(Guid id, CancellationToken cancellationToken)
        {
            var result = await _deliveryZoneService.GetByIdAsync(id, cancellationToken);
            return HandleResult(result);
        }

        /// <summary>
        /// Registra una nueva zona de entrega con su tarifa base de envío.
        /// </summary>
        /// <param name="dto">Datos de la zona de entrega.</param>
        /// <param name="cancellationToken">Token de cancelación.</param>
        /// <response code="201">Zona creada exitosamente.</response>
        /// <response code="400">Si los datos proporcionados son inválidos.</response>
        [HttpPost]
        [ProducesResponseType(typeof(DeliveryZoneDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<DeliveryZoneDto>> Create([FromBody] CreateDeliveryZoneDto dto, CancellationToken cancellationToken)
        {
            var result = await _deliveryZoneService.CreateAsync(dto, cancellationToken);
            return HandleCreatedResult(result, nameof(GetById), new { id = result.Value?.Id });
        }

        /// <summary>
        /// Actualiza el nombre o la tarifa de envío de una zona de entrega.
        /// </summary>
        /// <param name="id">Identificador de la zona a actualizar.</param>
        /// <param name="dto">Nuevos datos de la zona.</param>
        /// <param name="cancellationToken">Token de cancelación.</param>
        /// <response code="200">Zona actualizada con éxito.</response>
        /// <response code="400">Si los datos son inválidos.</response>
        /// <response code="404">Si la zona no existe.</response>
        [HttpPut("{id:guid}")]
        [ProducesResponseType(typeof(DeliveryZoneDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<DeliveryZoneDto>> Update(Guid id, [FromBody] UpdateDeliveryZoneDto dto, CancellationToken cancellationToken)
        {
            var result = await _deliveryZoneService.UpdateAsync(id, dto, cancellationToken);
            return HandleResult(result);
        }

        /// <summary>
        /// Habilita una zona para envíos a domicilio.
        /// </summary>
        /// <param name="id">Identificador de la zona.</param>
        /// <param name="cancellationToken">Token de cancelación.</param>
        /// <response code="204">Zona activada exitosamente.</response>
        /// <response code="404">Si la zona no existe.</response>
        [HttpPatch("{id:guid}/activate")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> Activate(Guid id, CancellationToken cancellationToken)
        {
            var result = await _deliveryZoneService.ActivateAsync(id, cancellationToken);
            return HandleResult(result);
        }

        /// <summary>
        /// Deshabilita temporalmente una zona de envíos.
        /// </summary>
        /// <param name="id">Identificador de la zona.</param>
        /// <param name="cancellationToken">Token de cancelación.</param>
        /// <response code="204">Zona desactivada exitosamente.</response>
        /// <response code="404">Si la zona no existe.</response>
        [HttpPatch("{id:guid}/deactivate")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> Deactivate(Guid id, CancellationToken cancellationToken)
        {
            var result = await _deliveryZoneService.DeactivateAsync(id, cancellationToken);
            return HandleResult(result);
        }

        /// <summary>
        /// Elimina permanentemente una zona de entrega.
        /// </summary>
        /// <param name="id">Identificador de la zona a eliminar.</param>
        /// <param name="cancellationToken">Token de cancelación.</param>
        /// <response code="204">Zona eliminada satisfactoriamente.</response>
        /// <response code="404">Si la zona no existe.</response>
        [HttpDelete("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> Delete(Guid id, CancellationToken cancellationToken)
        {
            var result = await _deliveryZoneService.DeleteAsync(id, cancellationToken);
            return HandleResult(result);
        }
    }
}
