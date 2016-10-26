using BL.Model.SystemCore;
using BL.Model.SystemCore.Filters;
using BL.Model.SystemCore.FrontModel;
using System;
using System.Collections.Generic;

namespace BL.CrossCutting.Interfaces
{
    public interface ILogger
    {
        IEnumerable<FrontSystemLog> GetSystemLogs(IContext context, FilterSystemLog filter, UIPaging paging);
        void Trace(IContext ctx, string message, params object[] args);
        void Information(IContext ctx, string message, int? objectId = null, int? actionId = null, int? recordId = null, object logObject = null);
        void Warning(IContext ctx, string message, params object[] args);
        void Error(IContext ctx, string message, params object[] args);
        void Error(IContext ctx, Exception exception, string message = null, params object[] args);
        void Fatal(IContext ctx, string message, params object[] args);
        void Fatal(IContext ctx, Exception exception, string message = null, params object[] args);
    }
}