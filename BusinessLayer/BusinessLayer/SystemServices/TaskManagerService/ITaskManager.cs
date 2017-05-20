using System;
using System.Collections.Generic;
using BL.CrossCutting.Interfaces;
using BL.Model.Context;

namespace BL.Logic.SystemServices.TaskManagerService
{
    public interface ITaskManager
    {
        void AddTask(DatabaseModelForAdminContext dbModel, int periodInMinutes, IContext ctx, Action<IContext, object> action, object param = null);
        void AddTask(List<DatabaseModelForAdminContext> dbModel, int periodInMinutes, Action<IContext, object> action, object param = null);
        void AddTask(DatabaseModelForAdminContext dbModel, int periodInMinutes, Action<IContext, object> action, object param = null);
        void RemoveTask(DatabaseModelForAdminContext dbModel);
        void RemoveTaskForClient(int clientId);
    }
}