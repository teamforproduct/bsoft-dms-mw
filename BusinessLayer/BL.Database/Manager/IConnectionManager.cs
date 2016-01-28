using BL.CrossCutting.Interfaces;
using BL.Database.DatabaseContext;

namespace BL.Database.Manager
{
    public interface IConnectionManager
    {
        DmsContext GetDbContext(IContext context);
        DmsContext GetSystemContext(IContext context);
    }
}