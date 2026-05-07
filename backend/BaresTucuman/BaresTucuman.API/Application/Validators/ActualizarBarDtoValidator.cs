using FluentValidation;
using BaresTucuman.API.Application.DTOs;

namespace BaresTucuman.API.Application.Validators
{
    public class ActualizarBarDtoValidator : AbstractValidator<ActualizarBarDto>
    {
        public ActualizarBarDtoValidator()
        {
            RuleFor(x => x.Nombre)
                .NotEmpty().WithMessage("El nombre del bar es obligatorio.")
                .MaximumLength(100).WithMessage("El nombre no puede superar los 100 caracteres.");

            RuleFor(x => x.Ubicacion)
                .NotEmpty().WithMessage("La ubicación es obligatoria.")
                .MaximumLength(200).WithMessage("La ubicación no puede superar los 200 caracteres.");

            RuleFor(x => x.Categoria)
                .MaximumLength(50).WithMessage("La categoría original no puede superar los 50 caracteres.");

            RuleFor(x => x.CategoriaAMostrar)
                .IsInEnum().WithMessage("La categoría para el sistema no es válida.");

            RuleFor(x => x.AiDescription)
                .MaximumLength(500).WithMessage("La descripción de la IA no puede superar los 500 caracteres.");
        }
    }
}