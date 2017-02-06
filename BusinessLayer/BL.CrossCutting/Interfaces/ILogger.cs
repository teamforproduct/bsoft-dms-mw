using BL.Model.SystemCore;
using BL.Model.SystemCore.Filters;
using BL.Model.SystemCore.FrontModel;
using System;
using System.Collections.Generic;
using BL.CrossCutting.Context;
using System.Linq;
using BL.Model.DictionaryCore.FrontModel;

namespace BL.CrossCutting.Interfaces
{
    public interface ILogger
    {
        IEnumerable<FrontSystemSession> GetSystemSessions(IContext context, IQueryable<FrontSystemSession> sessions, FilterSystemSession filter, UIPaging paging);
        IEnumerable<int> GetOnlineUsers(IContext context, IQueryable<FrontSystemSession> sessions);
        FrontAgentEmployeeUser GetLastUserLoginInfo(IContext context);
        IEnumerable<FrontSystemLog> GetSystemLogs(IContext context, FilterSystemLog filter, UIPaging paging);
        int? Trace(IContext ctx, string message, params object[] args);
        int? Information(IContext ctx, string message, int? objectId = null, int? actionId = null, int? recordId = null, object logObject = null, bool isCopyDate1 = false);
        int? Warning(IContext ctx, string message, params object[] args);
        int? Error(IContext ctx, string message, params object[] args);
        int? Error(IContext ctx, Exception exception, string message = null, params object[] args);
        int? Error(IContext ctx, string message = null, string exception = null, int? objectId = null, int? actionId = null, int? recordId = null, object logObject = null, int? agentId = null, params object[] args);
        int? Fatal(IContext ctx, string message, params object[] args);
        int? Fatal(IContext ctx, Exception exception, string message = null, params object[] args);
        void UpdateLogDate1(IContext ctx, int id, DateTime datetime);
    }
}