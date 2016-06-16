using System.Data.Common;
using System.Data.SqlClient;
using BL.CrossCutting.Helpers;
using BL.CrossCutting.Interfaces;
using BL.Model.Database;
using Oracle.ManagedDataAccess.Client;

namespace BL.Database.Helper
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
                    case DatabaseType.SQLServer:
                        return new SqlConnection(currentDB.ConnectionString);
                    case DatabaseType.Oracle:
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
                case DatabaseType.SQLServer:
                    return new SqlConnection(string.Format(@"Data Source={0};Initial Catalog={1};{2}", currentDB.Address, currentDB.DefaultDatabase, secur));
                case DatabaseType.Oracle:
                    return new OracleConnection($"Data Source={currentDB.Address};{secur}");
                default:
                    return null;
            }
        }

        //TODO remove in release version
        public static DbConnection GetDefaultConnection()
        {
            //return new OracleConnection(@"User Id=DMS;Password=DMS;Data Source=88.198.16.119:21521/XE");
            //return new SqlConnection(@"Data Source=109.197.217.79\SQLEXPRESS,1433;Initial Catalog=IRF_DMS_DEV;Persist Security Info=True;User ID=sa;Password=Harm1969");
            return new SqlConnection(@"Data Source=DESKTOP-5ETVK83\SQLEXPRESS;Initial Catalog=IRF_DMS_DEV;Persist Security Info=True;User ID=sa;Password=Harm1969");
            //return @"Data Source=109.197.217.79\SQLEXPRESS,1433;Initial Catalog=IRF_DMS;Persist Security Info=True;User ID=sa;Password=Harm1969";
        }
        //TODO remove in release version
        public static string GetDefaultSchema()
        {
            return "DMS";
        }
    }
}