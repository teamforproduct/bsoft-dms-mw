using System;
using System.Collections.Generic;
using BL.CrossCutting.Helpers;
using BL.Database.DatabaseContext;
using BL.CrossCutting.Interfaces;
using BL.Model.Database;

namespace BL.Database.Manager
{
    internal class ConnectionManager :IConnectionManager
    {
        private readonly IConnectionStringHelper _connectionStringHelper;
        private readonly Dictionary<string, StoreInfo> _connectionCach; 

        public ConnectionManager(IConnectionStringHelper connectionStringHelper)
        {
            _connectionStringHelper = connectionStringHelper;
            _connectionCach = new Dictionary<string, StoreInfo>();
        }

        public DmsContext GetDbContext(IContext context)
        {
            if (_connectionCach.ContainsKey(context.CurrentEmployee.Token))
            {
                var conn = _connectionCach[context.CurrentEmployee.Token];
                conn.LastUsage = DateTime.Now;
                return conn.StoreObject as DmsContext;
            }

            var nconn = new DmsContext(context, _connectionStringHelper.GetConnectionString(context));
            _connectionCach.Add(context.CurrentEmployee.Token, new StoreInfo
            {
                StoreObject = nconn,
                LastUsage = DateTime.Now
            });
            return nconn;
        }

        public DmsContext GetSystemContext(IContext context)
        {
            throw new NotImplementedException();
        }
    }
}