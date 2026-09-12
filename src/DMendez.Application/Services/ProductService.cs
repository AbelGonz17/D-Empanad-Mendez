using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DMendez.Application.Common.Models;
using DMendez.Application.DTOs.Products;
using DMendez.Application.Interfaces;
using DMendez.Application.Mappings;
using DMendez.Domain.Entities;
using DMendez.Domain.Interfaces;

namespace DMendez.Application.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;
        private readonly IProductInventoryRepository _inventoryRepository;
        private readonly IUnitOfWork _unitOfWork;

        public ProductService(
            IProductRepository productRepository, 
            IProductInventoryRepository inventoryRepository, 
            IUnitOfWork unitOfWork)
        {
            _productRepository = productRepository ?? throw new ArgumentNullException(nameof(productRepository));
            _inventoryRepository = inventoryRepository ?? throw new ArgumentNullException(nameof(inventoryRepository));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        public async Task<Result<IReadOnlyList<ProductDto>>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            var products = await _productRepository.GetAllAsync(cancellationToken);
            var result = new List<ProductDto>();
            foreach (var product in products)
            {
                var inventory = await _inventoryRepository.GetByProductIdAsync(product.Id, cancellationToken);
                result.Add(product.ToDto(inventory));
            }
            return Result.Success<IReadOnlyList<ProductDto>>(result);
        }

        public async Task<Result<IReadOnlyList<ProductDto>>> GetActiveAsync(CancellationToken cancellationToken = default)
        {
            var products = await _productRepository.GetActiveProductsAsync(cancellationToken);
            var result = new List<ProductDto>();
            foreach (var product in products)
            {
                var inventory = await _inventoryRepository.GetByProductIdAsync(product.Id, cancellationToken);
                result.Add(product.ToDto(inventory));
            }
            return Result.Success<IReadOnlyList<ProductDto>>(result);
        }

        public async Task<Result<IReadOnlyList<ProductDto>>> GetByCategoryIdAsync(Guid categoryId, CancellationToken cancellationToken = default)
        {
            var products = await _productRepository.GetByCategoryIdAsync(categoryId, cancellationToken);
            var result = new List<ProductDto>();
            foreach (var product in products)
            {
                var inventory = await _inventoryRepository.GetByProductIdAsync(product.Id, cancellationToken);
                result.Add(product.ToDto(inventory));
            }
            return Result.Success<IReadOnlyList<ProductDto>>(result);
        }

        public async Task<Result<ProductDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var product = await _productRepository.GetByIdAsync(id, cancellationToken);
            if (product == null)
                return Result.NotFound<ProductDto>($"No se encontró el producto con ID '{id}'.");

            var inventory = await _inventoryRepository.GetByProductIdAsync(product.Id, cancellationToken);
            return Result.Success(product.ToDto(inventory));
        }

        public async Task<Result<ProductDto>> CreateAsync(CreateProductDto dto, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(dto.Name))
                return Result.Failure<ProductDto>("El nombre del producto es requerido.");

            if (dto.Price < 0)
                return Result.Failure<ProductDto>("El precio del producto no puede ser negativo.");

            var product = dto.ToEntity();
            await _productRepository.AddAsync(product, cancellationToken);

            ProductInventory? inventory = null;
            if (dto.InitialStock >= 0)
            {
                inventory = new ProductInventory(product.Id, dto.InitialStock);
                await _inventoryRepository.AddAsync(inventory, cancellationToken);
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return Result.Success(product.ToDto(inventory));
        }

        public async Task<Result<ProductDto>> UpdateAsync(Guid id, UpdateProductDto dto, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(dto.Name))
                return Result.Failure<ProductDto>("El nombre del producto es requerido.");

            if (dto.Price < 0)
                return Result.Failure<ProductDto>("El precio del producto no puede ser negativo.");

            var product = await _productRepository.GetByIdAsync(id, cancellationToken);
            if (product == null)
                return Result.NotFound<ProductDto>($"No se encontró el producto con ID '{id}'.");

            product.UpdateDetails(dto.Name, dto.Description);
            product.ChangePrice(dto.Price);
            product.ChangeCategory(dto.CategoryId);
            if (dto.ImageUrl != null)
            {
                product.UpdateImage(dto.ImageUrl);
            }

            _productRepository.Update(product);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var inventory = await _inventoryRepository.GetByProductIdAsync(product.Id, cancellationToken);
            return Result.Success(product.ToDto(inventory));
        }

        public async Task<Result<ProductDto>> UpdateStockAsync(Guid id, UpdateStockDto dto, CancellationToken cancellationToken = default)
        {
            var product = await _productRepository.GetByIdAsync(id, cancellationToken);
            if (product == null)
                return Result.NotFound<ProductDto>($"No se encontró el producto con ID '{id}'.");

            var inventory = await _inventoryRepository.GetByProductIdAsync(product.Id, cancellationToken);
            if (inventory == null)
            {
                if (dto.Quantity < 0)
                    return Result.Failure<ProductDto>("El inventario inicial no puede ser negativo.");

                inventory = new ProductInventory(product.Id, dto.Quantity);
                await _inventoryRepository.AddAsync(inventory, cancellationToken);
            }
            else
            {
                if (dto.Quantity > 0)
                {
                    inventory.ReplenishStock(dto.Quantity);
                }
                else if (dto.Quantity < 0)
                {
                    var absQuantity = Math.Abs(dto.Quantity);
                    if (absQuantity > inventory.AvaibleQuantity)
                        return Result.Failure<ProductDto>($"Stock insuficiente. Las unidades disponibles son {inventory.AvaibleQuantity} y se intentó reducir en {absQuantity}.");

                    inventory.ReduceStock(absQuantity);
                }

                _inventoryRepository.Update(inventory);
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return Result.Success(product.ToDto(inventory));
        }

        public async Task<Result> ActivateAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var product = await _productRepository.GetByIdAsync(id, cancellationToken);
            if (product == null)
                return Result.NotFound($"No se encontró el producto con ID '{id}'.");

            product.Activate();
            _productRepository.Update(product);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return Result.Success();
        }

        public async Task<Result> DeactivateAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var product = await _productRepository.GetByIdAsync(id, cancellationToken);
            if (product == null)
                return Result.NotFound($"No se encontró el producto con ID '{id}'.");

            product.Desactive();
            _productRepository.Update(product);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return Result.Success();
        }

        public async Task<Result> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var product = await _productRepository.GetByIdAsync(id, cancellationToken);
            if (product == null)
                return Result.NotFound($"No se encontró el producto con ID '{id}'.");

            var inventory = await _inventoryRepository.GetByProductIdAsync(product.Id, cancellationToken);
            if (inventory != null)
            {
                _inventoryRepository.Delete(inventory);
            }

            _productRepository.Delete(product);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return Result.Success();
        }
    }
}
