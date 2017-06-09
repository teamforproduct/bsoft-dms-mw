using System.Collections.Generic;
using BL.CrossCutting.Interfaces;
using BL.Model.Enums;
using BL.Model.SystemCore.FrontModel;
using BL.Model.SystemCore.Filters;
using BL.Model.SystemCore;

namespace BL.Logic.PropertyCore.Interfaces
{
    public interface IPropertyService
    {
        object ExecuteAction(EnumActions act, IContext context, object param);

        #region Properties
        FrontProperty GetProperty(IContext context, int id);

        IEnumerable<FrontProperty> GetProperies(IContext context, FilterProperty filter);

        #endregion Properties   

        #region PropertyLinks
        FrontPropertyLink GetPropertyLink(IContext context, int id);

        IEnumerable<FrontPropertyLink> GetPropertyLinks(IContext context, FilterPropertyLink filter);

        #endregion PropertyLinks

        #region Filter Properties
        IEnumerable<BaseSystemUIElement> GetFilterProperties(IContext context, FilterProperties filter);
        #endregion Filter Properties
    }
}
