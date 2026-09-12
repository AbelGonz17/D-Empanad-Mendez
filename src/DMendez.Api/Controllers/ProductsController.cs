using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DMendez.Application.DTOs.Products;
using DMendez.Application.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DMendez.Api.Controllers
{
    /// <summary>
    /// Gestión del catálogo de productos y control de existencias en inventario.
    /// </summary>
    public class ProductsController : BaseApiController
    {
        private readonly IProductService _productService;

        public ProductsController(IProductService productService)
        {
            _productService = productService;
        }

        /// <summary>
        /// Obtiene la lista de productos, con opción de filtrado por categoría.
        /// </summary>
        /// <param name="categoryId">Identificador opcional de categoría para filtrar.</param>
        /// <param name="cancellationToken">Token de cancelación.</param>
        /// <response code="200">Lista de productos disponibles.</response>
        [HttpGet]
        [ProducesResponseType(typeof(IReadOnlyList<ProductDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IReadOnlyList<ProductDto>>> GetAll([FromQuery] Guid? categoryId, CancellationToken cancellationToken)
        {
            var result = categoryId.HasValue
                ? await _productService.GetByCategoryIdAsync(categoryId.Value, cancellationToken)
                : await _productService.GetAllAsync(cancellationToken);

            return HandleResult(result);
        }

        /// <summary>
        /// Obtiene únicamente los productos activos para el menú público.
        /// </summary>
        /// <param name="cancellationToken">Token de cancelación.</param>
        /// <response code="200">Lista de productos activos.</response>
        [HttpGet("active")]
        [ProducesResponseType(typeof(IReadOnlyList<ProductDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IReadOnlyList<ProductDto>>> GetActive(CancellationToken cancellationToken)
        {
            var result = await _productService.GetActiveAsync(cancellationToken);
            return HandleResult(result);
        }

        /// <summary>
        /// Obtiene un producto por su identificador único junto a su disponibilidad de stock.
        /// </summary>
        /// <param name="id">Identificador único del producto (GUID).</param>
        /// <param name="cancellationToken">Token de cancelación.</param>
        /// <response code="200">Producto encontrado con detalles de inventario.</response>
        /// <response code="404">Si el producto no existe.</response>
        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(ProductDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ProductDto>> GetById(Guid id, CancellationToken cancellationToken)
        {
            var result = await _productService.GetByIdAsync(id, cancellationToken);
            return HandleResult(result);
        }

        /// <summary>
        /// Crea un nuevo producto con su información de precio, categoría e inventario inicial.
        /// </summary>
        /// <param name="dto">Datos para la creación del producto.</param>
        /// <param name="cancellationToken">Token de cancelación.</param>
        /// <response code="201">Producto creado exitosamente.</response>
        /// <response code="400">Si los datos proporcionados son inválidos o incompletos.</response>
        [HttpPost]
        [ProducesResponseType(typeof(ProductDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ProductDto>> Create([FromBody] CreateProductDto dto, CancellationToken cancellationToken)
        {
            var result = await _productService.CreateAsync(dto, cancellationToken);
            return HandleCreatedResult(result, nameof(GetById), new { id = result.Value?.Id });
        }

        /// <summary>
        /// Actualiza la información comercial de un producto (nombre, descripción, precio, categoría e imagen).
        /// </summary>
        /// <param name="id">Identificador del producto a actualizar.</param>
        /// <param name="dto">Nuevos datos del producto.</param>
        /// <param name="cancellationToken">Token de cancelación.</param>
        /// <response code="200">Producto actualizado con éxito.</response>
        /// <response code="400">Si los datos enviados son inválidos.</response>
        /// <response code="404">Si el producto no existe.</response>
        [HttpPut("{id:guid}")]
        [ProducesResponseType(typeof(ProductDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ProductDto>> Update(Guid id, [FromBody] UpdateProductDto dto, CancellationToken cancellationToken)
        {
            var result = await _productService.UpdateAsync(id, dto, cancellationToken);
            return HandleResult(result);
        }

        /// <summary>
        /// Ajusta el inventario de un producto (ingreso o retiro de existencias).
        /// </summary>
        /// <param name="id">Identificador único del producto.</param>
        /// <param name="dto">Cantidad a sumar (positiva) o restar (negativa).</param>
        /// <param name="cancellationToken">Token de cancelación.</param>
        /// <response code="200">Stock actualizado exitosamente.</response>
        /// <response code="400">Si no hay suficiente stock disponible para reducir.</response>
        /// <response code="404">Si el producto no existe.</response>
        [HttpPatch("{id:guid}/stock")]
        [ProducesResponseType(typeof(ProductDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ProductDto>> UpdateStock(Guid id, [FromBody] UpdateStockDto dto, CancellationToken cancellationToken)
        {
            var result = await _productService.UpdateStockAsync(id, dto, cancellationToken);
            return HandleResult(result);
        }

        /// <summary>
        /// Activa un producto para que esté disponible para pedidos.
        /// </summary>
        /// <param name="id">Identificador del producto.</param>
        /// <param name="cancellationToken">Token de cancelación.</param>
        /// <response code="204">Producto activado con éxito.</response>
        /// <response code="404">Si el producto no existe.</response>
        [HttpPatch("{id:guid}/activate")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> Activate(Guid id, CancellationToken cancellationToken)
        {
            var result = await _productService.ActivateAsync(id, cancellationToken);
            return HandleResult(result);
        }

        /// <summary>
        /// Desactiva temporalmente un producto del catálogo.
        /// </summary>
        /// <param name="id">Identificador del producto.</param>
        /// <param name="cancellationToken">Token de cancelación.</param>
        /// <response code="204">Producto desactivado con éxito.</response>
        /// <response code="404">Si el producto no existe.</response>
        [HttpPatch("{id:guid}/deactivate")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> Deactivate(Guid id, CancellationToken cancellationToken)
        {
            var result = await _productService.DeactivateAsync(id, cancellationToken);
            return HandleResult(result);
        }

        /// <summary>
        /// Elimina permanentemente un producto y su inventario asociado.
        /// </summary>
        /// <param name="id">Identificador único del producto a eliminar.</param>
        /// <param name="cancellationToken">Token de cancelación.</param>
        /// <response code="204">Producto eliminado satisfactoriamente.</response>
        /// <response code="404">Si el producto no existe.</response>
        [HttpDelete("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> Delete(Guid id, CancellationToken cancellationToken)
        {
            var result = await _productService.DeleteAsync(id, cancellationToken);
            return HandleResult(result);
        }
    }
}
