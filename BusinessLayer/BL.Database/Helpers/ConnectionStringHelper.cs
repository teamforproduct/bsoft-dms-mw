using BL.CrossCutting.Interfaces;

namespace BL.Database.Helpers
{
    public class ConnectionStringHelper : IConnectionStringHelper
    {
        public string GetConnectionString(IContext context)
        {
            return @"Data Source=109.197.217.79\SQLEXPRESS,1433;Initial Catalog=IRF_DMS;Persist Security Info=True;User ID=sa;Password=Harm1969"; 
        }

        public static string GetDefaultConnectionString()
        {
            return @"Data Source=109.197.217.79\SQLEXPRESS,1433;Initial Catalog=IRF_DMS;Persist Security Info=True;User ID=sa;Password=Harm1969";
        }
    }
}