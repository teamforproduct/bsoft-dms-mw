using BL.CrossCutting.Interfaces;
using BL.Model.Context;
using System.Data.Common;

namespace BL.Database.Helper
{
    public interface IConnectionHelper
    {
        DbConnection GetConnection(IContext context);
        DbConnection GetConnection(DatabaseModel currentDB);
    }
}