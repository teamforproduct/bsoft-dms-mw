using System.Data.Common;
using BL.CrossCutting.Interfaces;
using BL.Model.Database;

namespace BL.Database.Helper
{
    public interface IConnectionHelper
    {
        DbConnection GetConnection(IContext context);
        DbConnection GetConnection(DatabaseModel currentDB);
    }
}