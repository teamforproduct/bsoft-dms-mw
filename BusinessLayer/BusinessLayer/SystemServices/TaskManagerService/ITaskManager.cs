using System;
using System.Collections.Generic;
using BL.CrossCutting.Interfaces;
using BL.Model.Context;

namespace BL.Logic.SystemServices.TaskManagerService
{
    public interface ITaskManager
    {
        void AddTask(int periodInMinutes, Action<IContext, object> action, DatabaseModelForAdminContext dbModel, IContext ctx, object param = null);
        void AddTask(int periodInMinutes, Action<IContext, object> action, List<DatabaseModelForAdminContext> dbModel, object param = null);
        void AddTask(int periodInMinutes, Action<IContext, object> action, DatabaseModelForAdminContext dbModel = null, object param = null);
        void RemoveTask(DatabaseModelForAdminContext dbModel);
        void RemoveTaskForClient(int clientId);
    }
}