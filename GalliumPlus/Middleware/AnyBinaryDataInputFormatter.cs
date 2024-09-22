﻿using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Net.Http.Headers;

namespace GalliumPlus.WebApi.Middleware
{
    public class AnyBinaryDataInputFormatter : InputFormatter
    {

        public AnyBinaryDataInputFormatter()
        {
            SupportedMediaTypes.Add(new MediaTypeHeaderValue("application/octet-stream"));
            SupportedMediaTypes.Add(new MediaTypeHeaderValue("image/*"));
        }

        public async override Task<InputFormatterResult> ReadRequestBodyAsync(InputFormatterContext context)
        {
            using (MemoryStream ms = new MemoryStream(100_000))
            {
                await context.HttpContext.Request.BodyReader.CopyToAsync(ms);
                return await InputFormatterResult.SuccessAsync(ms.ToArray());
            }
        }

        protected override bool CanReadType(Type type)
        {
            if (type == typeof(byte[]))
                return true;
            else
                return false;
        }
    }
}
