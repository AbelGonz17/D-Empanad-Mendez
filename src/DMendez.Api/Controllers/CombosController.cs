using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DMendez.Application.DTOs.Combos;
using DMendez.Application.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DMendez.Api.Controllers
{
    /// <summary>
    /// Gestión de paquetes promocionales o combos de empanadas y bebidas.
    /// </summary>
    public class CombosController : BaseApiController
    {
        private readonly IComboService _comboService;

        public CombosController(IComboService comboService)
        {
            _comboService = comboService;
        }

        /// <summary>
        /// Obtiene todos los combos registrados junto a la lista de ítems que los componen.
        /// </summary>
        /// <param name="cancellationToken">Token de cancelación.</param>
        /// <response code="200">Lista de combos.</response>
        [HttpGet]
        [ProducesResponseType(typeof(IReadOnlyList<ComboDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IReadOnlyList<ComboDto>>> GetAll(CancellationToken cancellationToken)
        {
            var result = await _comboService.GetAllAsync(cancellationToken);
            return HandleResult(result);
        }

        /// <summary>
        /// Obtiene únicamente los combos activos para los clientes.
        /// </summary>
        /// <param name="cancellationToken">Token de cancelación.</param>
        /// <response code="200">Lista de combos activos.</response>
        [HttpGet("active")]
        [ProducesResponseType(typeof(IReadOnlyList<ComboDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IReadOnlyList<ComboDto>>> GetActive(CancellationToken cancellationToken)
        {
            var result = await _comboService.GetActiveAsync(cancellationToken);
            return HandleResult(result);
        }

        /// <summary>
        /// Obtiene un combo por su identificador único, incluyendo sus productos y cantidades.
        /// </summary>
        /// <param name="id">Identificador único del combo (GUID).</param>
        /// <param name="cancellationToken">Token de cancelación.</param>
        /// <response code="200">Combo encontrado.</response>
        /// <response code="404">Si el combo no existe.</response>
        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(ComboDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ComboDto>> GetById(Guid id, CancellationToken cancellationToken)
        {
            var result = await _comboService.GetByIdAsync(id, cancellationToken);
            return HandleResult(result);
        }

        /// <summary>
        /// Crea un nuevo combo con precio especial y productos asociados.
        /// </summary>
        /// <param name="dto">Datos para la creación del combo.</param>
        /// <param name="cancellationToken">Token de cancelación.</param>
        /// <response code="201">Combo creado exitosamente.</response>
        /// <response code="400">Si los datos proporcionados son inválidos.</response>
        [HttpPost]
        [ProducesResponseType(typeof(ComboDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ComboDto>> Create([FromBody] CreateComboDto dto, CancellationToken cancellationToken)
        {
            var result = await _comboService.CreateAsync(dto, cancellationToken);
            return HandleCreatedResult(result, nameof(GetById), new { id = result.Value?.Id });
        }

        /// <summary>
        /// Actualiza los datos generales de un combo (nombre, precio e imagen).
        /// </summary>
        /// <param name="id">Identificador del combo a actualizar.</param>
        /// <param name="dto">Nuevos datos del combo.</param>
        /// <param name="cancellationToken">Token de cancelación.</param>
        /// <response code="200">Combo actualizado con éxito.</response>
        /// <response code="400">Si los datos son inválidos.</response>
        /// <response code="404">Si el combo no existe.</response>
        [HttpPut("{id:guid}")]
        [ProducesResponseType(typeof(ComboDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ComboDto>> Update(Guid id, [FromBody] UpdateComboDto dto, CancellationToken cancellationToken)
        {
            var result = await _comboService.UpdateAsync(id, dto, cancellationToken);
            return HandleResult(result);
        }

        /// <summary>
        /// Agrega un producto adicional a un combo existente.
        /// </summary>
        /// <param name="id">Identificador del combo.</param>
        /// <param name="dto">Producto y cantidad a añadir.</param>
        /// <param name="cancellationToken">Token de cancelación.</param>
        /// <response code="200">Combo con el ítem agregado.</response>
        /// <response code="400">Si la cantidad es inválida.</response>
        /// <response code="404">Si el combo o producto no existen.</response>
        [HttpPost("{id:guid}/items")]
        [ProducesResponseType(typeof(ComboDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ComboDto>> AddItem(Guid id, [FromBody] AddComboItemDto dto, CancellationToken cancellationToken)
        {
            var result = await _comboService.AddItemAsync(id, dto, cancellationToken);
            return HandleResult(result);
        }

        /// <summary>
        /// Remueve un producto perteneciente al combo.
        /// </summary>
        /// <param name="id">Identificador del combo.</param>
        /// <param name="productId">Identificador del producto a remover del combo.</param>
        /// <param name="cancellationToken">Token de cancelación.</param>
        /// <response code="200">Combo actualizado tras la remoción.</response>
        /// <response code="404">Si el combo no existe.</response>
        [HttpDelete("{id:guid}/items/{productId:guid}")]
        [ProducesResponseType(typeof(ComboDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ComboDto>> RemoveItem(Guid id, Guid productId, CancellationToken cancellationToken)
        {
            var result = await _comboService.RemoveItemAsync(id, productId, cancellationToken);
            return HandleResult(result);
        }

        /// <summary>
        /// Activa un combo para exhibición y pedidos.
        /// </summary>
        /// <param name="id">Identificador del combo.</param>
        /// <param name="cancellationToken">Token de cancelación.</param>
        /// <response code="204">Combo activado exitosamente.</response>
        /// <response code="404">Si el combo no existe.</response>
        [HttpPatch("{id:guid}/activate")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> Activate(Guid id, CancellationToken cancellationToken)
        {
            var result = await _comboService.ActivateAsync(id, cancellationToken);
            return HandleResult(result);
        }

        /// <summary>
        /// Desactiva temporalmente un combo.
        /// </summary>
        /// <param name="id">Identificador del combo.</param>
        /// <param name="cancellationToken">Token de cancelación.</param>
        /// <response code="204">Combo desactivado exitosamente.</response>
        /// <response code="404">Si el combo no existe.</response>
        [HttpPatch("{id:guid}/deactivate")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> Deactivate(Guid id, CancellationToken cancellationToken)
        {
            var result = await _comboService.DeactivateAsync(id, cancellationToken);
            return HandleResult(result);
        }

        /// <summary>
        /// Elimina permanentemente un combo del catálogo.
        /// </summary>
        /// <param name="id">Identificador del combo a eliminar.</param>
        /// <param name="cancellationToken">Token de cancelación.</param>
        /// <response code="204">Combo eliminado satisfactoriamente.</response>
        /// <response code="404">Si el combo no existe.</response>
        [HttpDelete("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> Delete(Guid id, CancellationToken cancellationToken)
        {
            var result = await _comboService.DeleteAsync(id, cancellationToken);
            return HandleResult(result);
        }
    }
}
