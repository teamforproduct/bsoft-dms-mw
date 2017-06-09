using BL.CrossCutting.Helpers;
using BL.Model.Enums;
using BL.Model.SystemCore.InternalModel;
using DMS_WebAPI.DatabaseContext;
using DMS_WebAPI.DBModel;
using System.Collections.Generic;
using System.Linq;

namespace DMS_WebAPI.Utilities
{
    internal partial class WebAPIDbProcess
    {

        public int MergeSetting(InternalGeneralSetting model)
        {
            var res = 0;
            using (var dbContext = new ApplicationDbContext()) using (var transaction = Transactions.GetTransaction())
            {
                var cset = dbContext.SystemSettingsSet.FirstOrDefault(x => x.Key == model.Key);
                if (cset == null)
                {
                    var nsett = new SystemSettings
                    {
                        Key = model.Key,
                        Value = model.Value,
                        ValueTypeId = (int)model.ValueType,
                        Name = model.Name,
                        Description = model.Description,
                        Order = model.Order,
                    };
                    dbContext.SystemSettingsSet.Add(nsett);
                    dbContext.SaveChanges();
                    res = nsett.Id;
                }
                else
                {
                    cset.Value = model.Value;

                    if (model.ValueType > 0)
                    {
                        cset.ValueTypeId = (int)model.ValueType;
                    }

                    dbContext.SaveChanges();
                    res = cset.Id;
                }
                transaction.Complete();
                return res;
            }
        }

        public string GetSettingValue(string key)
        {
            var res = string.Empty;
            using (var dbContext = new ApplicationDbContext()) using (var transaction = Transactions.GetTransaction())
            {
                res = dbContext.SystemSettingsSet.Where(x => x.Key == key)
                        .Select(x => x.Value)
                        .FirstOrDefault();
                transaction.Complete();
                return res;
            }
        }

        public IEnumerable<InternalGeneralSetting> GetSystemSettingsInternal()
        {
            using (var dbContext = new ApplicationDbContext()) using (var transaction = Transactions.GetTransaction())
            {
                var qry = dbContext.SystemSettingsSet.AsQueryable();

                qry = qry.OrderBy(x => x.Order);

                var res = qry.Select(x => new InternalGeneralSetting
                {
                    Key = x.Key,
                    Name = x.Name,
                    Value = x.Value,
                    ValueType = (EnumValueTypes)x.ValueTypeId,
                    Description = x.Description,
                    Order = x.Order
                }).ToList();

                transaction.Complete();

                return res;
            }
        }

    }
}