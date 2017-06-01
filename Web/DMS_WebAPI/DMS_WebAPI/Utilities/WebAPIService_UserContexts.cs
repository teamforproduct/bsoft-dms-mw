using BL.CrossCutting.Interfaces;
using BL.Model.WebAPI.Filters;
using DMS_WebAPI.DBModel;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DMS_WebAPI.Utilities
{
    internal partial class WebAPIService
    {
        public IEnumerable<AspNetUserContexts> GetUserContexts(FilterAspNetUserContext filter)
        {
            return _webDb.GetUserContexts(filter);
        }

        public int SaveUserContexts(IContext context)
        {

            var model = new AspNetUserContexts
            {
                Key = context.Key,
                UserId = context.User.Id,
                ClientId = context.Client.Id,
                CurrentPositionsIdList = string.Join(",", context.CurrentPositionsIdList),
                LastChangeDate = context.LastChangeDate,
                LogId = context.LoginLogId,
            };


            var uc = _webDb.GetUserContexts(new FilterAspNetUserContext
            {
                KeyExact = new List<string> { model.Key }
            }).FirstOrDefault();

            if (uc == null)
            {
                return _webDb.AddUserContext(model);
            }

            model.Id = uc.Id;
            _webDb.UpdateUserContext(model);
            return model.Id;
        }

        public void UpdateUserContextLastChangeDate(string key, DateTime date)
        {
            _webDb.UpdateUserContextLastChangeDate(key, date);
        }

        public void DeleteUserContexts(FilterAspNetUserContext filter)
        {
            _webDb.DeleteUserContexts(filter);
        }

    }
}