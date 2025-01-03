using FeedService.Domain.Norce;
using FluentValidation;

namespace FeedService.Domain.Validation;

public class VariantValidator : AbstractValidator<NorceFeedVariant>
{
    public VariantValidator(string cultureCode)
    {
        RuleFor(variant => variant.Names)
            .Must(names => names.Any(name => name.CultureCode.Equals(cultureCode)))
            .WithMessage($"Missing name for culture {cultureCode}");
    }
}