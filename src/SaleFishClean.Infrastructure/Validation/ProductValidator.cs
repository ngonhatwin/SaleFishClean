using FluentValidation;
using SaleFishClean.Domains.Entities;


namespace SaleFishClean.Infrastructure.Validation
{
    public class ProductValidator : AbstractValidator<Product>
    {
        public ProductValidator()
        {
            RuleFor(p => p.ProductName).NotEmpty().WithMessage("Product name must not be empty.");
            RuleFor(p => p.Price).GreaterThan(0).WithMessage("Price must be greater than zero.");
        }
    }
}
