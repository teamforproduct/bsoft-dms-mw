using BL.CrossCutting.Context;
using BL.CrossCutting.Interfaces;

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