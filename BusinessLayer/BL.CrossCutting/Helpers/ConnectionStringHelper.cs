using BL.CrossCutting.Interfaces;

namespace BL.Logic.Helpers
{
    public class ConnectionStringHelper : IConnectionStringHelper
    {
        public string GetConnectionString(IContext context)
        {
            if (!string.IsNullOrEmpty(context.CurrentDB.ConnectionString))
            {
                return context.CurrentDB.ConnectionString;
            }
            var secur = context.CurrentDB.IntegrateSecurity ? "Persist Security Info=false" : "Persist Security Info=True;User ID=sa;Password=Harm1969";
            switch (context.CurrentDB.ServerType)
            {
                case Model.Database.DatabaseType.SQLServer:
                    return string.Format(@"Data Source={0};Initial Catalog={1};{2}", context.CurrentDB.Address, context.CurrentDB.DefaultDatabase, secur);
                default:
                    return null;
            }
        }

        //TODO remove in release version
        public static string GetDefaultConnectionString()
        {
            return @"Data Source=109.197.217.79\SQLEXPRESS,1433;Initial Catalog=IRF_DMS;Persist Security Info=True;User ID=sa;Password=Harm1969";
        }
    }
}