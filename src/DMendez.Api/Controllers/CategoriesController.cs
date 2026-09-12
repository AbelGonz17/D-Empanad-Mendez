using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DMendez.Application.DTOs.Categories;
using DMendez.Application.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DMendez.Api.Controllers
{
    /// <summary>
    /// Gestión de categorías de productos en el catálogo.
    /// </summary>
    public class CategoriesController : BaseApiController
    {
        private readonly ICategoryService _categoryService;

        public CategoriesController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        /// <summary>
        /// Obtiene todas las categorías registradas.
        /// </summary>
        /// <param name="cancellationToken">Token de cancelación.</param>
        /// <response code="200">Lista completa de categorías.</response>
        [HttpGet]
        [ProducesResponseType(typeof(IReadOnlyList<CategoryDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IReadOnlyList<CategoryDto>>> GetAll(CancellationToken cancellationToken)
        {
            var result = await _categoryService.GetAllAsync(cancellationToken);
            return HandleResult(result);
        }

        /// <summary>
        /// Obtiene únicamente las categorías activas para el menú público.
        /// </summary>
        /// <param name="cancellationToken">Token de cancelación.</param>
        /// <response code="200">Lista de categorías activas.</response>
        [HttpGet("active")]
        [ProducesResponseType(typeof(IReadOnlyList<CategoryDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IReadOnlyList<CategoryDto>>> GetActive(CancellationToken cancellationToken)
        {
            var result = await _categoryService.GetActiveAsync(cancellationToken);
            return HandleResult(result);
        }

        /// <summary>
        /// Obtiene los datos detallados de una categoría por su identificador único.
        /// </summary>
        /// <param name="id">Identificador único de la categoría (GUID).</param>
        /// <param name="cancellationToken">Token de cancelación.</param>
        /// <response code="200">Categoría encontrada.</response>
        /// <response code="404">Si la categoría no existe.</response>
        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(CategoryDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<CategoryDto>> GetById(Guid id, CancellationToken cancellationToken)
        {
            var result = await _categoryService.GetByIdAsync(id, cancellationToken);
            return HandleResult(result);
        }

        /// <summary>
        /// Crea una nueva categoría de productos.
        /// </summary>
        /// <param name="dto">Datos para la creación de la categoría.</param>
        /// <param name="cancellationToken">Token de cancelación.</param>
        /// <response code="201">Categoría creada exitosamente con su URL de acceso.</response>
        /// <response code="400">Si los datos proporcionados son inválidos.</response>
        [HttpPost]
        [ProducesResponseType(typeof(CategoryDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<CategoryDto>> Create([FromBody] CreateCategoryDto dto, CancellationToken cancellationToken)
        {
            var result = await _categoryService.CreateAsync(dto, cancellationToken);
            return HandleCreatedResult(result, nameof(GetById), new { id = result.Value?.Id });
        }

        /// <summary>
        /// Actualiza los datos de una categoría existente.
        /// </summary>
        /// <param name="id">Identificador único de la categoría a actualizar.</param>
        /// <param name="dto">Nuevos datos de la categoría.</param>
        /// <param name="cancellationToken">Token de cancelación.</param>
        /// <response code="200">Categoría actualizada con éxito.</response>
        /// <response code="400">Si los datos enviados son inválidos.</response>
        /// <response code="404">Si la categoría no existe.</response>
        [HttpPut("{id:guid}")]
        [ProducesResponseType(typeof(CategoryDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<CategoryDto>> Update(Guid id, [FromBody] UpdateCategoryDto dto, CancellationToken cancellationToken)
        {
            var result = await _categoryService.UpdateAsync(id, dto, cancellationToken);
            return HandleResult(result);
        }

        /// <summary>
        /// Activa una categoría para que esté visible en el catálogo.
        /// </summary>
        /// <param name="id">Identificador único de la categoría.</param>
        /// <param name="cancellationToken">Token de cancelación.</param>
        /// <response code="204">Categoría activada con éxito.</response>
        /// <response code="404">Si la categoría no existe.</response>
        [HttpPatch("{id:guid}/activate")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> Activate(Guid id, CancellationToken cancellationToken)
        {
            var result = await _categoryService.ActivateAsync(id, cancellationToken);
            return HandleResult(result);
        }

        /// <summary>
        /// Desactiva una categoría temporalmente.
        /// </summary>
        /// <param name="id">Identificador único de la categoría.</param>
        /// <param name="cancellationToken">Token de cancelación.</param>
        /// <response code="204">Categoría desactivada con éxito.</response>
        /// <response code="404">Si la categoría no existe.</response>
        [HttpPatch("{id:guid}/deactivate")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> Deactivate(Guid id, CancellationToken cancellationToken)
        {
            var result = await _categoryService.DeactivateAsync(id, cancellationToken);
            return HandleResult(result);
        }

        /// <summary>
        /// Elimina permanentemente una categoría del sistema.
        /// </summary>
        /// <param name="id">Identificador único de la categoría a eliminar.</param>
        /// <param name="cancellationToken">Token de cancelación.</param>
        /// <response code="204">Categoría eliminada satisfactoriamente.</response>
        /// <response code="404">Si la categoría no existe.</response>
        [HttpDelete("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> Delete(Guid id, CancellationToken cancellationToken)
        {
            var result = await _categoryService.DeleteAsync(id, cancellationToken);
            return HandleResult(result);
        }
    }
}
