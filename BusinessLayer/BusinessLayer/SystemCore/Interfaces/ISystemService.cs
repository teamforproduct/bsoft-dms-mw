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
        void InitializerDatabase(IContext ctx);
        IEnumerable<FrontSystemFormats> GetSystemFormats(IContext context, FilterSystemFormat filter);
    }
}