using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using BL.CrossCutting.Context;
using BL.CrossCutting.Interfaces;
using BL.Model.Context;

namespace BL.Logic.SystemServices.TaskManagerService
{
    public class TaskManager : ITaskManager
    {
        private readonly List<TaskSettings> _taskSettingses = new List<TaskSettings>();
        private readonly object _lockObject = new object();


        public void AddTask(DatabaseModelForAdminContext dbModel, int periodInMinutes, Action<IContext> action)
        {
            lock (_lockObject)
            {
                var nextId = _taskSettingses.Any()
                    ? _taskSettingses.Max(x=>x.Id)+1
                    : 1;

                _taskSettingses.Add(new TaskSettings
                {
                    Id = nextId,
                    Context = new AdminContext(dbModel),
                    DatabaseModel = dbModel,
                    TaskAction = action,
                    PeriodInMinute = periodInMinutes,
                    TaskTimer = new Timer(OnTimer, nextId, periodInMinutes * 60000, Timeout.Infinite)
                });

            }
        }

        public void AddTask(List<DatabaseModelForAdminContext> dbModel, int periodInMinutes, Action<IContext> action)
        {
            foreach (var md in dbModel)
            {
                AddTask(md, periodInMinutes, action);
            }
        }

        public void RemoveTask(DatabaseModelForAdminContext dbModel)
        {
            lock (_lockObject)
            {
                var sett = _taskSettingses.FirstOrDefault(x => x.DatabaseModel == dbModel);
                if (sett == null) return;

                _taskSettingses.Remove(sett);
                sett.TaskTimer.Change(Timeout.Infinite, Timeout.Infinite);
            }
        }

        private void OnTimer(object state)
        {
            var settId = (int) state;
            var sett = _taskSettingses.FirstOrDefault(x => x.Id == settId);
            if (sett == null) return;


            sett.TaskTimer.Change(sett.PeriodInMinute*60000, Timeout.Infinite);
        }
    }
}