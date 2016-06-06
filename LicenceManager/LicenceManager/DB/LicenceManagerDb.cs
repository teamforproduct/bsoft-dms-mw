using System.Data.Entity;


namespace LicenceManager.DB
{

    public class LicenceManagerDb: DbContext
    {
        //default
        public LicenceManagerDb(): base("name=LicenceDBConnectionString")
        {
            Database.SetInitializer<LicenceManagerDb>(new DbInitialize());
        }

        public LicenceManagerDb(string connectionString):base(connectionString)
        {
            Database.SetInitializer<LicenceManagerDb>(new DbInitialize());
        }

        public virtual DbSet<ClientsInfo> ClientsInfos { get; set; }
        public virtual DbSet<LicenceType> LicenceTypes { get; set; }
        public virtual DbSet<ClientsLicence> ClientsLicences { get; set; }
    }
}