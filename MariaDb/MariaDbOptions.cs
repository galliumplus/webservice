namespace GalliumPlus.WebApi.Data.MariaDb
{
    public class MariaDbOptions
    {
        public string Host { get; set; }

        public string UserId { get; set; }

        public string Password { get; set; }

        public string Schema { get; set; }

        public uint Port { get; set; }

        public MariaDbOptions()
        {
            this.Host = "localhost";
            this.UserId = "";
            this.Password = "";
            this.Schema = "galliumplus";
            this.Port = 3306;
        }
    }
}
