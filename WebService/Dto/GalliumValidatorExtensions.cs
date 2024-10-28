using FluentValidation;

namespace GalliumPlus.WebService.Dto;

public static class GalliumValidatorExtensions
{
    public static IRuleBuilderOptions<T, string?> MustBeAValidUrl<T>(this IRuleBuilder<T, string?> ruleBuilder)
    {
        return ruleBuilder.Must(prop =>
        {
            if (prop is null)
            {
                return true;
            }

            try
            {
                var uri = new Uri(prop);
                return uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps;
            }
            catch (UriFormatException)
            {
                return false;
            }
        }).WithMessage("{PropertyValue} is not a valid HTTP(S) URL.");
    }
    
    public static IRuleBuilderOptions<T, string?> MaxLength<T>(this IRuleBuilder<T, string?> ruleBuilder, int length)
    {
        return ruleBuilder.Length(0, length);
    }
}