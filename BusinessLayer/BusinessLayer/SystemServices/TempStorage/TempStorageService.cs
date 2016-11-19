using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using BL.Model.Enums;
using BL.Model.SystemCore;

namespace BL.Logic.SystemServices.TempStorage
{
    public class TempStorageService : ITempStorageService
    {
        private List<TempStoreObject> storeObjects;
        private readonly Timer _cleanTimer;
        private readonly int timeToStore = 10; // minutes

        public TempStorageService()
        {
            var timerRefresh = timeToStore * 60 * 1000;
            storeObjects = new List<TempStoreObject>();
            _cleanTimer = new Timer(OnSinchronize, null, timerRefresh, timerRefresh);
        }

        public void AddToStore(EnumObjects ownerType, int ownerId, int objectId, object storeobject)
        {
            storeObjects.RemoveAll(x => x.ObjectId == objectId && x.OwnerId == ownerId && x.OwnerType == ownerType);
            storeObjects.Add(new TempStoreObject
            {
                LastUsage = DateTime.Now,
                OwnerId = ownerId,
                OwnerType = ownerType,
                ObjectId = objectId,
                StoreObject = storeobject
            });
        }

        public object GetStoreObject(EnumObjects ownerType, int ownerId, int objectId)
        {
            var obj = storeObjects.FirstOrDefault(x => x.ObjectId == objectId && x.OwnerId == ownerId && x.OwnerType == ownerType);
            if (obj!=null) obj.LastUsage = DateTime.Now;
            return obj?.StoreObject;
        }

        public object ExtractStoreObject(EnumObjects ownerType, int ownerId, int objectId)
        {
            var obj = storeObjects.FirstOrDefault(x => x.ObjectId == objectId && x.OwnerId == ownerId && x.OwnerType == ownerType);
            if (obj!=null) storeObjects.Remove(obj);
            return obj?.StoreObject;
        }

        private void OnSinchronize(object state)
        {
            storeObjects.RemoveAll(x => (DateTime.Now - x.LastUsage).TotalMinutes > timeToStore);
        }
    }
}