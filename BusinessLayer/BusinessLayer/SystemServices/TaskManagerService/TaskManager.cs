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
        private readonly ILogger _logger;
        private readonly List<TaskSettings> _taskSettingses = new List<TaskSettings>();
        private readonly object _lockObject = new object();

        public TaskManager(ILogger logger)
        {
            _logger = logger;
        }

        public void AddTask(DatabaseModelForAdminContext dbModel, int periodInMinutes, Action<IContext, object> action, object param = null)
        {
            var ctx = new AdminContext(dbModel);
            AddTask(dbModel, periodInMinutes, ctx, action, param);
        }

        public void AddTask(DatabaseModelForAdminContext dbModel, int periodInMinutes, IContext ctx, Action<IContext, object> action, object param = null)
        {
            lock (_lockObject)
            {
                var nextId = _taskSettingses.Any()
                    ? _taskSettingses.Max(x=>x.Id)+1
                    : 1;

                _taskSettingses.Add(new TaskSettings
                {
                    Id = nextId,
                    Context = ctx,
                    DatabaseModel = dbModel,
                    TaskAction = action,
                    PeriodInMinute = periodInMinutes,
                    Parameter = param,
                    TaskTimer = new Timer(OnTimer, nextId, periodInMinutes * 60000, Timeout.Infinite)
                });

            }
        }

        public void AddTask(List<DatabaseModelForAdminContext> dbModel, int periodInMinutes, Action<IContext, object> action, object param = null)
        {
            foreach (var md in dbModel)
            {
                AddTask(md, periodInMinutes, new AdminContext(md), action, param);
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
                sett.TaskTimer.Dispose();
            }
        }

        private void OnTimer(object state)
        {
            var settId = (int) state;
            var sett = _taskSettingses.FirstOrDefault(x => x.Id == settId);
            if (sett == null) return;

            try
            {
                sett.TaskAction(sett.Context, sett.Parameter);
            }
            catch (Exception ex)
            {
                if (sett.Context != null)
                {
                    _logger.Error(sett.Context, ex, "Task manager execution error", $"ClientId ={sett.Context.Client.Id}, DB = {sett.Context.CurrentDB.Address}");
                }
            }
            finally
            {
                sett.TaskTimer.Change(sett.PeriodInMinute * 60000, Timeout.Infinite);
            }
            
        }
    }
}