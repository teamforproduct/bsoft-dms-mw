using System;
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
                 
            }
            finally { _locker.ExitWriteLock(); }
        }

        public void AddOrUpdateCasheData(IContext ctx, string key, Func<object> getData)
        {
            var intK = GetInternalKey(key, ctx.CurrentClientId);
            _queryCashe.AddOrUpdate(intK, getData);
            UpdateData(intK);
        }

        public bool Exists(IContext ctx, string key)
        {
            _locker.EnterReadLock();
            try
            {
                return _queryCashe.Exists(GetInternalKey(key, ctx.CurrentClientId));
            }
            finally { _locker.ExitReadLock(); }
        }

        public object GetData(IContext ctx, string key)
        {
            _locker.EnterReadLock();
            try
            {
                object rv;
                return _dataCashe.TryGet(GetInternalKey(key, ctx.CurrentClientId), out rv) ? rv : null;
            }
            finally { _locker.ExitReadLock(); }
        }

        public void RefreshKey(IContext ctx, string key)
        {
            var intK = GetInternalKey(key, ctx.CurrentClientId);
            if (_queryCashe.Exists(intK))
            {
                UpdateData(intK);
            }
        }

        public void Dispose()
        {
            _dataCashe.Dispose();
            _queryCashe.Dispose();
            _locker.Dispose();
        }
    }
}