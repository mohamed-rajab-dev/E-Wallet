using E_Wallet.Application.Interfaces.Services;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace E_Wallet.Application.Validators
{
    public class TransferDtoValidator : AbstractValidator<TransferDto>
    {
        public TransferDtoValidator() 
        {
            RuleFor(x => x.ReceiverEmail)
                .NotEmpty().WithMessage("Receiver email is required.")
                .EmailAddress().WithMessage("Invalid email format.");

            RuleFor(x => x.Amount)
                .GreaterThan(0).WithMessage("Amount must be greater than zero.");

            RuleFor(x => x.SenderPassword)
                .NotEmpty().WithMessage("Password is required")
                .MinimumLength(8)
                .Matches(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^a-zA-Z0-9]).+$").WithMessage("Password is incorrect");
        }
    }
}
