using FluentValidation;
using DoctorApp.DTOs.Requests;

namespace DoctorApp.Validators;

/// <summary>
/// Validadores para DTOs de Doctor
/// </summary>
public class RegistrarDoctorValidator : AbstractValidator<DoctorRequestDto>
{
    public RegistrarDoctorValidator()
    {
        RuleFor(x => x.Nombre)
            .NotEmpty().WithMessage("El nombre es requerido")
            .MinimumLength(2).WithMessage("El nombre debe tener al menos 2 caracteres")
            .MaximumLength(100).WithMessage("El nombre no puede exceder 100 caracteres");

        RuleFor(x => x.Apellido)
            .NotEmpty().WithMessage("El apellido es requerido")
            .MinimumLength(2).WithMessage("El apellido debe tener al menos 2 caracteres")
            .MaximumLength(100).WithMessage("El apellido no puede exceder 100 caracteres");

        RuleFor(x => x.Especialidad)
            .NotEmpty().WithMessage("La especialidad es requerida")
            .MinimumLength(3).WithMessage("La especialidad debe tener al menos 3 caracteres")
            .MaximumLength(100).WithMessage("La especialidad no puede exceder 100 caracteres");

        RuleFor(x => x.Email)
            .EmailAddress().WithMessage("El email debe ser válido")
            .When(x => !string.IsNullOrEmpty(x.Email));

        RuleFor(x => x.Telefono)
            .Matches(@"^\+?[0-9\s\-()]{10,}$").WithMessage("El teléfono debe ser válido")
            .When(x => !string.IsNullOrEmpty(x.Telefono));
    }
}

public class DoctorLoginValidator : AbstractValidator<DoctorLoginRequestDto>
{
    public DoctorLoginValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("El email es requerido")
            .EmailAddress().WithMessage("El email debe ser válido");

        RuleFor(x => x.Contrasena)
            .NotEmpty().WithMessage("La contraseña es requerida")
            .MinimumLength(6).WithMessage("La contraseña debe tener al menos 6 caracteres");
    }
}
