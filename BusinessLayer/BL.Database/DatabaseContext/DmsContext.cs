using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using BL.Database.DBModel.Dictionary;
using BL.Database.Helpers;

namespace BL.Database.DatabaseContext
{
    public class DmsContext :DbContext
    {
        public DmsContext() : base(ConnectionStringHelper.GetDefaultConnectionString())
        {
        }

        public DmsContext(string connString) : base(connString)
        {
        }

        public virtual DbSet<DictionaryAgents> DictionaryAgentsSet { get; set; }
        public virtual DbSet<DictionaryCompanies> DictionaryCompaniesSet { get; set; }
        public virtual DbSet<DictionaryDepartments> DictionaryDepartmentsSet { get; set; }
        public virtual DbSet<DictionaryPositions> DictionaryPositionsSet { get; set; }

    }
}