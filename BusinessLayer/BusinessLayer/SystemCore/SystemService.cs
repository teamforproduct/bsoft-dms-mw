using BL.CrossCutting.DependencyInjection;
using BL.CrossCutting.Interfaces;
using BL.Database.SystemDb;
using BL.Logic.DocumentCore.Interfaces;
using BL.Logic.SystemCore.Interfaces;
using BL.Logic.TreeBuilder;
using BL.Model.Enums;
using BL.Model.SystemCore.Filters;
using BL.Model.SystemCore.FrontModel;
using BL.Model.Tree;
using System;
using System.Collections.Generic;
using System.Linq;

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
            var tmpSettings= DmsResolver.Current.Get<ISettings>();

            return _systemDb.GetSystemSettings(context, filter).Select(x => new FrontSystemSetting()
            {
                Key = x.Key,
                Value = tmpSettings.GetTypedValue(x.Value.ToString(), x.ValueType),
                AgentId = x.AgentId,
            } );
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
        public List<ITreeItem> GetSystemActionForDIP(IContext context, FilterTree filter)
        {
            var objects = _systemDb.GetSystemObjectsForTree(context, new FilterSystemObject());

            var actions = _systemDb.GetSystemActionsForTree(context, new FilterSystemAction());

            List<TreeItem> flatList = new List<TreeItem>();

            flatList.AddRange(objects);
            flatList.AddRange(actions);

            var res = Tree.GetList( Tree.Get(flatList, filter));

            return res;
        }
    }
}