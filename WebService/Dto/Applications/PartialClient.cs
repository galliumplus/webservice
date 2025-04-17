using FluentValidation;

namespace GalliumPlus.WebService.Dto.Applications;

public class PartialClient
{
    public string Name { get; set; } = "";
    public uint Allowed { get; set; }
    public uint Granted { get; set; }
    public bool IsEnabled { get; set; }
    
    public bool HasAppAccess { get; set; }
    public PartialSameSignOn? SameSignOn { get; set; }
    
    public class Validator : AbstractValidator<PartialClient>
    {
        public Validator()
        {
            this.RuleFor(client => client.Name).NotEmpty().MaxLength(50);
            this.RuleFor(client => client.SameSignOn).SetValidator(new PartialSameSignOn.Validator()!);
        }
    }
}