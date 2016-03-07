using System.Collections.Generic;
using System.Linq;
using BL.CrossCutting.Interfaces;
using BL.Logic.DocumentCore.Interfaces;
using BL.Model.Enums;
using BL.Database.SystemDb;
using BL.Logic.PropertyCore.Interfaces;
using BL.Model.SystemCore.FrontModel;
using BL.Model.SystemCore.Filters;

namespace BL.Logic.PropertyCore
{
    public class PropertyService : IPropertyService
    {
        private readonly ISystemDbProcess _systDb;
        private readonly ICommandService _commandService;

        public PropertyService(ISystemDbProcess systDb, ICommandService commandService)
        {
            _systDb = systDb;
            _commandService = commandService;
        }

        public object ExecuteAction(EnumPropertyAction act, IContext context, object param)
        {
            var cmd = PropertyCommandFactory.GetPropertyCommand(act, context, param);
            var res = _commandService.ExecuteCommand(cmd);
            return res;
        }

        #region SystemObjects
        public FrontSystemObject GetSystemObject(IContext context, int id)
        {
            return _systDb.GetSystemObjects(context, new FilterSystemObject { SystemObjectId = new List<int> { id } }).FirstOrDefault();
        }

        public IEnumerable<FrontSystemObject> GetSystemObjects(IContext context, FilterSystemObject filter)
        {
            return _systDb.GetSystemObjects(context, filter);
        }

        #endregion SystemObjects

        #region Properties
        public FrontProperty GetProperty(IContext context, int id)
        {
            return _systDb.GetProperties(context, new FilterProperty { PropertyId = new List<int> { id } }).FirstOrDefault();
        }

        public IEnumerable<FrontProperty> GetProperies(IContext context, FilterProperty filter)
        {
            return _systDb.GetProperties(context, filter);
        }

        #endregion Properties

        #region PropertyLinks
        public FrontPropertyLink GetPropertyLink(IContext context, int id)
        {
            return _systDb.GetPropertyLinks(context, new FilterPropertyLink { PropertyLinkId = new List<int> { id } }).FirstOrDefault();
        }

        public IEnumerable<FrontPropertyLink> GetPropertyLinks(IContext context, FilterPropertyLink filter)
        {
            return _systDb.GetPropertyLinks(context, filter);
        }

        #endregion PropertyLinks

        #region PropertyValues
        public FrontPropertyValue GetPropertyValue(IContext context, int id)
        {
            return _systDb.GetPropertyValues(context, new FilterPropertyValue { PropertyValuesId = new List<int> { id } }).FirstOrDefault();
        }

        public IEnumerable<FrontPropertyValue> GetPropertyValues(IContext context, FilterPropertyValue filter)
        {
            return _systDb.GetPropertyValues(context, filter);
        }

        #endregion PropertyValues
    }
}
