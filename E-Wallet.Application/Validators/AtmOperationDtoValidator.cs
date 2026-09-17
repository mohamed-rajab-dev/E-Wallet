using E_Wallet.Application.DTOs;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace E_Wallet.Application.Validators
{
    public class AtmOperationDtoValidator : AbstractValidator<AtmOperationDto>
    {
        public AtmOperationDtoValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("Invalid email format.");
            RuleFor(x => x.DynamicOtp)
                .NotEmpty().WithMessage("Dynamic OTP is required.")
                .Length(6).WithMessage("Dynamic OTP must be 6 digits.");
            RuleFor(x => x.Amount)
                .GreaterThan(0).WithMessage("Amount must be greater than zero.");
        }
    }
}
