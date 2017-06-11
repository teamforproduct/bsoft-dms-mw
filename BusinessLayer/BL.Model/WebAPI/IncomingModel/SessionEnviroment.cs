namespace DMS_WebAPI.DBModel
{
    public class SessionEnviroment
    {
        public string Session { get; set; }

        public string IP { get; set; }

        public string Platform { get; set; }

        public string Browser { get; set; }

        public string Fingerprint { get; set; }
    }
}