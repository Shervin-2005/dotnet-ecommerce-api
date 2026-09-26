using Application.DTOs.Review;
using FluentValidation;

namespace Application.Validators.Review;

public class UpdateReviewDtoValidator : AbstractValidator<UpdateReviewDto>
{
    public UpdateReviewDtoValidator()
    {
        RuleFor(x => x.Rating)
            .InclusiveBetween(1, 5)
            .WithMessage("Rating must be between 1 and 5.");

        RuleFor(x => x.Comment)
            .MaximumLength(2000)
            .WithMessage("Comment cannot be longer than 2000 characters.")
            .When(x => x.Comment is not null);
    }
}