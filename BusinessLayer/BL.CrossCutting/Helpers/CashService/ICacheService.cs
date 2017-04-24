using System;
using BL.CrossCutting.Interfaces;

namespace BL.CrossCutting.Helpers.CashService
{
    public interface ICacheService
    {
        void AddOrUpdateCasheData(IContext ctx, string key, Func<object> getData);
        bool Exists(IContext ctx, string key);
        object GetData(IContext ctx, string key);
        object GetDataWithoutLock(IContext ctx, string key);
        void RefreshKey(IContext ctx, string key);

        void AddUpdateDependency(IContext ctx, string depends, string dependsFrom);
        void RemoveUpdateDependency(IContext ctx, string depends, string dependsFrom);
    }
}