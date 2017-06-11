using BL.CrossCutting.DependencyInjection;
using BL.CrossCutting.Helpers;
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

        public object ExecuteAction(EnumActions act, IContext context, object param)
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
            var tmpSettings = DmsResolver.Current.Get<ISettings>();

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

        public IEnumerable<FrontSystemAction> GetImportSystemActions()
        {
            return DmsDbImportData.GetSystemActions().Select(x => new FrontSystemAction
            {
                Id = x.Id,
                Code = ((EnumActions)x.Id).ToString(),
                Description = "##l@Actions:" + ((EnumActions)x.Id).ToString() + "@l##",
            }).ToList();
        }

        public IEnumerable<FrontSystemObject> GetImportSystemObjects()
        {
            return DmsDbImportData.GetSystemObjects().Select(x => new FrontSystemObject
            {
                Id = x.Id,
            }).ToList();
        }

        public IEnumerable<FrontSystemModules> GetImportSystemModules()
        {
            DmsDbImportData.InitPermissions();
            return DmsDbImportData.GetSystemModules().Select(x => new FrontSystemModules
            {
                Id = x.Id,
                Code = x.Code,
                Name = x.Name,
                Order = x.Order
            }).ToList();
        }

        public IEnumerable<FrontSystemFeatures> GetImportSystemFeatures()
        {
            DmsDbImportData.InitPermissions();
            return DmsDbImportData.GetSystemFeatures().Select(x => new FrontSystemFeatures
            {
                Id = x.Id,
                Code = x.Code,
                Name = x.Name,
                Order = x.Order
            }).ToList();
        }

        public FrontSystemAction GetSystemAction(IContext context, int id)
        {
            return _systemDb.GetSystemActions(context, new FilterSystemAction { IDs = new List<int> { id } }).FirstOrDefault();
        }


        public IEnumerable<FrontSystemAction> GetSystemActions(IContext context, FilterSystemAction filter)
        {
            return _systemDb.GetSystemActions(context, filter);
        }


        public void RefreshSystemActions(IContext context)
        {
            var systemDbActions = _systemDb.GetInternalSystemActions(context, new FilterSystemAction());

            var systemImportActions = DmsDbImportData.GetSystemActions();

            _systemDb.DeleteSystemActions(context, new FilterSystemAction() { NotContainsIDs = systemImportActions.Select(x => x.Id).ToList() });

            foreach (var act in systemImportActions)
            {
                var i = systemDbActions.FirstOrDefault(x => x.Id == act.Id);

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
              (int?)intAction.Category == dbAction.CategoryId
              & (int)intAction.PermissionId == dbAction.PermissionId
              & (int)intAction.ObjectId == dbAction.ObjectId);

        }

        public void RefreshSystemObjects(IContext context)
        {
            var systemDbObjects = _systemDb.GetSystemObjects(context, new FilterSystemObject());

            var systemImportObjects = DmsDbImportData.GetSystemObjects();

            _systemDb.DeleteSystemObjects(context, new FilterSystemObject() { NotContainsIDs = systemImportObjects.Select(x => x.Id).ToList() });

            foreach (var act in systemImportObjects)
            {
                var i = systemDbObjects.FirstOrDefault(x => x.Id == act.Id);

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

        public void RefreshModuleFeature(IContext context)
        {
            _systemDb.RefreshModuleFeature(context);

        }

    }
}