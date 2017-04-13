using System.Collections.Generic;
using BL.CrossCutting.Interfaces;
using BL.Model.Enums;
using BL.Model.SystemCore.Filters;
using BL.Model.SystemCore.FrontModel;
using BL.Model.Tree;
using System;
using BL.Model.DictionaryCore.FrontModel;

namespace BL.Logic.SystemCore.Interfaces
{
    public interface ISystemService
    {
        object ExecuteAction(EnumSystemActions act, IContext context, object param);

        void InitializerDatabase(IContext ctx);

        IEnumerable<FrontSystemAction> GetImportSystemActions();
        IEnumerable<FrontSystemObject> GetImportSystemObjects();
        IEnumerable<FrontSystemModules> GetImportSystemModules();
        IEnumerable<FrontSystemFeatures> GetImportSystemFeatures();



        IEnumerable<FrontSystemFormat> GetSystemFormats(IContext context, FilterSystemFormat filter);
        IEnumerable<FrontSystemFormula> GetSystemFormulas(IContext context, FilterSystemFormula filter);
        IEnumerable<FrontSystemPattern> GetSystemPatterns(IContext context, FilterSystemPattern filter);
        IEnumerable<FrontSystemValueType> GetSystemValueTypes(IContext context, FilterSystemValueType filter);
        IEnumerable<FrontDictionarySettingType> GetSystemSettings(IContext context, FilterSystemSetting filter);
        FrontSystemObject GetSystemObject(IContext context, int id);
        IEnumerable<FrontSystemObject> GetSystemObjects(IContext context, FilterSystemObject filter);
        FrontSystemAction GetSystemAction(IContext context, int id);
        IEnumerable<FrontSystemAction> GetSystemActions(IContext context, FilterSystemAction filter);
        void RefreshSystemActions(IContext context);
        void RefreshSystemObjects(IContext context);

        void RefreshModuleFeature(IContext context);
    }
}