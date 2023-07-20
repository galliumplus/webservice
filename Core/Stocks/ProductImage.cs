using ImageMagick;
namespace GalliumPlus.WebApi.Core.Stocks
{
    /// <summary>
    /// Une image normalisée au format PNG de 400x400 pixels.
    /// </summary>
    public class ProductImage
    {
        // association des types MIME aux formats ImageMagick
        private static readonly Dictionary<string, MagickFormat> acceptedFormats
            = new Dictionary<string, MagickFormat>
            {
                ["image/png"] = MagickFormat.Png,
                ["image/jpeg"] = MagickFormat.Jpeg,
                ["image/bmp"] = MagickFormat.Bmp,
                ["image/webp"] = MagickFormat.WebP,
            };

        /// <summary>
        /// Les formats acceptés pour les images de produits.
        /// </summary>
        public static string[] AcceptedFormats => acceptedFormats.Keys.ToArray();

        /// <summary>
        /// Le format préféré pour les images de produits.
        /// </summary>
        public static string PreferredFormat => "image/png";


        private byte[] data;

        /// <summary>
        /// L'image sous forme d'un tableau de bytes.
        /// </summary>
        /// <remarks>
        /// L'image est au format PNG et d'une taille de 400x400 pixels.
        /// </remarks>
        public byte[] Bytes => data;

        /// <summary>
        /// L'image sous forme d'un flux en lecture seule.
        /// </summary>
        /// <remarks>
        /// L'image est au format PNG et d'une taille de 400x400 pixels.
        /// </remarks>
        public Stream Stream => new MemoryStream(data, writable: false);

        private ProductImage(byte[] data)
        {
            this.data = data;
        }

        /// <summary>
        /// Crée une nouvelle image à partir de n'importe quel format et taille.
        /// </summary>
        /// <param name="imageData">Les données encodées de l'image.</param>
        /// <param name="imageType">Le type MIME de <paramref name="imageData"/>.</param>
        public static async Task<ProductImage> FromAnyImage(byte[] imageData, string imageType)
        {
            MagickImage image;
            if (acceptedFormats.TryGetValue(imageType, out MagickFormat imageFormat))
            {
                image = new MagickImage(imageData, imageFormat);
            }
            else
            {
                image = new MagickImage(imageData);
            }

            image.BackgroundColor = MagickColor.FromRgba(0, 0, 0, 0); // transparent
            image.Thumbnail(400, 400);
            image.Extent(400, 400, Gravity.Center);
            image.Format = MagickFormat.Png;

            using MemoryStream output = new();
            await image.WriteAsync(output);
            return new ProductImage(output.GetBuffer());
        }

        /// <summary>
        /// Recrée une image à partir de données persistées.
        /// </summary>
        /// <remarks>
        /// Les données fournies doivent être une image PNG de 400 par 400 pixels.
        /// </remarks>
        /// <param name="data">Les données encodées de l'image.</param>
        public static ProductImage FromStoredData(byte[] data)
        {
            return new ProductImage(data);
        }
    }
}
