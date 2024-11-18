using FeedService.Domain.Norce;
using FluentValidation;

namespace FeedService.Domain.Validation;

public class ProductValidator : AbstractValidator<NorceFeedProductDto>
{
    public ProductValidator(string cultureCode)
    {
        RuleFor(product => product.PrimaryCategory)
            .Must(primaryCategory => primaryCategory?.Cultures.Any(culture => culture.CultureCode.Equals(cultureCode)) == true)
            .WithMessage($"Missing primary category for culture {cultureCode}");
    }
}