using System;
using System.Collections.Generic;
using BL.CrossCutting.Interfaces;
using BL.Model.Context;

namespace BL.Logic.SystemServices.TaskManagerService
{
    public interface ITaskManager
    {
        void AddTask(List<DatabaseModelForAdminContext> dbModel, int periodInMinutes, Action<IContext> action);
        void AddTask(DatabaseModelForAdminContext dbModel, int periodInMinutes, Action<IContext> action);
        void RemoveTask(DatabaseModelForAdminContext dbModel);
    }
}