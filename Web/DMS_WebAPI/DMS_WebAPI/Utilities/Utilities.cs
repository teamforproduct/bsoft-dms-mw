using BL.CrossCutting.Interfaces;
using BL.Logic.Context;

namespace DMS_WebAPI.Utilities
{
    public class Utilities
    {
        public IContext GetCurrentUserContext()
        {
            return new DefaultContext();
        }
    }
}