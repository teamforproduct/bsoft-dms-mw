using System;
using System.Collections.Generic;
using BL.Database.DatabaseContext;
using BL.CrossCutting.Interfaces;
using BL.Model.Database;
using BL.Database.Helpers;

namespace BL.Database.Manager
{
    internal class ConnectionManager :IConnectionManager
    {
        private readonly IConnectionStringHelper _connectionStringHelper;
        private readonly Dictionary<string, ConnectionStoreInfo> _connectionCach; 

        public ConnectionManager(IConnectionStringHelper connectionStringHelper)
        {
            _connectionStringHelper = connectionStringHelper;
            _connectionCach = new Dictionary<string, ConnectionStoreInfo>();
        }

        public DmsContext GetDbContext(IContext context)
        {
            if (_connectionCach.ContainsKey(context.CurrentEmployee.Token))
            {
                var conn = _connectionCach[context.CurrentEmployee.Token];
                conn.LastUsege = DateTime.Now;
                return conn.ConnectionObject as DmsContext;
            }

            var nconn = new DmsContext(_connectionStringHelper.GetConnectionString(context));
            _connectionCach.Add(context.CurrentEmployee.Token, new ConnectionStoreInfo
            {
                ConnectionObject = nconn,
                LastUsege = DateTime.Now
            });
            return nconn;
        }
    }
}