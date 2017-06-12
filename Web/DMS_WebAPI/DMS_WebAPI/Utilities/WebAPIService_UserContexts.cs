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

            var list = string.Empty;

            try
            {
                list = string.Join(",", context.CurrentPositionsIdList);
            }
            catch { }
            

            var model = new AspNetUserContexts
            {
                Key = context.Session.Key,
                ClientId = context.Client.Id,
                CurrentPositionsIdList = list,
                UserId = context.User.Id,
                LastChangeDate = DateTime.UtcNow,
                SignInId = context.Session.SignInId,
            };


            var uc = _webDb.GetUserContexts(new FilterAspNetUserContext
            {
                Key = model.Key
            }).FirstOrDefault();

            if (uc == null)
            {
                return _webDb.AddUserContext(model);
            }

            model.Id = uc.Id;
            _webDb.UpdateUserContext(model);
            return model.Id;
        }

        public void UpdateUserContextLastChangeDate(string token, DateTime date)
        {
            _webDb.UpdateUserContextLastChangeDate(token, date);
        }

        public void DeleteUserContext(string token)
        {
            _webDb.DeleteUserContext(token);
        }

    }
}