using System.Collections.Generic;
using BL.CrossCutting.Interfaces;
using BL.Model.Enums;
using BL.Model.SystemCore.FrontModel;
using BL.Model.SystemCore.Filters;

namespace BL.Logic.PropertyCore.Interfaces
{
    public interface IPropertyService
    {
        object ExecuteAction(EnumPropertyAction act, IContext context, object param);

        #region SystemObjects
        FrontSystemObject GetSystemObject(IContext context, int id);

        IEnumerable<FrontSystemObject> GetSystemObjects(IContext context, FilterSystemObject filter);

        #endregion SystemObjects

        #region Properties
        FrontProperty GetProperty(IContext context, int id);

        IEnumerable<FrontProperty> GetProperies(IContext context, FilterProperty filter);

        #endregion Properties   

        #region PropertyLinks
        FrontPropertyLink GetPropertyLink(IContext context, int id);

        IEnumerable<FrontPropertyLink> GetPropertyLinks(IContext context, FilterPropertyLink filter);

        #endregion PropertyLinks
    }
}
