using BL.CrossCutting.Interfaces;

namespace BL.Logic.Helpers
{
    public interface IConnectionStringHelper
    {
        string GetConnectionString(IContext context);
    }
}