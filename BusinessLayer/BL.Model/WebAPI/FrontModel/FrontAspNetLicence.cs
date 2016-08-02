namespace BL.Model.WebAPI.FrontModel
{
    public class FrontAspNetLicence
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public bool IsDateLimit { get; set; }

        public bool IsConcurenteLicence { get; set; }

        public bool IsNamedLicence { get; set; }

        public bool IsFunctionals { get; set; }


        public int? NamedNumberOfConnections { get; set; }
        public int? ConcurenteNumberOfConnections { get; set; }

        public int? DateLimit { get; set; }

        public string Functionals { get; set; }
    }
}