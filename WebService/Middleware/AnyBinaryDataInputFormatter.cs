using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Net.Http.Headers;

namespace GalliumPlus.WebService.Middleware;

public class AnyBinaryDataInputFormatter : InputFormatter
{
    public AnyBinaryDataInputFormatter()
    {
        this.SupportedMediaTypes.Add(new MediaTypeHeaderValue("application/octet-stream"));
        this.SupportedMediaTypes.Add(new MediaTypeHeaderValue("image/*"));
    }

    public override async Task<InputFormatterResult> ReadRequestBodyAsync(InputFormatterContext context)
    {
        using var ms = new MemoryStream(100_000);
        await context.HttpContext.Request.BodyReader.CopyToAsync(ms);
        return await InputFormatterResult.SuccessAsync(ms.ToArray());
    }

    protected override bool CanReadType(Type type) => type == typeof(byte[]);
}