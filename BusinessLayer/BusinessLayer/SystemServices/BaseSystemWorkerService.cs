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
        protected readonly ISettingValues SettingValues;
        protected readonly ILogger Logger;
        private readonly object _lockObjectContext;
        protected object LockObjectTimer;
        protected readonly Dictionary<string, AdminContext> ServerContext;

        public BaseSystemWorkerService(ISettingValues settingValues, ILogger logger)
        {
            _lockObjectContext = new object();
            LockObjectTimer = new object();
            ServerContext = new Dictionary<string, AdminContext>();
            Logger = logger;
            SettingValues = settingValues;
        }

        public void Initialize(IEnumerable<DatabaseModel> dbList)
        {
          ServerContext.Clear();

          dbList.Select(x => new AdminContext(x)).ToList().ForEach(x => 
          {
              ServerContext.Add(CommonSystemUtilities.GetServerKey(x), x);
          });

          Task.Factory.StartNew(InitializeServers);
        }

        protected abstract void InitializeServers();

        protected AdminContext GetAdminContext(string key)
        {
            AdminContext res = null;
            lock (_lockObjectContext)
            {
                if (ServerContext.ContainsKey(key))
                    res = ServerContext[key];
            }
            return res;
        }

        public abstract void Dispose();
    }
}