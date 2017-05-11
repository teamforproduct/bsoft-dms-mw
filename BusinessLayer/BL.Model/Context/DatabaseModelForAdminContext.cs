
namespace BL.Model.Context
{
    /// <summary>
    /// Model describe server parameters
    /// </summary>
    public class DatabaseModelForAdminContext : DatabaseModel
    {
        public DatabaseModelForAdminContext() { }
        public DatabaseModelForAdminContext(DatabaseModel model)
        {
            Id = model.Id;
            Address = model.Address;
            Name = model.Address;
            ServerType = model.ServerType;
            ServerTypeName = model.ServerTypeName;
            DefaultDatabase = model.DefaultDatabase;
            IntegrateSecurity = model.IntegrateSecurity;
            UserName = model.UserName;
            UserPassword = model.UserPassword;
            ConnectionString = model.ConnectionString;
            DefaultSchema = model.DefaultSchema;
        }

        public int ClientId { get; set; }
        public string ClientCode { get; set; }
    }
}