using System.Collections.Generic;
using System.Linq;
using BL.CrossCutting.Helpers;
using BL.CrossCutting.Interfaces;
using BL.Database.DatabaseContext;
using BL.Database.DBModel.System;
using BL.Model.SystemCore;
using BL.Model.Enums;
using BL.Model.SystemCore.InternalModel;
using BL.Model.SystemCore.Filters;
using BL.Model.SystemCore.FrontModel;


namespace BL.Database.SystemDb
{
    public class SystemDbProcess : CoreDb.CoreDb, ISystemDbProcess
    {
        private readonly IConnectionStringHelper _helper;

        public SystemDbProcess(IConnectionStringHelper helper)
        {
            _helper = helper;
        }

        public int AddLog(IContext ctx, LogInfo log)
        {
            using (var dbContext = new DmsContext(_helper.GetConnectionString(ctx)))
            {
                var nlog = new SystemLogs
                {
                    ExecutorAgentId = log.AgentId,
                    LogDate = log.Date,
                    LogLevel = (int)log.LogType,
                    LogException = log.LogException,
                    LogTrace = log.LogObjects,
                    Message = log.Message
                };
                dbContext.LogSet.Add(nlog);
                dbContext.SaveChanges();
                return nlog.Id;
            }
        }

        public int AddSetting(IContext ctx, string name, string value, int? agentId = null)
        {
            using (var dbContext = new DmsContext(_helper.GetConnectionString(ctx)))
            {
                var cset = dbContext.SettingsSet.FirstOrDefault(x => x.Key == name);
                if (cset == null)
                {
                    var nsett = new SystemSettings
                    {
                        ExecutorAgentId = agentId,
                        Key = name,
                        Value = value
                    };
                    dbContext.SettingsSet.Add(nsett);
                    dbContext.SaveChanges();
                    return nsett.Id;
                }

                cset.Value = value;
                cset.ExecutorAgentId = agentId;
                dbContext.SaveChanges();
                return cset.Id;
            }
        }

        public string GetSettingValue(IContext ctx, string name, int? agentId = null)
        {
            using (var dbContext = new DmsContext(_helper.GetConnectionString(ctx)))
            {
                if (agentId.HasValue)
                {
                    return
                        dbContext.SettingsSet.Where(x => x.Key == name && x.ExecutorAgentId == agentId.Value)
                            .Select(x => x.Value)
                            .FirstOrDefault();
                }
                return
                    dbContext.SettingsSet.Where(x => x.Key == name).OrderBy(x => x.ExecutorAgentId)
                        .Select(x => x.Value)
                        .FirstOrDefault();
            }
        }

        public IEnumerable<InternalSystemAction> GetSystemActions(IContext ctx, FilterSystemAction filter)
        {
            var res = new List<InternalSystemAction>();
            using (var dbContext = new DmsContext(_helper.GetConnectionString(ctx)))
            {
                foreach (int posId in filter.PositionsIdList)
                {
                    var qry = dbContext.SystemActionsSet.AsQueryable();

                    if (filter.ActionId?.Count > 0)
                    {
                        qry = qry.Where(x => filter.ActionId.Contains(x.Id));
                    }
                    if (filter.DocumentAction.HasValue)
                    {
                        qry = qry.Where(x => x.Id == (int)filter.DocumentAction);
                    }
                    if (filter.Object.HasValue)
                    {
                        qry = qry.Where(x => x.ObjectId == (int)filter.Object);
                    }
                    if (filter.IsAvailable ?? false)
                    {
                        qry = qry.Where(x => x.IsVisible &&
                                              (!x.IsGrantable
                                                || x.RoleActions.Any(y => (posId == y.Role.PositionId)
                                                                            &&
                                                                            y.Role.UserRoles.Any(z => z.UserId == ctx.CurrentAgentId))));
                    }
                    res.AddRange(qry.Select(
                              a => new InternalSystemAction
                              {
                                  DocumentAction = (EnumDocumentActions)a.Id,
                                  Object = (EnumObjects)a.ObjectId,
                                  ActionCode = a.Code,
                                  ObjectCode = a.Object.Code,
                                  API = a.API,
                                  Description = a.Description,
                              }).ToList());
                }
                return res;
            }
        }

        public IEnumerable<BaseSystemUIElement> GetSystemUIElements(IContext ctx, FilterSystemUIElement filter)
        {
            {

                using (var dbContext = new DmsContext(_helper.GetConnectionString(ctx)))
                {
                    var qry = dbContext.SystemUIElementsSet.AsQueryable();

                    if (filter.UIElementId?.Count > 0)
                    {
                        qry = qry.Where(x => filter.UIElementId.Contains(x.Id));
                    }
                    if (!string.IsNullOrEmpty(filter.Code))
                    {
                        qry = qry.Where(x => x.Code.Contains(filter.Code));
                    }
                    if (!string.IsNullOrEmpty(filter.ObjectCode))
                    {
                        qry = qry.Where(x => x.Action.Object.Code.Contains(filter.ObjectCode));
                    }
                    if (!string.IsNullOrEmpty(filter.ActionCode))
                    {
                        qry = qry.Where(x => x.Action.Code.Contains(filter.ActionCode));
                    }
                    return qry.Select(x => new BaseSystemUIElement
                    {
                        Id = x.Id,
                        ObjectCode = x.Action.Object.Code,
                        ActionCode = x.Action.Code,
                        Code = x.Code,
                        TypeCode = x.TypeCode,
                        Label = x.Label,
                        Hint = x.Hint,
                        ValueTypeCode = x.ValueType.Code,
                        IsMandatory = x.IsMandatory,
                        IsReadOnly = x.IsReadOnly,
                        IsVisible = x.IsVisible,
                        SelectAPI = x.SelectAPI,
                        SelectFilter = x.SelectFilter,
                        SelectFieldCode = x.SelectFieldCode,
                        SelectDescriptionFieldCode = x.SelectDescriptionFieldCode,
                        ValueFieldCode = x.ValueFieldCode,
                        ValueDescriptionFieldCode = x.ValueDescriptionFieldCode,
                        Format = x.Format
                    }).ToList();
                }
            }
        }

        #region SystemObjects

        public IEnumerable<FrontSystemObject> GetSystemObjects(IContext context, FilterSystemObject filter)
        {
            using (var dbContext = new DmsContext(_helper.GetConnectionString(context)))
            {
                var qry = dbContext.PropertiesSet.AsQueryable();

                if (filter.SystemObjectId?.Count > 0)
                {
                    qry = qry.Where(x => filter.SystemObjectId.Contains(x.Id));
                }

                return qry.Select(x => new FrontSystemObject
                {
                    Id = x.Id,
                    Code = x.Code,
                    Description = x.Description,
                }).ToList();
            }
        }


        #endregion SystemObjects

        #region Properties

        public InternalProperty GetProperty(IContext context, FilterProperty filter)
        {
            using (var dbContext = new DmsContext(_helper.GetConnectionString(context)))
            {
                var qry = dbContext.PropertiesSet.AsQueryable();

                if (filter.PropertyId?.Count > 0)
                {
                    qry = qry.Where(x => filter.PropertyId.Contains(x.Id));
                }

                return qry.Select(x => new InternalProperty
                {
                    Id = x.Id,
                    Code = x.Code,
                    Description = x.Description,
                    Label = x.Label,
                    Hint = x.Hint,
                    ValueTypeId = x.ValueTypeId,
                    OutFormat = x.OutFormat,
                    InputFormat = x.InputFormat,
                    SelectAPI = x.SelectAPI,
                    SelectFilter = x.SelectFilter,
                    SelectFieldCode = x.SelectFieldCode,
                    SelectDescriptionFieldCode = x.SelectDescriptionFieldCode,
                    LastChangeDate = x.LastChangeDate,
                    LastChangeUserId = x.LastChangeUserId,

                    ValueType = !x.ValueTypeId.HasValue ? null :
                        new InternalSystemValueType
                        {
                            Id = x.ValueType.Id,
                            Code = x.ValueType.Code,
                            Description = x.ValueType.Description
                        }
                }).FirstOrDefault();
            }
        }

        public IEnumerable<FrontProperty> GetProperties(IContext context, FilterProperty filter)
        {
            using (var dbContext = new DmsContext(_helper.GetConnectionString(context)))
            {
                var qry = dbContext.PropertiesSet.AsQueryable();

                if (filter.PropertyId?.Count > 0)
                {
                    qry = qry.Where(x => filter.PropertyId.Contains(x.Id));
                }

                return qry.Select(x => new FrontProperty
                {
                    Id = x.Id,
                    Code = x.Code,
                    Description = x.Description,
                    Label = x.Label,
                    Hint = x.Hint,
                    ValueTypeId = x.ValueTypeId,
                    OutFormat = x.OutFormat,
                    InputFormat = x.InputFormat,
                    SelectAPI = x.SelectAPI,
                    SelectFilter = x.SelectFilter,
                    SelectFieldCode = x.SelectFieldCode,
                    SelectDescriptionFieldCode = x.SelectDescriptionFieldCode,

                    ValueType = !x.ValueTypeId.HasValue ? null :
                        new InternalSystemValueType
                        {
                            Id = x.ValueType.Id,
                            Code = x.ValueType.Code,
                            Description = x.ValueType.Description
                        }
                }).ToList();
            }
        }

        public int AddProperty(IContext context, InternalProperty model)
        {
            using (var dbContext = new DmsContext(_helper.GetConnectionString(context)))
            {
                var item = new Properties
                {
                    Code = model.Code,
                    Description = model.Description,
                    Label = model.Label,
                    Hint = model.Hint,
                    ValueTypeId = model.ValueTypeId,
                    OutFormat = model.OutFormat,
                    InputFormat = model.InputFormat,
                    SelectAPI = model.SelectAPI,
                    SelectFilter = model.SelectFilter,
                    SelectFieldCode = model.SelectFieldCode,
                    SelectDescriptionFieldCode = model.SelectDescriptionFieldCode,
                    LastChangeDate = model.LastChangeDate,
                    LastChangeUserId = model.LastChangeUserId,
                };
                dbContext.PropertiesSet.Attach(item);

                dbContext.SaveChanges();
                model.Id = item.Id;
                return item.Id;
            }
        }

        public void UpdateProperty(IContext context, InternalProperty model)
        {
            using (var dbContext = new DmsContext(_helper.GetConnectionString(context)))
            {
                var item = new Properties
                {
                    Id = model.Id,
                    Code = model.Code,
                    Description = model.Description,
                    Label = model.Label,
                    Hint = model.Hint,
                    ValueTypeId = model.ValueTypeId,
                    OutFormat = model.OutFormat,
                    InputFormat = model.InputFormat,
                    SelectAPI = model.SelectAPI,
                    SelectFilter = model.SelectFilter,
                    SelectFieldCode = model.SelectFieldCode,
                    SelectDescriptionFieldCode = model.SelectDescriptionFieldCode,
                    LastChangeDate = model.LastChangeDate,
                    LastChangeUserId = model.LastChangeUserId,
                };
                dbContext.PropertiesSet.Attach(item);
                dbContext.Entry(item).State = System.Data.Entity.EntityState.Modified;

                dbContext.SaveChanges();
            }
        }

        public void DeleteProperty(IContext context, InternalProperty model)
        {
            using (var dbContext = new DmsContext(_helper.GetConnectionString(context)))
            {

                var item = dbContext.PropertiesSet.FirstOrDefault(x => x.Id == model.Id);
                if (item != null)
                {
                    dbContext.PropertiesSet.Remove(item);
                    dbContext.SaveChanges();
                }
            }
        }

        #endregion Properties

        #region PropertyLinks

        public InternalPropertyLink GetPropertyLink(IContext context, FilterPropertyLink filter)
        {
            using (var dbContext = new DmsContext(_helper.GetConnectionString(context)))
            {
                var qry = dbContext.PropertyLinksSet.AsQueryable();

                if (filter.PropertyLinkId?.Count > 0)
                {
                    qry = qry.Where(x => filter.PropertyLinkId.Contains(x.Id));
                }

                return qry.Select(x => new InternalPropertyLink
                {
                    Id = x.Id,
                    PropertyId = x.PropertyId,
                    ObjectId = x.ObjectId,
                    Filers = x.Filers,
                    IsMandatory = x.IsMandatory,
                    LastChangeDate = x.LastChangeDate,
                    LastChangeUserId = x.LastChangeUserId,
                }).FirstOrDefault();
            }
        }

        public IEnumerable<FrontPropertyLink> GetPropertyLinks(IContext context, FilterPropertyLink filter)
        {
            using (var dbContext = new DmsContext(_helper.GetConnectionString(context)))
            {
                var qry = dbContext.PropertyLinksSet.AsQueryable();

                if (filter.PropertyLinkId?.Count > 0)
                {
                    qry = qry.Where(x => filter.PropertyLinkId.Contains(x.Id));
                }

                return qry.Select(x => new FrontPropertyLink
                {
                    Id = x.Id,
                    PropertyId = x.PropertyId,
                    ObjectId = x.ObjectId,
                    Filers = x.Filers,
                    IsMandatory = x.IsMandatory
                }).ToList();
            }
        }

        public int AddPropertyLink(IContext context, InternalPropertyLink model)
        {
            using (var dbContext = new DmsContext(_helper.GetConnectionString(context)))
            {
                var item = new PropertyLinks
                {
                    PropertyId = model.PropertyId,
                    ObjectId = model.ObjectId,
                    Filers = model.Filers,
                    IsMandatory = model.IsMandatory,
                    LastChangeDate = model.LastChangeDate,
                    LastChangeUserId = model.LastChangeUserId,
                };
                dbContext.PropertyLinksSet.Attach(item);

                dbContext.SaveChanges();
                model.Id = item.Id;
                return item.Id;
            }
        }

        public void UpdatePropertyLink(IContext context, InternalPropertyLink model)
        {
            using (var dbContext = new DmsContext(_helper.GetConnectionString(context)))
            {
                var item = new PropertyLinks
                {
                    Id = model.Id,
                    PropertyId = model.PropertyId,
                    ObjectId = model.ObjectId,
                    Filers = model.Filers,
                    IsMandatory = model.IsMandatory,
                    LastChangeDate = model.LastChangeDate,
                    LastChangeUserId = model.LastChangeUserId,
                };
                dbContext.PropertyLinksSet.Attach(item);
                dbContext.Entry(item).State = System.Data.Entity.EntityState.Modified;

                dbContext.SaveChanges();
            }
        }

        public void DeletePropertyLink(IContext context, InternalPropertyLink model)
        {
            using (var dbContext = new DmsContext(_helper.GetConnectionString(context)))
            {

                var item = dbContext.PropertyLinksSet.FirstOrDefault(x => x.Id == model.Id);
                if (item != null)
                {
                    dbContext.PropertyLinksSet.Remove(item);
                    dbContext.SaveChanges();
                }
            }
        }

        #endregion PropertyLinks

        #region PropertyValues

        public InternalPropertyValue GetPropertyValue(IContext context, FilterPropertyValue filter)
        {
            using (var dbContext = new DmsContext(_helper.GetConnectionString(context)))
            {
                var qry = dbContext.PropertyValuesSet.AsQueryable();

                if (filter.PropertyValuesId?.Count > 0)
                {
                    qry = qry.Where(x => filter.PropertyValuesId.Contains(x.Id));
                }

                return qry.Select(x => new InternalPropertyValue
                {
                    Id = x.Id,
                    PropertyLinkId = x.PropertyLinkId,
                    RecordId = x.RecordId,
                    ValueString = x.ValueString,
                    ValueDate = x.ValueDate,
                    ValueNumeric = x.ValueNumeric,
                    LastChangeDate = x.LastChangeDate,
                    LastChangeUserId = x.LastChangeUserId,
                }).FirstOrDefault();
            }
        }

        public IEnumerable<FrontPropertyValue> GetPropertyValues(IContext context, FilterPropertyValue filter)
        {
            using (var dbContext = new DmsContext(_helper.GetConnectionString(context)))
            {
                var qry = dbContext.PropertyValuesSet.AsQueryable();

                if (filter.PropertyValuesId?.Count > 0)
                {
                    qry = qry.Where(x => filter.PropertyValuesId.Contains(x.Id));
                }

                return qry.Select(x => new FrontPropertyValue
                {
                    Id = x.Id,
                    PropertyLinkId = x.PropertyLinkId,
                    RecordId = x.RecordId,
                    Value= x.ValueString
                }).ToList();
            }
        }

        public int AddPropertyValue(IContext context, InternalPropertyValue model)
        {
            using (var dbContext = new DmsContext(_helper.GetConnectionString(context)))
            {
                var item = new PropertyValues
                {
                    PropertyLinkId = model.PropertyLinkId,
                    RecordId = model.RecordId,
                    ValueString = model.ValueString,
                    ValueDate = model.ValueDate,
                    ValueNumeric = model.ValueNumeric,
                    LastChangeDate = model.LastChangeDate,
                    LastChangeUserId = model.LastChangeUserId,
                };
                dbContext.PropertyValuesSet.Attach(item);

                dbContext.SaveChanges();
                model.Id = item.Id;
                return item.Id;
            }
        }

        public void ModifyPropertyValues(IContext context, InternalPropertyValues model)
        {
            using (var dbContext = new DmsContext(_helper.GetConnectionString(context)))
            {
                var propertyValues = dbContext.PropertyValuesSet.
                    Where(x => x.PropertyLink.ObjectId == (int)model.Object && x.RecordId == model.RecordId)
                    .Select(x=> new { x.Id, x.PropertyLinkId }).ToList();

                var groupJoinItems = propertyValues
                   .GroupJoin(model.PropertyValues,
                       x => x.PropertyLinkId,
                       y => y.PropertyLinkId,
                       (x, y) => new { propertyValueId = x.Id, values = y })
                   .ToList();

                #region modify
                var modifyItems = groupJoinItems
                    .Where(x=>x.values.Count()>0)
                    .Select(x=>new { x.propertyValueId, value = x.values.First() })
                    .Select(x => new PropertyValues
                    {
                        Id = x.propertyValueId,
                        ValueString = x.value.ValueString,
                        ValueDate = x.value.ValueDate,
                        ValueNumeric = x.value.ValueNumeric,
                        LastChangeDate = x.value.LastChangeDate,
                        LastChangeUserId = x.value.LastChangeUserId,
                    });

                foreach(var item in modifyItems)
                {
                    dbContext.PropertyValuesSet.Attach(item);
                    var entry = dbContext.Entry(item);
                    entry.Property(x => x.ValueString).IsModified = true;
                    entry.Property(x => x.ValueDate).IsModified = true;
                    entry.Property(x => x.ValueNumeric).IsModified = true;
                    entry.Property(x => x.LastChangeDate).IsModified = true;
                    entry.Property(x => x.LastChangeUserId).IsModified = true;
                }

                #endregion

                #region add

                var newItems = model.PropertyValues
                    .Where(x => !propertyValues.Select(y => y.PropertyLinkId).Contains(x.PropertyLinkId))
                    .Select(x => new PropertyValues
                    {
                        PropertyLinkId = x.PropertyLinkId,
                        RecordId = model.RecordId,
                        ValueString = x.ValueString,
                        ValueDate = x.ValueDate,
                        ValueNumeric = x.ValueNumeric,
                        LastChangeDate = x.LastChangeDate,
                        LastChangeUserId = x.LastChangeUserId,
                    }).ToList();

                dbContext.PropertyValuesSet.AddRange(newItems);

                #endregion

                #region delete
                foreach (var item in groupJoinItems.Where(x => x.values.Count() == 0).Select(x=>x.propertyValueId))
                {
                    var itemAtt = dbContext.PropertyValuesSet.Attach(new PropertyValues { Id = item });
                    dbContext.Entry(itemAtt).State = System.Data.Entity.EntityState.Deleted;
                }
                #endregion

                dbContext.SaveChanges();
            }
        }

        public void UpdatePropertyValue(IContext context, InternalPropertyValue model)
        {
            using (var dbContext = new DmsContext(_helper.GetConnectionString(context)))
            {
                var item = new PropertyValues
                {
                    Id = model.Id,
                    PropertyLinkId = model.PropertyLinkId,
                    RecordId = model.RecordId,
                    ValueString = model.ValueString,
                    ValueDate = model.ValueDate,
                    ValueNumeric = model.ValueNumeric,
                    LastChangeDate = model.LastChangeDate,
                    LastChangeUserId = model.LastChangeUserId,
                };
                dbContext.PropertyValuesSet.Attach(item);
                dbContext.Entry(item).State = System.Data.Entity.EntityState.Modified;

                dbContext.SaveChanges();
            }
        }

        public void DeletePropertyValue(IContext context, InternalPropertyValue model)
        {
            using (var dbContext = new DmsContext(_helper.GetConnectionString(context)))
            {

                var item = dbContext.PropertyValuesSet.FirstOrDefault(x => x.Id == model.Id);
                if (item != null)
                {
                    dbContext.PropertyValuesSet.Remove(item);
                    dbContext.SaveChanges();
                }
            }
        }

        #endregion PropertyValues
    }
}