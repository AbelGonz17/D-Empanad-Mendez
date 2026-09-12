using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
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

        public async Task<IReadOnlyList<ProductDto>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            var products = await _productRepository.GetAllAsync(cancellationToken);
            var result = new List<ProductDto>();
            foreach (var product in products)
            {
                var inventory = await _inventoryRepository.GetByProductIdAsync(product.Id, cancellationToken);
                result.Add(product.ToDto(inventory));
            }
            return result;
        }

        public async Task<IReadOnlyList<ProductDto>> GetActiveAsync(CancellationToken cancellationToken = default)
        {
            var products = await _productRepository.GetActiveProductsAsync(cancellationToken);
            var result = new List<ProductDto>();
            foreach (var product in products)
            {
                var inventory = await _inventoryRepository.GetByProductIdAsync(product.Id, cancellationToken);
                result.Add(product.ToDto(inventory));
            }
            return result;
        }

        public async Task<IReadOnlyList<ProductDto>> GetByCategoryIdAsync(Guid categoryId, CancellationToken cancellationToken = default)
        {
            var products = await _productRepository.GetByCategoryIdAsync(categoryId, cancellationToken);
            var result = new List<ProductDto>();
            foreach (var product in products)
            {
                var inventory = await _inventoryRepository.GetByProductIdAsync(product.Id, cancellationToken);
                result.Add(product.ToDto(inventory));
            }
            return result;
        }

        public async Task<ProductDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var product = await _productRepository.GetByIdAsync(id, cancellationToken);
            if (product == null)
                return null;

            var inventory = await _inventoryRepository.GetByProductIdAsync(product.Id, cancellationToken);
            return product.ToDto(inventory);
        }

        public async Task<ProductDto> CreateAsync(CreateProductDto dto, CancellationToken cancellationToken = default)
        {
            var product = dto.ToEntity();
            await _productRepository.AddAsync(product, cancellationToken);

            ProductInventory? inventory = null;
            if (dto.InitialStock >= 0)
            {
                inventory = new ProductInventory(product.Id, dto.InitialStock);
                await _inventoryRepository.AddAsync(inventory, cancellationToken);
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return product.ToDto(inventory);
        }

        public async Task<ProductDto?> UpdateAsync(Guid id, UpdateProductDto dto, CancellationToken cancellationToken = default)
        {
            var product = await _productRepository.GetByIdAsync(id, cancellationToken);
            if (product == null)
                return null;

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
            return product.ToDto(inventory);
        }

        public async Task<bool> UpdateStockAsync(Guid id, UpdateStockDto dto, CancellationToken cancellationToken = default)
        {
            var product = await _productRepository.GetByIdAsync(id, cancellationToken);
            if (product == null)
                return false;

            var inventory = await _inventoryRepository.GetByProductIdAsync(product.Id, cancellationToken);
            if (inventory == null)
            {
                inventory = new ProductInventory(product.Id, Math.Max(0, dto.Quantity));
                await _inventoryRepository.AddAsync(inventory, cancellationToken);
            }
            else
            {
                inventory.AddStock(dto.Quantity);
                _inventoryRepository.Update(inventory);
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return true;
        }

        public async Task<bool> ActivateAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var product = await _productRepository.GetByIdAsync(id, cancellationToken);
            if (product == null)
                return false;

            product.Activate();
            _productRepository.Update(product);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return true;
        }

        public async Task<bool> DeactivateAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var product = await _productRepository.GetByIdAsync(id, cancellationToken);
            if (product == null)
                return false;

            product.Desactive();
            _productRepository.Update(product);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return true;
        }

        public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var product = await _productRepository.GetByIdAsync(id, cancellationToken);
            if (product == null)
                return false;

            var inventory = await _inventoryRepository.GetByProductIdAsync(product.Id, cancellationToken);
            if (inventory != null)
            {
                _inventoryRepository.Delete(inventory);
            }

            _productRepository.Delete(product);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}
