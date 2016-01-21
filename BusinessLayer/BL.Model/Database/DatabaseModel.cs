namespace BL.Model.Database
{
    public class DatabaseModel
    {
        public string Address { get; set; }
        public DatabaseType ServerType { get; set; }
        public string DefaultDatabase { get; set; }
        public bool IntegrateSecurity { get; set; }
        public string UserName { get; set; }
        public string UserPassword { get; set; }
    }
}