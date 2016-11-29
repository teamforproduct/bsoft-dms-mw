using BL.CrossCutting.DependencyInjection;
using BL.CrossCutting.Interfaces;
using BL.Database.DatabaseContext;
using BL.Database.DBModel.System;
using BL.Database.SystemDb;
using BL.Logic.AdminCore.Interfaces;
using BL.Logic.DocumentCore.Interfaces;
using BL.Logic.SystemCore.Interfaces;
using BL.Logic.TreeBuilder;
using BL.Model.DictionaryCore.FrontModel;
using BL.Model.Enums;
using BL.Model.SystemCore;
using BL.Model.SystemCore.Filters;
using BL.Model.SystemCore.FrontModel;
using BL.Model.SystemCore.InternalModel;
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

        public SystemService(ISystemDbProcess systemDb, ICommandService commandService)
        {
            _systemDb = systemDb;
            _commandService = commandService;
        }

        public void InitializerDatabase(IContext ctx)
        {
            _systemDb.InitializerDatabase(ctx);
        }

        public IEnumerable<FrontDictionarySettingType> GetSystemSettings(IContext context, FilterSystemSetting filter)
        {
            var tmpSettings= DmsResolver.Current.Get<ISettings>();

            var list = _systemDb.GetSystemSettings(context, filter).Select(x => new FrontSystemSetting()
            {
                Id = x.Id,
                Key = x.Key,
                Value = (x.ValueType == EnumValueTypes.Password) ? null : tmpSettings.GetTypedValue(x.Value.ToString(), x.ValueType),
                Name = x.Name,
                Description = x.Description,
                Order = x.Order,
                OrderSettingType = x.OrderSettingType,
                SettingTypeName = x.SettingTypeName,
                ValueTypeCode = x.ValueTypeCode
            });

            var res = list.GroupBy(x => new { x.SettingTypeName, x.OrderSettingType })
                 .OrderBy(x => x.Key.OrderSettingType)
                 .Select(x => new FrontDictionarySettingType()
                 {
                     Name = x.Key.SettingTypeName,
                     Setting = x.OrderBy(y => y.Order).ToList()
                 });


            return res;

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

        public FrontSystemObject GetSystemObject(IContext context, int id)
        {
            return _systemDb.GetSystemObjects(context, new FilterSystemObject { IDs = new List<int> { id } }).FirstOrDefault();
        }

        public IEnumerable<FrontSystemObject> GetSystemObjects(IContext context, FilterSystemObject filter)
        {
            return _systemDb.GetSystemObjects(context, filter);
        }

        public FrontSystemAction GetSystemAction(IContext context, int id)
        {
            return _systemDb.GetSystemActions(context, new FilterSystemAction { IDs = new List<int> { id } }).FirstOrDefault();
        }

        public IEnumerable<FrontSystemAction> GetSystemActions(IContext context, FilterSystemAction filter)
        {
            return _systemDb.GetSystemActions(context, filter);
        }

        public IEnumerable<ITreeItem> GetSystemActionForDIP(IContext context, int roleId, FilterTree filter)
        {

            var actions = _systemDb.GetSystemActionsForTree(context, roleId, new FilterSystemAction()
            {
                IsGrantable = true,
                IsVisible = true,
                IsGrantableByRecordId = false,
            });

            var objectList = (List<int>)_systemDb.GetObjectsByActions(context, new FilterSystemAction { IDs = actions.Select(x => x.Id).ToList() });

            var objects = _systemDb.GetSystemObjectsForTree(context, roleId, new FilterSystemObject()
            {
                IDs = objectList,
            });

            List<TreeItem> flatList = new List<TreeItem>();

            flatList.AddRange(objects);
            flatList.AddRange(actions);

            // перевожу на пользовательский язык лейблы в SearchText

            var languageService = DmsResolver.Current.Get<ILanguages>();

            foreach (var item in flatList)
            {
                item.SearchText  = languageService.GetTranslation(item.SearchText);
            }

            var res = Tree.GetList( Tree.Get(flatList, filter));

            return res;
        }

        public void RefreshSystemActions(IContext context)
        {
            var systemDbActions = _systemDb.GetInternalSystemActions(context, new FilterSystemAction());

            var systemImportActions = DmsDbImportData.GetSystemActions();

            _systemDb.DeleteSystemActions(context, new FilterSystemAction() { NotContainsIDs = systemImportActions.Select(x => x.Id).ToList() });

            foreach (var act in systemImportActions)
            {
                var i = systemDbActions.Where(x => x.Id == act.Id).FirstOrDefault();

                if (i == null)
                {
                    _systemDb.AddSystemAction(context, act);
                }
                else
                {
                    if (!EqualsAction(systemDbActions.FirstOrDefault(x => x.Id == act.Id), act))
                    { _systemDb.UpdateSystemAction(context, act); }

                }
            }
        }

        private bool EqualsAction(InternalSystemAction intAction, SystemActions dbAction)
        {
            if (intAction.Id != dbAction.Id) throw new Exception("EqualsAction");

            return (
              intAction.API == dbAction.API
              & intAction.Category == dbAction.Category
              & intAction.Code == dbAction.Code
              & intAction.Description == dbAction.Description
              & intAction.GrantId == dbAction.GrantId
              & intAction.IsGrantable == dbAction.IsGrantable
              & intAction.IsGrantableByRecordId == dbAction.IsGrantableByRecordId
              & intAction.IsVisible == dbAction.IsVisible
              & intAction.IsVisibleInMenu == dbAction.IsVisibleInMenu
              & (int)intAction.ObjectId == dbAction.ObjectId);

        }

        public void RefreshSystemObjects(IContext context)
        {
            var systemDbObjects = _systemDb.GetSystemObjects(context, new FilterSystemObject());

            var systemImportObjects = DmsDbImportData.GetSystemObjects();

            _systemDb.DeleteSystemObjects(context, new FilterSystemObject() { NotContainsIDs = systemImportObjects.Select(x => x.Id).ToList() });

            foreach (var act in systemImportObjects)
            {
                var i = systemDbObjects.Where(x => x.Id == act.Id).FirstOrDefault();

                if (i == null)
                {
                    _systemDb.AddSystemObject(context, act);
                }
                else
                {
                    _systemDb.UpdateSystemObject(context, act);
                }
            }

        }

        public void CheckSystemActions()
        {
            DmsDbImportData.CheckSystemActions();
        }

        public int AddSystemDate(IContext ctx, DateTime date)
        {
            return _systemDb.AddSystemDate(ctx, date);
        }

        public DateTime GetSystemDate(IContext ctx)
        {
            return _systemDb.GetSystemDate(ctx);
        }

    }
}