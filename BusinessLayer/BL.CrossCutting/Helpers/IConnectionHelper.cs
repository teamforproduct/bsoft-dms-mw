using BL.CrossCutting.Interfaces;
using BL.Model.Database;
using System.Data.Common;

namespace BL.CrossCutting.Helpers
{
    public interface IConnectionHelper
    {
        DbConnection GetConnection(IContext context);
        DbConnection GetConnection(DatabaseModel currentDB);
    }
}