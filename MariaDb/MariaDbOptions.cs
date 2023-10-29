namespace GalliumPlus.WebApi.Data.MariaDb
{
    public class MariaDbOptions
    {
        public string Host { get; set; } = "localhost";

        public string UserId { get; set; } = "";

        public string Password { get; set; } = "";

        public string Schema { get; set; } = "galliumplus";

        public uint Port { get; set; } = 3306;
    }
}
