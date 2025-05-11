using FluentValidation;

namespace GalliumPlus.WebService.Dto.Applications;

public class PartialSameSignOn
{
    public uint Scope { get; set; }
    public string? DisplayName { get; set; }
    public string? LogoUrl { get; set; }
    public string RedirectUrl { get; set; } = "";

    public class Validator : AbstractValidator<PartialSameSignOn>
    {
        public Validator()
        {
            this.RuleFor(sso => sso.DisplayName).MaxLength(50);
            this.RuleFor(sso => sso.LogoUrl).MustBeAValidUrl().MaxLength(120);
            this.RuleFor(sso => sso.RedirectUrl).NotEmpty().MustBeAValidUrl().MaxLength(120);
        }   
        
    }
}