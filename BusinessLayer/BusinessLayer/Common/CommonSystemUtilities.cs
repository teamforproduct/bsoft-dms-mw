using BL.CrossCutting.Interfaces;

namespace BL.Logic.Common
{
    public static class CommonSystemUtilities
    {
        public static string GetServerKey(IContext ctx)
        {
            return $"{ctx.CurrentDB.Address}/{ctx.CurrentDB.DefaultDatabase}";
        }
    }
}