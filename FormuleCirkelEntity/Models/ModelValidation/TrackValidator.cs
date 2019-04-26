using FluentValidation;

namespace FormuleCirkelEntity.Models.ModelValidation
{
    public class TrackValidator : AbstractValidator<Track>
    {
        public TrackValidator()
        {
            RuleFor(t => t.Name).NotEmpty().WithMessage("Naam van circuit is verplicht!");
            RuleFor(t => t.Location).NotEmpty().WithMessage("Locatie voor circuit is verplicht!");
        }
    }
}
