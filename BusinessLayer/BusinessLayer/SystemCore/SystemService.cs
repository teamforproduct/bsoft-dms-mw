using BL.CrossCutting.Interfaces;
using BL.Database.SystemDb;
using BL.Logic.DocumentCore.Interfaces;
using BL.Logic.SystemCore.Interfaces;
using BL.Model.Enums;
using BL.Model.SystemCore.Filters;
using BL.Model.SystemCore.FrontModel;
using System.Collections.Generic;

namespace BL.Logic.SystemCore
{
    internal class SystemService : ISystemService
    {
        private readonly ISystemDbProcess _systemDb;

        private readonly ICommandService _commandService;

        public object ExecuteAction(EnumSystemActions act, IContext context, object param)
        {
            var cmd = SystemCommandFactory.GetCommand(act, context, param);
            var res = _commandService.ExecuteCommand(cmd);
            return res;
        }

        public SystemService(ISystemDbProcess systemDb)
        {
            _systemDb = systemDb;
        }

        public void InitializerDatabase(IContext ctx)
        {
            _systemDb.InitializerDatabase(ctx);
        }

        public IEnumerable<FrontSystemSetting> GetSystemSettings(IContext context, FilterSystemSetting filter)
        {
            return _systemDb.GetSystemSettings(context, filter);
        }

        public IEnumerable<FrontSystemFormat> GetSystemFormats(IContext context, FilterSystemFormat filter)
        {
            return _systemDb.GetSystemFormats(context, filter);
        }
        public IEnumerable<FrontSystemFormula> GetSystemFormulas(IContext context, FilterSystemFormula filter)
        {

            return _systemDb.GetSystemFormulas(context, filter);
        }
        public IEnumerable<FrontSystemPattern> GetSystemPatterns(IContext context, FilterSystemPattern filter)
        {

            return _systemDb.GetSystemPatterns(context, filter);
        }
        public IEnumerable<FrontSystemValueType> GetSystemValueTypes(IContext context, FilterSystemValueType filter)
        {

            return _systemDb.GetSystemValueTypes(context, filter);
        }
    }
}