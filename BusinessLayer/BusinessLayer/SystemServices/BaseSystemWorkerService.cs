using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BL.CrossCutting.Context;
using BL.CrossCutting.Interfaces;
using BL.Logic.Common;
using BL.Logic.SystemServices.MailWorker;
using BL.Model.Database;

namespace BL.Logic.SystemServices
{
    public abstract class BaseSystemWorkerService: ISystemWorkerService, IDisposable
    {
        protected readonly ISettings _settings;
        protected readonly ILogger _logger;
        private object _lockObjectContext;
        protected object _lockObjectTimer;
        protected readonly Dictionary<string, AdminContext> _serverContext;
        private Task _initializeThread;

        public BaseSystemWorkerService(ISettings settings, ILogger logger)
        {
            _lockObjectContext = new object();
            _lockObjectTimer = new object();
            _serverContext = new Dictionary<string, AdminContext>();
            _settings = settings;
            _logger = logger;
        }

        public void Initialize(IEnumerable<DatabaseModel> dbList)
        {
            _serverContext.Clear();

            dbList.Select(x => new AdminContext(x)).ToList().ForEach(
            x => _serverContext.Add(CommonSystemUtilities.GetServerKey(x), x));

            _initializeThread = Task.Factory.StartNew(InitializeServers);
        }

        protected abstract void InitializeServers();

        protected AdminContext GetAdminContext(string key)
        {
            AdminContext res = null;
            lock (_lockObjectContext)
            {
                if (_serverContext.ContainsKey(key))
                    res = _serverContext[key];
            }
            return res;
        }

        public abstract void Dispose();
    }
}