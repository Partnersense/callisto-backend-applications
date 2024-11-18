using FeedService.Domain.Norce;
using FluentValidation;

namespace FeedService.Domain.Validation;

public class VariantValidator : AbstractValidator<NorceFeedVariant>
{
    public VariantValidator(string pricelistCode, string cultureCode)
    {
        RuleFor(variant => variant.Prices)
            .Must(prices => prices.Any(price => price.PriceListCode.Equals(pricelistCode)))
            .WithMessage($"Missing price for price list {pricelistCode}");

        RuleFor(variant => variant.Names)
            .Must(names => names.Any(name => name.CultureCode.Equals(cultureCode)))
            .WithMessage($"Missing name for culture {cultureCode}");
    }
}