using BL.Model.Database;

namespace BL.Model.SystemCore.InternalModel
{
    public class InternalServer
    {
        public int Id { get; set; }
        public string Address { get; set; }
        public string Name { get; set; }
        public DatabaseType ServerType { get; set; }
        public string DefaultDatabase { get; set; }
        public bool IntegrateSecurity { get; set; }
        public string UserName { get; set; }
        public string UserPassword { get; set; }
        public string ConnectionString { get; set; }
    }
}