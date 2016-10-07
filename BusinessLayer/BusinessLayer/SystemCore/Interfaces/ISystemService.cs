using System.Collections.Generic;
using BL.CrossCutting.Interfaces;
using BL.Model.DocumentCore.Filters;
using BL.Model.DocumentCore.FrontModel;
using BL.Model.Enums;
using BL.Model.SystemCore;
using BL.Model.DocumentCore.Actions;
using BL.Model.SystemCore.Filters;
using BL.Model.SystemCore.FrontModel;

namespace BL.Logic.SystemCore.Interfaces
{
    public interface ISystemService
    {
        object ExecuteAction(EnumSystemActions act, IContext context, object param);

        void InitializerDatabase(IContext ctx);
        IEnumerable<FrontSystemFormat> GetSystemFormats(IContext context, FilterSystemFormat filter);
        IEnumerable<FrontSystemFormula> GetSystemFormulas(IContext context, FilterSystemFormula filter);
        IEnumerable<FrontSystemPattern> GetSystemPatterns(IContext context, FilterSystemPattern filter);
        IEnumerable<FrontSystemValueType> GetSystemValueTypes(IContext context, FilterSystemValueType filter);
        IEnumerable<FrontSystemSetting> GetSystemSettings(IContext context, FilterSystemSetting filter);
    }
}