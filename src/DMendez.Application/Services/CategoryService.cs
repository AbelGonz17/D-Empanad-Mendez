using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
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

        public async Task<IReadOnlyList<CategoryDto>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            var categories = await _categoryRepository.GetAllAsync(cancellationToken);
            return categories.ToDtoList();
        }

        public async Task<IReadOnlyList<CategoryDto>> GetActiveAsync(CancellationToken cancellationToken = default)
        {
            var categories = await _categoryRepository.GetActiveCategoriesAsync(cancellationToken);
            return categories.ToDtoList();
        }

        public async Task<CategoryDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var category = await _categoryRepository.GetByIdAsync(id, cancellationToken);
            return category?.ToDto();
        }

        public async Task<CategoryDto> CreateAsync(CreateCategoryDto dto, CancellationToken cancellationToken = default)
        {
            var category = dto.ToEntity();
            await _categoryRepository.AddAsync(category, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return category.ToDto();
        }

        public async Task<CategoryDto?> UpdateAsync(Guid id, UpdateCategoryDto dto, CancellationToken cancellationToken = default)
        {
            var category = await _categoryRepository.GetByIdAsync(id, cancellationToken);
            if (category == null)
                return null;

            category.Rename(dto.Name);
            if (dto.ImageUrl != null)
            {
                category.UpdateImage(dto.ImageUrl);
            }

            _categoryRepository.Update(category);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return category.ToDto();
        }

        public async Task<bool> ActivateAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var category = await _categoryRepository.GetByIdAsync(id, cancellationToken);
            if (category == null)
                return false;

            category.Activate();
            _categoryRepository.Update(category);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return true;
        }

        public async Task<bool> DeactivateAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var category = await _categoryRepository.GetByIdAsync(id, cancellationToken);
            if (category == null)
                return false;

            category.Desactive();
            _categoryRepository.Update(category);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return true;
        }

        public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var category = await _categoryRepository.GetByIdAsync(id, cancellationToken);
            if (category == null)
                return false;

            _categoryRepository.Delete(category);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}
