
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


        public static bool operator == (DatabaseModelForAdminContext obj1, DatabaseModelForAdminContext obj2)
        {
            if (ReferenceEquals(obj1, obj2))
            {
                return true;
            }

            if (ReferenceEquals(obj1, null))
            {
                return false;
            }
            if (ReferenceEquals(obj2, null))
            {
                return false;
            }

            return obj1.Equals(obj2);
        }

        public static bool operator !=(DatabaseModelForAdminContext obj1, DatabaseModelForAdminContext obj2)
        {
            return !(obj1 == obj2);
        }

        public override bool Equals(object Obj)
        {
            DatabaseModelForAdminContext other = (DatabaseModelForAdminContext)Obj;
            if (other == null) return false;

            return ClientId == other.ClientId && ServerType == other.ServerType && ((ConnectionString == other.ConnectionString) 
                || (UserName == other.UserName && UserPassword == other.UserPassword && Address == other.Address && DefaultDatabase == other.DefaultDatabase) );
        }
    }
}