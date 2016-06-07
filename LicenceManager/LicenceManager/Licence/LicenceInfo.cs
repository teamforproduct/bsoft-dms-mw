namespace LicenceManager.Licence
{
    public class LicenceInfo
    {
        public int LicenceId { get; set; }
        public string ClientName { get; set; }
        public int? NamedNumberOfConnections { get; set; }
        public int? ConcurenteNumberOfConnections { get; set; }
        public int? DateLimit { get; set; }
        public string Functionals { get; set; }
        public string LicenceKey { get; set; }
    }
}
