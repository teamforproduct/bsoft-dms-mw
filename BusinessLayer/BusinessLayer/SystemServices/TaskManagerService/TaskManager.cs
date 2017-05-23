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

        public void AddTask(int periodInMinutes, Action<IContext, object> action, DatabaseModelForAdminContext dbModel = null, object param = null)
        {
            AddTask(periodInMinutes, action, dbModel, dbModel == null ? null:new AdminContext(dbModel), param);
        }

        public void AddTask(int periodInMinutes, Action<IContext, object> action, DatabaseModelForAdminContext dbModel, IContext ctx, object param = null)
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

        public void AddTask(int periodInMinutes, Action<IContext, object> action, List<DatabaseModelForAdminContext> dbModel, object param = null)
        {
            foreach (var md in dbModel)
            {
                AddTask(periodInMinutes, action, md, new AdminContext(md), param);
            }
        }

        public void RemoveTask(DatabaseModelForAdminContext dbModel)
        {
            lock (_lockObject)
            {
                var sett = _taskSettingses.Where(x => x.DatabaseModel == dbModel).ToList();
                if (!sett.Any() ) return;
                foreach (var s in sett)
                {
                    _taskSettingses.Remove(s);
                    s.TaskTimer.Change(Timeout.Infinite, Timeout.Infinite);
                    s.TaskTimer.Dispose();
                }
                
            }
        }

        public void RemoveTaskForClient(int clientId)
        {
            lock (_lockObject)
            {
                var sett = _taskSettingses.Where(x => x.DatabaseModel.ClientId == clientId).ToList();
                if (!sett.Any()) return;
                foreach (var s in sett)
                {
                    _taskSettingses.Remove(s);
                    s.TaskTimer.Change(Timeout.Infinite, Timeout.Infinite);
                    s.TaskTimer.Dispose();
                }

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