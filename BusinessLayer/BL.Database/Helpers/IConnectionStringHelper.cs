using BL.CrossCutting.Interfaces;

namespace BL.Database.Helpers
{
    public interface IConnectionStringHelper
    {
        string GetConnectionString(IContext context);
    }
}