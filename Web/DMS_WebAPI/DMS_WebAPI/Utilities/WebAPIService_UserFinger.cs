using BL.Model.WebAPI.Filters;
using BL.Model.WebAPI.FrontModel;
using BL.Model.WebAPI.IncomingModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DMS_WebAPI.Utilities
{
    internal partial class WebAPIService
    {

        public IEnumerable<FrontAspNetUserFingerprint> GetUserFingerprints(FilterAspNetUserFingerprint filter)
        {
            return _webDb.GetUserFingerprints(filter);
        }

        public FrontAspNetUserFingerprint GetUserFingerprint(int id)
        {
            return _webDb.GetUserFingerprints(new FilterAspNetUserFingerprint { IDs = new List<int> { id } }).FirstOrDefault();
        }

        public bool ExistsUserFingerprints(FilterAspNetUserFingerprint filter)
        {
            return _webDb.ExistsUserFingerprints(filter);
        }

        //public int MergeUserFingerprint(IContext userContext, AddAspNetUserFingerprint model)
        //{
        //    if (string.IsNullOrEmpty(model.UserId))
        //    {
        //        var user = GetUser(userContext, userContext.CurrentAgentId);
        //        model.UserId = user.Id;
        //    }

        //    var fp = _webDb.GetUserFingerprints(new FilterAspNetUserFingerprint
        //    {
        //        UserIDs = new List<string> { model.UserId },
        //        FingerprintExact = model.Fingerprint
        //    }).FirstOrDefault();

        //    return fp?.Id ?? AddUserFingerprint(userContext, model);
        //}

        public int AddUserFingerprint(AddAspNetUserFingerprint model)
        {

            //if (string.IsNullOrEmpty(model.UserId))
            //{
            //    //TODO ASYNC
            //    var user = GetUser(userContext, userContext.CurrentAgentId);
            //    model.UserId = user.Id;
            //}

            HttpBrowserCapabilities bc = HttpContext.Current.Request.Browser;

            model.Browser = bc.Browser;
            model.Platform = bc.Platform;

            if (string.IsNullOrEmpty(model.Name))
            {
                model.Name = bc.Browser + " " + bc.Platform + " " + DateTime.UtcNow.ToString("HHmmss");
            }

            return _webDb.AddUserFingerprint(model);
        }

        public void UpdateUserFingerprint(ModifyAspNetUserFingerprint model)
        {
            _webDb.UpdateUserFingerprint(model);
        }

        public void DeleteUserFingerprint(int id, string userId)
        {
            _webDb.DeleteUserFingerprints(new FilterAspNetUserFingerprint { IDs = new List<int> { id } , UserIDs = new List<string> { userId } });
        }

        

    }
}