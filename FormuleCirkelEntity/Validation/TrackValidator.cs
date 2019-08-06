using FluentValidation;

namespace FormuleCirkelEntity.Models.ModelValidation
{
    public class TrackValidator : AbstractValidator<Track>
    {
        public TrackValidator()
        {
            RuleFor(t => t.Name).NotEmpty().WithMessage("Name is required!");
            RuleFor(t => t.Location).NotEmpty().WithMessage("Location is required!");
        }
    }
}