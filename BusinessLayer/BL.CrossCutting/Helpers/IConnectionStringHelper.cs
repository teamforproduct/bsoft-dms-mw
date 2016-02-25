using BL.CrossCutting.Interfaces;

namespace BL.CrossCutting.Helpers
{
    public interface IConnectionStringHelper
    {
        string GetConnectionString(IContext context);
    }
}