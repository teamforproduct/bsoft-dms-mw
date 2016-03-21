using BL.CrossCutting.Interfaces;
using BL.Model.Database;
using Oracle.ManagedDataAccess.Client;
using System.Data.Common;
using System.Data.SqlClient;

namespace BL.CrossCutting.Helpers
{
    public class ConnectionHelper : IConnectionHelper
    {
        public DbConnection GetConnection(IContext context)
        {
            return GetConnection(context.CurrentDB);
        }

        public DbConnection GetConnection(DatabaseModel currentDB)
        {
            if (!string.IsNullOrEmpty(currentDB.ConnectionString))
            {
                switch (currentDB.ServerType)
                {
                    case Model.Database.DatabaseType.SQLServer:
                        return new SqlConnection(currentDB.ConnectionString);
                    case Model.Database.DatabaseType.Oracle:
                        return new OracleConnection(currentDB.ConnectionString);
                    default:
                        return null;
                }
                //return context.CurrentDB.ConnectionString;
            }
            //var secur = context.CurrentDB.IntegrateSecurity ? "Persist Security Info=false" : "Persist Security Info=True;User ID=sa;Password=Harm1969";
            var secur = currentDB.IntegrateSecurity ? "Persist Security Info=false" : $"Persist Security Info=True;User ID={currentDB.UserName};Password={currentDB.UserPassword}";
            switch (currentDB.ServerType)
            {
                case Model.Database.DatabaseType.SQLServer:
                    return new SqlConnection(string.Format(@"Data Source={0};Initial Catalog={1};{2}", currentDB.Address, currentDB.DefaultDatabase, secur));
                case Model.Database.DatabaseType.Oracle:
                    return new OracleConnection($"Data Source={currentDB.Address};{secur}");
                default:
                    return null;
            }
        }

        //TODO remove in release version
        public static DbConnection GetDefaultConnection()
        {
            return new OracleConnection(@"User Id=BSOFT_DOB_REAL;Password=BSOFT_DOB_REAL;Data Source=88.198.16.119:21521/XE");
            //return new SqlConnection(@"Data Source=109.197.217.79\SQLEXPRESS,1433;Initial Catalog=IRF_DMS_DEV;Persist Security Info=True;User ID=sa;Password=Harm1969");
            //return @"Data Source=109.197.217.79\SQLEXPRESS,1433;Initial Catalog=IRF_DMS;Persist Security Info=True;User ID=sa;Password=Harm1969";
        }
        //TODO remove in release version
        public static string GetDefaultSchema()
        {
            return "BSOFT_DOB_REAL";
            //return "dbo";
            //return @"Data Source=109.197.217.79\SQLEXPRESS,1433;Initial Catalog=IRF_DMS;Persist Security Info=True;User ID=sa;Password=Harm1969";
        }
    }
}