using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DMendez.Application.Common.Models;
using DMendez.Application.DTOs.Categories;
using DMendez.Application.Interfaces;
using DMendez.Application.Mappings;
using DMendez.Domain.Interfaces;

namespace DMendez.Application.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IUnitOfWork _unitOfWork;

        public CategoryService(ICategoryRepository categoryRepository, IUnitOfWork unitOfWork)
        {
            _categoryRepository = categoryRepository ?? throw new ArgumentNullException(nameof(categoryRepository));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        public async Task<Result<IReadOnlyList<CategoryDto>>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            var categories = await _categoryRepository.GetAllAsync(cancellationToken);
            return Result.Success(categories.ToDtoList());
        }

        public async Task<Result<IReadOnlyList<CategoryDto>>> GetActiveAsync(CancellationToken cancellationToken = default)
        {
            var categories = await _categoryRepository.GetActiveCategoriesAsync(cancellationToken);
            return Result.Success(categories.ToDtoList());
        }

        public async Task<Result<CategoryDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var category = await _categoryRepository.GetByIdAsync(id, cancellationToken);
            if (category == null)
                return Result.NotFound<CategoryDto>($"No se encontró la categoría con ID '{id}'.");

            return Result.Success(category.ToDto());
        }

        public async Task<Result<CategoryDto>> CreateAsync(CreateCategoryDto dto, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(dto.Name))
                return Result.Failure<CategoryDto>("El nombre de la categoría es requerido.");

            var category = dto.ToEntity();
            await _categoryRepository.AddAsync(category, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return Result.Success(category.ToDto());
        }

        public async Task<Result<CategoryDto>> UpdateAsync(Guid id, UpdateCategoryDto dto, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(dto.Name))
                return Result.Failure<CategoryDto>("El nombre de la categoría es requerido.");

            var category = await _categoryRepository.GetByIdAsync(id, cancellationToken);
            if (category == null)
                return Result.NotFound<CategoryDto>($"No se encontró la categoría con ID '{id}'.");

            category.Rename(dto.Name);
            if (dto.ImageUrl != null)
            {
                category.UpdateImage(dto.ImageUrl);
            }

            _categoryRepository.Update(category);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return Result.Success(category.ToDto());
        }

        public async Task<Result> ActivateAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var category = await _categoryRepository.GetByIdAsync(id, cancellationToken);
            if (category == null)
                return Result.NotFound($"No se encontró la categoría con ID '{id}'.");

            category.Activate();
            _categoryRepository.Update(category);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return Result.Success();
        }

        public async Task<Result> DeactivateAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var category = await _categoryRepository.GetByIdAsync(id, cancellationToken);
            if (category == null)
                return Result.NotFound($"No se encontró la categoría con ID '{id}'.");

            category.Desactive();
            _categoryRepository.Update(category);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return Result.Success();
        }

        public async Task<Result> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var category = await _categoryRepository.GetByIdAsync(id, cancellationToken);
            if (category == null)
                return Result.NotFound($"No se encontró la categoría con ID '{id}'.");

            _categoryRepository.Delete(category);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return Result.Success();
        }
    }
}
