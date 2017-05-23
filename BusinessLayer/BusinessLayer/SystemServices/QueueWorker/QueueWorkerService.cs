using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using BL.CrossCutting.Context;
using BL.CrossCutting.Interfaces;
using BL.Logic.Common;

namespace BL.Logic.SystemServices.QueueWorker
{
    public class QueueWorkerService: BaseSystemWorkerService, IQueueWorkerService
    {
        private readonly Dictionary<string, QueueWorker> _workers;
        protected object _lockObjectWorker;

        public QueueWorkerService(ISettingValues settingValues, ILogger logger) : base(settingValues, logger)
        {
            _workers = new Dictionary<string, QueueWorker>();
            _lockObjectWorker = new object();
        }

        protected override void InitializeServers()
        {
            foreach (var keyValuePair in ServerContext)
            {
                try
                {
                   _workers.Add(keyValuePair.Key, new QueueWorker());
                }
                catch (Exception ex)
                {
                    Logger.Error(keyValuePair.Value, "Could not start QueueWorkerService for server", ex);
                }
            }
        }

        public void AddNewClient(AdminContext ctx)
        {
            try
            {
                _workers.Add(CommonSystemUtilities.GetServerKey(ctx), new QueueWorker());
            }
            catch (Exception ex)
            {
                Logger.Error(ctx, "Could not start QueueWorkerService for new client", ex);
            }
        }

        public void RemoveWorkersForClient(int clientId)
        {
            var wrks = _workers.Where(x => x.Key.EndsWith($"/{clientId}")).Select(x=>x.Key).ToList();
            foreach (var key in wrks)
            {
                var wrk = _workers[key];
                _workers.Remove(key);
                wrk.StopWorker();
            }
        }

        public void AddNewTask(IContext ctx, ICommand command)
        {
            AddNewTask(ctx, new QueueTask(command) { CurrentContext = ctx });
        }

        public void AddNewTask(IContext ctx, QueueTask command)
        {
            command.CurrentContext = ctx;
            var srvKey = CommonSystemUtilities.GetServerKey(ctx);
            var worker = GetWorker(srvKey);
            worker?.AddToQueue(command);
        }

        public void AddNewTask(IContext ctx, Action command)
        {
            AddNewTask(ctx, new QueueTask(command) { CurrentContext = ctx });
        }

        private QueueWorker GetWorker(string key)
        {
            QueueWorker res = null;
            lock (_lockObjectWorker)
            {
                if (_workers.ContainsKey(key))
                    res = _workers[key];
            }
            return res;
        }

        public override void Dispose()
        {
            foreach (var wrk in _workers.Values)
            {
                wrk.StopWorker();
            }

        }
    }
}