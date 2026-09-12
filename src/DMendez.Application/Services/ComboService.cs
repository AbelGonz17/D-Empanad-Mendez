using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DMendez.Application.DTOs.Combos;
using DMendez.Application.Interfaces;
using DMendez.Application.Mappings;
using DMendez.Domain.Entities;
using DMendez.Domain.Interfaces;

namespace DMendez.Application.Services
{
    public class ComboService : IComboService
    {
        private readonly IComboRepository _comboRepository;
        private readonly IUnitOfWork _unitOfWork;

        public ComboService(IComboRepository comboRepository, IUnitOfWork unitOfWork)
        {
            _comboRepository = comboRepository ?? throw new ArgumentNullException(nameof(comboRepository));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        public async Task<IReadOnlyList<ComboDto>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            var combos = await _comboRepository.GetAllAsync(cancellationToken);
            return combos.ToDtoList();
        }

        public async Task<IReadOnlyList<ComboDto>> GetActiveAsync(CancellationToken cancellationToken = default)
        {
            var combos = await _comboRepository.GetActiveCombosWithItemsAsync(cancellationToken);
            return combos.ToDtoList();
        }

        public async Task<ComboDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var combo = await _comboRepository.GetComboWithItemsAsync(id, cancellationToken);
            return combo?.ToDto();
        }

        public async Task<ComboDto> CreateAsync(CreateComboDto dto, CancellationToken cancellationToken = default)
        {
            var combo = new Combo(dto.Name, dto.Price);
            if (dto.ImageUrl != null)
            {
                combo.UpdateImage(dto.ImageUrl);
            }

            foreach (var item in dto.Items)
            {
                combo.AddItem(item.ProductId, item.Quantity);
            }

            if (dto.Items.Count > 0)
            {
                combo.Activate();
            }

            await _comboRepository.AddAsync(combo, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return combo.ToDto();
        }

        public async Task<ComboDto?> UpdateAsync(Guid id, UpdateComboDto dto, CancellationToken cancellationToken = default)
        {
            var combo = await _comboRepository.GetComboWithItemsAsync(id, cancellationToken);
            if (combo == null)
                return null;

            combo.Rename(dto.Name);
            combo.ChangePrice(dto.Price);
            if (dto.ImageUrl != null)
            {
                combo.UpdateImage(dto.ImageUrl);
            }

            _comboRepository.Update(combo);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return combo.ToDto();
        }

        public async Task<ComboDto?> AddItemAsync(Guid id, AddComboItemDto dto, CancellationToken cancellationToken = default)
        {
            var combo = await _comboRepository.GetComboWithItemsAsync(id, cancellationToken);
            if (combo == null)
                return null;

            combo.AddItem(dto.ProductId, dto.Quantity);
            _comboRepository.Update(combo);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return combo.ToDto();
        }

        public async Task<ComboDto?> RemoveItemAsync(Guid id, Guid productId, CancellationToken cancellationToken = default)
        {
            var combo = await _comboRepository.GetComboWithItemsAsync(id, cancellationToken);
            if (combo == null)
                return null;

            combo.RemoveItem(productId);
            _comboRepository.Update(combo);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return combo.ToDto();
        }

        public async Task<bool> ActivateAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var combo = await _comboRepository.GetComboWithItemsAsync(id, cancellationToken);
            if (combo == null)
                return false;

            combo.Activate();
            _comboRepository.Update(combo);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return true;
        }

        public async Task<bool> DeactivateAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var combo = await _comboRepository.GetComboWithItemsAsync(id, cancellationToken);
            if (combo == null)
                return false;

            combo.Deactivate();
            _comboRepository.Update(combo);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return true;
        }

        public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var combo = await _comboRepository.GetComboWithItemsAsync(id, cancellationToken);
            if (combo == null)
                return false;

            _comboRepository.Delete(combo);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}
