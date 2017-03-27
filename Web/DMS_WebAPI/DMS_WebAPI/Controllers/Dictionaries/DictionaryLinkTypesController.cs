﻿using BL.CrossCutting.DependencyInjection;
using BL.Logic.DictionaryCore.Interfaces;
using BL.Model.DictionaryCore.FilterModel;
using DMS_WebAPI.Results;
using DMS_WebAPI.Utilities;
using System.Web.Http;

namespace DMS_WebAPI.Controllers.Dictionaries
{
    [Authorize]
    public class DictionaryLinkTypesController : ApiController
    {
        // GET: api/DictionaryLinkTypes
        public IHttpActionResult Get([FromUri] FilterDictionaryLinkType filter)
        {
            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var tmpDictProc = DmsResolver.Current.Get<IDictionaryService>();
            var tmpDicts = tmpDictProc.GetDictionaryLinkTypes(ctx, filter);
            return new JsonResult(tmpDicts, this);
        }

        // GET: api/DictionaryLinkTypes/5
        public IHttpActionResult Get(int id)
        {
            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var tmpDictProc = DmsResolver.Current.Get<IDictionaryService>();
            var tmpDict = tmpDictProc.GetDictionaryLinkType(ctx, id);
            return new JsonResult(tmpDict, this);
        }
    }
}