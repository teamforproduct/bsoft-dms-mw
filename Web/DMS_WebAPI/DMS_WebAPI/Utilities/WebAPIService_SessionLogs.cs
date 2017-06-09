using BL.Model.SystemCore;
using BL.Model.WebAPI.Filters;
using BL.Model.WebAPI.FrontModel;
using DMS_WebAPI.DBModel;
using System;
using System.Collections.Generic;

namespace DMS_WebAPI.Utilities
{
    internal partial class WebAPIService
    {

        public IEnumerable<FrontSessionLog> GetSessionLogs(FilterSessionsLog filter, UIPaging paging)
        {
            return _webDb.GetSessionLogs(filter, paging);
        }

        public int AddSessionLog(AddSessionLog model)
        {
            return _webDb.AddSessionLog(model);
        }

        public void SetSessionLogLastUsage(DateTime lastUsage, FilterSessionsLog filter)
        {
            _webDb.SetSessionLogLastUsage(lastUsage, filter);
        }

        public void SetSessionLogEnabled(bool enabled, FilterSessionsLog filter)
        {
            _webDb.SetSessionLogEnabled(enabled, filter);
        }
        public void DeleteSessionLogs(FilterSessionsLog filter)
        {
            _webDb.DeleteSessionLogs(filter);
        }

    }
}