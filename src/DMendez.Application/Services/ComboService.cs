using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DMendez.Application.Common.Models;
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

        public async Task<Result<IReadOnlyList<ComboDto>>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            var combos = await _comboRepository.GetAllAsync(cancellationToken);
            return Result.Success(combos.ToDtoList());
        }

        public async Task<Result<IReadOnlyList<ComboDto>>> GetActiveAsync(CancellationToken cancellationToken = default)
        {
            var combos = await _comboRepository.GetActiveCombosWithItemsAsync(cancellationToken);
            return Result.Success(combos.ToDtoList());
        }

        public async Task<Result<ComboDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var combo = await _comboRepository.GetComboWithItemsAsync(id, cancellationToken);
            if (combo == null)
                return Result.NotFound<ComboDto>($"No se encontró el combo con ID '{id}'.");

            return Result.Success(combo.ToDto());
        }

        public async Task<Result<ComboDto>> CreateAsync(CreateComboDto dto, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(dto.Name))
                return Result.Failure<ComboDto>("El nombre del combo es requerido.");

            if (dto.Price < 0)
                return Result.Failure<ComboDto>("El precio del combo no puede ser negativo.");

            var combo = new Combo(dto.Name, dto.Price);
            if (dto.ImageUrl != null)
            {
                combo.UpdateImage(dto.ImageUrl);
            }

            foreach (var item in dto.Items)
            {
                if (item.Quantity <= 0)
                    return Result.Failure<ComboDto>("La cantidad de cada ítem debe ser mayor que cero.");

                combo.AddItem(item.ProductId, item.Quantity);
            }

            if (dto.Items.Count > 0)
            {
                combo.Activate();
            }

            await _comboRepository.AddAsync(combo, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return Result.Success(combo.ToDto());
        }

        public async Task<Result<ComboDto>> UpdateAsync(Guid id, UpdateComboDto dto, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(dto.Name))
                return Result.Failure<ComboDto>("El nombre del combo es requerido.");

            if (dto.Price < 0)
                return Result.Failure<ComboDto>("El precio del combo no puede ser negativo.");

            var combo = await _comboRepository.GetComboWithItemsAsync(id, cancellationToken);
            if (combo == null)
                return Result.NotFound<ComboDto>($"No se encontró el combo con ID '{id}'.");

            combo.Rename(dto.Name);
            combo.ChangePrice(dto.Price);
            if (dto.ImageUrl != null)
            {
                combo.UpdateImage(dto.ImageUrl);
            }

            _comboRepository.Update(combo);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return Result.Success(combo.ToDto());
        }

        public async Task<Result<ComboDto>> AddItemAsync(Guid id, AddComboItemDto dto, CancellationToken cancellationToken = default)
        {
            if (dto.Quantity <= 0)
                return Result.Failure<ComboDto>("La cantidad a agregar debe ser mayor que cero.");

            var combo = await _comboRepository.GetComboWithItemsAsync(id, cancellationToken);
            if (combo == null)
                return Result.NotFound<ComboDto>($"No se encontró el combo con ID '{id}'.");

            combo.AddItem(dto.ProductId, dto.Quantity);
            _comboRepository.Update(combo);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return Result.Success(combo.ToDto());
        }

        public async Task<Result<ComboDto>> RemoveItemAsync(Guid id, Guid productId, CancellationToken cancellationToken = default)
        {
            var combo = await _comboRepository.GetComboWithItemsAsync(id, cancellationToken);
            if (combo == null)
                return Result.NotFound<ComboDto>($"No se encontró el combo con ID '{id}'.");

            combo.RemoveItem(productId);
            _comboRepository.Update(combo);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return Result.Success(combo.ToDto());
        }

        public async Task<Result> ActivateAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var combo = await _comboRepository.GetComboWithItemsAsync(id, cancellationToken);
            if (combo == null)
                return Result.NotFound($"No se encontró el combo con ID '{id}'.");

            combo.Activate();
            _comboRepository.Update(combo);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return Result.Success();
        }

        public async Task<Result> DeactivateAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var combo = await _comboRepository.GetComboWithItemsAsync(id, cancellationToken);
            if (combo == null)
                return Result.NotFound($"No se encontró el combo con ID '{id}'.");

            combo.Deactivate();
            _comboRepository.Update(combo);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return Result.Success();
        }

        public async Task<Result> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var combo = await _comboRepository.GetComboWithItemsAsync(id, cancellationToken);
            if (combo == null)
                return Result.NotFound($"No se encontró el combo con ID '{id}'.");

            _comboRepository.Delete(combo);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return Result.Success();
        }
    }
}
