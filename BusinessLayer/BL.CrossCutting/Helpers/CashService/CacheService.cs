using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using BL.CrossCutting.Interfaces;
using BL.Model.Exception;

namespace BL.CrossCutting.Helpers.CashService
{
    public class CacheService : IDisposable, ICacheService
    {
        private readonly CasheBase<string, object> _dataCashe = new CasheBase<string, object>();
        private readonly CasheBase<string, Func<object>> _queryCashe = new CasheBase<string, Func<object>>();
        private readonly ReaderWriterLockSlim _locker = new ReaderWriterLockSlim();

        private List<KeyValuePair<string, string>> _updateDependency = new List<KeyValuePair<string, string>>();

        private string GetInternalKey(string key, int clientId)
        {
            return $"{clientId}|{key}";
        }

        private void UpdateData(string internalKey)
        {
            _locker.EnterWriteLock();
            try
            {
                Func<object> qry;
                if (_queryCashe.TryGet(internalKey, out qry))
                {
                    var data = qry();
                    _dataCashe.AddOrUpdate(internalKey, data);
                }
                else
                {
                    throw new WrongCasheKey();
                }

                foreach (var depend in _updateDependency.Where(x => x.Key == internalKey))
                {
                    if (_queryCashe.TryGet(depend.Value, out qry))
                    {
                        var data = qry();
                        _dataCashe.AddOrUpdate(depend.Value, data);
                    }
                    else
                    {
                        throw new WrongCasheKey();
                    }
                }

            }
            finally { _locker.ExitWriteLock(); }
        }

        public void AddOrUpdateCasheData(IContext ctx, string key, Func<object> getData)
        {
            var intK = GetInternalKey(key, ctx.Client.Id);
            _queryCashe.AddOrUpdate(intK, getData);
            UpdateData(intK);
        }

        public bool Exists(IContext ctx, string key)
        {
            _locker.EnterReadLock();
            try
            {
                return _queryCashe.Exists(GetInternalKey(key, ctx.Client.Id));
            }
            finally { _locker.ExitReadLock(); }
        }

        public object GetData(IContext ctx, string key)
        {
            _locker.EnterReadLock();
            try
            {
                object rv;
                return _dataCashe.TryGet(GetInternalKey(key, ctx.Client.Id), out rv) ? rv : null;
            }
            finally { _locker.ExitReadLock(); }
        }

        // be careful with that method. It should be used only inside AddOrUpdate method
        public object GetDataWithoutLock(IContext ctx, string key)
        {
            try
            {
                object rv;
                return _dataCashe.TryGet(GetInternalKey(key, ctx.Client.Id), out rv) ? rv : null;
            }
            catch 
            {
                return null;
            }
        }

        public void RefreshKey(IContext ctx, string key)
        {
            var intK = GetInternalKey(key, ctx.Client.Id);
            if (_queryCashe.Exists(intK))
            {
                UpdateData(intK);
            }
        }

        public void AddUpdateDependency(IContext ctx, string depends, string dependsFrom)
        {
            _updateDependency.Add(new KeyValuePair<string, string>(GetInternalKey(dependsFrom, ctx.Client.Id), GetInternalKey(depends, ctx.Client.Id)));
        }

        public void RemoveUpdateDependency(IContext ctx, string depends, string dependsFrom)
        {
            _updateDependency.RemoveAll(x => x.Key == GetInternalKey(dependsFrom, ctx.Client.Id) && x.Value == GetInternalKey(depends, ctx.Client.Id));
        }

        public void Dispose()
        {
            _dataCashe.Dispose();
            _queryCashe.Dispose();
            _locker.Dispose();
        }
    }
}