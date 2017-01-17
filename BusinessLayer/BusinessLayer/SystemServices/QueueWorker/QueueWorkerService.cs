using System;
using System.Collections.Generic;
using BL.CrossCutting.Interfaces;
using BL.Logic.Common;

namespace BL.Logic.SystemServices.QueueWorker
{
    public class QueueWorkerService: BaseSystemWorkerService, IQueueWorkerService
    {
        private Dictionary<string, QueueWorker> _workers;
        protected object _lockObjectWorker;

        public QueueWorkerService(ISettings settings, ILogger logger) : base(settings, logger)
        {
            _workers = new Dictionary<string, QueueWorker>();
            _lockObjectWorker = new object();
        }

        protected override void InitializeServers()
        {
            foreach (var keyValuePair in _serverContext)
            {
                try
                {
                   _workers.Add(keyValuePair.Key, new QueueWorker());
                }
                catch (Exception ex)
                {
                    _logger.Error(keyValuePair.Value, "Could not start QueueWorkerService for server", ex);
                }
            }
        }

        public void AddNewTask(IContext ctx, ICommand command)
        {
            AddNewTask(ctx, new QueueTask(command));
        }

        public void AddNewTask(IContext ctx, QueueTask command)
        {
            var srvKey = CommonSystemUtilities.GetServerKey(ctx);
            var worker = GetWorker(srvKey);
            worker?.AddToQueue(command);
        }

        public void AddNewTask(IContext ctx, Action command)
        {
            AddNewTask(ctx, new QueueTask(command));
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