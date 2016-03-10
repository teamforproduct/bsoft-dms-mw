using BL.CrossCutting.Interfaces;
using BL.Database.SystemDb;
using BL.Logic.DependencyInjection;
using BL.Model.Enums;
using BL.Model.Exception;
using BL.Model.SystemCore;
using BL.Model.SystemCore.Filters;
using BL.Model.SystemCore.InternalModel;
using System.Collections.Generic;
using System.Linq;

namespace BL.Logic.Common
{
    public static class CommonSystemUtilities
    {
        public static string GetServerKey(IContext ctx)
        {
            return $"{ctx.CurrentDB.Address}/{ctx.CurrentDB.DefaultDatabase}";
        }

        private static bool IsContainsInFilter(string propertyFilter, string[] template)
        {
            if (string.IsNullOrEmpty(propertyFilter)) return true;

            template = template.Select(x => x.ToLower()).ToArray();

            var patterns = propertyFilter.ToLower().Split(new char[] { ';' });
            foreach (var pattern in patterns)
            {
                var filters = pattern.Split(new char[] { '&' });
                var isContains = true;
                foreach (var filter in filters)
                {
                    if (!template.Contains(filter))
                    {
                        isContains = false;
                        break;
                    }
                }

                if (isContains)
                    return true;
            }
            return false;
        }

        public static void VerifyPropertyValues(IContext ctx, InternalPropertyValues model, string[] template)
        {
            var _systemDb = DmsResolver.Current.Get<ISystemDbProcess>();

            var propertyLinks = _systemDb.GetPropertyLinks(ctx, new FilterPropertyLink { Object = new List<EnumObjects> { model.Object } });

            propertyLinks = propertyLinks.Where(x => IsContainsInFilter(x.Filers, template)).ToList();

            model.PropertyValues = model.PropertyValues.Where(x => propertyLinks.Select(y => y.Id).Contains(x.PropertyLinkId)).ToList();

            if (propertyLinks
                .Any(x => x.IsMandatory
                        && (
                            !model.PropertyValues.Select(y => y.PropertyLinkId).Contains(x.Id)
                            || model.PropertyValues
                                .Any(y => y.PropertyLinkId == x.Id && string.IsNullOrEmpty(y.ValueString)))))
            {
                throw new NotFilledWithAdditionalRequiredAttributes();
            }
        }

        public static IEnumerable<BaseSystemUIElement> GetPropertyUIElements(IContext ctx, EnumObjects systemObject, string[] template)
        {
            var _systemDb = DmsResolver.Current.Get<ISystemDbProcess>();

            var propertyLinks = _systemDb.GetPropertyLinks(ctx, new FilterPropertyLink { Object = new List<EnumObjects> { systemObject } });

            propertyLinks = propertyLinks.Where(x => IsContainsInFilter(x.Filers, template)).ToList();

            return _systemDb.GetPropertyUIElements(ctx, new FilterPropertyLink { PropertyLinkId = propertyLinks.Select(x => x.Id).ToList() });
        }
    }
}