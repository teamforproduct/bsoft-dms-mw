using BL.CrossCutting.Interfaces;
using BL.Database.SystemDb;
using BL.Logic.SystemCore.Interfaces;
using BL.Model.SystemCore.Filters;
using BL.Model.SystemCore.FrontModel;
using System.Collections.Generic;

namespace BL.Logic.SystemCore
{
    internal class SystemService : ISystemService
    {
        private readonly ISystemDbProcess _systemDb;
        public SystemService(ISystemDbProcess systemDb)
        {
            _systemDb = systemDb;
        }

        public void InitializerDatabase(IContext ctx)
        {
            _systemDb.InitializerDatabase(ctx);
        }

        public IEnumerable<FrontSystemFormats> GetSystemFormats(IContext context, FilterSystemFormat filter)
        {

            return null;// _dictDb.GetSendTypes(context, filter);
        }
    }
}